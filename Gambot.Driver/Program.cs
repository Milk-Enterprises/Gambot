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
using Gambot.Modules.People;
using Gambot.Modules.Reply;
using Gambot.Modules.Variables;
using SimpleInjector;

namespace Gambot.Driver
{
    public class Program
    {
        private static IMessenger messenger;

        static void Main(string[] args)
        {
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                Console.WriteLine("Shutting down...");
                if (messenger != null)
                    messenger.Dispose();
                Environment.Exit(0);
            };
            
            Console.WriteLine("Starting up... ");

            var container = CreateContainer();
            container.Verify();
#if DEBUG
            messenger = new ConsoleMessenger();
            var dispatcher = container.GetInstance<IMessageDispatcher>();

            var modules = container.GetAllInstances<IModule>();
            var handlers = modules.SelectMany(mo => mo.GetMessageHandlers());
            foreach (var handler in handlers) dispatcher.AddHandler(handler);
#else
            // TODO: Select implementation at run-time
            messenger = new IrcMessenger();
#endif

            messenger.MessageReceived += (sender, eventArgs) => 
                dispatcher.Digest(messenger, eventArgs.Message, eventArgs.Addressed);

            Thread.Sleep(Timeout.Infinite);
        }

        private static Container CreateContainer()
        {
            var container = new Container();
            
            container.RegisterSingle<IMessageDispatcher, MessageDispatcher>();
            container.RegisterSingle<IVariableHandler, VariableHandler>();
            container.RegisterSingle<IDataStoreManager, InMemoryDataStoreManager>();

            // Register all the IModules in the currently loaded assemblies
            var execPath = Assembly.GetExecutingAssembly().CodeBase;
            var assemblyPath =  new Uri(Path.GetDirectoryName(execPath)).LocalPath; 
            LoadAssembliesFromPath(assemblyPath);

            var moduleTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof (IModule).IsAssignableFrom(t) && t != typeof (IModule) && !t.IsAbstract);

            container.RegisterAll(typeof (IModule), moduleTypes);

            return container;
        }

        private static void LoadAssembliesFromPath(string p)
        {
            var files = new DirectoryInfo(p).GetFiles("*.dll", SearchOption.AllDirectories);

            foreach (var fi in files)
            {
                var assemblyName = fi.FullName;
                var assembly = AssemblyName.GetAssemblyName(assemblyName);

                // not currently loaded? load dat shit
                if (!AppDomain.CurrentDomain.GetAssemblies().Any(ass => AssemblyName.ReferenceMatchesDefinition(assembly, ass.GetName()))) {
                    Assembly.Load(assembly);
                }
            }
        }
    }
}
