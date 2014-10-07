using System;
using System.Threading;
using Gambot.Core;

namespace Gambot.IO.Console
{
    public class ConsoleMessenger : IMessenger
    {
        protected Thread inputThread;
        protected string name;

        public event EventHandler<MessageEventArgs> MessageReceived = delegate { }; 

        public ConsoleMessenger()
        {
            inputThread = new Thread(() =>
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
            inputThread.Start();
            name = Config.Get("Name", "gambot");
        }

        public void SendMessage(string message, string destination, bool action = false)
        {
            System.Console.WriteLine(action ? "*\t{0} {1}" : "{0}:\t{1}", name, message);
        }

        public void Dispose()
        {
            inputThread.Abort();
        }
    }
}
