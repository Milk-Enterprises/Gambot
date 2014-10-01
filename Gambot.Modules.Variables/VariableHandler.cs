﻿using System;
using System.Text.RegularExpressions;

namespace Gambot.Modules.Variables
{
    public class VariableHandler : IMessageHandler, IVariableFallbackHandler
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

        public bool Digest(IMessenger messenger, IMessage message, bool addressed)
        {
            if (addressed)
            {
                var match = Regex.Match(message.Text, @"create var .+", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    messenger.SendMessage(
                        String.Format((string) "{0}: To create a variable, just start adding values to it.", (object) message.Who),
                        message.Where);
                    return false;
                }

                match = Regex.Match(message.Text, @"add value ([a-z][a-z0-9_-]*) (.+)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    messenger.SendMessage(
                        String.Format(variableStore.Put(match.Groups[1].Value.ToLower(), match.Groups[2].Value)
                            ? "Okay, {0}."
                            : "I already had it that way, {0}!", (object) message.Who), message.Where);
                    return false;
                }

                match = Regex.Match(message.Text, @"remove value ([a-z][a-z0-9_-]*) (.+)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    messenger.SendMessage(
                        String.Format(variableStore.RemoveValue(match.Groups[1].Value.ToLower(), match.Groups[2].Value)
                            ? "Okay, {0}."
                            : "There's no such value, {0}!", (object) message.Who), message.Where);
                    return false;
                }

                match = Regex.Match(message.Text, @"delete var ([a-z][a-z0-9_-]*)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var values = variableStore.RemoveAllValues(match.Groups[1].Value.ToLower());
                    messenger.SendMessage(String.Format(values == 0 ? "That variable doesn't exist, {0}!" : 
                                                        values == 1 ? "Removed {1} and its value, {0}."  : 
                                                        "Removed {1} and its {2} values, {0}.", message.Who, match.Groups[1].Value, values),
                                                        message.Where);
                    return false;
                }
            }

            return true;
        }
    }
}