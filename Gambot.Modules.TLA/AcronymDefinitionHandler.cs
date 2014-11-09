using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gambot.Core;
using Gambot.Data;
using MiscUtil;

namespace Gambot.Modules.TLA
{
    public class AcronymDefinitionHandler : IMessageHandler
    {
        private IDataStore factoidDataStore;
        private IDataStore tlaDataStore;
        private IVariableHandler variableHandler;

        private const string DefaultBandNameReply =
            "<reply> \"$band\" would be a cool name for a band.";

        public HandlerPriority Priority
        {
            get { return HandlerPriority.Normal; }
        }

        public AcronymDefinitionHandler(IVariableHandler variableHandler)
        {
            this.variableHandler = variableHandler;
        }

        public void Initialize(IDataStoreManager dataStoreManager)
        {
            factoidDataStore = dataStoreManager.Get("Factoids");
            tlaDataStore = dataStoreManager.Get("TLA");
        }

        public string Process(string currentResponse, IMessage message, bool addressed)
        {
            var match = Regex.Match(message.Text, @"^([a-z]\w*)\s+([a-z]\w*)\s+([a-z]\w*)$",
                RegexOptions.IgnoreCase);

            if (match.Success)
            {
                var words = new[] {match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value};

                var expandedAcronym = String.Join(" ", words);
                var tlaChance = int.Parse(Config.Get("PercentChanceOfNewTLA"));
                var shouldCreateNewAcronym = StaticRandom.Next(0, 100) < tlaChance;

                if (shouldCreateNewAcronym)
                {
                    var acronym = new String(words.Select(s => s.First()).ToArray()).ToUpperInvariant();
                    if (!tlaDataStore.Put(acronym, expandedAcronym))
                        return currentResponse;

                    // grab a random band name reply factoid and :shipit:
                    var bandNameFactoidStr = factoidDataStore.GetRandomValue("band name reply") ?? DefaultBandNameReply;
                    var bandNameFactoid =
                        FactoidUtilities.GetVerbAndResponseFromPartialFactoid(bandNameFactoidStr);

                    // GHETTO ALERT
                    var coercedResponse = bandNameFactoid.Response.Replace("$band", expandedAcronym)
                        .Replace("$tla", expandedAcronym);
                    return variableHandler.Substitute(coercedResponse, message);
                }
            }

            return currentResponse;
        }
    }
}
