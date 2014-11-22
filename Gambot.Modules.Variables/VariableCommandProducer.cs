﻿using System;
using System.Text.RegularExpressions;
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
            return variableStore.GetRandomValue(variable);
        }

        public ProducerResponse Process(IMessage message)
        {
            var match = Regex.Match(message.Text, @"create var .+",
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
                                @"add value ([a-z][a-z0-9_-]*) (.+)",
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
                                @"remove value ([a-z][a-z0-9_-]*) (.+)",
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

            match = Regex.Match(message.Text,
                                @"delete var ([a-z][a-z0-9_-]*)",
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

            return null;
        }
    }
}
