using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace WF
{
    public class TweetsAnalyzer
    {
        public List<Tweet> AnalyzedTweets = new List<Tweet>();
        public Dictionary<string, double> Sentiments;

        public TweetsAnalyzer()
        {
            Sentiments = LoadSentiments();
        }

        private Dictionary<string, double> LoadSentiments()
        {
            Dictionary<string, double> sentiments = new Dictionary<string, double>();
            string[] lines = File.ReadAllLines("sentiments.csv");

            foreach (string line in lines)
            {
                string[] parts = line.Split(',');

                string phrase = parts[0].Trim().ToLower();
                if (double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                {
                    sentiments[phrase] = value;
                }
            }
            return sentiments;
        }

        public void TweetAnalyzer(List<Tweet> tweets)
        {
            foreach (Tweet tweet in tweets)
            {
                List<string> words = tweet.Text.Select(w => w.ToLower()).ToList();
                List<double> matchedSent = new List<double>();
                HashSet<int> matchedIndices = new HashSet<int>(); 

                for (int phraseLength = 7; phraseLength >= 1; phraseLength--)
                {
                    for (int i = 0; i <= words.Count - phraseLength; i++)
                    {
                        if (matchedIndices.Contains(i)) continue;

                        string phrase = "";
                        for (int j = i; j < i + phraseLength; j++)
                        {
                            phrase += (j > i ? " " : "") + words[j];
                        }

                        if (Sentiments.TryGetValue(phrase, out double score))
                        {
                            matchedSent.Add(score); 
                            for (int j = i; j < i + phraseLength; j++)
                            {
                                matchedIndices.Add(j);
                            }
                        }
                    }
                }

                for (int i = 0; i < words.Count; i++)
                {
                    if (!matchedIndices.Contains(i) && Sentiments.TryGetValue(words[i], out double score))
                    {
                        matchedSent.Add(score);
                    }
                }

                tweet.Sentiment = matchedSent.Count > 0 ? matchedSent.Average() : double.NaN;
            }
        }
    }
}

//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.IO; 
//using System.Linq;
//using static System.Net.Mime.MediaTypeNames;

//namespace WF
//{
//    public class TweetsAnalyzer
//    {
//        public List<Tweet> AnalyzedTweets = new List<Tweet>();
//        public Dictionary<string, double> Sentiments;

//        public TweetsAnalyzer()
//        {
//            Sentiments = LoadSentiments();
//        }

//        private Dictionary<string, double> LoadSentiments()
//        {
//            var sentiments = new Dictionary<string, double>();
//            var lines = File.ReadAllLines("sentiments.csv");

//            foreach (var line in lines)
//            {
//                var parts = line.Split(',');

//                var phrase = parts[0].Trim().ToLower();
//                if (double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
//                {
//                    sentiments[phrase] = value;
//                }
//            }
//            return sentiments;
//        }

//        // Метод для анализа твитов
//        public void TweetAnalyzer(List<Tweet> tweets)
//        {
//            foreach (var tweet in tweets)
//            {
//                // Приводим текст твита к нижнему регистру и разбиваем на слова
//                var words = tweet.Text.Select(w => w.ToLower()).ToList();
//                var matchedScores = new List<double>();
//                var matchedIndices = new HashSet<int>(); // Для отслеживания учтённых слов

//                // Сначала проверяем фразы от самых длинных (7 слов) до отдельных слов
//                for (int phraseLength = 7; phraseLength >= 1; phraseLength--)
//                {
//                    for (int i = 0; i <= words.Count - phraseLength; i++)
//                    {
//                        // Пропускаем уже учтённые слова
//                        if (matchedIndices.Contains(i)) continue;

//                        var phrase = string.Join(" ", words.Skip(i).Take(phraseLength));
//                        // Проверяем, есть ли фраза в словаре сентиментов
//                        if (Sentiments.TryGetValue(phrase, out var score))
//                        {
//                            matchedScores.Add(score); // Добавляем сентимент в список
//                                                      // Помечаем все слова фразы как учтённые
//                            for (int j = i; j < i + phraseLength; j++)
//                            {
//                                matchedIndices.Add(j);
//                            }
//                        }
//                    }
//                }

//                // Добавляем сентименты для оставшихся отдельных слов
//                for (int i = 0; i < words.Count; i++)
//                {
//                    if (!matchedIndices.Contains(i) && Sentiments.TryGetValue(words[i], out var score))
//                    {
//                        matchedScores.Add(score); // Добавляем сентимент для отдельного слова
//                    }
//                }

//                // Рассчитываем средний сентимент или NaN, если сентименты не найдены
//                tweet.Sentiment = matchedScores.Count > 0 ? matchedScores.Average() : double.NaN;
//            }
//        }

//    }
//}
