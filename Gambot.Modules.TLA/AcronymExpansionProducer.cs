using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.TLA
{
    internal class AcronymExpansionProducer : IMessageProducer
    {
        private const char Wildcard = '*';
        private const string AcronymKey = "Acronyms";
        private IDataStore tlaDataStore;

        public AcronymExpansionProducer(IVariableHandler variableHandler)
        {
            variableHandler.DefineMagicVariable("tla", msg => GetRandomAcronym("tla", msg));
            variableHandler.DefineMagicVariable("band", msg => GetRandomAcronym("band", msg));
        }

        public void Initialize(IDataStoreManager dataStoreManager)
        {
            tlaDataStore = dataStoreManager.Get("TLA");
        }

        public ProducerResponse Process(IMessage message, bool addressed)
        {
            var trimmedMsg = message.Text.Trim();
            if (trimmedMsg.Length != 3 ||
                !trimmedMsg.All(c => Char.IsLetter(c) || c == Wildcard))
            {
                return null;
            }

            var allAcronyms =
                tlaDataStore.GetAllKeys().Where(str => str != AcronymKey);

            var matchingAcronym =
                allAcronyms.FirstOrDefault(
                    acro => EssentiallyEquivalent(acro, trimmedMsg));

            if (matchingAcronym == null)
                return null;

            var expandedAcronym = tlaDataStore.GetRandomValue(matchingAcronym);

            return expandedAcronym == null ? null : new ProducerResponse(expandedAcronym, false);
        }
        
        // :smug:
        private bool EssentiallyEquivalent(string a, string b)
        {
            if (a.Length != b.Length)
                return false;

            for (var i = 0; i < a.Length; i++)
            {
                if (!AnyCharactersAreWildcard(a[i], b[i]) &&
                    Char.ToUpperInvariant(a[i]) != Char.ToUpperInvariant(b[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool AnyCharactersAreWildcard(params char[] chars)
        {
            return chars.Any(c => c == Wildcard);
        }

        private string GetRandomAcronym(string variableName, IMessage message)
        {
            var quote = tlaDataStore.GetRandomValue(AcronymKey);

            return quote ?? variableName;
        }
    }
}