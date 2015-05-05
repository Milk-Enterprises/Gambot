using System;
using System.Text.RegularExpressions;
using System.Linq;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Modules.Ratings
{
    public class RatingsProducer : IMessageProducer
    {
        private class RatingsResult 
        {
            public int Score { get { return Up - Down; } }
            public int Up { get; set; }
            public int Down { get; set; }
        }

        IDataStore ratingsStore;

        public void Initialize(IDataStoreManager dataStoreManager)
        {
            ratingsStore = dataStoreManager.Get("Ratings");
        }

        public ProducerResponse Process(IMessage message, bool addressed)
        {
            var match = Regex.Match(message.Text, @"^(.+)(\+\+|--)$");
            if (match.Success)
            {
                ratingsStore.Put(match.Groups[1].Value, match.Groups[2].Value);
                var result = GetScore(match.Groups[1].Value);

                return new ProducerResponse(String.Format("\"{0}\" is now at {1:+#;-#;0}", match.Groups[1].Value, result.Score), false);
            }

            match = Regex.Match(message.Text, @"^(?:what(?:'| i)s the )?score for (.+)$");
            if (match.Success)
            {
                var result = GetScore(match.Groups[1].Value);

                return new ProducerResponse(String.Format("\"{0}\" is at {1:+#;-#;0} with {2} up and {3} down",
                        match.Groups[1].Value, result.Score, result.Up, result.Down), false);
            }

            match = Regex.Match(message.Text, @"^top (?:ten|10)$");
            if (match.Success)
            {
                var top = ratingsStore.GetAllKeys().Select(key => 
                    new { key, result = GetScore(key) }).OrderByDescending(x => x.result.Score).Take(10).ToList();

                var strings = new string[top.Count];
                for (var i = 0; i < top.Count; i++)
                {
                    strings[i] = String.Format("#{0}: {1} ({2:+#;-#;0})", i + 1, top[i].key, top[i].result.Score);
                }

                return new ProducerResponse(String.Join(", ", strings), false);
            }

            match = Regex.Match(message.Text, @"^bottom (?:ten|10)$");
            if (match.Success)
            {
                var top = ratingsStore.GetAllKeys().Select(key => 
                    new { key, result = GetScore(key) }).OrderBy(x => x.result.Score).Take(10).ToList();

                var strings = new string[top.Count];
                for (var i = 0; i < top.Count; i++)
                {
                    strings[i] = String.Format("#{0}: {1} ({2:+#;-#;0})", i + 1, top[i].key, top[i].result.Score);
                }

                return new ProducerResponse(String.Join(", ", strings), false);
            }

            return null;
        }

        private RatingsResult GetScore(string key)
        {
            var ratings = ratingsStore.GetAllValues(key).ToList();
            var up = ratings.Count(dsv => dsv.Value == "++");
            var down = ratings.Count - up;
            return new RatingsResult
            {
                Up = up,
                Down = down
            };
        }
    }
}

