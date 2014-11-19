using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Conjugation
{
    internal class Conjugator
    {
        private readonly IDataStore irregularDataStore;

        public Conjugator(IDataStore irregularDataStore)
        {
            this.irregularDataStore = irregularDataStore;
        }

        protected string Stem(string infinitive)
        {
            var irregular = irregularDataStore.GetRandomValue(infinitive + ".stem");
            if (irregular != null)
                return irregular;

            if (Regex.IsMatch(infinitive, @"[bcdfghjklmnpqrstvwxyz][aeiou][bcdfghjklmnpqrstv]$", RegexOptions.IgnoreCase))
                return Regex.Replace(infinitive, @"(\w)$", "$1$1");

            return infinitive;
        }

        public string Verbed(string infinitive)
        {
            var to = infinitive.EndsWith(" to");
            if (to)
                infinitive = infinitive.Substring(0, infinitive.Length - 3);

            var irregular = irregularDataStore.GetRandomValue(infinitive + ".participle");
            if (irregular != null)
                return irregular + (to ? " to" : "");

            var stem = Stem(infinitive);
            var participle = stem + "ed";
            participle = Regex.Replace(participle, @"([bcdfghjklmnpqrstvwxyz])eed$", "$1ed");
            participle = Regex.Replace(participle, @"([bcdfghjklmnpqrstvwxyz])yed$", "$1ied");
            participle = Regex.Replace(participle, @"eed$", "ed");

            return participle + (to ? " to" : "");
        }

        public string Verbing(string infinitive)
        {
            var to = infinitive.EndsWith(" to");
            if (to)
                infinitive = infinitive.Substring(0, infinitive.Length - 3);

            var irregular = irregularDataStore.GetRandomValue(infinitive + ".gerund");
            if (irregular != null)
                return irregular + (to ? " to" : "");

            var stem = Stem(infinitive);
            var gerund = stem + "ing";
            gerund = Regex.Replace(gerund, @"(.[bcdfghjklmnpqrstvwxyz])eing$", "$1ing");
            gerund = Regex.Replace(gerund, @"ieing$", "ying");
            return gerund + (to ? " to" : "");
        }

        public string Verbs(string infinitive)
        {
            var to = infinitive.EndsWith(" to");
            if (to)
                infinitive = infinitive.Substring(0, infinitive.Length - 3);

            var irregular = irregularDataStore.GetRandomValue(infinitive + ".present");
            if (irregular != null)
                return irregular + (to ? " to" : "");

            if (Regex.IsMatch(infinitive, @"ch$|[xs]$"))
                return infinitive + "es";
            if (Regex.IsMatch(infinitive, @"[bcdfghjklmnpqrstvwxyz]y$"))
                return infinitive.Substring(0, infinitive.Length - 1) + "ies";
            return infinitive + "s" + (to ? " to" : "");
        }

        public string Nouns(string singular)
        {
            var irregular = irregularDataStore.GetRandomValue(singular + ".plural");
            if (irregular != null)
                return irregular;

            string plural = null;
            Action<string, string> replace = (string regex, string replacement) =>
            {
                if (plural != null)
                    return;
                var final = Regex.Replace(singular, regex, replacement, RegexOptions.IgnoreCase);
                if (final != singular)
                    plural = final;
            };

            // families of irregular plurals  
            replace(@"(.*)man$", "$1men");
            replace(@"(.*[ml])ouse$", "$1ice");
            replace(@"(.*)goose$", "$1geese");
            replace(@"(.*)tooth$", "$1teeth");
            replace(@"(.*)foot$", "$1feet");

            // unassimilated imports
            replace(@"(.*)ceps$", "$1ceps");
            replace(@"(.*)zoon$", "$1zoa");
            replace(@"(.*[csx])is$", "$1es");

            // incompletely assimilated imports
            replace(@"(.*)trix$", "$1trices");
            replace(@"(.*)eau$", "$1eaux");
            replace(@"(.*)ieu$", "$1ieux");
            replace(@"(.{2,}[yia])nx$", "$1nges");
            
            // singular nouns ending in "s" or other silibants
            replace(@"^(.*s)$", "$1es");
            replace(@"^(.*[^z])(z)$", "$1zzes");
            replace(@"^(.*)([cs]h|x|zz|ss)$", "$1$2es");

            // f -> ves
            replace(@"(.*[eao])lf", "$1lves");
            replace(@"(.*[^d])eaf$", "$1eaves");
            replace(@"(.*[nlw])ife$", "$1ives");
            replace(@"(.*)arf$", "$1arves");

            // y
            replace(@"(.*[aeiou])y$", "$1ys");
            replace(@"(.*)y$", "$1ies");

            // o
            replace(@"(.*[bcdfghjklmnpqrstvwxyz]o)$", "$1es");

            return plural ?? singular + "s";
        }
    }
}
