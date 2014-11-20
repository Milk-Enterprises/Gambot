using Gambot.Core;

namespace Gambot.Modules.Inventory
{
    class InventoryModule : AbstractModule
    {
        public InventoryModule(IVariableHandler varHandler)
        {
            MessageReactors.Add(new InventoryCommandReactor(varHandler));
            MessageProducers.Add(new InventoryInventoryCommandProducer());
        }
    }
}
