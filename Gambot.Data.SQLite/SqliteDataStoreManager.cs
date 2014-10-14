using System.Collections.Generic;
using System.IO;
using Gambot.Core;

namespace Gambot.Data.SQLite
{
    public class SqliteDataStoreManager : IDataStoreManager
    {
        private readonly IDictionary<string, IDataStore> dataStores;

        public SqliteDataStoreManager()
        {
            dataStores = new Dictionary<string, IDataStore>();
        }

        public IDataStore Get(string name)
        {
            IDataStore dataStore;
            var found = dataStores.TryGetValue(name, out dataStore);
            if (found) return dataStore;

            var dbLoc = MakeDbPath(name);
            dataStore = new SqliteDataStore(dbLoc);
            dataStores[name] = dataStore;
            return dataStore;
        }

        private string MakeDbPath(string name)
        {
            var dir = Config.Get("Sqlite.DatastoreDirectory");
            return Path.Combine(dir, name + ".db");
        }
    }
}
