using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gambot.Data;
using Gambot.Data.InMemory;

namespace Gambot.Core
{
    public interface IMessageDispatcher
    {
        void AddHandler<T>() where T : IMessageHandler, new();
        void Digest(IMessenger messenger, IMessage message, bool addressed);
    }

    public class MessageDispatcher : IMessageDispatcher
    {
        private readonly List<IMessageHandler> messageHandlers = new List<IMessageHandler>();
        private readonly IDataStoreManager dataStoreManager; // todo: make this entire class not fucking static for the love of shit

        public MessageDispatcher()
        {
            dataStoreManager = new InMemoryDataStoreManager(); // todo: di
        }

        public void AddHandler<T>() where T : IMessageHandler, new()
        {
            var handler = new T();
            handler.Initialize(dataStoreManager);
            messageHandlers.Add(handler);

            // it awaits
            if (typeof(T).GetInterfaces().Contains(typeof(IVariableFallbackHandler)))
                VariableHandler.AddFallbackHandler((IVariableFallbackHandler)handler);
        }

        public void Digest(IMessenger messenger, IMessage message, bool addressed)
        {
            foreach (var handler in messageHandlers)
            {
                if (!handler.Digest(messenger, message, addressed))
                    break;
            }
        }
    }
}
