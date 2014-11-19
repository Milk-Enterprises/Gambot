using System;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Factoid
{
    internal class FactoidTriggerReactor : IMessageReactor
    {
        private readonly IVariableHandler variableHandler;
        private IDataStore dataStore;

        internal FactoidTriggerReactor(IVariableHandler variableHandler)
        {
            this.variableHandler = variableHandler;
        }

        public void Initialize(IDataStoreManager dataStoreManager)
        {
            dataStore = dataStoreManager.Get("Factoids");
        }

        public ProducerResponse Process(IMessage message, bool addressed)
        {
            if (!(addressed ||
                  message.Text.Length >
                  int.Parse(Config.Get("FactoidTriggerLength"))))
                return null;

            var randomReply = dataStore.GetRandomValue(message.Text);
            if (randomReply == null)
                return null;

            var factoid =
                FactoidUtilities.GetVerbAndResponseFromPartialFactoid(
                    randomReply);
            factoid.Trigger = message.Text;

            var factoidResponse = variableHandler.Substitute(factoid.Response,
                                                             message);

            switch (factoid.Verb)
            {
                case "reply":
                    return new ProducerResponse(factoidResponse, false);
                case "action":
                    return new ProducerResponse(factoidResponse, true);
                default:
                    return
                        new ProducerResponse(String.Format("{0} {1} {2}",
                                                           message.Text,
                                                           factoid.Verb,
                                                           factoid.Response),
                                             false);
            }
        }
    }
}
