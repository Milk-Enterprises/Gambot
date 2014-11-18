using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gambot.Core;

namespace Gambot.Modules.Conjugation
{
    public class ConjugationModule : AbstractModule
    {
        public ConjugationModule()
        {
            MessageProducers.Add(new ConjugationProducer());
        }
    }
}
