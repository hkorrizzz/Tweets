using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace WF
{
    public partial class Form1 : Form
    {
        private readonly StateVisualizer _stateVisualizer;
        private readonly SentimentPanel _sentimentPanel;
        private readonly ColorPalette _colorPalette;
        private readonly TopicLabel _topicLabel;
        private Dictionary<string, double> _stateSentiments;

        public Form1(Dictionary<string, double> stateSentiments)
        {
            InitializeComponent();

            _stateSentiments = stateSentiments;

            _topicLabel = new TopicLabel(this);
            _stateVisualizer = new StateVisualizer(StatesAnalyzer.StatesLocation);
            _sentimentPanel = new SentimentPanel(this, _stateSentiments);
            _colorPalette = new ColorPalette(this);

            this.DoubleBuffered = true;
            this.WindowState = FormWindowState.Maximized;
            this.Paint += Form1_Paint;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            _stateVisualizer.DrawStates(e.Graphics, this.ClientSize, _stateSentiments);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItem = comboBox1.SelectedItem?.ToString();
            _topicLabel.UpdateText(selectedItem?.ToUpper() ?? string.Empty);

            UpdateTweets(selectedItem + "_tweets2014.txt");
        }

        private void UpdateTweets(string filename)
        {
            TweetProcessor tweetProcessor = new TweetProcessor();
            _stateSentiments = tweetProcessor.ProcessTweetsFromFile(filename);

            _sentimentPanel.UpdateSentiments(_stateSentiments);
            this.Invalidate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
