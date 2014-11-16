using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gambot.Core
{
    public abstract class AbstractModule : IModule
    {
        protected IList<IMessageProducer> MessageHandlers { get; set; }

        protected AbstractModule()
        {
            MessageHandlers = new List<IMessageProducer>();
        }

        public virtual IEnumerable<IMessageProducer> GetMessageHandlers()
        {
            return MessageHandlers;
        }
    }
}
