using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gambot.Core;

namespace Gambot.Modules.Inventory
{
    class InventoryModule : AbstractModule
    {
        public InventoryModule(IVariableHandler varHandler)
        {
            MessageHandlers.Add(new InventoryCommandHandler(varHandler));
        }
    }
}
