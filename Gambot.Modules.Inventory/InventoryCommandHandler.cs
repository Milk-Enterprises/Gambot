using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Inventory
{
    class InventoryCommandHandler : IMessageHandler
    {
        private IDataStore invDataStore;
        private IDataStore factoidDataStore;

        public HandlerPriority Priority
        {
            get { return HandlerPriority.Normal; }
        }
        
        public void Initialize(IDataStoreManager dataStoreManager)
        {
            invDataStore = dataStoreManager.Get("Inventory");
            factoidDataStore = dataStoreManager.Get("Factoid");
        }

        public string Process(string currentResponse, IMessage message, bool addressed)
        {
            return currentResponse;
        }
    }
}
