﻿using System;
using Gambot.Core;
using SlackAPI;

namespace Gambot.IO.Slack
{
    public class SlackMessenger : IMessenger
    {
        protected SlackSocketClient client;

        public event EventHandler<MessageEventArgs> MessageReceived;

        public SlackMessenger()
        {
            var token = Config.Get("Slack.Token");
            var name = Config.Get("Name", "gambot");
            client = new SlackSocketClient(token);
            client.TestAuth((AuthTestResponse obj) =>
            {
                Console.WriteLine("Result: " + obj.error);
            });

            client.OnMessageReceived += (slackMessage) =>
            {
                var message = new SlackMessage(slackMessage, client.UserLookup);
                Console.WriteLine(slackMessage.text);
                MessageReceived?.Invoke(this,
                                        new MessageEventArgs
                                        {
                                            Message = message,
                                            Addressed = String.Equals(message.To, name, StringComparison.CurrentCultureIgnoreCase)
                                        });
            };

            Console.WriteLine("Connecting...");
            client.Connect((LoginResponse obj) => {
                obj.AssertOk();
                Console.WriteLine("Connected.");
            });
        }

        public void SendMessage(string message, string destination, bool action = false)
        {
            client.SendMessage((_) => {}, destination, message);
        }

        public void Dispose()
        {
            client.CloseSocket();
        }
    }
}
