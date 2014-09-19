using Gambot.Core;

namespace Gambot
{
    public class TestMessageHandler : IMessageHandler
    {
        public void Initialize()
        {
            
        }

        public bool Digest(IMessenger messenger, IMessage message)
        {
            if (message.Text == ":hi:")
            {
                messenger.SendMessage(message.Who + ": :hi:", message.Where);
                return false;
            }

            return true;
        }
    }
}
