using System.Text.RegularExpressions;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Simple
{
    internal class SimpleResponseHandler : IMessageHandler
    {
        private readonly IVariableHandler variableHandler;

        internal SimpleResponseHandler(IVariableHandler variableHandler)
        {
            this.variableHandler = variableHandler;
        }

        public void Initialize(IDataStoreManager dataStoreManager) { }

        public string Process(string currentResponse, IMessage message, bool addressed)
        {
            Match match;
            if (addressed) {
                match = Regex.Match(message.Text, "say \"(.+)\"");
                if (match.Success) {
                    return variableHandler.Substitute(match.Groups[1].Value, message);
                }
            }

            match = Regex.Match(message.Text, @"say (\S)([^.?!]+)[.?!]*$");
            if (match.Success) {
                return match.Groups[1].Value.ToUpper() + match.Groups[2].Value + "!";
            }

            return currentResponse;
        }
    }
}
