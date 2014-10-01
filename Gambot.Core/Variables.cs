using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Gambot.Core
{
    public static class Variables
    {
        private static Dictionary<string, Func<IMessage, string>> magicVariables =
            new Dictionary<string, Func<IMessage, string>>();
        private static readonly Regex variableRegex = new Regex(@"\$([a-z][a-z0-9_-]*)", RegexOptions.IgnoreCase);

        public static void DefineMagicVariable(string name, Func<IMessage, string> getter)
        {
            magicVariables.Add(name, getter);
        }

        public static string Substitute(string input, IMessage context)
        {
            return variableRegex.Replace(input, match =>
            {
                var var = match.Groups[1].Value.ToLower();
                if (magicVariables.ContainsKey(var))
                    return magicVariables[var](context);

                // TODO: SQL variable lookup

                return match.Value; // ¯\_(ツ)_/¯
            });
        }
    }
}
