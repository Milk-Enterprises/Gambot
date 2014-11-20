using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Gambot.Core
{
    public class Factoid
    {
        public string Trigger { get; set; }
        public string Verb { get; set; }
        public string Response { get; set; }
    }

    public static class FactoidUtilities
    {
        public static Factoid GetVerbAndResponseFromPartialFactoid(
            string factoid)
        {
            var match = Regex.Match(factoid, @"<(.+?)> (.+)");

            if (match.Success)
            {
                return new Factoid()
                       {
                           Verb = match.Groups[1].Value,
                           Response = match.Groups[2].Value
                       };
            }

            return null;
        }
    }
}
