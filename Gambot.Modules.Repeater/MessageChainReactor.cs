using System;
using Gambot.Core;
using Gambot.Data;
using MiscUtil;

namespace Gambot.Modules.Repeater
{
    internal class MessageChainReactor : IMessageReactor
    {
        private readonly IMessageChainStore chainStore;

        public MessageChainReactor(IMessageChainStore chainStore)
        {
            this.chainStore = chainStore;
        }

        public void Initialize(IDataStoreManager dataStoreManager)
        {}

        public ProducerResponse Process(IMessage message, bool addressed)
        {
            chainStore.AddMessage(message.Where, message.Text);
            var currentChain = chainStore.GetCurrentChain(message.Where);

            if (ShouldParticipateInChain(currentChain))
            {
                chainStore.ResetChain(currentChain);
                return new ProducerResponse(message.Text, message.Action);
            }
            
            return null;
        }

        private bool ShouldParticipateInChain(MessageChainData currentChain)
        {
            var baseChance = Double.Parse(Config.Get("MessageChainBaseChance", "0.125"));
            var multiplier = Double.Parse(Config.Get("MessageChainChanceMultiplier", "2"));
            var chanceOfParticipating = 0.0;

            if (currentChain.Length > 1)
            {
                chanceOfParticipating = baseChance * Math.Pow(multiplier, currentChain.Length-2);
            }

            return StaticRandom.NextDouble() < chanceOfParticipating;
        }
    }
}
