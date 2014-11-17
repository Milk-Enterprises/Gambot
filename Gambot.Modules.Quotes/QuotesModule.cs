using System;
using Gambot.Core;

namespace Gambot.Modules.Quotes
{
    public class QuotesModule : AbstractModule
    {
        public QuotesModule(IVariableHandler variableHandler)
        {
            // todo: maybe move out to ioc, incase other shit need want this thing do?
            var recentMessageStore =
                new RecentMessageStore(
                    Int32.Parse(Config.Get("MaxMessagesRememberedPerUser")));
                // todo: use tryparse + default value + logging

            MessageListeners.Add(new RecentMessageListener(recentMessageStore));

            MessageProducers.Add(new QuoteCommandProducer(variableHandler));
            MessageProducers.Add(new RememberCommandProducer(recentMessageStore));
        }
    }
}
