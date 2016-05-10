using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Gambot.Core;
using Gambot.Data;
using Gambot.Data.InMemory;
using Gambot.Data.SQLite;
using Gambot.IO.Console;
using Gambot.IO.IRC;
using Gambot.Modules.Config;
using Gambot.Modules.Factoid;
using Gambot.Modules.Inventory;
using Gambot.Modules.Quotes;
using Gambot.Modules.People;
using Gambot.Modules.Repeater;
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

            AppDomain.CurrentDomain.UnhandledException += (sender, e) => 
            {
                logger.Error(e.ExceptionObject);
                if (messenger != null)
                {
                    messenger.SendMessage("Farewell, cruel world!", "#botulism");
                    messenger.SendMessage(((Exception)e.ExceptionObject).Message, "#rob");
                    messenger.Dispose();
                }
                Environment.Exit(0);
            };

            logger.Info("Starting up... ");

            var container = CreateContainer();
            container.Verify();
#if DEBUG
            messenger = new ConsoleMessenger();
#else
            // TODO: Select implementation at run-time
            messenger = new IrcMessenger();
#endif

            var pipeline = container.GetInstance<IMessageProcessOverseer>();

            var modules = container.GetAllInstances<IModule>().ToList();
            var listeners = modules.SelectMany(mo => mo.GetMessageListeners());
            var producers = modules.SelectMany(mo => mo.GetMessageProducers());
            var reactors = modules.SelectMany(mo => mo.GetMessageReactors());
            var transformers = modules.SelectMany(mo => mo.GetMessageTransformers());
            
            foreach (var listener in listeners)
                pipeline.AddListener(listener);

            foreach (var producer in producers)
                pipeline.AddProducer(producer);

            foreach (var reactor in reactors)
                pipeline.AddReactor(reactor);

            foreach (var transformer in transformers)
                pipeline.AddTransformer(transformer);

            messenger.MessageReceived += (sender, eventArgs) =>
                                         pipeline.Process(messenger,
                                                          eventArgs.Message,
                                                          eventArgs.Addressed);

            Thread.Sleep(Timeout.Infinite);
        }

        private static Container CreateContainer()
        {
            var container = new Container();

            container.RegisterSingle<IMessageProcessOverseer, MessageProcessOverseer>();
            container.RegisterSingle<IVariableHandler, VariableHandler>();
#if DEBUG
            container
                .RegisterSingle<IDataStoreManager, InMemoryDataStoreManager>();
#else
            container
                .RegisterSingle<IDataStoreManager, SqliteDataStoreManager>();
#endif

            // Register all the IModules in the currently loaded assemblies
            var execPath = Assembly.GetExecutingAssembly().CodeBase;
            var assemblyPath = Environment.CurrentDirectory;

            LoadAssembliesFromPath(assemblyPath);

            var moduleTypes = AppDomain.CurrentDomain.GetAssemblies()
                                       .SelectMany(a => a.GetTypes())
                                       .Where(
                                           t =>
                                           typeof(IModule).IsAssignableFrom(t) &&
                                           t != typeof(IModule) && !t.IsAbstract)
                                       .ToList();

            foreach (var moduleType in moduleTypes)
            {
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
#if DEBUG
                                  AssemblyName.ReferenceMatchesDefinition(
                                      assembly, ass.GetName())))
#else
                                      assembly.Name == ass.GetName().Name))
#endif
                {
                    logger.Info("Loading {0}...", assemblyName);
                    Assembly.Load(assembly);
                }
            }
        }
    }
}
