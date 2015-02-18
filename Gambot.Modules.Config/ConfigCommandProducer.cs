using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Config
{
    internal class ConfigCommandProducer : IMessageProducer
    {
        public void Initialize(IDataStoreManager dataStoreManager) {}

        public ProducerResponse Process(IMessage message, bool addressed)
        {
            if (addressed)
            {
                var match = Regex.Match(message.Text, @"^(set|get) config (\w+)( .+)?$",
                                        RegexOptions.IgnoreCase);
                
                if (match.Success)
                {
                    var action = match.Groups[1].Value;
                    var key = match.Groups[2].Value;

                    if (action == "set")
                    {
                        if (!UserCanUpdateConfig(message.Who))
                        {
                            return new ProducerResponse("Sorry {0}, you do not have permissions to update the config.", false);
                        }

                        var value = match.Groups[3].Value.TrimStart();
                        Core.Config.Set(key, value);

                        return new ProducerResponse(String.Format("Okay {0}, changed the value of \"{1}\" to \"{2}.\"", message.Who, key, value), false);
                    }
                    else
                    {
                        var currentValue = Core.Config.Get(key);
                        return new ProducerResponse(String.Format("The current value for \"{0}\" is {1}.", key, currentValue != null ? "\"" + currentValue + "\"" : "<null>"), false);
                    }
                }
            }

            return null;
        }

        private bool UserCanUpdateConfig(string username)
        {
            return GetUsersWhoCanChangeConfig().Contains(username, StringComparer.InvariantCultureIgnoreCase);
        }

        private IEnumerable<string> GetUsersWhoCanChangeConfig()
        {
            return new[] { "rob" }; // todo: change to config.getlist once its in
        }
    }
}
