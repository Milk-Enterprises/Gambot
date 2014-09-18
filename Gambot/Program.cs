using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChatSharp;

namespace Gambot
{
    public class Program
    {
        private static IrcClient IRC;

        static void Main(string[] args)
        {
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                Console.WriteLine("Shutting down...");
                if (IRC != null)
                    IRC.Quit();
                Environment.Exit(0);
            };

            var server = Config.Get("Irc.Server");
            var user = Config.Get("Irc.Nick", "gambot");
            var password = Config.Get("Irc.Password");
            var ssl = Config.GetBool("Irc.Ssl");
            IRC = new IrcClient(server, new IrcUser(user, user, password), ssl);

            Console.Write("Starting up... ");
            IRC.ConnectionComplete += (sender, eventArgs) => Console.WriteLine("Connected.");

            GrandMessageHandler.AddHandler<TestMessageHandler>();

            IRC.PrivateMessageRecieved += (sender, eventArgs) =>
            {
                Console.WriteLine("[<-] [{0}]\t[{1}]\t{2}", eventArgs.PrivateMessage.Source, eventArgs.PrivateMessage.User.Nick, eventArgs.PrivateMessage.Message);
                GrandMessageHandler.Digest(IRC, eventArgs.PrivateMessage);
            };
            
            IRC.ConnectAsync();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
