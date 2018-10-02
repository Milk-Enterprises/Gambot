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
            "<reply> \"$who\" rolled a \"$diceRoll\"";
        private const string DefaultRollFailedReply =
            "<reply> That's a leaner";

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
                Match match = Regex.Match(message.Text.Trim(), "roll (.+)");
                if (match.Success)
                {
                    string diceSyntaxRegex = @"^(\d+)[dD](\d+)$|^(\d+)$";
                    string[] rolls = match.Groups[1].Value.Split('+').Select(roll => roll.Trim());
                    bool validExpressions = rolls.All(
                        roll => 
                        {
                            Match diceSyntax = Regex.Match(roll, diceSyntaxRegex);
                            return diceSyntax.Success;
                        }
                    );
                    if (!validExpressions)
                    {
                        // wgas.gif
                        /*
                        var rollFailedFactoidStr = factoidDataStore.GetRandomValue("dice roll failed reply")?.Value ?? DefaultRollFailedReply;
                        var rollFailedFactoid = FactoidUtilities.GetVerbAndResponseFromPartialFactoid(rollFailedFactoidStr);
                        return new ProducerResponse(variableHandler.Substitute(rollFailedFactoid.Response, message), false);
                        */
                        return new ProducerResponse(variableHandler.Substitute("That's a leaner", message), false);
                    }

                    Random rngesus;
                    int sumOfRolls = rolls.Sum(
                        roll =>
                        {
                            Match diceSyntax = Regex.Match(roll, diceSyntaxRegex);

                            Group numDice = diceSyntax.Groups[1];
                            Group valueRange = diceSyntax.Groups[2];
                            Group constValue = diceSyntax.Groups[3];

                            int total = (constValue.Success) ? constValue.Value : 0;
                            if (!constValue.Success)
                            {
                                for (int i = 0; i < numDice.Value; ++i)
                                {
                                    total += rngesus.Next(valueRange.Value);
                                }
                            }
                            return total;
                        }
                    );
                    // wgas.gif
                    /*
                    var rollSuccessFactoidStr = factoidDataStore.GetRandomValue("dice roll success reply")?.Value ?? DefaultRollSuccessReply;
                    var rollSuccessFactoid = FactoidUtilities.GetVerbAndResponseFromPartialFactoid(rollSuccessFactoidStr);
                    var coercedResponse = Regex.Replace(rollSuccessFactoid.Response, @"\$(?:diceRoll)", sumOfRolls, RegexOptions.IgnoreCase);
                    return new ProducerResponse(variableHandler.Substitute(coercedResponse, message), false);
                    */
                    return new ProducerResponse(variableHandler.Substitute($"Rolled a {sumOfRolls}"), false);
                }
            }
            return null;
        }
    }
}
