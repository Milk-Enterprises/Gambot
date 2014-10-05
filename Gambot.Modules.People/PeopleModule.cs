using System.Collections.Generic;
using Gambot.Core;

namespace Gambot.Modules.People
{
    public class PeopleModule : AbstractModule
    {
        public PeopleModule()
        {
            MessageHandlers.Add(new GenderHandler());
            MessageHandlers.Add(new PeopleHandler());
        }
    }
}
