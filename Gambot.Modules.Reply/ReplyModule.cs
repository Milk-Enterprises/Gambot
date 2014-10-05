using Gambot.Core;

namespace Gambot.Modules.Reply
{
    public class ReplyModule : AbstractModule
    {
        public ReplyModule()
        {
            MessageHandlers.Add(new ReplyCommandHandler());
            MessageHandlers.Add(new ReplyTriggerHandler());
        }
    }
}
