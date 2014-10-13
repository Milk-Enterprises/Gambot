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
        public HandlerPriority Priority { get { return HandlerPriority.Normal; } }

        private readonly IRecentMessageStore recentMessageStore;

        public RecentMessageListener(IRecentMessageStore recentMessageStore)
        {
            this.recentMessageStore = recentMessageStore;
        }

        public void Initialize(IDataStoreManager dataStoreManager)
        {}

        public string Process(string currentResponse, IMessage message, bool addressed)
        {
            recentMessageStore.AddMessageFromUser(message.Who, message);

            return currentResponse;
        }
    }
}
