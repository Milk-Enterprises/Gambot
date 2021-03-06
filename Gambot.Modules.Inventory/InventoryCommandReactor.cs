﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Inventory
{
    class InventoryCommandReactor : IMessageReactor
    {
        private IDataStore invDataStore;
        private IDataStore factoidDataStore;
        private readonly IVariableHandler variableHandler;

        private const string CurrentInventoryKey = "CurrentInventory";
        private const string HistoryKey = "History";

        public InventoryCommandReactor(IVariableHandler variableHandler)
        {
            this.variableHandler = variableHandler;
        }
        
        public void Initialize(IDataStoreManager dataStoreManager)
        {
            invDataStore = dataStoreManager.Get("Inventory");
            factoidDataStore = dataStoreManager.Get("Factoids");
            
            variableHandler.DefineMagicVariable("item", GetRandomItem);
            variableHandler.DefineMagicVariable("giveitem", GetRandomItemAndDiscard);
            variableHandler.DefineMagicVariable("newitem", GetNewItem);
        }

        public ProducerResponse Process(IMessage message, bool addressed)
        {
            var botName = Config.Get("Name", "gambot");
            var match = Regex.Match(message.Text,
                                    String.Format(@"^(?:I give|gives) (?:(.+) to {0}|{0} (.+))$", botName),
                                    RegexOptions.IgnoreCase);

            if (!match.Success)
                return null;

            var itemName = String.IsNullOrEmpty(match.Groups[1].Value)
                               ? match.Groups[2].Value
                               : match.Groups[1].Value;
            if (itemName.EndsWith("?"))
                return null;

            var inventoryLimit = Int32.Parse(Config.Get("InventoryLimit", "10"));
            var allItems = GetInventory();
            var currentInventorySize = allItems.Count(); // we dont have a .GetCount lololo

            if (allItems.Contains(itemName))
            {
                var randomDuplicateAddReply = factoidDataStore.GetRandomValue("duplicate item")?.Value ?? "<reply> I already have $item.";
                var duplicateFactoid =
                    FactoidUtilities.GetVerbAndResponseFromPartialFactoid(randomDuplicateAddReply);
                return
                    new ProducerResponse(
                        variableHandler.Substitute(
                            duplicateFactoid.Response,
                            message,
                            Replace.VarWith("item", itemName)), false);
            }

            if (currentInventorySize >= inventoryLimit)
            {
                var randomItemToDrop = RemoveRandomItem();
                if (randomItemToDrop == null)
                    return null;

                AddItem(itemName);

                var randomDropItemReply = factoidDataStore.GetRandomValue("drops item")?.Value ?? "<reply> I take $newitem and drop $giveitem.";
                var dropItemFactoid =
                    FactoidUtilities.GetVerbAndResponseFromPartialFactoid(randomDropItemReply);
                return new ProducerResponse(variableHandler.Substitute(dropItemFactoid.Response,
                                                  message,
                                                  Replace.VarWith("giveitem", randomItemToDrop),
                                                  Replace.VarWith("newitem", itemName)), true);
            }
            else
            {
                AddItem(itemName);

                var randomSuccessfulAddReply = factoidDataStore.GetRandomValue("takes item")?.Value ?? "<reply> I now have $item.";
                var successfulFactoid =
                    FactoidUtilities.GetVerbAndResponseFromPartialFactoid(
                        randomSuccessfulAddReply);
                return new ProducerResponse(variableHandler.Substitute(successfulFactoid.Response,
                                                  message,
                                                  Replace.VarWith("item", itemName)), false);
            }

            return null;
        }

        private string GetRandomItem(IMessage msg)
        {
            return invDataStore.GetRandomValue(CurrentInventoryKey)?.Value ?? "bananas";
        }

        private string GetRandomItemAndDiscard(IMessage msg)
        {
            return RemoveRandomItem();
        }

        private string GetNewItem(IMessage msg)
        {
            string randomItemFromHistory;
            var currentInventory = GetInventory();
            do
            {
                randomItemFromHistory = GetRandomItemFromHistory();
            }
            while (currentInventory.Contains(randomItemFromHistory));

            AddItem(randomItemFromHistory);

            var inventoryLimit = Int32.Parse(Config.Get("InventoryLimit", "10"));
            if (GetInventory().Count >= inventoryLimit)
                RemoveRandomItem();
            
            return randomItemFromHistory;
        }
        
        private void AddItem(string itemName)
        {
            if (String.IsNullOrWhiteSpace(itemName))
                return;

            invDataStore.Put(CurrentInventoryKey, itemName);
            invDataStore.Put(HistoryKey, itemName);
        }

        private bool RemoveItem(string itemName)
        {
            return invDataStore.RemoveValue(CurrentInventoryKey, itemName);
        }

        private string RemoveRandomItem()
        {
            var randomItemToDrop = invDataStore.GetRandomValue(CurrentInventoryKey)?.Value;
            if (randomItemToDrop == null)
                return "bananas";

            RemoveItem(randomItemToDrop);

            return randomItemToDrop;
        }

        private List<string> GetInventory()
        {
            return invDataStore.GetAllValues(CurrentInventoryKey).Select(dsv => dsv.Value).ToList();
        }

        private string GetRandomItemFromHistory()
        {
            return invDataStore.GetRandomValue(HistoryKey)?.Value;
        }
    }
}
