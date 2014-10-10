using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Quotes
{
    internal class RecentMessageListener : IMessageHandler
    {
        private readonly IRecentMessageStore recentMessageStore;

        public RecentMessageListener(IRecentMessageStore recentMessageStore)
        {
            this.recentMessageStore = recentMessageStore;
        }

        public void Initialize(IDataStoreManager dataStoreManager)
        {}

        public bool Digest(IMessenger messenger, IMessage message, bool addressed)
        {
            recentMessageStore.AddMessageFromUser(message.Who, message);

            return true;
        }
    }
}
