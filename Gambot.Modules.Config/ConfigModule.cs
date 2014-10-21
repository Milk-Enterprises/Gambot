using Gambot.Core;

namespace Gambot.Modules.Config
{
    public class ConfigModule : AbstractModule
    {
        public ConfigModule()
        {
            MessageHandlers.Add(new ConfigCommandHandler());
        }
    }
}
