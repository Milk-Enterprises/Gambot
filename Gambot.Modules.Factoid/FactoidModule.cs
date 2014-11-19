using Gambot.Core;

namespace Gambot.Modules.Factoid
{
    public class FactoidModule : AbstractModule
    {
        public FactoidModule(IVariableHandler variableHandler)
        {
            MessageProducers.Add(new FactoidCommandProducer());
            MessageReactors.Add(new FactoidTriggerReactor(variableHandler));
        }
    }
}
