using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Inventory
{
    class InventoryInventoryCommandProducer : IMessageProducer
    {
        private IDataStore invDataStore;

        public void Initialize(IDataStoreManager dataStoreManager)
        {
            invDataStore = dataStoreManager.Get("Inventory");
        }

        public ProducerResponse Process(IMessage message, bool addressed)
        {
            //if (message.Action)
            //    return null;

            var match = Regex.Match(message.Text, @"^inventory\??$");

            if (match.Success)
            {
                var items = invDataStore.GetAllValues("Items").ToList();

                return new ProducerResponse(String.Format("contains {0}.", items.Any() ? String.Join(", ", items) : "nothing"), true);
            }

            return null;
        }
    }
}
