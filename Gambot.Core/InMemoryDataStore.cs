using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gambot.Core
{
    internal class InMemoryDataStore : IDataStore
    {
        public bool Put(string key, string val)
        {
            throw new NotImplementedException();
        }

        public int RemoveAllValues(string key)
        {
            throw new NotImplementedException();
        }

        public bool RemoveValue(string key, string val)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetAllValues(string key)
        {
            throw new NotImplementedException();
        }

        public string GetRandomValue(string key)
        {
            throw new NotImplementedException();
        }
    }
}
