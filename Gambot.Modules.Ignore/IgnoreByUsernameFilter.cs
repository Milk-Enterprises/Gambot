using System;
using System.Collections.Generic;
using System.Linq;
using Gambot.Core;
using Gambot.Data;
using NLog;

namespace Gambot.Modules.Ignore
{
    internal class IgnoreByUsernameFilter : IMessageFilter
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private IList<string> ignoredUsernames;

        public void Initialize(IDataStoreManager dataStoreManager)
        {
            ignoredUsernames = new List<string>();

            var ignoredUserDelimitedStr = Config.Get("IgnoredUsers", String.Empty);

            try
            {
                ignoredUsernames =
                    ignoredUserDelimitedStr.Split(new[] {','},
                                                  StringSplitOptions
                                                      .RemoveEmptyEntries)
                                           .ToList();
            }
            catch(Exception e)
            {
                logger.Error("Unable to load ignored users list; IgnoredUsers is in an improper state.", e);
            }
        }

        public bool ShouldMessagePassThrough(IMessage message, bool addressed)
        {
            return !IsUserIgnored(message.Who);
        }

        private bool IsUserIgnored(string username)
        {
            return ignoredUsernames.Contains(username, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
