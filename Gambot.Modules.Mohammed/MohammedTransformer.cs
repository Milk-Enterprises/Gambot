using System;
using MiscUtil;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Mohammed
{
    internal class MohammedTransformer : IMessageTransformer
    {
        public void Initialize(IDataStoreManager dataStoreManager)
        {
            // nothing to be done
        }

        public string Transform(bool isAction, string messageText, bool addressed)
        {
            char[] fuckupKeys = { 'a', 's', 'd', 'g', 'h', 'f', 'j', 'k', 'l' };

            if (StaticRandom.Next(100) < Int32.Parse(Config.Get("PercentChanceOfMohammed", "1")))
            {
                int index = messageText.IndexOfAny(fuckupKeys);

                if (index < 0)
                    return messageText;

                while (StaticRandom.Next(messageText.Length * 2) > index)
                {
                    int newIndex = messageText.IndexOfAny(fuckupKeys, index);
                    if (newIndex < 0)
                        break;
                    index = newIndex;
                }

                char offendingChar = messageText[index];
                string newString = messageText.Substring(0, index + 1);
                int repeats = StaticRandom.Next(4, 10);
                for (int i = 0; i < repeats; i++)
                    newString += offendingChar;
                // low chance of further characters
                for (int i = index; i < messageText.Length; i++)
                {
                    if (StaticRandom.Next(10) == 0)
                        newString += messageText[i];
                }
                return newString;
            }

            return messageText;
        }
    }
}