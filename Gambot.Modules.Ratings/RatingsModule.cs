using System.Collections.Generic;
using Gambot.Core;

namespace Gambot.Modules.Ratings
{
    public class PeopleModule : AbstractModule
    {
        public PeopleModule(IVariableHandler variableHandler)
        {
            MessageProducers.Add(new RatingsProducer());
        }
    }
}
