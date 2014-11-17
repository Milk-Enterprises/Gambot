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
    class InventoryCommandProducer : IMessageProducer
    {
        private IDataStore invDataStore;
        private IDataStore factoidDataStore;
        private readonly IVariableHandler variableHandler;

        private const string CurrentInventoryKey = "CurrentInventory";
        private const string HistoryKey = "History";

        public InventoryCommandProducer(IVariableHandler variableHandler)
        {
            this.variableHandler = variableHandler;
        }
        
        public void Initialize(IDataStoreManager dataStoreManager)
        {
            invDataStore = dataStoreManager.Get("Inventory");
            factoidDataStore = dataStoreManager.Get("Factoid");
            
            variableHandler.DefineMagicVariable("item", GetRandomItem);
            variableHandler.DefineMagicVariable("giveitem", GetRandomItemAndDiscard);
            variableHandler.DefineMagicVariable("newitem", GetNewItem);
        }

        public ProducerResponse Process(IMessage message, bool addressed)
        {
            if (message.Action)
            {
                var botName = Config.Get("Name");
                var match = Regex.Match(message.Text,
                                        String.Format(@"gives (?:(.+) to {0}|{0} (.+))", botName),
                                        RegexOptions.IgnoreCase);

                if (!match.Success)
                    return null;

                var itemName = String.IsNullOrEmpty(match.Groups[1].Value)
                                   ? match.Groups[2].Value
                                   : match.Groups[1].Value;
                if (itemName.EndsWith("?"))
                    return null;

                var inventoryLimit = Int32.Parse(Config.Get("InventoryLimit"));
                var allItems = GetInventory();
                var currentInventorySize = allItems.Count(); // we dont have a .GetCount lololo

                if (allItems.Contains(itemName))
                {
                    var randomDuplicateAddReply = factoidDataStore.GetRandomValue("duplicate item");
                    var duplicateFactoid =
                        FactoidUtilities.GetVerbAndResponseFromPartialFactoid(
                            randomDuplicateAddReply);
                    return
                        new ProducerResponse(
                            variableHandler.Substitute(
                                duplicateFactoid.Response,
                                message,
                                Replace.VarWith("who", message.Who)), false);
                }

                if (currentInventorySize >= inventoryLimit)
                {
                    var randomItemToDrop = RemoveRandomItem();
                    if (randomItemToDrop == null)
                        return null;

                    AddItem(itemName);

                    const string reply = "drops $item and takes $newitem.";
                    return new ProducerResponse(variableHandler.Substitute(reply,
                                                      message,
                                                      Replace.VarWith("item", randomItemToDrop),
                                                      Replace.VarWith("newitem", itemName)), true);
                }
                else
                {
                    AddItem(itemName);

                    var randomSuccessfulAddReply = factoidDataStore.GetRandomValue("takes item");
                    var successfulFactoid =
                        FactoidUtilities.GetVerbAndResponseFromPartialFactoid(
                            randomSuccessfulAddReply);
                    return new ProducerResponse(variableHandler.Substitute(successfulFactoid.Response,
                                                      message,
                                                      Replace.VarWith("item", itemName)), false);
                }
            }
            else
            {

            }

            return null;
        }

        private string GetRandomItem(IMessage msg)
        {
            return invDataStore.GetRandomValue(CurrentInventoryKey) ?? "bananas";
        }

        private string GetRandomItemAndDiscard(IMessage msg)
        {
            return RemoveRandomItem();
        }

        private string GetNewItem(IMessage msg)
        {
            string randomItemFromHistory = null;
            var currentInventory = GetInventory();
            while (currentInventory.Contains(randomItemFromHistory))
            {
                randomItemFromHistory = GetRandomItemFromHistory();
            }

            AddItem(randomItemFromHistory);
            
            return randomItemFromHistory;
        }
        
        private void AddItem(string itemName)
        {
            invDataStore.Put(CurrentInventoryKey, itemName);
            invDataStore.Put(HistoryKey, itemName);
        }

        private bool RemoveItem(string itemName)
        {
            return invDataStore.RemoveValue(CurrentInventoryKey, itemName);
        }

        private string RemoveRandomItem()
        {
            var randomItemToDrop = invDataStore.GetRandomValue(CurrentInventoryKey);
            if (randomItemToDrop == null)
                return null;

            RemoveItem(randomItemToDrop);

            return randomItemToDrop;
        }

        private List<string> GetInventory()
        {
            return invDataStore.GetAllValues(CurrentInventoryKey).ToList();
        }

        private string GetRandomItemFromHistory()
        {
            return invDataStore.GetRandomValue(HistoryKey);
        }
    }
}
