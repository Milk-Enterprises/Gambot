using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gambot.Core;

namespace Gambot.Modules.TLA
{
    public class TLAModule : AbstractModule
    {
        public TLAModule(IVariableHandler variableHandler)
        {
            MessageHandlers.Add(new AcronymDefinitionHandler(variableHandler));
            MessageHandlers.Add(new AcronymExpansionHandler(variableHandler));
        }
    }
}
