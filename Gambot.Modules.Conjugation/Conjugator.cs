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
#region CHECK 'EM
        public List<string> NoDoubles = new List<string>
        {
            "abandon",
            "accouter",
            "accredit",
            "adhibit",
            "administer",
            "alter",
            "anchor",
            "answer",
            "attrit",
            "audit",
            "author",
            "ballot",
            "banner",
            "batten",
            "bedizen",
            "bespatter",
            "betoken",
            "bewilder",
            "billet",
            "blacken",
            "blither",
            "blossom",
            "bother",
            "brighten",
            "broaden",
            "broider",
            "burden",
            "caparison",
            "catalog",
            "censor",
            "center",
            "charter",
            "chatter",
            "cheapen",
            "chipper",
            "chirrup",
            "christen",
            "clobber",
            "cluster",
            "coarsen",
            "cocker",
            "coedit",
            "cohabit",
            "concenter",
            "corner",
            "cover",
            "covet",
            "cower",
            "credit",
            "custom",
            "dampen",
            "deafen",
            "decipher",
            "deflower",
            "delimit",
            "deposit",
            "develop",
            "differ",
            "disaccustom",
            "discover",
            "discredit",
            "disencumber",
            "dishearten",
            "disinherit",
            "dismember",
            "dispirit",
            "dither",
            "dizen",
            "dodder",
            "edit",
            "elicit",
            "embitter",
            "embolden",
            "embosom",
            "embower",
            "empoison",
            "empower",
            "enamor",
            "encipher",
            "encounter",
            "endanger",
            "enfetter",
            "engender",
            "enlighten",
            "enter",
            "envelop",
            "envenom",
            "environ",
            "exhibit",
            "exit",
            "fasten",
            "fatten",
            "feather",
            "fester",
            "filter",
            "flatten",
            "flatter",
            "flounder",
            "fluster",
            "flutter",
            "foreshorten",
            "founder",
            "fritter",
            "gammon",
            "gather",
            "gladden",
            "glimmer",
            "glisten",
            "glower",
            "greaten",
            "hamper",
            "hanker",
            "happen",
            "harden",
            "harken",
            "hasten",
            "hearten",
            "hoarsen",
            "honor",
            "imprison",
            "inhabit",
            "inhibit",
            "inspirit",
            "interpret",
            "iron",
            "laten",
            "launder",
            "lengthen",
            "liken",
            "limber",
            "limit",
            "linger",
            "litter",
            "liven",
            "loiter",
            "lollop",
            "louden",
            "lower",
            "lumber",
            "madden",
            "malinger",
            "market",
            "matter",
            "misinterpret",
            "misremember",
            "monitor",
            "moulder",
            "murder",
            "murmur",
            "muster",
            "number",
            "offer",
            "open",
            "order",
            "outmaneuver",
            "overmaster",
            "pamper",
            "pilot",
            "pivot",
            "plaster",
            "plunder",
            "powder",
            "power",
            "prohibit",
            "reckon",
            "reconsider",
            "recover",
            "redden",
            "redeliver",
            "register",
            "rejigger",
            "remember",
            "renumber",
            "reopen",
            "reposit",
            "rewaken",
            "richen",
            "roister",
            "roughen",
            "sadden",
            "savor",
            "scatter",
            "scupper",
            "sharpen",
            "shatter",
            "shelter",
            "shimmer",
            "shiver",
            "shorten",
            "shower",
            "sicken",
            "smolder",
            "smoothen",
            "soften",
            "solicit",
            "squander",
            "stagger",
            "stiffen",
            "stopper",
            "stouten",
            "straiten",
            "strengthen",
            "stutter",
            "suffer",
            "sugar",
            "summon",
            "surrender",
            "swelter",
            "sypher",
            "tamper",
            "tauten",
            "tender",
            "thicken",
            "threaten",
            "thunder",
            "totter",
            "toughen",
            "tower",
            "transit",
            "tucker",
            "unburden",
            "uncover",
            "unfetter",
            "unloosen",
            "upholster",
            "utter",
            "visit",
            "vomit",
            "wander",
            "water",
            "weaken",
            "whiten",
            "winter",
            "wonder",
            "worsen",
        };
#endregion

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

            if (!NoDoubles.Contains(infinitive.ToLower()) &&
                Regex.IsMatch(infinitive, @"[bcdfghjklmnpqrstvwxyz][aeiou][bcdfghjklmnpqrstv]$", RegexOptions.IgnoreCase))
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
