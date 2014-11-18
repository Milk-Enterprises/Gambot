using System.Collections.Generic;
using Gambot.Core;

namespace Gambot.Modules.People
{
    public class PeopleModule : AbstractModule
    {
        public PeopleModule(IVariableHandler variableHandler)
        {
            MessageListeners.Add(new KnownPeopleListener(variableHandler));

            MessageProducers.Add(new GenderCommandProducer(variableHandler));
        }
    }
}
