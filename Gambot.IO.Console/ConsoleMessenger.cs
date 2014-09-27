using System;
using System.Threading;
using Gambot.Core;

namespace Gambot.IO.Console
{
    public class ConsoleMessenger : IMessenger
    {
        protected Thread InputThread;
        protected string Name;

        public event EventHandler<MessageEventArgs> MessageReceived;

        public ConsoleMessenger()
        {
            InputThread = new Thread(() =>
            {
                while (true)
                {
                    System.Console.Write("{0}:\t", Environment.UserName);
                    var message = System.Console.ReadLine();
                    if (message != null && MessageReceived != null)
                    {
                        MessageReceived(this,
                            new MessageEventArgs
                            {
                                Message = new ConsoleMessage(message),
                                Addressed = true
                            });
                    }
                }
            });
            InputThread.Start();
            Name = Config.Get("Name", "gambot");
        }

        public void SendMessage(string message, string destination, bool action = false)
        {
            System.Console.WriteLine(action ? "*\t{0} {1}" : "{0}:\t{1}", Name, message);
        }

        public void Dispose()
        {
            InputThread.Abort();
        }
    }
}
