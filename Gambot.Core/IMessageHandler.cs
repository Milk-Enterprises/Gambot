using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gambot.Core;

namespace Gambot
{
    public interface IMessageHandler
    {
        void Initialize();
        bool Digest(IMessenger messenger, IMessage message);
    }
}
