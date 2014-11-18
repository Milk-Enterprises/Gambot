using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Quotes
{
    internal class RecentMessageListener : IMessageListener
    {
        private readonly IRecentMessageStore recentMessageStore;

        public RecentMessageListener(IRecentMessageStore recentMessageStore)
        {
            this.recentMessageStore = recentMessageStore;
        }

        public void Initialize(IDataStoreManager dataStoreManager) { }

        public void Listen(IMessage message, bool addressed)
        {
            recentMessageStore.AddMessageFromUser(message.Who, message);
        }
    }
}
