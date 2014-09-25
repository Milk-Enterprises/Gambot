using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gambot.Core
{
    public static class GrandMessageHandler
    {
        private static readonly List<IMessageHandler> MessageHandlers = new List<IMessageHandler>();

        public static void AddHandler<T>() where T : IMessageHandler, new()
        {
            var handler = new T();
            handler.Initialize(null /* TODO */);
            MessageHandlers.Add(handler);
        }

        public static void Digest(IMessenger messenger, IMessage message, bool addressed)
        {
            foreach (var handler in MessageHandlers)
            {
                if (!handler.Digest(messenger, message, addressed))
                    break;
            }
        }
    }
}
