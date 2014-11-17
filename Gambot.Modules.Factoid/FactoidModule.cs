using Gambot.Core;

namespace Gambot.Modules.Factoid
{
    public class FactoidModule : AbstractModule
    {
        public FactoidModule(IVariableHandler variableHandler)
        {
            MessageHandlers.Add(new FactoidCommandProducer());
            MessageHandlers.Add(new FactoidTriggerProducer(variableHandler));
        }
    }
}
