using System;
using System.Collections.Generic;
using Gambot.Core;

namespace Gambot.Modules.Quotes
{
    internal interface IRecentMessageStore
    {
        int MaxMessagesStoredPerUser { get; }
        void AddMessageFromUser(string username, IMessage message);
        IEnumerable<IMessage> GetRecentMessagesFromUser(string username);
    }

    internal class RecentMessageStore : IRecentMessageStore
    {
        public int MaxMessagesStoredPerUser
        {
            get { return maxMessagesStoredPerUser; }
        }

        private readonly IDictionary<string, Queue<IMessage>> userToMessageQueueMap;
        private readonly int maxMessagesStoredPerUser;

        public RecentMessageStore(int maxMessagesStoredPerUser)
        {
            userToMessageQueueMap = new Dictionary<string, Queue<IMessage>>(StringComparer.InvariantCultureIgnoreCase);
            this.maxMessagesStoredPerUser = maxMessagesStoredPerUser;
        }

        public void AddMessageFromUser(string username, IMessage message)
        {
            if (!userToMessageQueueMap.ContainsKey(username)) {
                userToMessageQueueMap.Add(username, new Queue<IMessage>(MaxMessagesStoredPerUser));
            }

            // todo: the whole concurrency/thread-safety thing
            var usersMsgQueue = userToMessageQueueMap[username];
            while (usersMsgQueue.Count >= MaxMessagesStoredPerUser) {
                usersMsgQueue.Dequeue();
            }

            usersMsgQueue.Enqueue(message);
        }

        public IEnumerable<IMessage> GetRecentMessagesFromUser(string username)
        {
            return !userToMessageQueueMap.ContainsKey(username) ? null : userToMessageQueueMap[username];
        }
    }
}
