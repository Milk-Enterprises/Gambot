using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using NLog;

namespace Gambot.Core
{
    public static class Config
    {
        private static readonly Logger logger =
            LogManager.GetCurrentClassLogger();

        public static string Get(string key, string defaultValue = null)
        {
            logger.Info("Getting config value for \"{0}\"...", key);
            var val = ConfigurationManager.AppSettings[key];
            if (val == null)
            {
                logger.Info(
                    "No value found for \"{0}\", defaulting value to \"{1}.\"",
                    key, defaultValue ?? "null");
            }
            else
                logger.Info("Success! Value is {0}.", val);

            return val ?? defaultValue;
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            logger.Info("Getting config value for \"{0}\"...", key);
            var val = ConfigurationManager.AppSettings[key];
            if (val == null)
            {
                logger.Info(
                    "No value found for \"{0}\", defaulting value to {1}.", key,
                    defaultValue);
            }
            else
                logger.Info("Success! Value is {0}.", val);

            return val != null ? bool.Parse(val) : defaultValue;
        }

        public static void Set(string key, string value)
        {
            // simple, works even if the user destroys appSettings, works even if the key does not exist prior to setting
            var config =
                ConfigurationManager.OpenExeConfiguration(
                    ConfigurationUserLevel.None);

            if (!config.AppSettings.Settings.AllKeys.Contains(key))
                config.AppSettings.Settings.Add(key, value);
            else
                config.AppSettings.Settings[key].Value = value;

            config.Save(ConfigurationSaveMode.Modified);
        }
    }
}
