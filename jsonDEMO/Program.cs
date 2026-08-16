using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;


namespace Project_1._Twitter_Trends
{

    class Program
    {
        static void Main(string[] args)
        {
            string inputText = File.ReadAllText("shopping_tweets2014.txt");
            TweetsExstraction tweet = new TweetsExstraction(inputText);
            tweet.Exstraction();

            List<Tweet> tweets = new List<Tweet>();
            tweets = tweet.Exstraction();

            TweetsAnalyzer analyzedTweets = new TweetsAnalyzer();
            analyzedTweets.TweetAnalyzer(tweets);

            StatesAnalyzer stateFromTweets = new StatesAnalyzer();
            stateFromTweets.FindStateByCoordinates(tweets);

            // Получаем результаты анализа
            var stateSentiments = stateFromTweets.GetSentiments();

            // Запускаем Windows Forms приложение 
            RunWF();
        }
        static void RunWF()
        {
            string wfExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WF.exe");

            if (!File.Exists(wfExePath))
            {
                Console.WriteLine("Ошибка: WF.exe не найден в папке с приложением.");
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = wfExePath,
                UseShellExecute = true
            });
        }
    }
}

