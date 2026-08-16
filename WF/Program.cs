using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace WF
{
    internal static class Program
    {
        
        [STAThread]
        static void Main()
        {
            string inputText = File.ReadAllText("snow_tweets2014.txt");
            TweetsExstraction tweet = new TweetsExstraction(inputText);
            tweet.Exstraction();

            List<Tweet> tweets = new List<Tweet>();
            tweets = tweet.Exstraction();

            TweetsAnalyzer analyzedTweets = new TweetsAnalyzer();
            analyzedTweets.TweetAnalyzer(tweets);

            StatesAnalyzer stateFromTweets = new StatesAnalyzer();
            stateFromTweets.FindStateByCoordinates(tweets);

            // Получаем результаты анализа
           
            Dictionary<string, double> stateSentiments = stateFromTweets.GetSentiments();


            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new Form1(stateSentiments));
        }
    }
}
