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
        private readonly IVariableHandler variableHandler;
        private IDataStore quotesDataStore;

        public QuoteCommandHandler(IVariableHandler variableHandler)
        {
            this.variableHandler = variableHandler;
        }

        public void Initialize(IDataStoreManager dataStoreManager)
        {
            quotesDataStore = dataStoreManager.Get("Quotes");

            variableHandler.DefineMagicVariable("quote", msg => GetRandomQuoteFromAnyUser());
        }

        public bool Digest(IMessenger messenger, IMessage message, bool addressed)
        {
            if (addressed) {
                var match = Regex.Match(message.Text, @"quote (\w+)", RegexOptions.IgnoreCase);
                if (match.Success) {
                    var quoteTarget = match.Groups[1].Value.Trim();
                    var replyMsg = GetRandomQuoteFromUser(quoteTarget);

                    messenger.SendMessage(replyMsg, message.Where);

                    return false;
                }
                else {
                    // TODO: add the snide comment gambot makes when someone FUCKS UP
                }
            }

            return true;
        }

        private string GetRandomQuoteFromAnyUser()
        {
            var randomUser = quotesDataStore.GetRandomKey();
            if (String.IsNullOrWhiteSpace(randomUser)) {
                return String.Format("Sorry, no one has said anything quote-worthy.");
            }

            return GetRandomQuoteFromUser(randomUser);
        }

        private string GetRandomQuoteFromUser(string username)
        {
            var randomQuote = quotesDataStore.GetRandomValue(username);
            if (randomQuote == null) {
                return String.Format("Sorry, {0} has not said anything quote-worthy.", username);
            }
            else {
                return String.Format("<{0}> {1}", username, randomQuote);
            }
        }
    }
}
