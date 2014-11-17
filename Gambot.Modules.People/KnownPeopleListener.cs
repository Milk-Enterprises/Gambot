using System;
using System.Collections.Generic;
using System.Linq;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.People
{
    internal class KnownPeopleListener : IMessageListener
    {
        private readonly IVariableHandler variableHandler;

        protected class Person
        {
            public string Name { get; set; }
            public string Room { get; set; }
            public DateTime LastActive { get; set; }
        }

        protected List<Person> knownPeople = new List<Person>();
        protected Random random;

        public static string LastReferencedPerson { get; protected set; }

        internal KnownPeopleListener(IVariableHandler variableHandler)
        {
            this.variableHandler = variableHandler;
        }

        protected string GetSomeone(string room)
        {
            var recentPeople =
                knownPeople.Where(p => p.Room == room &&
                                       DateTime.Now - p.LastActive <
                                       TimeSpan.FromMinutes(15)).ToList();
            return recentPeople.Any()
                       ? recentPeople[random.Next(recentPeople.Count)].Name
                       : null;
        }

        public void Initialize(IDataStoreManager dataStoreManager)
        {
            random = new Random();

            variableHandler.DefineMagicVariable("who", (message) =>
            {
                LastReferencedPerson = message.Who;
                return message.Who;
            });

            variableHandler.DefineMagicVariable("to", (message) =>
            {
                var person = message.To ??
                             GetSomeone(message.Where) ?? message.Who;
                LastReferencedPerson = person;
                return person;
            });

            variableHandler.DefineMagicVariable("someone", (message) =>
            {
                var person = GetSomeone(message.Who) ?? message.Who;
                LastReferencedPerson = person;
                return person;
            });
        }

        public void Listen(IMessage message, bool addressed)
        {
            var person =
                knownPeople.SingleOrDefault(
                    p => p.Name == message.Who && p.Room == message.Where);
            if (person != null)
                person.LastActive = DateTime.Now;
            else
            {
                knownPeople.Add(new Person
                                {
                                    Name = message.Who,
                                    Room = message.Where,
                                    LastActive = DateTime.Now
                                });
            }
        }
    }
}
