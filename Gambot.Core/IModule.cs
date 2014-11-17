using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gambot.Core
{
    public interface IModule
    {
        IEnumerable<IMessageListener> GetMessageListeners();

        IEnumerable<IMessageProducer> GetMessageProducers();

        IEnumerable<IMessageReactor> GetMessageReactors();

        IEnumerable<IMessageTransformer> GetMessageTransformers();
    }
}
