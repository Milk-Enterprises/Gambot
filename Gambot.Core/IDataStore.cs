using System.Collections.Generic;

namespace Gambot.Core
{
    public interface IDataStore
    {
        void Put(string key, string val);

        void RemoveAllValues(string key);

        void RemoveValue(string key, string val);

        IEnumerable<string> GetAllValues(string key);

        string GetRandomValue(string key);
    }
}
