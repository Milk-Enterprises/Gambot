using System.Text.RegularExpressions;
using Gambot.Core;
using SlackAPI.WebSocketMessages;
namespace Gambot.IO.Slack
{
    public class SlackMessage : IMessage
    {
        public string Who { get; protected set; }
        public string To { get; protected set; }
        public string Text { get; protected set; }
        public string Where { get; protected set; }
        public bool Action { get; protected set; }

        public SlackMessage(NewMessage raw)
        {
            Who = raw.user;
            Where = raw.channel;

            var toMatch = Regex.Match(raw.text, @"^((?:[^:]+?)|(?::.+?:))[,:]\s");
            if (toMatch.Success)
            {
                To = toMatch.Groups[1].Value;
                Text = raw.text.Substring(toMatch.Length);
            }
            else
                Text = raw.text;
        }
    }
}
