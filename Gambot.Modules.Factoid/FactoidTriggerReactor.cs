﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Factoid
{
    internal class FactoidTriggerReactor : IMessageReactor
    {
        private readonly IVariableHandler variableHandler;
        private IDataStore dataStore;

        // :smug:
        internal static string LastFactoid = "";

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
            if (!addressed &&
                  message.Text.Length < int.Parse(Config.Get("FactoidTriggerLength", "6")))
                return null;

            return ProcessFactoid(message.Text, message);
        }

        private ProducerResponse ProcessFactoid(string messageText, IMessage message)
        {
            var seenAliases = new HashSet<string>();
            while (true)
            {
                var randomReply = dataStore.GetRandomValue(messageText);
                if (randomReply == null)
                    return null;

                var factoid = FactoidUtilities.GetVerbAndResponseFromPartialFactoid(randomReply.Value);
                factoid.Trigger = randomReply.Key;

                var factoidResponse = variableHandler.Substitute(factoid.Response, message);

                LastFactoid = String.Format("(#{3}) {0} <{1}> {2}", factoid.Trigger, factoid.Verb, factoid.Response, randomReply.Id);

                if (IsAddressedToBot(message.To ?? message.Who, factoid.Response) && ShouldFuckShitUp())
                {
                    var factoidResponseBytes =
                        Encoding.UTF8.GetBytes(factoidResponse);
                    factoidResponse =
                        Encoding.UTF7.GetString(factoidResponseBytes);
                }

                switch (factoid.Verb)
                {
                    case "reply":
                        return new ProducerResponse(factoidResponse, false);
                    case "action":
                        return new ProducerResponse(factoidResponse, true);
                    case "alias":
                        if(!seenAliases.Add(messageText))
                            return new ProducerResponse(String.Format("Sorry {0}, but this factoid resolves to a circular reference.", message.Who), false);
                        messageText = factoid.Response;
                        continue;
                    default:
                        return new ProducerResponse(String.Format("{0} {1} {2}", message.Text, factoid.Verb, factoid.Response), false);
                }
            }
        }

        private bool ShouldFuckShitUp()
        {
            return Config.GetBool("ShouldMangleSelfMessages");
        }

        private bool IsAddressedToBot(string target, string responseMessage)
        {
            var botName = Config.Get("Name", "gambot");
            return Regex.Match(responseMessage, @"^\$to[:,]").Success && String.Equals(target, botName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
