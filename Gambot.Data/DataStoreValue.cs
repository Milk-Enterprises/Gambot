using System;

namespace Gambot.Data
{
    public class DataStoreValue
    {
        public long Id { get; protected set; }
        public string Key { get; protected set; }
        public string Value { get; protected set; }

        public DataStoreValue(long id, string key, string value) 
        {
            Id = id;
            Key = key;
            Value = value;
        }
    }
}

