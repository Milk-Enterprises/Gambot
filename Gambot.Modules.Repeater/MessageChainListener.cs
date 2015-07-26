using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Repeater
{
    internal class MessageChainListener : IMessageListener
    {
        private readonly IMessageChainStore chainStore;

        public MessageChainListener(IMessageChainStore chainStore)
        {
            this.chainStore = chainStore;
        }

        public void Initialize(IDataStoreManager dataStoreManager) { }

        public void Listen(IMessage message, bool addressed)
        {
            chainStore.AddMessage(message.Where, message.Text);
        }
    }
}
