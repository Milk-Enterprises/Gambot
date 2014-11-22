using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gambot.Core;

namespace Gambot.Modules.Ignore
{
    public class IgnoreModule : AbstractModule
    {
        public IgnoreModule()
        {
            MessageFilters.Add(new IgnoreByUsernameFilter());
            MessageProducers.Add(new IgnoreCommandProducer());
        }
    }
}
