#nullable disable

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Lab02
{
    internal sealed class CardPanel : Panel
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color AccentColor { get; set; } = Color.FromArgb(200, 55, 36);
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color BorderColor { get; set; } = Color.FromArgb(236, 222, 204);

        public CardPanel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = Color.White;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(BackColor);

            using (Pen borderPen = new Pen(BorderColor))
            {
                graphics.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
            }

            using (SolidBrush accentBrush = new SolidBrush(AccentColor))
            {
                graphics.FillRectangle(accentBrush, 0, 0, Width, LogicalToDeviceUnits(4));
            }
        }
    }
}
