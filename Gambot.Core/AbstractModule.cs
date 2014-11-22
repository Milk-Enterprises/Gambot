using System.Collections.Generic;

namespace Gambot.Core
{
    public abstract class AbstractModule : IModule
    {
        protected IList<IMessageFilter> MessageFilters { get; set; }
        protected IList<IMessageListener> MessageListeners { get; set; }
        protected IList<IMessageProducer> MessageProducers { get; set; }
        protected IList<IMessageReactor> MessageReactors { get; set; }
        protected IList<IMessageTransformer> MessageTransformers { get; set; }

        protected AbstractModule()
        {
            MessageFilters = new List<IMessageFilter>();
            MessageListeners = new List<IMessageListener>();
            MessageProducers = new List<IMessageProducer>();
            MessageReactors = new List<IMessageReactor>();
            MessageTransformers = new List<IMessageTransformer>();
        }

        public virtual IEnumerable<IMessageFilter> GetMessageFilters()
        {
            return MessageFilters;
        }

        public virtual IEnumerable<IMessageListener> GetMessageListeners()
        {
            return MessageListeners;
        }

        public virtual IEnumerable<IMessageProducer> GetMessageProducers()
        {
            return MessageProducers;
        }

        public virtual IEnumerable<IMessageReactor> GetMessageReactors()
        {
            return MessageReactors;
        }

        public virtual IEnumerable<IMessageTransformer> GetMessageTransformers()
        {
            return MessageTransformers;
        }
    }
}
