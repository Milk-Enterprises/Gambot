using System;
using System.Threading;
using Gambot.Core;
using Gambot.IO.Console;
using Gambot.IO.IRC;
using Gambot.SimpleResponses;

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
            
#if DEBUG
            messenger = new ConsoleMessenger();
            GrandMessageHandler.AddHandler<SimpleResponseHandler>();
#else
            // TODO: Select implementation at run-time
            messenger = new IrcMessenger();
#endif

            messenger.MessageReceived += (sender, eventArgs) => 
                GrandMessageHandler.Digest(messenger, eventArgs.Message, eventArgs.Addressed);

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
