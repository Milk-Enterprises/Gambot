using Gambot.Core;

namespace Gambot.Modules.Roll
{
    public class RollModule : AbstractModule
    {
        public RollModule(IVariableHandler variableHandler)
        {
            MessageProducers.Add(new RollResponseProducer(variableHandler));
        }
    }
}
