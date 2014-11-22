﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Conjugation
{
    internal class ConjugationProducer : IMessageProducer, IVariableFallbackHandler
    {
        protected Conjugator conjugator;
        protected IDataStore variableTypeStore;
        protected IDataStore variableStore;
        protected IDataStore irregularStore;

        public void Initialize(IDataStoreManager dataStoreManager)
        {
            variableTypeStore = dataStoreManager.Get("ConjugationTypes");
            variableStore = dataStoreManager.Get("Variables");
            irregularStore = dataStoreManager.Get("Irregulars");
            conjugator = new Conjugator(irregularStore);
        }

        public string Fallback(string variable, IMessage context)
        {
            if (variable.EndsWith("ed"))
            {
                foreach (var verb in variableTypeStore.GetAllValues("verb"))
                {
                    if (conjugator.Verbed(verb) == variable)
                    {
                        var sub = variableStore.GetRandomValue(verb);
                        return VariableHandler.MatchCase(sub, conjugator.Verbed(sub));
                    }
                }
            }
            else if (variable.EndsWith("ing"))
            {
                foreach (var verb in variableTypeStore.GetAllValues("verb"))
                {
                    if (conjugator.Verbing(verb) == variable)
                    {
                        var sub = variableStore.GetRandomValue(verb);
                        return VariableHandler.MatchCase(sub, conjugator.Verbing(sub));
                    }
                }
            }
            else if (variable.EndsWith("s"))
            {
                foreach (var noun in variableTypeStore.GetAllValues("noun"))
                {
                    if (conjugator.Nouns(noun) == variable)
                    {
                        var sub = variableStore.GetRandomValue(noun);
                        return VariableHandler.MatchCase(sub, conjugator.Nouns(sub));
                    }
                }

                foreach (var verb in variableTypeStore.GetAllValues("verb"))
                {
                    if (conjugator.Verbs(verb) == variable)
                    {
                        var sub = variableStore.GetRandomValue(verb);
                        return VariableHandler.MatchCase(sub, conjugator.Verbs(sub));
                    }
                }
            }

            return null;
        }

        public ProducerResponse Process(IMessage message)
        {
            var match = Regex.Match(message.Text, "var ([a-z0-9_-]+) type (var|noun|verb)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var var = match.Groups[1].Value.ToLower();
                var type = match.Groups[2].Value.ToLower();

                foreach (var t in new[] { "noun", "verb" })
                {
                    if (type == t)
                        variableTypeStore.Put(t, var);
                    else
                        variableTypeStore.RemoveValue(t, var);
                }

                return new ProducerResponse(String.Format("Okay, {0}.", message.Who), false);
            }

            match = Regex.Match(message.Text, "irregular (stem|participle|gerund|present|plural) (.+) => (.+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var type = match.Groups[1].Value.ToLower();
                var word = match.Groups[2].Value.ToLower();
                var conjugation = match.Groups[3].Value.ToLower();

                irregularStore.RemoveAllValues(word + "." + type);
                irregularStore.Put(word + "." + type, conjugation);

                return new ProducerResponse(String.Format("Okay, {0}.", message.Who), false);
            }

            return null;
        }
    }
}
