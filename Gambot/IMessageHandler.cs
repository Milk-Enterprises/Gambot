using ChatSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gambot
{
    public interface IMessageHandler
    {
        void Initialize();
        bool Digest(IrcClient irc, Message message);
    }
}
