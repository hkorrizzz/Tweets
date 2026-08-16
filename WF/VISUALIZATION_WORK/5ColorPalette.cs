using System.Drawing;
using System.Windows.Forms;

namespace WF
{
    public class ColorPalette
    {
        private readonly Panel _panel;

        public ColorPalette(Control parent)
        {
            _panel = new Panel
            {
                Width = 80,
                Dock = DockStyle.Right,
                BackColor = Color.WhiteSmoke,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5)
            };

            InitializePalette();
            parent.Controls.Add(_panel);
            _panel.BringToFront();
        }

        private void InitializePalette()
        {
            var titleLabel = new Label
            {
                Text = "POS",
                Font = new Font(_panel.Font.FontFamily, 10, FontStyle.Bold),
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 30
            };
            _panel.Controls.Add(titleLabel);

            Color[] colors = new[]
            {
                Color.Yellow,
                Color.Orange,
                Color.Gold,
                Color.Khaki,
                Color.White, 
                Color.LightGray, 
                Color.White,
                Color.IndianRed,
                Color.Firebrick,
                Color.SaddleBrown,
                Color.DarkRed
            };

            int yPosition = titleLabel.Bottom + 5;
            int colorBlockHeight = 65;

            foreach (var color in colors)
            {
                var colorBlock = new Panel
                {
                    BackColor = color,
                    BorderStyle = BorderStyle.FixedSingle,
                    Size = new Size(80, colorBlockHeight),
                    Location = new Point(0, yPosition)
                };
                _panel.Controls.Add(colorBlock);
                yPosition += colorBlockHeight;
            }

            var bottomLabel = new Label
            {
                Text = "NEG",
                Font = new Font(_panel.Font.FontFamily, 10, FontStyle.Bold),
                Dock = DockStyle.Bottom,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 30
            };
            _panel.Controls.Add(bottomLabel);

            _panel.Height = titleLabel.Height + (colors.Length * colorBlockHeight) + bottomLabel.Height + 20;
        }
    }
}