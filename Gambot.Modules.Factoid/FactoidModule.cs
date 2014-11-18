using Gambot.Core;

namespace Gambot.Modules.Factoid
{
    public class FactoidModule : AbstractModule
    {
        public FactoidModule(IVariableHandler variableHandler)
        {
            MessageProducers.Add(new FactoidCommandProducer());
            MessageProducers.Add(new FactoidTriggerProducer(variableHandler));
        }
    }
}
