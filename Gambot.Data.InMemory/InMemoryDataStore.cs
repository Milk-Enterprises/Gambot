using System;
using System.Collections.Generic;
using System.Linq;
using MiscUtil;
using MiscUtil.Linq;

namespace Gambot.Data.InMemory
{
    internal class InMemoryDataStore : IDataStore
    {
        private readonly EditableLookup<string, string> data;

        public InMemoryDataStore()
        {
            data =
                new EditableLookup<string, string>(
                    StringComparer.InvariantCultureIgnoreCase);
        }

        public bool Put(string key, string val)
        {
            var alreadyExists = data.Contains(key, val);
            if (!alreadyExists)
                data.Add(key, val);

            return !alreadyExists;
        }

        public int RemoveAllValues(string key)
        {
            var count = data[key].Count();
            if (count > 0)
                data.Remove(key);

            return count;
        }

        public bool RemoveValue(string key, string val)
        {
            return data.Remove(key, val);
        }

        public bool RemoveValue(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetAllKeys()
        {
            return data.Select(group => group.Key);
        }

        public IEnumerable<DataStoreValue> GetAllValues(string key)
        {
            return data[key].Select(s => new DataStoreValue(-1, key, s));
        }

        public DataStoreValue GetRandomValue(string key)
        {
            var values = data[key].ToList();
            return values.Count == 0
                       ? null
                    : new DataStoreValue(-1, key, values.ElementAt(StaticRandom.Next(0, values.Count)));
        }

        public DataStoreValue GetRandomValue()
        {
            throw new NotImplementedException(); // :smug:
        }
    }
}
