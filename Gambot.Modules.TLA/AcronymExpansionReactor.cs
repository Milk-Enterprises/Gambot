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
    internal class AcronymExpansionReactor : IMessageReactor
    {
        private const char Wildcard = '*';
        private const string AcronymKey = "band";
        private IDataStore tlaDataStore;

        public AcronymExpansionReactor(IVariableHandler variableHandler)
        {
            variableHandler.DefineMagicVariable("tla", msg => tlaDataStore.GetRandomValue());
            variableHandler.DefineMagicVariable("band", msg => tlaDataStore.GetRandomValue());
        }

        public void Initialize(IDataStoreManager dataStoreManager)
        {
            tlaDataStore = dataStoreManager.Get("TLAs");
        }

        public ProducerResponse Process(IMessage message, bool addressed)
        {
            var trimmedMsg = message.Text.Trim();
            if (trimmedMsg.Length != 3 ||
                !trimmedMsg.All(c => (Char.IsLetter(c) && Char.IsUpper(c)) || c == Wildcard))
            {
                return null;
            }

            // TODO: wildcards
            // don't attempt to do this before the datastore supports queries!

            var tla = tlaDataStore.GetRandomValue(trimmedMsg);
            return tla != null ? new ProducerResponse(tla, false) : null;
        }

        private string GetRandomAcronym(string variableName, IMessage message)
        {
            var quote = tlaDataStore.GetRandomValue(AcronymKey);

            return quote ?? variableName;
        }
    }
}
