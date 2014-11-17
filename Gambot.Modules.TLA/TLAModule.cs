using Gambot.Core;

namespace Gambot.Modules.TLA
{
    public class TLAModule : AbstractModule
    {
        public TLAModule(IVariableHandler variableHandler)
        {
            MessageProducers.Add(new AcronymExpansionProducer(variableHandler));

            MessageReactors.Add(new AcronymDefinitionReactor(variableHandler));
        }
    }
}
