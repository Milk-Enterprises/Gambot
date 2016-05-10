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

            var match = Regex.Match(message.Text, @"^inventory\??$", RegexOptions.IgnoreCase);

            if (match.Success)
            {
                var items = invDataStore.GetAllValues("CurrentInventory").ToList();

                var itemString = "nothing";
                if (items.Count == 1)
                    itemString = items.Single().Value;
                else if (items.Count == 2)
                    itemString = items[0].Value + " and " + items[1].Value;
                else if (items.Count > 0)
                    itemString = String.Join(", ", items.Take(items.Count - 1).Select(i => i.Value)) + ", and " + items.Last().Value;

                return new ProducerResponse("I have " + itemString + ".", false);
            }

            return null;
        }
    }
}
