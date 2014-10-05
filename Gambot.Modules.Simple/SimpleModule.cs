using Gambot.Core;

namespace Gambot.Modules.Simple
{
    public class SimpleModule : AbstractModule
    {
        public SimpleModule()
        {
            MessageHandlers.Add(new SimpleResponseHandler());
        }
    }
}
