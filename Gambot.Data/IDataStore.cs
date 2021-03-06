﻿using System.Collections.Generic;

namespace Gambot.Data
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
        /// Attempts to remove the key-value pair with the specified ID.
        /// </summary>
        /// <returns><b>true</b>, if the operation succeeded; <b>false</b> otherwise.</returns>
        /// <param name="id">ID of the key-value pair (returned when fetching a value).</param>
        bool RemoveValue(long id);

        /// <summary>
        /// Gets all keys from the data store.
        /// </summary>
        IEnumerable<string> GetAllKeys();

        /// <summary>
        /// Gets all values associated with <paramref name="key"/>.
        /// </summary>
        IEnumerable<DataStoreValue> GetAllValues(string key);

        /// <summary>
        /// Gets a random value associated with <paramref name="key"/>.
        /// </summary>
        /// <returns>A random value associated with <paramref name="key"/> if <paramref name="key"/> exists as a key in the data store; <b>null</b> otherwise.</returns>
        DataStoreValue GetRandomValue(string key);
        DataStoreValue GetRandomValue();
    }
}
