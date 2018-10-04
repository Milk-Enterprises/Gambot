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
        private DiceEvaluator diceEval;

        // $diceQuery   Expression evaluated
        // $diceRoll    Computed value
        // $diceCast    Dice rolled and kept string
        private const string DefaultRollSuccessReply =
            "<reply> $Who rolled a $diceRoll.";
        private const string DefaultRollFailedReply =
            "<reply> That's a leaner.";

        public RollResponseProducer(IVariableHandler variableHandler)
        {
            this.variableHandler = variableHandler;
            this.diceEval = new DiceEvaluator();
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
                    var originalQuery = match.Groups[1].Value.Trim();

                    var results = diceEval.Evaluate(originalQuery);

                    var sumOfRolls = results.Result;
                    var validExpression = results.IsValid;
                    var rollsDescription = results.IndividualRollResultString;

                    if (!validExpression)
                    {
                        var rollFailedFactoidStr = factoidDataStore.GetRandomValue("dice roll failed reply")?.Value ?? DefaultRollFailedReply;
                        var rollFailedFactoid = FactoidUtilities.GetVerbAndResponseFromPartialFactoid(rollFailedFactoidStr);
                        return new ProducerResponse(variableHandler.Substitute(rollFailedFactoid.Response, message), false);
                    }

                    var rollSuccessFactoidStr = factoidDataStore.GetRandomValue("dice roll success reply")?.Value ?? DefaultRollSuccessReply;
                    var rollSuccessFactoid = FactoidUtilities.GetVerbAndResponseFromPartialFactoid(rollSuccessFactoidStr);

                    var replaceForOriginal = Regex.Replace(rollSuccessFactoid.Response, @"\$(?:diceQuery)", originalQuery, RegexOptions.IgnoreCase);
                    var replaceForIndividuals = Regex.Replace(replaceForOriginal, @"\$(?:diceCast)", rollsDescription, RegexOptions.IgnoreCase);
                    var replaceForTotal = Regex.Replace(replaceForIndividuals, @"\$(?:diceRoll)", sumOfRolls.ToString(), RegexOptions.IgnoreCase);

                    return new ProducerResponse(variableHandler.Substitute(replaceForTotal, message), false);
                }
            }
            return null;
        }
    }
}
