using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatSharp;
using Gambot.Core;

namespace Gambot.IO.IRC
{
    public class IrcMessenger : IMessenger
    {
        protected IrcClient Client; 

        public event EventHandler<MessageEventArgs> MessageReceived;

        public IrcMessenger()
        {
            var server   = Config.Get("Irc.Server");
            var nick     = Config.Get("Irc.Nick", Config.Get("Name", "gambot"));
            var user     = Config.Get("Irc.User", nick);
            var password = Config.Get("Irc.Password");
            var ssl      = Config.GetBool("Irc.Ssl");
            Client = new IrcClient(server, new IrcUser(nick, user, password), ssl);

            Client.PrivateMessageRecieved += (sender, args) =>
            {
                if (MessageReceived != null)
                {
                    var message = new IrcMessage(args.PrivateMessage);
                    MessageReceived(this,
                        new MessageEventArgs
                        {
                            Message = message,
                            Addressed = (!args.PrivateMessage.IsChannelMessage || 
                                String.Equals(message.To, nick, StringComparison.CurrentCultureIgnoreCase))
                        });
                }
            };

            Client.ConnectAsync();
        }

        public void SendMessage(string message, string destination, bool action = false)
        {
            if (action)
                Client.SendAction(message, destination);
            else
                Client.SendMessage(message, destination);
        }

        public void Dispose()
        {
            Client.Quit();
        }
    }
}
