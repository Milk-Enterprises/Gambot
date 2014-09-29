using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Gambot.Core
{
    public static class Variables
    {
        private static readonly Dictionary<string, Func<string>> MagicVariables = new Dictionary<string, Func<string>>();
        private static readonly Regex VariableRegex = new Regex(@"\$[a-z][a-z0-9_-]*", RegexOptions.IgnoreCase);

        public static void DefineMagicVariable(string name, Func<string> getter)
        {
            MagicVariables.Add(name, getter);
        }

        public static string Substitute(string input)
        {
            return VariableRegex.Replace(input, match =>
            {
                var var = match.Value.ToLower();
                if (MagicVariables.ContainsKey(var))
                    return MagicVariables[var]();

                // TODO: SQL variable lookup

                return match.Value; // ¯\_(ツ)_/¯
            });
        }
    }
}
