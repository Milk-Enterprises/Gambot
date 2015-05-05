using System.Collections.Generic;
using Gambot.Core;

namespace Gambot.Modules.Mohammed
{
    public class PeopleModule : AbstractModule
    {
        public PeopleModule(IVariableHandler variableHandler)
        {
            MessageTransformers.Add(new MohammedTransformer());
        }
    }
}
