using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Gambot.Core;
using Gambot.Data;
using MiscUtil;

namespace Gambot.Modules.Quotes
{
    internal class QuoteCommandHandler : IMessageProducer
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

            variableHandler.DefineMagicVariable("quote",
                                                msg =>
                                                GetRandomQuoteFromAnyUser());
        }

        public string Process(string currentResponse, IMessage message,
                              bool addressed)
        {
            if (addressed)
            {
                var match = Regex.Match(message.Text, @"quote (\w+)",
                                        RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var quoteTarget = match.Groups[1].Value.Trim();
                    return GetRandomQuoteFromUser(quoteTarget);
                }
                else
                {
                    // TODO: add the snide comment gambot makes when someone FUCKS UP
                }
            }

            return currentResponse;
        }

        private string GetRandomQuoteFromAnyUser()
        {
            var allUsers = quotesDataStore.GetAllKeys().ToList();
            if (!allUsers.Any())
            {
                return
                    String.Format(
                        "Sorry, no one has said anything quote-worthy.");
            }

            var randomIdx = StaticRandom.Next(0, allUsers.Count);

            return GetRandomQuoteFromUser(allUsers[randomIdx]);
        }

        private string GetRandomQuoteFromUser(string username)
        {
            var randomQuote = quotesDataStore.GetRandomValue(username);
            if (randomQuote == null)
            {
                return
                    String.Format(
                        "Sorry, {0} has not said anything quote-worthy.",
                        username);
            }
            else
                return String.Format("<{0}> {1}", username, randomQuote);
        }
    }
}
