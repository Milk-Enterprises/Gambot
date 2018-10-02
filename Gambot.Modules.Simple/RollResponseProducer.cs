using System.Text.RegularExpressions;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Roll
{
    internal class RollResponseProducer : IMessageProducer
    {
        internal RollResponseProducer() { }

        public void Initialize(IDataStoreManager dataStoreManager) { }

        public ProducerResponse Process(IMessage message, bool addressed)
        {
            Match match;
            if (addressed)
            {
                match = Regex.Match(message.Text, "roll (.+)");
                if (match.Success)
                {
                    return
                        new ProducerResponse("", false);
                }
            }
            return null;
        }
    }
}
