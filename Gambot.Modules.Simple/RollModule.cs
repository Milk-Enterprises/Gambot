using Gambot.Core;

namespace Gambot.Modules.Roll
{
    public class RollModule : AbstractModule
    {
        public RollModule()
        {
            MessageProducers.Add(new RollResponseProducer());
        }
    }
}
