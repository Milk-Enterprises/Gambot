using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatSharp;

namespace Gambot
{
    public class Message
    {
        public string Who { get; protected set; }
        public string To { get; protected set; }
        public string Text { get; protected set; }
        public string Where { get; protected set; }
        public bool Action { get; protected set; }

        public Message(PrivateMessage raw)
        {
            Who = raw.User.Nick;
            Text = raw.Message;
            Where = raw.Source;
        }
    }

    public interface IMessageHandler
    {
        void Initialize();
        bool Digest(IrcClient irc, Message message);
    }

    public static class GrandMessageHandler
    {
        private static readonly List<IMessageHandler> MessageHandlers = new List<IMessageHandler>();

        public static void AddHandler<T>() where T : IMessageHandler, new()
        {
            var handler = new T();
            handler.Initialize();
            MessageHandlers.Add(handler);
        }

        public static void Digest(IrcClient irc, PrivateMessage pm)
        {
            var message = new Message(pm);

            foreach (var handler in MessageHandlers)
            {
                if (!handler.Digest(irc, message))
                    break;
            }
        }
    }
}
