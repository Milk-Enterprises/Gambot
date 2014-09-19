using ChatSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gambot
{
    public class Message : Gambot.IMessage
    {
        public string Who { get; protected set; }
        public string To { get; protected set; }
        public string Text { get; protected set; }
        public string Where { get; protected set; }
        public bool Action { get; protected set; }

        public Message(PrivateMessage raw)
        {
            Who = raw.User.Nick;
            Text = raw.Message;
            Where = raw.Source;
        }
    }
}
