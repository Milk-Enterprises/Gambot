using System;
using System.Linq;
using System.Text.RegularExpressions;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Ignore
{
    internal class IgnoreCommandProducer : IMessageProducer
    {
        private const string IgnoredUsersKey = "IgnoredUsers";

        public void Initialize(IDataStoreManager dataStoreManager)
        {
        }

        public ProducerResponse Process(IMessage message)
        {
            var match = Regex.Match(message.Text, @"(unignore|ignore) (\w+)", RegexOptions.IgnoreCase);

            if (match.Success)
            {
                var command = match.Groups[1].Value;
                var username = match.Groups[2].Value;

                if (command == "ignore")
                {
                    var currentIgnoredUsers = Config.Get(IgnoredUsersKey,
                                                         String.Empty);
                    var ignoredUsersWithUsername = String.Format("{0}{1}{2}",
                                                                 currentIgnoredUsers,
                                                                 currentIgnoredUsers ==
                                                                 String.Empty
                                                                     ? String.Empty
                                                                     : ",",
                                                                 username);

                    Config.Set(IgnoredUsersKey, ignoredUsersWithUsername);

                    return new ProducerResponse(String.Format("Okay {0}, ignoring {1}.", message.Who, username), false);
                }
                else if(command == "unignore")
                {
                    var currentIgnoredUsers = Config.Get(IgnoredUsersKey,
                                                         String.Empty);
                    var ignoredUsers = currentIgnoredUsers.Split(new[]{','}, StringSplitOptions.RemoveEmptyEntries).ToList();
                    ignoredUsers.Remove(username);

                    var ignoredUsersWithUsername = String.Join(",", ignoredUsers);

                    Config.Set(IgnoredUsersKey, ignoredUsersWithUsername);

                    return new ProducerResponse(String.Format("Okay {0}, unignoring {1}.", message.Who, username), false);
                }
            }

            return null;
        }
    }
}
