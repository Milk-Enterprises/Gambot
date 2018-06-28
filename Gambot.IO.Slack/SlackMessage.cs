using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using Gambot.Core;
using SlackAPI;
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

        public SlackMessage(NewMessage raw, IDictionary<string, User> userLookup)
        {
            Who = raw.subtype == "bot_message"
                ? "slackbot" // probably
                : userLookup[raw.user].name;
            Where = raw.channel;

            var text = WebUtility.HtmlDecode(raw.text);
            var toMatch = Regex.Match(text, @"^((?:[^:]+?)|(?::.+?:))[,:]\s");
            if (toMatch.Success)
            {
                To = toMatch.Groups[1].Value;
                Text = text.Substring(toMatch.Length);
            }
            else
                Text = text;
        }
    }
}
