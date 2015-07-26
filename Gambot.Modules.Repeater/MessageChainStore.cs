using System;
using System.Collections.Generic;

namespace Gambot.Modules.Repeater
{
    internal class MessageChainData
    {
        public string Message { get; set; }
        public int Length { get; set; }
    }

    internal interface IMessageChainStore
    {
        void AddMessage(string channel, string message);
        MessageChainData GetCurrentChain(string channel);
    }

    internal class MessageChainStore : IMessageChainStore
    {
        // channel name -> chain
        private readonly IDictionary<string, MessageChainData> chains;

        public MessageChainStore()
        {
            chains = new Dictionary<string, MessageChainData>();
        }

        public void AddMessage(string channel, string message)
        {
            MessageChainData currentChain;

            if (!chains.TryGetValue(channel, out currentChain))
            {
                currentChain = new MessageChainData()
                {
                    Message = message,
                    Length = 0
                };

                chains[channel] = currentChain;
            }

            if (currentChain.Message.Equals(message, StringComparison.InvariantCultureIgnoreCase))
            {
                currentChain.Length++;
            }
            else
            {
                ResetChain(currentChain, message);
            }
        }

        public MessageChainData GetCurrentChain(string channel)
        {
            MessageChainData chain;
            chains.TryGetValue(channel, out chain);

            return chain;
        }

        private void ResetChain(MessageChainData chain, string newMessage)
        {
            chain.Message = newMessage;
            chain.Length = 0;
        }
    }
}
