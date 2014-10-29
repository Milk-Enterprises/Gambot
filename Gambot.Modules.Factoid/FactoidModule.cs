using Gambot.Core;

namespace Gambot.Modules.Factoid
{
    public class FactoidModule : AbstractModule
    {
        public FactoidModule(IVariableHandler variableHandler)
        {
            MessageHandlers.Add(new FactoidCommandHandler());
            MessageHandlers.Add(new FactoidTriggerHandler(variableHandler));
        }
    }
}
