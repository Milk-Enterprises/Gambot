using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gambot.Core;

namespace Gambot.SimpleResponses
{
    public class SimpleResponseHandler : IMessageHandler
    {
        public void Initialize(IDataStore dataStore) { }

        public bool Digest(IMessenger messenger, IMessage message, bool addressed)
        {
            Match match;
            if (addressed)
            {
                match = Regex.Match(message.Text, "say \"(.+)\"");
                if (match.Success)
                {
                    messenger.SendMessage(Variables.Substitute(match.Groups[1].Value), message.Where);
                    return false;
                }
            }

            match = Regex.Match(message.Text, @"say (\S)([^.?!]+)[.?!]*$");
            if (match.Success)
            {
                messenger.SendMessage(match.Groups[1].Value.ToUpper() + match.Groups[2].Value + "!",
                    message.Where);
                return false;
            }

            return true;
        }
    }
}
