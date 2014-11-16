using System;
using Gambot.Core;
using Gambot.Data;
using NLog;

namespace Gambot.Modules.Factoid
{
    internal class FactoidTriggerHandler : IMessageProducer
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IVariableHandler variableHandler;
        private IDataStore dataStore;

        internal FactoidTriggerHandler(IVariableHandler variableHandler)
        {
            this.variableHandler = variableHandler;
        }

        public void Initialize(IDataStoreManager dataStoreManager)
        {
            dataStore = dataStoreManager.Get("Factoid");
        }

        public string Process(string currentResponse, IMessage message,
                              bool addressed)
        {
            if (!(addressed ||
                message.Text.Length >
                int.Parse(Config.Get("FactoidTriggerLength"))))
                return currentResponse;

            var randomReply = dataStore.GetRandomValue(message.Text);
            if (randomReply == null)
                return currentResponse;

            var factoid =
                FactoidUtilities.GetVerbAndResponseFromPartialFactoid(
                    randomReply);
            factoid.Trigger = message.Text;

            var factoidResponse = variableHandler.Substitute(factoid.Response,
                                                             message);

            switch (factoid.Verb)
            {
                case "reply":
                    return factoidResponse;
                case "action":
                    return "/me " + factoidResponse;
                default:
                    return String.Format("{0} {1} {2}", message.Text,
                                         factoid.Verb, factoid.Response);
            }
        }
    }
}
