using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Gambot
{
    public static class Config
    {
        public static string Get(string key, string defaultValue = null)
        {
            return ConfigurationManager.AppSettings[key] ?? defaultValue;
        }

        public static void Set(string key, string value)
        {
            ConfigurationManager.AppSettings.Set(key, value);
        }
    }
}
