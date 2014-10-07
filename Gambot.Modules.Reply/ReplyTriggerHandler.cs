using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Reply
{
    internal class ReplyTriggerHandler : IMessageHandler
    {
        private readonly IVariableHandler variableHandler;
        private IDataStore dataStore;

        internal ReplyTriggerHandler(IVariableHandler variableHandler)
        {
            this.variableHandler = variableHandler;
        }

        public void Initialize(IDataStoreManager dataStoreManager)
        {
            dataStore = dataStoreManager.Get("Reply");
        }

        public bool Digest(IMessenger messenger, IMessage message, bool addressed)
        {
            var randomReply = dataStore.GetRandomValue(message.Text);
            if (randomReply == null) return true;
            
            messenger.SendMessage(variableHandler.Substitute(randomReply, message), message.Where);

            return false;
        }
    }
}
