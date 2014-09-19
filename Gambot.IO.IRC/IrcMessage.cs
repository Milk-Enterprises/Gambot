using System.Text.RegularExpressions;
using ChatSharp;

namespace Gambot.IO.IRC
{
    public class IrcMessage : IMessage
    {
        public string Who    { get; protected set; }
        public string To     { get; protected set; }
        public string Text   { get; protected set; }
        public string Where  { get; protected set; }
        public bool   Action { get; protected set; }

        public IrcMessage(PrivateMessage raw)
        {
            Who = raw.User.Nick;
            Where = raw.Source;

            var toMatch = Regex.Match(raw.Message, @"(.+):\s");
            if (toMatch.Success)
            {
                To = toMatch.Captures[1].Value;
                Text = raw.Message.Substring(toMatch.Length);
            }
            else
                Text = raw.Message;
        }
    }
}
