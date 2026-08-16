using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WF
{
    public class SentimentPanel
    {
        private readonly Panel _panel;
        private readonly Control _parentControl;
        private Dictionary<string, double> _sentiments;

        public SentimentPanel(Control parent, Dictionary<string, double> initialSentiments)
        {
            _parentControl = parent;
            _sentiments = initialSentiments;

            _panel = new Panel
            {
                Width = 150,
                Dock = DockStyle.Left,
                AutoScroll = true,
                BackColor = Color.WhiteSmoke,
                BorderStyle = BorderStyle.FixedSingle
            };

            InitializePanel();
            _parentControl.Controls.Add(_panel);
            _panel.BringToFront();
        }

        private void InitializePanel()
        {
            // Title label
            var titleLabel = new Label
            {
                Text = "State Sentiments",
                Font = new Font(_parentControl.Font.FontFamily, 10, FontStyle.Bold),
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 30
            };
            _panel.Controls.Add(titleLabel);

            UpdateSentimentLabels();
        }

        public void UpdateSentiments(Dictionary<string, double> newSentiments)
        {
            _sentiments = newSentiments;
            UpdateSentimentLabels();
        }

        private void UpdateSentimentLabels()
        {
            // Remove existing labels (except title)
            for (int i = _panel.Controls.Count - 1; i >= 1; i--)
            {
                _panel.Controls.RemoveAt(i);
            }

            // Add new labels
            int yPosition = _panel.Controls[0].Bottom + 5;
            foreach (var state in _sentiments.OrderBy(s => s.Key))
            {
                var stateLabel = new Label
                {
                    Text = $"{state.Key}: {state.Value:F2}",
                    Location = new Point(5, yPosition),
                    AutoSize = true,
                    Font = new Font(_parentControl.Font.FontFamily, 9),
                    ForeColor = GetSentimentColor(state.Value)
                };
                _panel.Controls.Add(stateLabel);
                yPosition += 25;
            }
        }

        private Color GetSentimentColor(double sentiment)
        {
            if (double.IsNaN(sentiment)) return Color.Gray;

            if (sentiment < 0)
            {
                return Color.FromArgb(200 + (int)(55 * Math.Min(-sentiment, 1)), 50, 50);
            }
            else
            {
                return Color.FromArgb(50, 200 + (int)(55 * Math.Min(sentiment, 1)), 50);
            }
        }
    }
}