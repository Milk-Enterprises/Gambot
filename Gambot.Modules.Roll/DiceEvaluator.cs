using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gambot.Modules.Roll
{
    public class DiceToken
    {
        public bool IsNumber { get; set; }
        public string StringValue { get; set; }
        public int IntValue { get; set; }
        public DiceToken PrecedingToken { get; set; }
        public DiceToken ProceedingToken { get; set; }
    }

    public class DiceExpressionResults
    {
        public bool IsValid { get; set; } = false;
        public float Result { get; set; } = 0;
        public List<string> RollDescriptionList { get; set; } = new List<string>();
        public string IndividualRollResultString { get; set; } = null;
    }

    /*
     * Credits to Simon Gellis for writing the typescript version
     * https://gist.github.com/SupernaviX/2a4ee385026ccc6566203db6c7464f01
     */
    public class DiceEvaluator
    {
        private static Random _RNGesus = new Random();

        private DiceExpressionResults _results;

        public DiceEvaluator()
        {
            _results = new DiceExpressionResults();
        }

        private int RollDie(int numberOfSides)
        {
            return _RNGesus.Next(numberOfSides) + 1;
        }

        private IEnumerable<int> RollDice(int numberOfDice, int numberOfSides)
        {
            List<int> rolls = new List<int>();
            for(var i = 0; i < numberOfDice; ++i)
                rolls.Add(RollDie(numberOfSides));

            var rollsCSV = String.Join(", ", rolls.Select(k => Convert.ToInt32(k)));
            _results.RollDescriptionList.Add($" (Rolled: {rollsCSV})");

            return rolls;
        }

        private IEnumerable<int> KeepHighest(int numberOfDice, int numberOfSides, int? numberToKeep = null)
        {
            IEnumerable<int> rolls = RollDice(numberOfDice, numberOfSides);
            return rolls.OrderByDescending(r => r).Take((numberToKeep == null) ? numberOfDice : numberToKeep.Value);
        }

        private IEnumerable<int> KeepLowest(int numberOfDice, int numberOfSides, int numberToKeep)
        {
            IEnumerable<int> rolls = RollDice(numberOfDice, numberOfSides);
            return rolls.OrderBy(r => r).Take(numberToKeep);
        }

        private string _Symbols = "khld*/+-()";
        private IEnumerable<DiceToken> Tokenize(string expression)
        {
            List<DiceToken> tokens = new List<DiceToken>();
            var expr = expression.ToLower();

            var i = 0;
            while (i < expr.Length)
            {
                if (_Symbols.Contains(expr[i]))
                {
                    var token = new DiceToken() { IsNumber = false, StringValue = expr[i].ToString() };
                    if (tokens.Count > 0)
                    {
                        token.PrecedingToken = tokens[tokens.Count - 1];
                        tokens[tokens.Count - 1].ProceedingToken = token;
                    }
                    tokens.Add(token);
                }
                else if (Char.IsDigit(expr[i]))
                {
                    var start = i;
                    do { ++i; } while (i < expr.Length && Char.IsDigit(expr[i]));

                    var token = new DiceToken() { IsNumber = true, IntValue = Convert.ToInt32(expr.Substring(start, i - start)) };
                    if (tokens.Count > 0)
                    {
                        token.PrecedingToken = tokens[tokens.Count - 1];
                        tokens[tokens.Count - 1].ProceedingToken = token;
                    }
                    tokens.Add(token);
                    continue;
                }
                ++i;
            }

            var tmp = new List<DiceToken>();
            foreach (var token in tokens)
            {
                if (token.StringValue == "d" && !(token.PrecedingToken != null && token.PrecedingToken.IsNumber))
                {
                    tmp.Add(new DiceToken() { IsNumber = true, IntValue = 1 });
                }

                tmp.Add(token);

                if ((token.StringValue == "h" || token.StringValue == "k" || token.StringValue == "l") && !(token.ProceedingToken != null && token.ProceedingToken.IsNumber))
                {
                    tmp.Add(new DiceToken() { IsNumber = true, IntValue = 1 });
                }
            }
            tokens.Clear();

            for (var tmpIdx = 0; tmpIdx < tmp.Count(); ++tmpIdx)
            {
                tmp[tmpIdx].ProceedingToken = null;
                tmp[tmpIdx].PrecedingToken = (tmpIdx == 0) ? null : tmp[tmpIdx - 1];

                if (tmpIdx > 0)
                {
                    tmp[tmpIdx - 1].ProceedingToken = tmp[tmpIdx];
                }
            }

            return tmp;
        }

        private bool IsHigherPrecedence(string first, string second)
        {
            var precString =_Symbols.Substring(0, _Symbols.Length - 2);
            var firstPrec = precString.IndexOf(first);
            var secondPrec = precString.IndexOf(second);
            return firstPrec <= secondPrec;
        }

        private int NumberOfArgumentsForSymbol(string symbol)
        {
            return (symbol == "k" || symbol == "h" || symbol == "l") ? 3 : 2;
        }

        // https://en.wikipedia.org/wiki/Shunting-yard_algorithm
        private IEnumerable<DiceToken> ConvertToReversePolishNotation(string expression)
        {
            var stack = new Stack<DiceToken>();
            var rpn = new Queue<DiceToken>();

            var tokens = Tokenize(expression);
            foreach (var token in tokens)
            {
                if (token.IsNumber)
                {
                    rpn.Enqueue(token);
                }
                else if (token.StringValue == "(")
                {
                    stack.Push(token);
                }
                else if (token.StringValue == ")")
                {
                    while (stack.Peek().StringValue != "(")
                    {
                        rpn.Enqueue(stack.Pop());
                    }
                    stack.Pop();
                }
                else
                {
                    while (
                        stack.Count > 0
                        && IsHigherPrecedence(stack.Peek().StringValue, token.StringValue)
                        && stack.Peek().StringValue != "("
                    )
                    {
                        rpn.Enqueue(stack.Pop());
                    }
                    if (NumberOfArgumentsForSymbol(token.StringValue) == 3)
                    {
                        // throw out extra token for ternary operators
                        stack.Pop();
                    }
                    stack.Push(token);
                }
            }

            while (stack.Count > 0)
            {
                rpn.Enqueue(stack.Pop());
            }

            return rpn;
        }

        public DiceExpressionResults Evaluate(string expression)
        {
            _results = new DiceExpressionResults();
            try
            {
                var numStack = new Stack<float>();
                foreach (var token in ConvertToReversePolishNotation(expression))
                {
                    if (token.IsNumber)
                    {
                        numStack.Push(token.IntValue);
                        continue;
                    }
                    else
                    {
                        var arguments = new Stack<float>();
                        for (var i = 0; i < NumberOfArgumentsForSymbol(token.StringValue); ++i)
                        {
                            arguments.Push(numStack.Pop());
                        }

                        float calc = 0;
                        if (token.StringValue == "h" || token.StringValue == "k" || token.StringValue == "l")
                        {
                            var keep = (token.StringValue != "l") ?
                                KeepHighest((int)arguments.Pop(), (int)arguments.Pop(), (int)arguments.Pop())
                                : KeepLowest((int)arguments.Pop(), (int)arguments.Pop(), (int)arguments.Pop());
                            var keepCSV = String.Join(", ", keep.Select(k => Convert.ToInt32(k)));
                            _results.RollDescriptionList.Add($"(Kept: {keepCSV})");
                            calc = keep.Sum();
                        }
                        else if (token.StringValue == "d")
                        {
                            var rolls = RollDice((int)arguments.Pop(), (int)arguments.Pop());
                            calc = rolls.Sum();
                        }
                        else if (token.StringValue == "*")
                        {
                            calc = arguments.Pop() * arguments.Pop();
                        }
                        else if (token.StringValue == "/")
                        {
                            calc = arguments.Pop() / arguments.Pop();
                        }
                        else if (token.StringValue == "+")
                        {
                            calc = arguments.Pop() + arguments.Pop();
                        }
                        else if (token.StringValue == "-")
                        {
                            calc = arguments.Pop() - arguments.Pop();
                        }

                        numStack.Push(calc);
                    }
                }

                _results.IsValid = true;
                _results.Result = numStack.ElementAt(0);
                _results.IndividualRollResultString = String.Join("", _results.RollDescriptionList).Trim();

                return _results;
            }
            catch (Exception e)
            {
                _results.IsValid = false;
                return _results;
            }
        }
    }
}
