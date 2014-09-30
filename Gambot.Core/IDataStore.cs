using System.Collections.Generic;

namespace Gambot.Core
{
    public interface IDataStore
    {
        /// <summary>
        /// Adds <paramref name="val"/> to the data store and associates it with <paramref name="key"/>.
        /// </summary>
        /// <returns><b>true</b> if the operation succeeded; <b>false</b> otherwise (such as when the value already exists in the data store).</returns>
        bool Put(string key, string val);

        /// <summary>
        /// Removes all values associated with <paramref name="key"/>.
        /// </summary>
        /// <returns>The number of values removed.</returns>
        int RemoveAllValues(string key);

        /// <summary>
        /// Attempts to dissociate and remove <paramref name="val"/> from <paramref name="key"/>.
        /// </summary>
        /// <returns><b>true</b> if the operation succeeded; <b>false</b> otherwise (such as when the value does not exist in the data store).</returns>
        bool RemoveValue(string key, string val);

        /// <summary>
        /// Gets all values associated with <paramref name="key"/>.
        /// </summary>
        IEnumerable<string> GetAllValues(string key);

        /// <summary>
        /// Gets a random value associated with <paramref name="key"/>.
        /// </summary>
        /// <returns>A random value associated with <paramref name="key"/> if <paramref name="key"/> exists as a key in the data store; <b>null</b> otherwise.</returns>
        string GetRandomValue(string key);
    }
}
