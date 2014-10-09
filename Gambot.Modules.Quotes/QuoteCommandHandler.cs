using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Quotes
{
    internal class QuoteCommandHandler : IMessageHandler
    {
        private IDataStore quotesDataStore;

        public void Initialize(IDataStoreManager dataStoreManager)
        {
            quotesDataStore = dataStoreManager.Get("Quotes");
        }

        public bool Digest(IMessenger messenger, IMessage message, bool addressed)
        {
            if (addressed)
            {
                var match = Regex.Match(message.Text, @"quote (\w+)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var quoteTarget = match.Groups[1].Value.Trim();
                    var randomQuote = quotesDataStore.GetRandomValue(quoteTarget);

                    string replyMsg;
                    if (randomQuote == null) {
                        replyMsg = String.Format("Sorry, {0} has not said anything quote-worthy.", quoteTarget);
                    }
                    else {
                        replyMsg = String.Format("<{0}> {1}", quoteTarget, randomQuote);
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
