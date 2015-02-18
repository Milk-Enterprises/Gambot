using System;
using System.Linq;
using System.Text.RegularExpressions;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Quotes
{
    internal class RememberCommandProducer : IMessageProducer
    {
        private IDataStore quotesDataStore;
        private readonly IRecentMessageStore recentMessageStore;

        public RememberCommandProducer(IRecentMessageStore recentMessageStore)
        {
            this.recentMessageStore = recentMessageStore;
        }

        public void Initialize(IDataStoreManager dataStoreManager)
        {
            quotesDataStore = dataStoreManager.Get("Quotes");
        }

        public ProducerResponse Process(IMessage message, bool addressed)
        {
            if (addressed)
            {
                var match = Regex.Match(message.Text, @"^remember (\w+) (.+)$",
                                        RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var rememberTarget = match.Groups[1].Value.Trim();
                    var rememberMsg = match.Groups[2].Value.Trim();

                    // cant let the user remember his own quotes
                    if (rememberTarget.Equals(message.Who,
                                              StringComparison
                                                  .InvariantCultureIgnoreCase))
                    {
                        return new ProducerResponse(String.Format("Sorry {0}, but you can't quote yourself.", message.Who), false);
                    }

                    // check if target said such a thing
                    var usersRecentMessages =
                        recentMessageStore.GetRecentMessagesFromUser(
                            rememberTarget);
                    if (usersRecentMessages == null)
                    {
                        return new ProducerResponse(String.Format("Sorry, I don't know anyone named \"{0}.\"", rememberTarget), false);
                    }

                    var matchingMsg =
                        usersRecentMessages.FirstOrDefault(
                            msg =>
                            msg.Text.IndexOf(rememberMsg,
                                             StringComparison
                                                 .InvariantCultureIgnoreCase) !=
                            -1);

                    if (matchingMsg == null)
                    {
                        return new ProducerResponse(String.Format("Sorry, I don't remember what {0} said about \"{1}.\"", rememberTarget, rememberMsg), false);
                    }

                    quotesDataStore.Put(matchingMsg.Who, matchingMsg.Text);
                    return
                        new ProducerResponse(
                            String.Format("Okay, {0}, remembering \"{1}.\"",
                                          message.Who, matchingMsg.Text), false);
                }
                else
                {
                    // TODO: add the snide comment gambot makes when someone FUCKS UP
                }
            }

            return null;
        }
    }
}
