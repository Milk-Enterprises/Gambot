using System;
using Gambot.Core;

namespace Gambot.Modules.Quotes
{
    public class QuotesModule : AbstractModule
    {
        public QuotesModule()
        {
            var recentMessageStore = new RecentMessageStore(Int32.Parse(Config.Get("MaxMessagesRememberedPerUser"))); // todo: use tryparse + default value + logging

            MessageHandlers.Add(new QuoteCommandHandler());
            MessageHandlers.Add(new RememberCommandHandler(recentMessageStore));
        }
    }
}
