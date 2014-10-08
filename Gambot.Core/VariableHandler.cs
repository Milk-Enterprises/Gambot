using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Gambot.Core
{
    public interface IVariableHandler
    {
        void AddFallbackHandler<T>(T instance) where T : IVariableFallbackHandler;
        void DefineMagicVariable(string name, Func<IMessage, string> getter);
        string Substitute(string input, IMessage context);
    }

    public class VariableHandler : IVariableHandler
    {
        private Dictionary<string, Func<IMessage, string>> magicVariables =
            new Dictionary<string, Func<IMessage, string>>();
        private readonly Regex variableRegex = new Regex(@"\$([a-z][a-z0-9_-]*)(?:\[([^\]]+)\])?", RegexOptions.IgnoreCase);
        private readonly List<IVariableFallbackHandler> fallbackHandlers = new List<IVariableFallbackHandler>();

        public void AddFallbackHandler<T>(T instance) where T : IVariableFallbackHandler
        {
            fallbackHandlers.Add(instance);
        }

        public void DefineMagicVariable(string name, Func<IMessage, string> getter)
        {
            magicVariables.Add(name, getter);
        }

        public string Substitute(string input, IMessage context)
        {
            // { variable => { key => value } }
            var memoizedDic = new Dictionary<string, Dictionary<string, string>>();

            return variableRegex.Replace(input, match =>
            {
                var var = match.Groups[1].Value.ToLower();
                var key = match.Groups[2].Success ? match.Groups[2].Value.ToLower() : null;

                var subVal = match.Value;

                if (key != null && memoizedDic.ContainsKey(var) && memoizedDic[var].ContainsKey(key))
                    subVal = memoizedDic[var][key];
                else if (magicVariables.ContainsKey(var))
                    subVal = magicVariables[var](context);
                else
                {
                    foreach (var fallback in fallbackHandlers)
                    {
                        var value = fallback.Fallback(var, context);
                        if (value == null) continue;

                        subVal = value;
                        break;
                    }
                }

                if (key != null)
                {
                    if (!memoizedDic.ContainsKey(var))
                        memoizedDic[var] = new Dictionary<string, string>();
                    memoizedDic[var][key] = subVal;
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
