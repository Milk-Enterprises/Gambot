using Gambot.Core;

namespace Gambot.Modules.Inventory
{
    class InventoryModule : AbstractModule
    {
        public InventoryModule(IVariableHandler varHandler)
        {
            MessageProducers.Add(new InventoryCommandProducer(varHandler));
            MessageProducers.Add(new InventoryInventoryCommandProducer());
        }
    }
}
