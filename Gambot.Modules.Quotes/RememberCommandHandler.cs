using System;
using System.Linq;
using System.Text.RegularExpressions;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Quotes
{
    internal class RememberCommandHandler : IMessageHandler
    {
        private IDataStore quotesDataStore;
        private readonly IRecentMessageStore recentMessageStore;

        public RememberCommandHandler(IRecentMessageStore recentMessageStore)
        {
            this.recentMessageStore = recentMessageStore;
        }

        public void Initialize(IDataStoreManager dataStoreManager)
        {
            quotesDataStore = dataStoreManager.Get("Quotes");
        }

        public bool Digest(IMessenger messenger, IMessage message, bool addressed)
        {
            if (addressed)
            {
                var match = Regex.Match(message.Text, @"remember (\w+) (.+)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var rememberTarget = match.Groups[1].Value.Trim();
                    var rememberMsg = match.Groups[2].Value.Trim();
                    string replyMsg;

                    // cant let the user remember his own quotes
                    if (rememberTarget.Equals(message.Who, StringComparison.InvariantCultureIgnoreCase)) {
                        replyMsg = String.Format("Sorry {0}, but you can't quote yourself.", message.Who);
                    }
                    else {
                        // check if target said such a thing
                        var usersRecentMessages = recentMessageStore.GetRecentMessagesFromUser(rememberTarget);

                        if (usersRecentMessages == null) {
                            replyMsg = String.Format("Sorry, I don't know anyone named \"{0}.\"", rememberTarget);
                        }
                        else {
                            var matchingMsg =
                                usersRecentMessages.FirstOrDefault(
                                    msg => msg.Text.IndexOf(rememberMsg, StringComparison.InvariantCultureIgnoreCase) != -1);

                            if (matchingMsg == null) {
                                replyMsg = String.Format("Sorry, I don't remember what {0} said about \"{1}.\"", rememberTarget, rememberMsg);
                            }
                            else {
                                replyMsg = String.Format("Okay, {0}, remembering \"{1}.\"", message.Who, matchingMsg.Text);

                                quotesDataStore.Put(matchingMsg.Who, matchingMsg.Text);
                            }
                        }
                    }

                    messenger.SendMessage(replyMsg, message.Where);

                    return false;
                }
                else
                {
                    // TODO: add the snide comment gambot makes when someone FUCKS UP
                }
            }

            return true;
        }
    }
}
