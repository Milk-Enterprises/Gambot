using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Gambot.Core
{
    public static class Config
    {
        public static string Get(string key, string defaultValue = null)
        {
            return ConfigurationManager.AppSettings[key] ?? defaultValue;
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            var val = ConfigurationManager.AppSettings[key];
            return val == null ? defaultValue : bool.Parse(val);
        }

        public static void Set(string key, string value)
        {
            ConfigurationManager.AppSettings.Set(key, value);
        }
    }
}
