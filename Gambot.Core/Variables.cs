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
        private static readonly List<IVariableFallbackHandler> fallbackHandlers = new List<IVariableFallbackHandler>();

        internal static void AddFallbackHandler<T>(T instance) where T : IVariableFallbackHandler
        {
            fallbackHandlers.Add(instance);
        }

        public static void DefineMagicVariable(string name, Func<IMessage, string> getter)
        {
            magicVariables.Add(name, getter);
        }

        public static string Substitute(string input, IMessage context)
        {
            return variableRegex.Replace(input, match =>
            {
                var var = match.Groups[1].Value.ToLower();
                var subVal = match.Value;
                if (magicVariables.ContainsKey(var))
                    subVal = magicVariables[var](context);

                foreach (var fallback in fallbackHandlers)
                {
                    var value = fallback.Fallback(var, context);
                    if (value == null) continue;

                    subVal = value;
                    break;
                }

                if (match.Groups[1].Value.All(c => !Char.IsLetter(c) || Char.IsUpper(c)))
                    subVal = subVal.ToUpper();
                else if (Char.IsUpper(match.Groups[1].Value[0]))
                    subVal = String.Join(" ", subVal.Split(' ').Select(word => Char.ToUpper(word[0]).ToString() + word.Substring(1)));

                return subVal;
            });
        }
    }
}
