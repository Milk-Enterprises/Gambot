using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Gambot.Core
{
    public interface IVariableHandler
    {
        void AddFallbackHandler<T>(T instance)
            where T : IVariableFallbackHandler;

        void DefineMagicVariable(string name, Func<IMessage, string> getter);
        string Substitute(string input, IMessage context, params VariableReplacement[] replacements);
    }

    public class VariableReplacement
    {
        public string VariableToReplace { get; internal set; }
        public string ValueToReplaceWith { get; internal set; }
    }

    public static class Replace
    {
        public static VariableReplacement VarWith(string varToReplace,
                                                  string valueToReplaceWith)
        {
            return new VariableReplacement()
                   {
                       VariableToReplace = varToReplace,
                       ValueToReplaceWith = valueToReplaceWith
                   };
        }
    }

    public class VariableHandler : IVariableHandler
    {
        private Dictionary<string, Func<IMessage, string>> magicVariables =
            new Dictionary<string, Func<IMessage, string>>();

        private readonly Regex variableRegex =
            new Regex(@"((?:^| )an? )?\$([a-z][a-z0-9_]*(?<!_))(?:\[([^\]]+)\])?",
                      RegexOptions.IgnoreCase);

        private readonly List<IVariableFallbackHandler> fallbackHandlers =
            new List<IVariableFallbackHandler>();

        public void AddFallbackHandler<T>(T instance)
            where T : IVariableFallbackHandler
        {
            fallbackHandlers.Add(instance);
        }

        public void DefineMagicVariable(string name,
                                        Func<IMessage, string> getter)
        {
            magicVariables.Add(name, getter);
        }

        public string Substitute(string input, IMessage context, params VariableReplacement[] replacements)
        {
            // { variable => { key => value } }
            var memoizedDic =
                new Dictionary<string, Dictionary<string, string>>();
            var memoizedReplacements =
                replacements.ToDictionary(vr => vr.VariableToReplace,
                                          vr => vr.ValueToReplaceWith);

            return variableRegex.Replace(input, match =>
            {
                var var = match.Groups[2].Value.ToLower();
                var key = match.Groups[3].Success
                              ? match.Groups[3].Value.ToLower()
                              : null; 

                var subVal = match.Value;

                if (memoizedReplacements.ContainsKey(var))
                    subVal = memoizedReplacements[var];
                else if (key != null && memoizedDic.ContainsKey(var) &&
                    memoizedDic[var].ContainsKey(key))
                    subVal = memoizedDic[var][key];
                else if (magicVariables.ContainsKey(var))
                    subVal = magicVariables[var](context);
                else
                {
                    foreach (var fallback in fallbackHandlers)
                    {
                        var value = fallback.Fallback(var, context);
                        if (value == null)
                            continue;

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

                subVal = MatchCase(match.Groups[2].Value, subVal);

                // a/an
                if (match.Groups[1].Success)
                {
                    var an = match.Groups[1].Value;
                    var leadingSpace = an[0] == ' ';
                    var vowel = new[] {'a', 'e', 'i', 'o', 'u'}.Contains(subVal.ToLower()[0]);
                    subVal = (leadingSpace ? " " : "") +
                             MatchCase(match.Groups[1].Value.Trim(), vowel ? "an" : "a") + 
                             " " + subVal;
                }

                return subVal;
            });
        }

        public static string MatchCase(string sourceCase, string destination)
        {
            if (sourceCase.Length > 1 && sourceCase.All(c => !Char.IsLetter(c) || Char.IsUpper(c)))
                return destination.ToUpper();
            if (Char.IsUpper(sourceCase[0]))
                return String.Join(" ",
                    destination.Split(' ').Where(word => !String.IsNullOrWhiteSpace(word))
                                          .Select(word =>
                                             Char.ToUpper(word[0]).ToString() +
                                             word.Substring(1)));
            return destination;
        }
    }
}
