using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Reply
{
    internal class ReplyTriggerHandler : IMessageHandler
    {
        public HandlerPriority Priority { get { return HandlerPriority.Normal; } }

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

        public string Process(string currentResponse, IMessage message, bool addressed)
        {
            var randomReply = dataStore.GetRandomValue(message.Text);
            return randomReply == null ? currentResponse : variableHandler.Substitute(randomReply, message);
        }
    }
}
