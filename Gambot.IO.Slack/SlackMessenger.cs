using System;
using System.Linq;
using System.Threading.Tasks;
using Gambot.Core;
using SlackConnector;
using SlackConnector.Models;
using SCSlackMessage = SlackConnector.Models.SlackMessage;

namespace Gambot.IO.Slack
{
    public class SlackMessenger : IMessenger
    {
        public event EventHandler<MessageEventArgs> MessageReceived;

        private readonly string _name;
        private bool _deliberateDisconnect;
        private ISlackConnection _connection;

        public SlackMessenger()
        {
            _name = Config.Get("Name", "gambot");
            _deliberateDisconnect = false;
            Connect().Wait();
        }

        private async Task Connect()
        {
            Console.WriteLine("Connecting...");
            var token = Config.Get("Slack.Token");
            var connector = new SlackConnector.SlackConnector();
            _connection = await connector.Connect(token);
            Console.WriteLine("Connected!");
            _connection.OnMessageReceived += OnMessageReceived;
            _connection.OnDisconnect += OnDisconnect;
        }

        private void OnDisconnect()
        {
            if (!_deliberateDisconnect)
            {
                Reconnect();
            }
        }

        private void Reconnect()
        {
            Console.WriteLine("Reconnecting...");
            if (_connection != null)
            {
                _connection.OnMessageReceived -= OnMessageReceived;
                _connection.OnDisconnect -= OnDisconnect;
                _connection = null;
            }
            Connect().Wait();
        }

        private async Task Disconnect()
        {
            _deliberateDisconnect = true;
            if (_connection != null && _connection.IsConnected)
            {
                await _connection.Close();
            }
            _connection = null;
        }

        private Task OnMessageReceived(SCSlackMessage raw)
        {
            var who = raw.User.Name;
            var where = raw.ChatHub;
            foreach (var text in raw.Text.Split('\n'))
            {
                var isAction = raw.MessageSubType == SlackMessageSubType.MeMessage;
                var message = new SlackMessage(who, where.Id, text, isAction);
                Console.WriteLine($"({where.Name}) {who}{(isAction ? " " : ": ")}{message.Text}");

                var addressed = where.Type == SlackChatHubType.DM
                    || String.Equals(message.To, _name, StringComparison.CurrentCultureIgnoreCase);
                MessageReceived?.Invoke(this,
                                        new MessageEventArgs
                                        {
                                            Message = message,
                                            Addressed = addressed,
                                        });
            }
            return Task.CompletedTask;
        }

        public void SendMessage(string message, string destination, bool action = false)
        {
            var hub = GetChatHub(destination);
            if (hub == null)
            {
                Console.WriteLine($"Could not send message <{message}>: Could not find destination <{destination}>");
                return;
            }
            Console.WriteLine($"({hub.Name}) {_name}: {message}");
            _connection.Say(new BotMessage
            {
                ChatHub = hub,
                Text = message,
            }).Wait();
        }

        private SlackChatHub GetChatHub(string destination)
        {
            if (destination.StartsWith("#") || destination.StartsWith("@"))
            {
                return _connection.ConnectedHubs.Values.FirstOrDefault(h => h.Name == destination);
            }
            return _connection.ConnectedHubs[destination];
        }

        public async void Dispose()
        {
            await Disconnect();
        }
    }
}
