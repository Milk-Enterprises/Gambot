using System;
using System.Collections.Generic;
using System.Linq;
using Gambot.Data;

namespace Gambot.Core
{
    public interface IMessagePipeline
    {
        void AddHandler(IMessageHandler variableHandler);
        void Process(IMessenger messenger, IMessage message, bool addressed);
    }

    public class MessagePipeline : IMessagePipeline
    {
        private readonly List<IMessageHandler> messageHandlers = new List<IMessageHandler>();

        private readonly IDataStoreManager dataStoreManager;
        private readonly IVariableHandler variableHandler;

        public MessagePipeline(IDataStoreManager dataStoreManager, IVariableHandler variableHandler)
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

        public void Process(IMessenger messenger, IMessage message, bool addressed)
        {
            var response = String.Empty;
            foreach (var handler in messageHandlers) {
                response = handler.Process(response, message, addressed);
                if (response == null) {
                    break;
                }
            }

            if (!String.IsNullOrEmpty(response)) {
                messenger.SendMessage(response, message.Where, message.Action);
            }
        }
    }
}
