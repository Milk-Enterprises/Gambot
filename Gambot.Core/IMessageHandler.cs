using Gambot.Data;

namespace Gambot.Core
{
    public interface IMessageHandler
    {
        void Initialize(IDataStoreManager dataStoreManager);
        bool Digest(IMessenger messenger, IMessage message, bool addressed);
    }
}
