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

        public StubMessage(string text = "", string to = "", string @where = "",
                           string who = "", bool action = false)
        {
            Action = action;
            Text = text;
            To = to;
            Where = @where;
            Who = who;
        }
    }
}
