using System.Collections.Generic;
using System.IO;

namespace WF
{
    public class TweetProcessor
    {
        public Dictionary<string, double> ProcessTweetsFromFile(string filename)
        {
            string inputText = File.ReadAllText(filename);
            TweetsExstraction tweet = new TweetsExstraction(inputText);
            List<Tweet> tweets = tweet.Exstraction();

            TweetsAnalyzer analyzedTweets = new TweetsAnalyzer();
            analyzedTweets.TweetAnalyzer(tweets);

            StatesAnalyzer stateFromTweets = new StatesAnalyzer();
            stateFromTweets.FindStateByCoordinates(tweets);

            return stateFromTweets.GetSentiments();
        }
    }
}