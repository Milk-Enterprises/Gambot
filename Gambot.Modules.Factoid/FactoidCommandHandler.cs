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
                var match = Regex.Match(message.Text, @"(.+?)\s(.+?)\s(.+)",
                                        RegexOptions.IgnoreCase);
                if (match.Success) 
                {
                    var term = match.Groups[1].Value;
                    var verb = match.Groups[2].Value;
                    var response = match.Groups[3].Value;

                    // some verbs will be malformed and need to be coaxed into the right form
                    switch (verb)
                    {
                        case "<is>":
                        case "is":
                        {
                            // need to check if the response has an <is>, which means that the term needs to be updated
                            var refinedMatch = Regex.Match(message.Text,
                                                           @"(.+)\s\<is\>\s(.+)",
                                                           RegexOptions
                                                               .IgnoreCase);
                            if (refinedMatch.Success)
                            {
                                term = refinedMatch.Groups[1].Value;
                                response = refinedMatch.Groups[2].Value;
                            }

                            verb = "<is>";
                            
                            break;
                        }
                        case "<are>":
                        case "are":
                            verb = "<are>";
                            break;
                        case "<reply>":
                            break;
                        case "<action>":
                            break;
                        default:
                            return currentResponse;
                    }

                    return String.Format(dataStore.Put(term, verb + " " + response) ? "Okay, {0}." : "{0}: I already had it that way!", message.Who);
                }
            }

            return currentResponse;
        }
    }
}
