using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gambot.Core
{
    public abstract class AbstractModule : IModule
    {
        protected IList<IMessageHandler> MessageHandlers { get; set; }

        protected AbstractModule()
        {
            MessageHandlers = new List<IMessageHandler>();
        }

        public virtual IEnumerable<IMessageHandler> GetMessageHandlers()
        {
            return MessageHandlers;
        }
    }
}
