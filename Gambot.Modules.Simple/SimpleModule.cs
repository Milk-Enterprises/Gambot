using Gambot.Core;

namespace Gambot.Modules.Simple
{
    public class SimpleModule : AbstractModule
    {
        public SimpleModule(IVariableHandler variableHandler)
        {
            MessageHandlers.Add(new SimpleResponseProducer(variableHandler));
        }
    }
}
