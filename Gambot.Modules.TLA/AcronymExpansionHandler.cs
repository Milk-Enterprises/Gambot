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
    internal class AcronymExpansionHandler : IMessageHandler
    {
        private const char Wildcard = '*';
        private const string AcronymKey = "Acronyms";
        private readonly IVariableHandler variableHandler;
        private IDataStore tlaDataStore;

        public HandlerPriority Priority
        {
            get { return HandlerPriority.Normal; }
        }

        public AcronymExpansionHandler(IVariableHandler variableHandler)
        {
            this.variableHandler = variableHandler;

            variableHandler.DefineMagicVariable("tla", msg => GetRandomAcronym("tla", msg));
            variableHandler.DefineMagicVariable("band", msg => GetRandomAcronym("band", msg));
        }

        public void Initialize(IDataStoreManager dataStoreManager)
        {
            tlaDataStore = dataStoreManager.Get("TLA");
        }

        public string Process(string currentResponse, IMessage message, bool addressed)
        {
            var trimmedMsg = message.Text.Trim();
            if (trimmedMsg.Length != 3 || !trimmedMsg.All(c => Char.IsLetter(c) || c == Wildcard))
            {
                return currentResponse;
            }

            var allAcronyms =
                tlaDataStore.GetAllKeys().Where(str => str != AcronymKey);

            var matchingAcronym =
                allAcronyms.FirstOrDefault(
                    acro => EssentiallyEquivalent(acro, trimmedMsg));

            if (matchingAcronym == null)
                return currentResponse;

            var expandedAcronym = tlaDataStore.GetRandomValue(matchingAcronym);

            return expandedAcronym ?? currentResponse;
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