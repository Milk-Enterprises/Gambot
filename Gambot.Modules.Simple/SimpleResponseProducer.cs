using System.Text.RegularExpressions;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Simple
{
    internal class SimpleResponseProducer : IMessageProducer
    {
        private readonly IVariableHandler variableHandler;

        internal SimpleResponseProducer(IVariableHandler variableHandler)
        {
            this.variableHandler = variableHandler;
        }

        public void Initialize(IDataStoreManager dataStoreManager) { }

        public ProducerResponse Process(IMessage message, bool addressed)
        {
            Match match;
            if (addressed)
            {
                match = Regex.Match(message.Text, "say \"(.+)\"");
                if (match.Success)
                {
                    return
                        new ProducerResponse(
                            variableHandler.Substitute(match.Groups[1].Value,
                                                       message), false);
                }
            }

            match = Regex.Match(message.Text, @"^say (\S)([^.?!]+)[.?!]*$");
            if (match.Success)
            {
                return
                    new ProducerResponse(
                        match.Groups[1].Value.ToUpper() + match.Groups[2].Value +
                        "!", false);
            }

            return null;
        }
    }
}
