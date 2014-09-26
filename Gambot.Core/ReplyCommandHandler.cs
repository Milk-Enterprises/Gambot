using System;
using System.Text.RegularExpressions;

namespace Gambot.Core
{
    public class ReplyCommandHandler : IMessageHandler
    {
        private IDataStore _dataStore;
        
        public void Initialize(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public bool Digest(IMessenger messenger, IMessage message, bool addressed)
        {
            if (addressed) {
                var match = Regex.Match(message.Text, @"(.+)\s\<reply\>\s(.+)", RegexOptions.IgnoreCase);
                if (match.Success) {
                    var replyTrigger = match.Groups[1].Value.Trim();
                    var replyMsg = match.Groups[2].Value.Trim();

                    _dataStore.Put(replyTrigger, replyMsg);

                    messenger.SendMessage(String.Format("Okay, {0}.", message.Who), message.Where);

                    return false;
                }
                else {
                    // TODO: add the snide comment gambot makes when someone FUCKS UP
                }
            }

            return true;
        }
    }
}
