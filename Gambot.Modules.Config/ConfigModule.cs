using Gambot.Core;

namespace Gambot.Modules.Config
{
    public class ConfigModule : AbstractModule
    {
        public ConfigModule()
        {
            MessageProducers.Add(new ConfigCommandProducer());
        }
    }
}
