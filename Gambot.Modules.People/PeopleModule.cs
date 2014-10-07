using System.Collections.Generic;
using Gambot.Core;

namespace Gambot.Modules.People
{
    public class PeopleModule : AbstractModule
    {
        public PeopleModule(IVariableHandler variableHandler)
        {
            MessageHandlers.Add(new GenderHandler(variableHandler));
            MessageHandlers.Add(new PeopleHandler(variableHandler));
        }
    }
}
