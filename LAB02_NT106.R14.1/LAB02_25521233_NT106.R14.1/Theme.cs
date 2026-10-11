using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Lab02
{
    internal static class Theme
    {
        public static readonly Color NavyDeep = Color.FromArgb(13, 24, 46);
        public static readonly Color Navy = Color.FromArgb(22, 36, 66);
        public static readonly Color NavyLight = Color.FromArgb(34, 54, 94);
        public static readonly Color Gold = Color.FromArgb(201, 162, 39);
        public static readonly Color GoldDark = Color.FromArgb(176, 138, 25);
        public static readonly Color Ivory = Color.FromArgb(247, 244, 236);
        public static readonly Color IvoryDeep = Color.FromArgb(252, 249, 240);
        public static readonly Color Border = Color.FromArgb(229, 223, 207);
        public static readonly Color TextMain = Color.FromArgb(31, 41, 55);
        public static readonly Color TextMuted = Color.FromArgb(107, 114, 128);
        public static readonly Color Burgundy = Color.FromArgb(127, 29, 29);
        public static readonly Color BurgundyDark = Color.FromArgb(96, 20, 20);

        public static void PaintHeaderBackground(object sender, PaintEventArgs e)
        {
            Control header = (Control)sender;
            if (header.Width == 0 || header.Height == 0)
            {
                return;
            }

            using LinearGradientBrush brush = new LinearGradientBrush(header.ClientRectangle, NavyDeep, NavyLight, LinearGradientMode.Horizontal);
            e.Graphics.FillRectangle(brush, header.ClientRectangle);
        }

        public static void StylePrimary(Button button)
        {
            ApplyBase(button);
            button.BackColor = Navy;
            button.ForeColor = Color.White;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = NavyLight;
        }

        public static void StyleAccent(Button button)
        {
            ApplyBase(button);
            button.BackColor = Gold;
            button.ForeColor = NavyDeep;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = GoldDark;
        }

        public static void StyleSecondary(Button button)
        {
            ApplyBase(button);
            button.BackColor = Color.White;
            button.ForeColor = Navy;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Border;
            button.FlatAppearance.MouseOverBackColor = IvoryDeep;
        }

        public static void StyleDanger(Button button)
        {
            ApplyBase(button);
            button.BackColor = Burgundy;
            button.ForeColor = Color.White;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = BurgundyDark;
        }

        private static void ApplyBase(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.Cursor = Cursors.Hand;
            button.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button.UseVisualStyleBackColor = false;
        }
    }
}
