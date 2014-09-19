using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gambot.Core
{
    public class MessageEventArgs : EventArgs
    {
        public IMessage Message;
        public bool Addressed;
    }

    public interface IMessenger : IDisposable
    {
        event EventHandler<MessageEventArgs> MessageReceived;
        void SendMessage(string message, string destination);
    }
}
