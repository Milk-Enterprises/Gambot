namespace Gambot.Core
{
    public class ReplyTriggerHandler : IMessageHandler
    {
        private IDataStore _dataStore;

        public void Initialize(IDataStoreManager dataStoreManager)
        {
            _dataStore = dataStoreManager.Get("Reply");
        }

        public bool Digest(IMessenger messenger, IMessage message, bool addressed)
        {
            var randomReply = _dataStore.GetRandomValue(message.Text);
            if (randomReply == null) return true;
            
            messenger.SendMessage(Variables.Substitute(randomReply), message.Where);

            return false;
        }
    }
}
