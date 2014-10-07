using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gambot.Core
{
    public interface IModule
    {
        IEnumerable<IMessageHandler> GetMessageHandlers();
    }
}
