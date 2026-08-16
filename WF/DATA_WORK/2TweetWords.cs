using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace WF
{
    public class TweetsExstraction 
    {
        public List<Tweet> Tweets { get; set; } = new List<Tweet>();
        public string tweetsInfo { get; set; }

        public TweetsExstraction(string inputTweets)
        {
            tweetsInfo = inputTweets;
        }

        public List<Tweet> Exstraction()
        {
            string[] lines = tweetsInfo.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                List<string> tweet = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (tweet.Count > 3)
                {
                    string[] location = tweet[0].Split(' ');
                    double latitude = LocationToDouble(location[0]);
                    double longitude = LocationToDouble(location[1]);

                    string tweetText = tweet[3];
                    List<string> cleanedWords = CleanTweetText(tweetText);

                    Tweet parsedTweet = new Tweet(cleanedWords, latitude, longitude);

                    if (parsedTweet.Text != null)
                    {
                        Tweets.Add(parsedTweet);
                    }
                }
            }
            return Tweets;
        }

        private List<string> CleanTweetText(string tweetText)
        {
            string cleanedText = Regex.Replace(tweetText, @"[^\w\s'-]", string.Empty);
            return cleanedText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private double LocationToDouble(string location)
        {
            location = location.Replace("[", "").Replace(",", "").Replace("]", "");
            double locNumber;
            Double.TryParse(location, NumberStyles.Any, CultureInfo.InvariantCulture, out locNumber);
            return locNumber;
        }
    }
}
