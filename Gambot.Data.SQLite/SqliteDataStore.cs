using System;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using System.Linq;

namespace Gambot.Data.SQLite
{
    internal class SqliteDataStore : IDataStore
    {
        private readonly IDbConnection connection;

        public SqliteDataStore(string path)
        {
            var connStr =
                string.Format(
                    "Data Source={0};Version=3;New=False;Compress=True;", path);
            connection = new SqliteConnection(connStr);
            connection.Open();

            Initialize();
        }

        public bool Put(string key, string val)
        {
            const string query =
                "INSERT INTO data(\"key\", value) VALUES(@key, @value);";
            var queryArgs = new Dictionary<string, object>
            {
                {"@key", key},
                {"@value", val}
            };
            return Execute(query, queryArgs) > 0;
        }

        public int RemoveAllValues(string key)
        {
            const string query = "DELETE FROM data WHERE \"key\" LIKE @key;";
            var queryArgs = new Dictionary<string, object>
            {
                {"@key", key},
            };
            return Execute(query, queryArgs);
        }

        public bool RemoveValue(string key, string val)
        {
            const string query =
                "DELETE FROM data WHERE \"key\" LIKE @key AND value LIKE @value;";
            var queryArgs = new Dictionary<string, object>
            {
                {"@key", key},
                {"@value", val}
            };
            return Execute(query, queryArgs) > 0;
        }

        public bool RemoveValue(long id)
        {
            const string query =
                "DELETE FROM data WHERE \"rowid\" = @id;";
            var queryArgs = new Dictionary<string, object>
            {
                {"@id", id}
            };
            return Execute(query, queryArgs) > 0;
        }

        public IEnumerable<string> GetAllKeys()
        {
            const string query = "SELECT DISTINCT \"key\" FROM data;";
            return GetRows(query).Select(r => (string)r["key"]);
        }

        public IEnumerable<DataStoreValue> GetAllValues(string key)
        {
            const string query = "SELECT rowid, key, value FROM data WHERE \"key\" LIKE @key;";
            var queryArgs = new Dictionary<string, object>
            {
                {"@key", key},
            };
            return GetRows(query, queryArgs).Select(r => new DataStoreValue((long)r["rowid"], (string)r["key"], (string)r["value"]));
        }

        public DataStoreValue GetRandomValue(string key)
        {
            const string query =
                "SELECT rowid, key, value FROM data WHERE \"key\" LIKE @key ORDER BY RANDOM() LIMIT 1;";
            var queryArgs = new Dictionary<string, object>
            {
                {"@key", key},
            };
            return
                GetRows(query, queryArgs)
                    .Select(r => new DataStoreValue((long)r["rowid"], (string)r["key"], (string)r["value"]))
                    .FirstOrDefault();
        }
        
        public DataStoreValue GetRandomValue() {
            const string query =
                "SELECT rowid, key, value FROM data ORDER BY RANDOM() LIMIT 1;";
            return GetRows(query).Select(r => new DataStoreValue((long)r["rowid"], (string)r["key"], (string)r["value"])).FirstOrDefault();
        }

        private IEnumerable<Dictionary<string, object>> GetRows(string query,
                                                                Dictionary
                                                                    <string,
                                                                    object>
                                                                    queryArgs =
                                                                    null)
        {
            using (var cmd = CreateCommand(query, queryArgs))
            {
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var rowDict = new Dictionary<string, object>();

                        for (var i = 0; i < rdr.FieldCount; i++)
                        {
                            var name = rdr.GetName(i);
                            var val = rdr.GetValue(i);
                            rowDict[name] = val;
                        }

                        yield return rowDict;
                    }
                }
            }
        }

        private int Execute(string query,
                            Dictionary<string, object> queryArgs = null)
        {
            using (var cmd = CreateCommand(query, queryArgs))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        private IDbCommand CreateCommand(string query,
                                         Dictionary<string, object> queryArgs =
                                             null)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = query;
            if (queryArgs == null)
                return cmd;

            foreach (var kvp in queryArgs)
                cmd.Parameters.Add(new SqliteParameter(kvp.Key, kvp.Value));

            return cmd;
        }

        private void Initialize()
        {
            const string query =
                "CREATE TABLE IF NOT EXISTS data(\"key\" TEXT, \"value\" TEXT);";
            Execute(query);
        }
    }
}
