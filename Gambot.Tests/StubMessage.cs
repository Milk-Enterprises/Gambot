using Gambot.Core;

namespace Gambot.Tests
{
    internal class StubMessage : IMessage
    {
        public bool Action { get; set; }
        public string Text { get; set; }
        public string To { get; set; }
        public string Where { get; set; }
        public string Who { get; set; }
    }
}
