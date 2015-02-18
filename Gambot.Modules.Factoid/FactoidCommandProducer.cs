using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Factoid
{
    internal class FactoidCommandProducer : IMessageProducer
    {
        private IDataStore dataStore;

        public void Initialize(IDataStoreManager dataStoreManager)
        {
            dataStore = dataStoreManager.Get("Factoids");
        }

        public ProducerResponse Process(IMessage message, bool addressed)
        {
            if (addressed)
            {
                var match = Regex.Match(message.Text, @"^(.+) (<[^>]+>) (.+)$",
                                        RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var term = match.Groups[1].Value;
                    var verb = match.Groups[2].Value;
                    var response = match.Groups[3].Value;

                    if (verb == "alias" && term == response)
                    {
                        return
                            new ProducerResponse(
                                String.Format(
                                    "Sorry {0}, but you can't alias {1} to itself.",
                                    message.Who, term), false);
                    }

                    return new ProducerResponse(String.Format(dataStore.Put(term, verb + " " + response) ? "Okay, {0}." : "{0}: I already had it that way!", message.Who), false);
                }

                match = Regex.Match(message.Text, @"^literal (.+)$");
                if (match.Success)
                {
                    var term = match.Groups[1].Value;
                    var values = dataStore.GetAllValues(term);
                    if (!values.Any())
                        return new ProducerResponse(String.Format("Sorry, {0}, but I don't know about \"{1}\"", message.Who, term), false);
                    var result = String.Format("{0}: {1}", term, String.Join(", ", values));
                    if (result.Length < 500)
                        return new ProducerResponse(result, false);
                    var safeTerm = String.Join("", term.Where(c => !Path.GetInvalidFileNameChars().Contains(c)));
                    if (String.IsNullOrWhiteSpace(safeTerm))
                        safeTerm = "_";
                    File.WriteAllLines(Path.Combine(Config.Get("FactoidDumpDir", "dump/factoid"), safeTerm + ".txt"), values.ToArray());
                    return new ProducerResponse(String.Format("{0}: {1}{2}.txt", term, 
                        Config.Get("FactoidDumpUrl", "https://aorist.co/gambot/factoid/"), Uri.EscapeUriString(safeTerm)), false);
                }

                match = Regex.Match(message.Text, @"^what was that\??$");
                if (match.Success) 
                {
                    if (String.IsNullOrWhiteSpace(FactoidTriggerReactor.LastFactoid))
                        return new ProducerResponse("¯\\_(ツ)_/¯", false);
                    return new ProducerResponse(String.Format("{0}: that was \"{1}\"", message.Who, FactoidTriggerReactor.LastFactoid), false);
                }
            }

            return null;
        }
    }
}