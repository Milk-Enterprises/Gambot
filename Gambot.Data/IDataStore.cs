using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gambot.Data
{
    interface IDataStore
    {
        void Put(string key, string val);

        void RemoveAllValues(string key);

        void RemoveValue(string key, string val);

        IEnumerable<string> GetAllValues(string key);

        string GetRandomValue(string key);
    }
}
