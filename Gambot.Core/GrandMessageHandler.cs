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
        private static readonly IDataStoreManager _dataStoreManager; // todo: make this entire class not fucking static for the love of shit

        static GrandMessageHandler()
        {
            //_dataStoreManager = new InMemoryDataStoreManager(); // todo: kill me in the fucking MOUTH
        }

        public static void AddHandler<T>() where T : IMessageHandler, new()
        {
            var handler = new T();
            handler.Initialize(_dataStoreManager);
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
