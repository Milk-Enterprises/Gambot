using System;
using System.Text.RegularExpressions;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Reply
{
    internal class ReplyCommandHandler : IMessageHandler
    {
        private IDataStore dataStore;
        
        public void Initialize(IDataStoreManager dataStoreManager)
        {
            dataStore = dataStoreManager.Get("Reply");
        }

        public string Process(string currentResponse, IMessage message, bool addressed)
        {
            if (addressed) {
                var match = Regex.Match(message.Text, @"(.+)\s\<reply\>\s(.+)", RegexOptions.IgnoreCase);
                if (match.Success) {
                    var replyTrigger = match.Groups[1].Value.Trim();
                    var replyMsg = match.Groups[2].Value.Trim();

                    dataStore.Put(replyTrigger, replyMsg);

                    return String.Format("Okay, {0}.", message.Who);
                }
                else {
                    // TODO: add the snide comment gambot makes when someone FUCKS UP
                }
            }

            return currentResponse;
        }
    }
}
