using System.Drawing;

namespace WF
{
    public class SentimentColorCalculator
    {
        public Color CalculateColor(double sentiment, double middlePositive, double middleNegative)
        {

            if (double.IsNaN(sentiment)) return Color.LightGray;

            if (sentiment > 3 * middlePositive) return Color.Yellow;
            if (sentiment > 2 * middlePositive) return Color.Orange;
            if (sentiment > 1 * middlePositive) return Color.Gold;
            if (sentiment > 0) return Color.Khaki;

            if (sentiment == 0) return Color.White;

            if (sentiment < -3 * middleNegative) return Color.DarkRed;
            if (sentiment < -2 * middleNegative) return Color.SaddleBrown;
            if (sentiment < -1 * middleNegative) return Color.Firebrick;
            if (sentiment < 0) return Color.IndianRed;

            return Color.White;
        }
    }
}