using System;
using System.Collections.Generic;
using System.Linq;
using Gambot.Data;

namespace Gambot.Core
{
    public interface IMessageProcessor
    {
        void AddHandler(IMessageProducer messageProducer);
        void Process(IMessenger messenger, IMessage message, bool addressed);
    }

    public class MessageProcessor : IMessageProcessor
    {
        private readonly List<IMessageProducer> messageHandlers;
        private readonly IDataStoreManager dataStoreManager;
        private readonly IVariableHandler variableHandler;

        public MessageProcessor(IDataStoreManager dataStoreManager,
                               IVariableHandler variableHandler)
        {
            this.dataStoreManager = dataStoreManager;
            this.variableHandler = variableHandler;
            this.messageHandlers = new List<IMessageProducer>();
        }

        public void AddHandler(IMessageProducer messageProducer)
        {
            messageProducer.Initialize(dataStoreManager);
            messageHandlers.Add(messageProducer);

            // it awaits
            var instance = messageProducer as IVariableFallbackHandler;
            if (instance != null)
                variableHandler.AddFallbackHandler(instance);
        }

        public void Process(IMessenger messenger, IMessage message,
                            bool addressed)
        {
            var response = String.Empty;
            foreach (var handler in messageHandlers)
            {
                response = handler.Process(message, addressed);

                if (response == null)
                    break;
            }

            if (!String.IsNullOrEmpty(response))
                messenger.SendMessage(response, message.Where, message.Action);
        }
    }
}
