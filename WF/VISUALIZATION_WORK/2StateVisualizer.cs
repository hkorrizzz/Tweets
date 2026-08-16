using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace WF
{
    public class StateVisualizer
    {
        private Dictionary<string, object> _states;

        public StateVisualizer(Dictionary<string, object> states)
        {
            _states = states;
        }

        public void DrawStates(Graphics g, Size clientSize, Dictionary<string, double> stateSentiments)
        {
            g.Clear(Color.White);

            (double minX, double minY, double maxX, double maxY) bounds = CalculateStateBounds();
            (double scale, float offsetX, float offsetY) scaleInfo = CalculateScale(clientSize, bounds);
            (double middlePositive, double middleNegative) sentimentStats = CalculateSentimentStats(stateSentiments);

            foreach (KeyValuePair<string, object> state in _states)
            {
                Color stateColor = GetStateColor(state.Key, stateSentiments, sentimentStats);
                List<List<double>> largestPolygon = FindLargestPolygon(state.Value);

                
                DrawState(g, state.Value, bounds, scaleInfo, stateColor);
                DrawStateAbbreviation(g, largestPolygon, state.Key, bounds, scaleInfo);
                
            }
        }

        private (double minX, double minY, double maxX, double maxY) CalculateStateBounds()
        {
            double minX = double.MaxValue, minY = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue;

            foreach (object state in _states.Values)
            {
                if (state is List<List<List<List<double>>>> stateData4D)
                {
                    UpdateBoundsFromLLLL(stateData4D, ref minX, ref minY, ref maxX, ref maxY);
                }
                else if (state is List<List<List<double>>> stateData3D)
                {
                    UpdateBoundsFromLLL(stateData3D, ref minX, ref minY, ref maxX, ref maxY);
                }
            }

            return (minX, minY, maxX, maxY);
        }
        private void UpdateBoundsFromLLLL(List<List<List<List<double>>>> data, ref double minX, ref double minY, ref double maxX, ref double maxY)
        {
            foreach (List<List<List<double>>> polygonGroup in data)
            {
                UpdateBoundsFromLLL(polygonGroup, ref minX, ref minY, ref maxX, ref maxY);
            }
        }
        private void UpdateBoundsFromLLL(List<List<List<double>>> data, ref double minX, ref double minY, ref double maxX, ref double maxY)
        {
            foreach (List<List<double>> polygon in data)
            {
                foreach (List<double> point in polygon)
                {
                    minX = Math.Min(minX, point[0]);
                    minY = Math.Min(minY, point[1]);
                    maxX = Math.Max(maxX, point[0]);
                    maxY = Math.Max(maxY, point[1]);
                }
            }
        }


        private (double scale, float offsetX, float offsetY) CalculateScale(Size clientSize, (double minX, double minY, double maxX, double maxY) bounds)
        {
            double mapWidth = bounds.maxX - bounds.minX;
            double mapHeight = bounds.maxY - bounds.minY;

            double scaleX = (clientSize.Width * 0.8) / mapWidth;
            double scaleY = (clientSize.Height * 0.8) / mapHeight;
            double scale = Math.Min(scaleX, scaleY) * 3;

            float centerX = clientSize.Width / 2;
            float centerY = clientSize.Height / 2;
            float mapCenterX = (float)(mapWidth * scale / 2);
            float mapCenterY = (float)(mapHeight * scale / 2);

            float offsetX = centerX - mapCenterX + 1280;
            float offsetY = centerY - mapCenterY;

            return (scale, offsetX, offsetY);
        }


        private (double middlePositive, double middleNegative) CalculateSentimentStats(Dictionary<string, double> stateSentiments)
        {
            double middlePositive = stateSentiments.Values.Where(v => !double.IsNaN(v) && v > 0).DefaultIfEmpty(0).Average() / 4;

            double middleNegative = Math.Abs(stateSentiments.Values.Where(v => !double.IsNaN(v) && v < 0).DefaultIfEmpty(0).Average() / 4);

            return (middlePositive, middleNegative);
        }
        private Color GetStateColor(string stateKey, Dictionary<string, double> stateSentiments, (double middlePositive, double middleNegative) stats)
        {
            if (!stateSentiments.TryGetValue(stateKey, out double sentiment))
                return Color.LightGray;

            if (double.IsNaN(sentiment))
                return Color.LightGray;

            return new SentimentColorCalculator().CalculateColor(sentiment, stats.middlePositive, stats.middleNegative);
        }


        private List<List<double>> FindLargestPolygon(object stateData)
        {
            double largestArea = 0;
            List<List<double>> largestPolygon = null;

            if (stateData is List<List<List<List<double>>>> stateData4D)
            {
                foreach (List<List<List<double>>> polygonGroup in stateData4D)
                {
                    UpdateLargestPolygon(polygonGroup, ref largestArea, ref largestPolygon);
                }
            }
            else if (stateData is List<List<List<double>>> stateData3D)
            {
                UpdateLargestPolygon(stateData3D, ref largestArea, ref largestPolygon);
            }

            return largestPolygon;
        }
        private void UpdateLargestPolygon(List<List<List<double>>> polygons, ref double largestArea, ref List<List<double>> largestPolygon)
        {
            foreach (List<List<double>> polygon in polygons)
            {
                double area = CalculatePolygonArea(polygon);
                if (area > largestArea)
                {
                    largestArea = area;
                    largestPolygon = polygon;
                }
            }
        }
        private double CalculatePolygonArea(List<List<double>> polygon)
        {
            double area = 0;
            int j = polygon.Count - 1;

            for (int i = 0; i < polygon.Count; i++)
            {
                area += (polygon[j][0] + polygon[i][0]) * (polygon[j][1] - polygon[i][1]);
                j = i;
            }

            return Math.Abs(area / 2.0);
        }


        private void DrawState(Graphics g, object stateData, (double minX, double minY, double maxX, double maxY) bounds, (double scale, float offsetX, float offsetY) scaleInfo, Color color)
        {
            if (stateData is List<List<List<List<double>>>> stateDataLLLL)
            {
                foreach (var polygonGroup in stateDataLLLL)
                {
                    DrawPolygonGroup(g, polygonGroup, bounds, scaleInfo, color);
                }
            }
            else if (stateData is List<List<List<double>>> stateData3D)
            {
                DrawPolygonGroup(g, stateData3D, bounds, scaleInfo, color);
            }
        }

        private void DrawPolygonGroup(Graphics g, List<List<List<double>>> polygonGroup,(double minX, double minY, double maxX, double maxY) bounds,(double scale, float offsetX, float offsetY) scaleInfo, Color color)
        {
            foreach (var polygon in polygonGroup)
            {
                if (polygon.Count < 2) return;

                PointF[] points = new PointF[polygon.Count];

                for (int i = 0; i < polygon.Count; i++)
                {
                    double x = polygon[i][0];
                    double y = polygon[i][1]; 

                    float transformedX = (float)((x - bounds.minX) * scaleInfo.scale + scaleInfo.offsetX);
                    float transformedY = (float)((bounds.maxY - y) * scaleInfo.scale + scaleInfo.offsetY);

                    points[i] = new PointF(transformedX, transformedY);
                }

                using (Brush brush = new SolidBrush(color))
                {
                    g.FillPolygon(brush, points);
                }
                g.DrawPolygon(Pens.Black, points);
            }
        }

        private void DrawStateAbbreviation(Graphics g, List<List<double>> polygon,string abbreviation, (double minX, double minY, double maxX, double maxY) bounds, (double scale, float offsetX, float offsetY) scaleInfo)
        {
            double centerX = polygon.Average(point => point[0]);
            double centerY = polygon.Average(point => point[1]);

            float x = (float)((centerX - bounds.minX) * scaleInfo.scale + scaleInfo.offsetX);
            float y = (float)((bounds.maxY - centerY) * scaleInfo.scale + scaleInfo.offsetY);

            using (Font boldFont = new Font("Arial", 12, FontStyle.Bold))
            {
                SizeF textSize = g.MeasureString(abbreviation, boldFont);
                float textX = Math.Max(x - textSize.Width / 2, 0);
                float textY = Math.Max(y - textSize.Height / 2, 0);

                using (Brush brush = new SolidBrush(Color.Black))
                {
                    g.DrawString(abbreviation, boldFont, brush, textX, textY);
                }
            }
        }
    }
}