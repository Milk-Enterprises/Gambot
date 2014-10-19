using Gambot.Core;

namespace Gambot.Modules.Reply
{
    public class ReplyModule : AbstractModule
    {
        public ReplyModule(IVariableHandler variableHandler)
        {
            MessageHandlers.Add(new FactoidCommandHandler());
            MessageHandlers.Add(new FactoidTriggerHandler(variableHandler));
        }
    }
}
