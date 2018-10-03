using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Roll
{
    public class RollResponseProducer : IMessageProducer
    {
        private IDataStore factoidDataStore;
        private readonly IVariableHandler variableHandler;

        private const string DefaultRollSuccessReply =
            "<reply> $who rolled a $diceRoll.";
        private const string DefaultRollFailedReply =
            "<reply> That's a leaner.";

        public RollResponseProducer(IVariableHandler variableHandler)
        {
            this.variableHandler = variableHandler;
        }

        public void Initialize(IDataStoreManager dataStoreManager)
        {
            factoidDataStore = dataStoreManager.Get("Factoids");
        }

        public ProducerResponse Process(IMessage message, bool addressed)
        {
            if (addressed)
            {
                var trimmedMessage = message.Text.Trim();
                var match = Regex.Match(trimmedMessage, "roll (.+)");
                if (match.Success)
                {
                    var diceSyntaxRegex = @"^(\d+)[dD](\d+)$|^(\d+)$";
                    var rolls = match.Groups[1].Value.Split('+').Select(roll => roll.Trim());
                    bool validExpressions = rolls.All(
                        roll => 
                        {
                            var diceSyntax = Regex.Match(roll, diceSyntaxRegex);
                            return diceSyntax.Success;
                        }
                    );
                    if (!validExpressions)
                    {
                        var rollFailedFactoidStr = factoidDataStore.GetRandomValue("dice roll failed reply")?.Value ?? DefaultRollFailedReply;
                        var rollFailedFactoid = FactoidUtilities.GetVerbAndResponseFromPartialFactoid(rollFailedFactoidStr);
                        return new ProducerResponse(variableHandler.Substitute(rollFailedFactoid.Response, message), false);
                    }

                    Random rngesus = new Random();
                    int sumOfRolls = rolls.Sum(
                        roll =>
                        {
                            var diceSyntax = Regex.Match(roll, diceSyntaxRegex);

                            var numDice = diceSyntax.Groups[1];
                            var valueRange = diceSyntax.Groups[2];
                            var constValue = diceSyntax.Groups[3];

                            var total = (constValue.Success) ? Convert.ToInt32(constValue.Value) : 0;
                            if (!constValue.Success)
                            {
                                for (int i = 0; i < Convert.ToInt32(numDice.Value); ++i)
                                {
                                    total += rngesus.Next(Convert.ToInt32(valueRange.Value) + 1);
                                }
                            }
                            return total;
                        }
                    );

                    var rollSuccessFactoidStr = factoidDataStore.GetRandomValue("dice roll success reply")?.Value ?? DefaultRollSuccessReply;
                    var rollSuccessFactoid = FactoidUtilities.GetVerbAndResponseFromPartialFactoid(rollSuccessFactoidStr);
                    var coercedResponse = Regex.Replace(rollSuccessFactoid.Response, @"\$(?:diceRoll)", sumOfRolls.ToString(), RegexOptions.IgnoreCase);
                    return new ProducerResponse(variableHandler.Substitute(coercedResponse, message), false);
                }
            }
            return null;
        }
    }
}
