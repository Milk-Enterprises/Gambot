using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Inventory
{
    class InventoryInventoryCommandHandler : IMessageHandler
    {
        private IDataStore invDataStore;

        public HandlerPriority Priority
        {
            get { return HandlerPriority.Normal; }
        }

        public void Initialize(IDataStoreManager dataStoreManager)
        {
            invDataStore = dataStoreManager.Get("Inventory");
        }

        public string Process(string currentResponse, IMessage message, bool addressed)
        {
            if (message.Action)
                return currentResponse;

            var match = Regex.Match(message.Text, @"^inventory\??$");

            if (match.Success)
            {
                var items = invDataStore.GetAllValues("Items").ToList();

                return String.Format("/me contains {0}.",
                                     items.Any()
                                         ? String.Join(", ", items)
                                         : "nothing");
            }

            return currentResponse;
        }
    }
}
