namespace Gambot.Data.InMemory
{
    // todo: move this to the appropriate project/namespace
    internal class InMemoryDataStoreManager : IDataStoreManager
    {
        private readonly IDictionary<string, IDataStore> dataStores;

        public InMemoryDataStoreManager()
        {
            dataStores = new Dictionary<string, IDataStore>(); // todo: maybe use a ConcurrentDictionary but whos gives a shit really
        }
 
        public IDataStore Get(string name)
        {
            IDataStore dataStore;
            var worked = dataStores.TryGetValue(name, out dataStore);
            if (worked) return dataStore;

            var newDataStore = new InMemoryDataStore();
            dataStores[name] = newDataStore;

            return newDataStore;
        }
    }
}