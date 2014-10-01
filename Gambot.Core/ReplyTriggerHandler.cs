namespace Gambot.Core
{
    public class ReplyTriggerHandler : IMessageHandler
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
            
            messenger.SendMessage(Variables.Substitute(randomReply, message), message.Where);

            return false;
        }
    }
}
