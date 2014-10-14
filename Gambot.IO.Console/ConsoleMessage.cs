using System;
using Gambot.Core;

namespace Gambot.IO.Console
{
    public class ConsoleMessage : IMessage
    {
        public string Who { get; protected set; }
        public string To { get; protected set; }
        public string Text { get; protected set; }
        public string Where { get; protected set; }
        public bool Action { get; protected set; }

        public ConsoleMessage(string message)
        {
            Who = Environment.UserName;
            To = Config.Get("Name", "gambot");

            if (message.StartsWith("/me "))
            {
                Action = true;
                Text = message.Substring(4);
                ;
            }
            else
                Text = message;
        }
    }
}
