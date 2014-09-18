using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatSharp;

namespace Gambot
{
    public class TestMessageHandler : IMessageHandler
    {
        public void Initialize()
        {
            
        }

        public bool Digest(IrcClient irc, Message message)
        {
            if (message.Text == ":hi:")
            {
                irc.SendMessage(message.Who + ": :hi:", message.Where);
                return false;
            }

            return true;
        }
    }
}
