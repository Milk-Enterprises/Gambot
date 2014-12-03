using Gambot.Core;

namespace Gambot.Modules.TLA
{
    public class TLAModule : AbstractModule
    {
        public TLAModule(IVariableHandler variableHandler)
        {
            MessageReactors.Add(new AcronymExpansionReactor(variableHandler));
            MessageReactors.Add(new AcronymDefinitionReactor(variableHandler));
        }
    }
}
