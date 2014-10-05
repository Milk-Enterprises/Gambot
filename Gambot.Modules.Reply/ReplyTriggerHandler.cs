using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Reply
{
    internal class ReplyTriggerHandler : IMessageHandler
    {
        private IDataStore dataStore;

        public void Initialize(IDataStoreManager dataStoreManager)
        {
            dataStore = dataStoreManager.Get("Reply");
        }

        public bool Digest(IMessenger messenger, IMessage message, bool addressed)
        {
            var randomReply = dataStore.GetRandomValue(message.Text);
            if (randomReply == null) return true;
            
            messenger.SendMessage(VariableHandler.Substitute(randomReply, message), message.Where);

            return false;
        }
    }
}
