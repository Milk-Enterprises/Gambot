﻿using System;
using System.Text.RegularExpressions;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Config
{
    internal class ConfigCommandHandler : IMessageProducer
    {
        public void Initialize(IDataStoreManager dataStoreManager) {}

        public string Process(string currentResponse, IMessage message, bool addressed)
        {
            if (addressed)
            {
                var match = Regex.Match(message.Text, @"(set|get) (\w+)( .+)?",
                                        RegexOptions.IgnoreCase);

                if (match.Success)
                {
                    var action = match.Groups[1].Value;
                    var key = match.Groups[2].Value;

                    if (action == "set")
                    {
                        var value = match.Groups[3].Value.TrimStart();
                        Core.Config.Set(key, value);

                        return
                            String.Format(
                                "Okay {0}, changed the value of \"{1}\" to \"{2}.\"",
                                message.Who, key, value);
                    }
                    else
                    {
                        var currentValue = Core.Config.Get(key);
                        return String.Format(
                            "The current value for \"{0}\" is {1}.", key,
                            currentValue != null
                                ? "\"" + currentValue + "\""
                                : "<null>");
                    }
                }
            }

            return currentResponse;
        }
    }
}
