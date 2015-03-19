using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Variables
{
    internal class VariableCommandProducer : IMessageProducer, IVariableFallbackHandler
    {
        protected IDataStore variableStore;

        public void Initialize(IDataStoreManager dataStoreManager)
        {
            variableStore = dataStoreManager.Get("Variables");
        }

        public string Fallback(string variable, IMessage context)
        {
            return variableStore.GetRandomValue(variable).Value;
        }

        public ProducerResponse Process(IMessage message, bool addressed)
        {
            if (addressed)
            {
                var match = Regex.Match(message.Text, @"^create var .+$",
                                        RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    return
                        new ProducerResponse(
                            String.Format(
                                "{0}: To create a variable, just start adding values to it.",
                                message.Who), false);
                }

                match = Regex.Match(message.Text,
                                    @"^add value ([a-z][a-z0-9_-]*) (.+)$",
                                    RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    return
                        new ProducerResponse(
                            String.Format(
                                variableStore.Put(
                                    match.Groups[1].Value.ToLower(),
                                    match.Groups[2].Value)
                                    ? "Okay, {0}."
                                    : "I already had it that way, {0}!",
                                message.Who), false);
                }

                match = Regex.Match(message.Text,
                                    @"^remove value ([a-z][a-z0-9_-]*) (.+)$",
                                    RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    return
                        new ProducerResponse(
                            String.Format(
                                variableStore.RemoveValue(
                                    match.Groups[1].Value.ToLower(),
                                    match.Groups[2].Value)
                                    ? "Okay, {0}."
                                    : "{0}: There's no such value!", message.Who),
                            false);
                }

                match = Regex.Match(message.Text, @"^remove value #([0-9]+)$", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    return
                        new ProducerResponse(
                            String.Format(
                                variableStore.RemoveValue(Int32.Parse(match.Groups[1].Value))
                                ? "Okay, {0}."
                                : "{0}: There's no such value!", message.Who),
                            false);
                }

                match = Regex.Match(message.Text,
                                    @"^delete var ([a-z][a-z0-9_-]*)$",
                                    RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var values =
                        variableStore.RemoveAllValues(
                            match.Groups[1].Value.ToLower());
                    return
                        new ProducerResponse(
                            String.Format(
                                values == 0
                                    ? "{0}: That variable doesn't exist!"
                                    : values == 1
                                          ? "{0}: Deleted variable \"{1}\" and its value."
                                          : "{0}: Deleted variable \"{1}\" and its {2} values.",
                                message.Who, match.Groups[1].Value, values),
                            false);
                }

                match = Regex.Match(message.Text, @"^list var ([a-z][a-z0-9_-]*)$", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var term = match.Groups[1].Value;
                    var values = variableStore.GetAllValues(term);
                    if (!values.Any())
                        return new ProducerResponse(String.Format("Sorry, {0}, but the variable \"{1}\" does not exist.", message.Who, term), false);
                    var result = String.Format("${0}: {1}", term, String.Join(", ", values));
                    if (result.Length < 500)
                        return new ProducerResponse(result, false);
                    var safeTerm = String.Join("", term.Where(c => !Path.GetInvalidFileNameChars().Contains(c)));
                    if (String.IsNullOrWhiteSpace(safeTerm))
                        safeTerm = "_";
                    File.WriteAllLines(Path.Combine(Config.Get("VariableDumpDir", "dump/variable"), safeTerm + ".txt"), 
                        values.Select(dsv => String.Format("(#{0}) {1}", dsv.Id, dsv.Value)).ToArray());
                    return new ProducerResponse(String.Format("{0}: {1}{2}.txt", term, 
                        Config.Get("VariableDumpUrl", "https://aorist.co/gambot/variable/"), Uri.EscapeUriString(safeTerm)), false);
                }
            }

            return null;
        }
    }
}
