using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gambot.Data;
using Gambot.Data.InMemory;

namespace Gambot.Core
{
    public static class GrandMessageHandler
    {
        private static readonly List<IMessageHandler> messageHandlers = new List<IMessageHandler>();
        private static readonly IDataStoreManager dataStoreManager; // todo: make this entire class not fucking static for the love of shit

        static GrandMessageHandler()
        {
            dataStoreManager = new InMemoryDataStoreManager(); // todo: di
        }

        public static void AddHandler<T>() where T : IMessageHandler, new()
        {
            var handler = new T();
            handler.Initialize(dataStoreManager);
            messageHandlers.Add(handler);

            // it awaits
            if (typeof(T).GetInterfaces().Contains(typeof(IVariableFallbackHandler)))
                Variables.AddFallbackHandler((IVariableFallbackHandler)handler);
        }

        public static void Digest(IMessenger messenger, IMessage message, bool addressed)
        {
            foreach (var handler in messageHandlers)
            {
                if (!handler.Digest(messenger, message, addressed))
                    break;
            }
        }
    }
}
