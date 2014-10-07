using System.Collections.Generic;
using System.Linq;
using Gambot.Data;

namespace Gambot.Core
{
    public interface IMessageDispatcher
    {
        void AddHandler(IMessageHandler variableHandler);
        void Digest(IMessenger messenger, IMessage message, bool addressed);
    }

    public class MessageDispatcher : IMessageDispatcher
    {
        private readonly List<IMessageHandler> messageHandlers = new List<IMessageHandler>();

        private readonly IDataStoreManager dataStoreManager;
        private readonly IVariableHandler variableHandler;

        public MessageDispatcher(IDataStoreManager dataStoreManager, IVariableHandler variableHandler)
        {
            this.dataStoreManager = dataStoreManager;
            this.variableHandler = variableHandler;
        }

        public void AddHandler(IMessageHandler handler)
        {
            handler.Initialize(dataStoreManager);
            messageHandlers.Add(handler);

            // it awaits
            var instance = handler as IVariableFallbackHandler;
            if (instance != null)
                variableHandler.AddFallbackHandler(instance);
        }

        public void Digest(IMessenger messenger, IMessage message, bool addressed)
        {
            foreach (var handler in messageHandlers) {
                if (!handler.Digest(messenger, message, addressed))
                    break;
            }
        }
    }
}
