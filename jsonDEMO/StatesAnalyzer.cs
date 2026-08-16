using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Project_1._Twitter_Trends
{
    public class StatesAnalyzer
    {
        public Dictionary<string, double> GetSentiments()
        {
            return MiddleSentimentsState;
        }



        private Dictionary<string, double> MiddleSentimentsState = new Dictionary<string, double>(); 

        private Dictionary<string, object> StatesLocation = new Dictionary<string, object>();

        public StatesAnalyzer()//Tweet tweet)
        {
            statesLocation();
        }

        private void statesLocation()
        {
            string json = "states.json";
            StatesLocation = jsonToDictionary(json);
            
        }       
        private Dictionary<string, object> jsonToDictionary(string filePath)
        {
            string jsonString = File.ReadAllText(filePath);
            var jsonDocument = JsonDocument.Parse(jsonString);

            Dictionary<string, object> result = new Dictionary<string, object>();

            foreach (var element in jsonDocument.RootElement.EnumerateObject())
            {
                string key = element.Name;
                var value = element.Value;

                if (value.ValueKind == JsonValueKind.Array)
                {
                    if (IsLIstListList(value))
                    {
                        result[key] = JsonSerializer.Deserialize<List<List<List<double>>>>(value.GetRawText());
                    }
                    else if (IsLIstListListList(value))
                    {
                        result[key] = JsonSerializer.Deserialize<List<List<List<List<double>>>>>(value.GetRawText());
                    }
                }
            }
            return result;
        }
        private bool IsLIstListList(JsonElement element)
        {
            return element[0].ValueKind == JsonValueKind.Array && element[0][0].ValueKind == JsonValueKind.Array && element[0][0][0].ValueKind == JsonValueKind.Number;
        }
        private bool IsLIstListListList(JsonElement element)
        {
            return element[0].ValueKind == JsonValueKind.Array && element[0][0].ValueKind == JsonValueKind.Array && element[0][0][0].ValueKind == JsonValueKind.Array && element[0][0][0][0].ValueKind == JsonValueKind.Number;
        }



        public void FindStateByCoordinates(List<Tweet> tweets) 
        {
            Dictionary<string, List<double>> SentimentsStates = new Dictionary<string, List<double>>();
            foreach (var state in StatesLocation)
            { 
                SentimentsStates[state.Key] = new List<double>();

                foreach (Tweet tweet in tweets)
                {
                    double longitude = tweet.Latitude;
                    double latitude = tweet.Longitude;

                    if (IsPointInState(longitude, latitude, state.Value))
                    {
                        SentimentsStates[state.Key].Add(tweet.Sentiment);
                        //Console.WriteLine($"Твит в штате {state.Key}");
                    }
                  // else { Console.WriteLine($"Твит не в штате {state.Key}"); }
                }
                
                
            }

            CalculateMiddleSentimentsState(SentimentsStates);
            foreach (var s in MiddleSentimentsState)
            {
                Console.WriteLine($"{s.Key}     {s.Value}  ");
            }
        }

        private Dictionary<string, double> CalculateMiddleSentimentsState(Dictionary <string, List<double>> SentimentsStates)
        {
            foreach (var sentimentsState in SentimentsStates)
            {
                if (sentimentsState.Value.Count() != double.NaN)
                {
                    MiddleSentimentsState[sentimentsState.Key] = sentimentsState.Value.Where(v => !double.IsNaN(v)).Sum() / sentimentsState.Value.Count(v => !double.IsNaN(v));
                }
                else
                {
                    MiddleSentimentsState[sentimentsState.Key] = double.NaN;
                }
            }         
            return MiddleSentimentsState;
        }

        private bool IsPointInState(double latitude, double longitude, object statePoligons)
        {
            if (statePoligons is List<List<List<List<double>>>> LLLL)
            {
                foreach (var poligons in LLLL)
                {
                    foreach (var poligon in poligons)
                    {
                        if (IsPointInPolygon(latitude, longitude, poligon))
                        {
                            return true;
                        }
                    }
                }
            }
            else if (statePoligons is List<List<List<double>>> LLL) {
                    foreach (var poligon in LLL)
                    {
                        if (IsPointInPolygon(latitude, longitude, poligon))
                        {
                            return true;
                        }
                    }   
            }
            return false;
        }
        private bool IsPointInPolygon(double latitude, double longitude, List<List<double>> polygon)
        {

            bool inside = false;
            int j = polygon.Count - 1;
           

            for (int i = 0; i < polygon.Count; i++)
            {
                double lat1 = polygon[i][1], lon1 = polygon[i][0];
                double lat2 = polygon[j][1], lon2 = polygon[j][0];

                if (((lat1 > latitude) != (lat2 > latitude))
                    && (longitude < (lon2 - lon1) * (latitude - lat1) / (lat2 - lat1) + lon1))
                {
                    inside = !inside;
                }
                j = i;
            }

            return inside;
        }
    }   
}
