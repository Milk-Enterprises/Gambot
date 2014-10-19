using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Gambot.Core;
using Gambot.Data;
using Gambot.Data.InMemory;
using Gambot.IO.Console;
using Gambot.Modules.Quotes;
using Gambot.Modules.People;
using Gambot.Modules.Reply;
using Gambot.Modules.TLA;
using Gambot.Modules.Variables;
using NLog;
using SimpleInjector;

namespace Gambot.Driver
{
    public class Program
    {
        private static IMessenger messenger;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                logger.Info("Shutting down...");
                if (messenger != null)
                    messenger.Dispose();
                Environment.Exit(0);
            };

            logger.Info("Starting up... ");

            var container = CreateContainer();
            container.Verify();
#if DEBUG
            messenger = new ConsoleMessenger();
            var pipeline = container.GetInstance<IMessagePipeline>();

            var modules = container.GetAllInstances<IModule>();
            var handlers = modules.SelectMany(mo => mo.GetMessageHandlers());
            foreach (var handler in handlers)
                pipeline.AddHandler(handler);
#else
    // TODO: Select implementation at run-time
            messenger = new IrcMessenger();
#endif

            messenger.MessageReceived += (sender, eventArgs) =>
                                         pipeline.Process(messenger,
                                                          eventArgs.Message,
                                                          eventArgs.Addressed);

            Thread.Sleep(Timeout.Infinite);
        }

        private static Container CreateContainer()
        {
            var container = new Container();

            container.RegisterSingle<IMessagePipeline, MessagePipeline>();
            container.RegisterSingle<IVariableHandler, VariableHandler>();
            container
                .RegisterSingle<IDataStoreManager, InMemoryDataStoreManager>();

            // Register all the IModules in the currently loaded assemblies
            var execPath = Assembly.GetExecutingAssembly().CodeBase;
            var assemblyPath =
                new Uri(Path.GetDirectoryName(execPath)).LocalPath;
            LoadAssembliesFromPath(assemblyPath);

            var moduleTypes = AppDomain.CurrentDomain.GetAssemblies()
                                       .SelectMany(a => a.GetTypes())
                                       .Where(
                                           t =>
                                           typeof(IModule).IsAssignableFrom(t) &&
                                           t != typeof(IModule) && !t.IsAbstract).ToList();

            foreach (var moduleType in moduleTypes) {
                container.RegisterSingle(moduleType, moduleType);
            }
            container.RegisterAll<IModule>(moduleTypes);

            return container;
        }

        private static void LoadAssembliesFromPath(string p)
        {
            logger.Info("Loading assemblies from {0}...", p);
            var files = new DirectoryInfo(p).GetFiles("*.dll",
                                                      SearchOption
                                                          .AllDirectories);

            foreach (var fi in files)
            {
                var assemblyName = fi.FullName;
                var assembly = AssemblyName.GetAssemblyName(assemblyName);

                // not currently loaded? load dat shit
                if (
                    !AppDomain.CurrentDomain.GetAssemblies()
                              .Any(
                                  ass =>
                                  AssemblyName.ReferenceMatchesDefinition(
                                      assembly, ass.GetName())))
                {
                    logger.Info("Loading {0}...", assemblyName);
                    Assembly.Load(assembly);
                }
            }
        }
    }
}
