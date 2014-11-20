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
            var queryArgs = new Dictionary<string, string>
            {
                {"@key", key},
                {"@value", val}
            };
            return Execute(query, queryArgs) > 0;
        }

        public int RemoveAllValues(string key)
        {
            const string query = "DELETE FROM data WHERE \"key\" LIKE @key;";
            var queryArgs = new Dictionary<string, string>
            {
                {"@key", key},
            };
            return Execute(query, queryArgs);
        }

        public bool RemoveValue(string key, string val)
        {
            const string query =
                "DELETE FROM data WHERE \"key\" LIKE @key AND value LIKE @value;";
            var queryArgs = new Dictionary<string, string>
            {
                {"@key", key},
                {"@value", val}
            };
            return Execute(query, queryArgs) > 0;
        }

        public IEnumerable<string> GetAllKeys()
        {
            const string query = "SELECT DISTINCT \"key\" FROM data;";
            return GetRows(query).Select(r => r["key"]);
        }

        public IEnumerable<string> GetAllValues(string key)
        {
            const string query = "SELECT value FROM data WHERE \"key\" LIKE @key;";
            var queryArgs = new Dictionary<string, string>
            {
                {"@key", key},
            };
            return GetRows(query, queryArgs).Select(r => r["value"]);
        }

        public string GetRandomValue(string key)
        {
            const string query =
                "SELECT value FROM data WHERE \"key\" LIKE @key ORDER BY RANDOM() LIMIT 1;";
            var queryArgs = new Dictionary<string, string>
            {
                {"@key", key},
            };
            return
                GetRows(query, queryArgs)
                    .Select(r => r["value"])
                    .FirstOrDefault();
        }

        private IEnumerable<Dictionary<string, string>> GetRows(string query,
                                                                Dictionary
                                                                    <string,
                                                                    string>
                                                                    queryArgs =
                                                                    null)
        {
            using (var cmd = CreateCommand(query, queryArgs))
            {
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var rowDict = new Dictionary<string, string>();

                        for (var i = 0; i < rdr.FieldCount; i++)
                        {
                            var name = rdr.GetName(i);
                            var val = rdr.GetValue(i).ToString();
                            rowDict[name] = val;
                        }

                        yield return rowDict;
                    }
                }
            }
        }

        private int Execute(string query,
                            Dictionary<string, string> queryArgs = null)
        {
            using (var cmd = CreateCommand(query, queryArgs))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        private IDbCommand CreateCommand(string query,
                                         Dictionary<string, string> queryArgs =
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
