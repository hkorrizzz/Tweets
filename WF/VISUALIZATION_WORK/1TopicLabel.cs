using System.Drawing;
using System.Windows.Forms;

namespace WF
{
    public class TopicLabel
    {
        private  Label _label;

        private Control _parent;

        public TopicLabel(Control parent)
        {
            _parent = parent;

            _label = new Label
            {
                Text = "SNOW",
                AutoSize = true,
                Font = new Font(parent.Font.FontFamily, 16, FontStyle.Bold),
                ForeColor = Color.Black,
                Anchor = AnchorStyles.Top
            };

            parent.Controls.Add(_label);
            PositionLabel();
        }




        public void UpdateText(string text)
        {
            _label.Text = text;
            PositionLabel();
        }






        private void PositionLabel()
        {
            _label.Location = new Point((_parent.ClientSize.Width - _label.Width) / 2,10);
        }
    }
}