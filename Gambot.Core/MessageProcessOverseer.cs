using System;
using System.Collections.Generic;
using System.Linq;
using Gambot.Data;

namespace Gambot.Core
{
    public interface IMessageProcessOverseer
    {
        void AddListener(IMessageListener messageListener);
        void AddProducer(IMessageProducer messageProducer);
        void AddReactor(IMessageReactor messageReactor);
        void AddTransformer(IMessageTransformer messageTransformer);
        void Process(IMessenger messenger, IMessage message, bool addressed);
    }

    public class MessageProcessOverseer : IMessageProcessOverseer
    {
        private readonly List<IMessageListener> messageListeners;
        private readonly List<IMessageProducer> messageProducers;
        private readonly List<IMessageReactor> messageReactors;
        private readonly List<IMessageTransformer> messageTransformers;
        private readonly IDataStoreManager dataStoreManager;
        private readonly IVariableHandler variableHandler;

        public MessageProcessOverseer(IDataStoreManager dataStoreManager,
                               IVariableHandler variableHandler)
        {
            this.dataStoreManager = dataStoreManager;
            this.variableHandler = variableHandler;

            messageListeners = new List<IMessageListener>();
            messageProducers = new List<IMessageProducer>();
            messageReactors = new List<IMessageReactor>();
            messageTransformers = new List<IMessageTransformer>();
        }

        public void AddListener(IMessageListener messageListener)
        {
            messageListener.Initialize(dataStoreManager);
            messageListeners.Add(messageListener);

            var instance = messageListener as IVariableFallbackHandler;
            if (instance != null)
                variableHandler.AddFallbackHandler(instance);
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
            messageTransformers.Add(messageTransformer);

            var instance = messageTransformer as IVariableFallbackHandler;
            if (instance != null)
                variableHandler.AddFallbackHandler(instance);
        }

        public void Process(IMessenger messenger, IMessage message,
                            bool addressed)
        {
            // listeners -> producers -> reactors (if no message was produced) -> transformers (if any message was produced)

            // listeners
            foreach (var listener in messageListeners)
                listener.Listen(message, addressed);

            // producers
            ProducerResponse response = null;
            if (addressed)
            {
                foreach (var producer in messageProducers)
                {
                    response = producer.Process(message, addressed);

                    if (response != null)
                        break;
                }
            }

            // reactors
            if (response == null)
            {
                foreach (var reactor in messageReactors)
                {
                    response = reactor.Process(message, addressed);

                    if (response != null)
                        break;
                }
            }

            var transformedResponseText = response == null ? null : response.Message;

            // finally, transformers
            if (response != null)
            {
                foreach (var transformer in messageTransformers)
                {
                    // TODO: this indicates cumulative transformations on the message
                    // perhaps we limit it to only 1 transformation at a time?
                    transformedResponseText = transformer.Transform(response.IsAction, transformedResponseText, addressed);
                }
            }

            if (response != null)
                messenger.SendMessage(transformedResponseText, message.Where, response.IsAction);
        }
    }
}
