using Gambot.Core;

namespace Gambot.Modules.Variables
{
    public class VariablesModule : AbstractModule
    {
        public VariablesModule()
        {
            MessageProducers.Add(new VariableCommandProducer());
        }
    }
}
