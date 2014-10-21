using System;
using System.Text.RegularExpressions;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Factoid
{
    internal class FactoidCommandHandler : IMessageHandler
    {
        public HandlerPriority Priority
        {
            get { return HandlerPriority.Normal; }
        }

        private IDataStore dataStore;

        public void Initialize(IDataStoreManager dataStoreManager)
        {
            dataStore = dataStoreManager.Get("Factoid");
        }

        public string Process(string currentResponse, IMessage message,
                              bool addressed)
        {
            if (addressed)
            {
                var match = Regex.Match(message.Text, @"(.+) (is|are|<[^>]+>) (.+)",
                                        RegexOptions.IgnoreCase);
                if (match.Success) 
                {
                    var term = match.Groups[1].Value;
                    var verb = match.Groups[2].Value;
                    var response = match.Groups[3].Value;

                    if (!verb.StartsWith("<"))
                    {
                        verb = String.Format("<{0}>", verb);
                    }

                    return String.Format(dataStore.Put(term, verb + " " + response) ? "Okay, {0}." : "{0}: I already had it that way!", message.Who);
                }
            }

            return currentResponse;
        }
    }
}
