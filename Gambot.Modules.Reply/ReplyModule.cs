using Gambot.Core;

namespace Gambot.Modules.Reply
{
    public class ReplyModule : AbstractModule
    {
        public ReplyModule(IVariableHandler variableHandler)
        {
            MessageHandlers.Add(new ReplyCommandHandler());
            MessageHandlers.Add(new ReplyTriggerHandler(variableHandler));
        }
    }
}
