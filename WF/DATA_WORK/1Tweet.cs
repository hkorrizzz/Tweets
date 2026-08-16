using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WF
{
    public class Tweet
    {
        public List<string> Text { get; }
        public double Latitude { get; }
        public double Longitude { get; }

        public double Sentiment { get; set; }

        public Tweet(List<string> text, double latitude, double longitude) 
        {
            Text = text;
            Latitude = latitude;
            Longitude = longitude;
        }

    }
}
