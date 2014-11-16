using System;
using System.Collections.Generic;
using System.Linq;
using Gambot.Data;

namespace Gambot.Core
{
    public interface IMessageProcessor
    {
        void AddProducer(IMessageProducer messageProducer);
        void AddReactor(IMessageReactor messageReactor);
        void AddTransformer(IMessageTransformer messageTransformer);
        void Process(IMessenger messenger, IMessage message, bool addressed);
    }

    public class MessageProcessor : IMessageProcessor
    {
        private readonly List<IMessageProducer> messageProducers,
                                                messageReactors,
                                                messageTransformers;
        private readonly IDataStoreManager dataStoreManager;
        private readonly IVariableHandler variableHandler;

        public MessageProcessor(IDataStoreManager dataStoreManager,
                               IVariableHandler variableHandler)
        {
            this.dataStoreManager = dataStoreManager;
            this.variableHandler = variableHandler;

            messageProducers = new List<IMessageProducer>();
            messageReactors = new List<IMessageProducer>();
            messageTransformers = new List<IMessageProducer>();
        }
        
        public void AddProducer(IMessageProducer messageProducer)
        {
            messageProducer.Initialize(dataStoreManager);
            messageProducers.Add(messageProducer);

            var instance = messageProducer as IVariableFallbackHandler;
            if (instance != null)
                variableHandler.AddFallbackHandler(instance);
        }

        public void AddReactor(IMessageReactor messageReactor)
        {
            messageReactor.Initialize(dataStoreManager);
            messageReactors.Add(messageReactor);

            var instance = messageReactor as IVariableFallbackHandler;
            if (instance != null)
                variableHandler.AddFallbackHandler(instance);
        }

        public void AddTransformer(IMessageTransformer messageTransformer)
        {
            messageTransformer.Initialize(dataStoreManager);
            messageReactors.Add(messageTransformer);

            var instance = messageTransformer as IVariableFallbackHandler;
            if (instance != null)
                variableHandler.AddFallbackHandler(instance);
        }

        public void Process(IMessenger messenger, IMessage message,
                            bool addressed)
        {
            // producers -> reactors (if no message was produced) -> transformers (if any message was produced)

            // producers
            IMessage responseMessage = null;
            foreach (var producer in messageProducers)
            {
                responseMessage = producer.Process(message, addressed);

                if (responseMessage != null)
                    break;
            }
            
            // reactors
            if (responseMessage == null)
            {
                foreach (var reactor in messageReactors)
                {
                    responseMessage = reactor.Process(message, addressed);

                    if (responseMessage != null)
                        break;
                }
            }

            // finally, transformers
            if (responseMessage != null)
            {
                foreach (var transformer in messageTransformers)
                {
                    // TODO: this indicates cumulative transformations on the message
                    // perhaps we limit it to only 1 transformation at a time?
                    responseMessage = transformer.Process(responseMessage,
                                                          addressed);
                }
            }

            if (responseMessage != null)
                messenger.SendMessage(responseMessage.Text, responseMessage.Where, responseMessage.Action);
        }
    }
}
