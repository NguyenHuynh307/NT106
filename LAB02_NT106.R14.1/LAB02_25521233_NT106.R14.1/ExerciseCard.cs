using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Lab02
{
    internal class ExerciseCard : Control
    {
        private readonly Font numberFont = new Font("Palatino Linotype", 24F, FontStyle.Bold);
        private readonly Font headingFont = new Font("Segoe UI", 11.5F, FontStyle.Bold);
        private readonly Font detailFont = new Font("Segoe UI", 9F);
        private bool isHovered;

        public string Number { get; set; } = string.Empty;
        public string Heading { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;

        public ExerciseCard()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            isHovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            isHovered = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle bounds = new Rectangle(0, 0, Width - 1, Height - 1);
            using (SolidBrush backBrush = new SolidBrush(isHovered ? Theme.IvoryDeep : Color.White))
            {
                graphics.FillRectangle(backBrush, bounds);
            }

            using (Pen borderPen = new Pen(isHovered ? Theme.Gold : Theme.Border, isHovered ? 2F : 1F))
            {
                graphics.DrawRectangle(borderPen, bounds);
            }

            int accentWidth = Math.Max(4, Width / 45);
            using (SolidBrush accentBrush = new SolidBrush(Theme.Gold))
            {
                graphics.FillRectangle(accentBrush, 0, 0, accentWidth, Height);
            }

            int left = accentWidth + Width / 12;
            int textWidth = Width - left - Width / 20;
            int gap = Height / 30;
            int totalHeight = numberFont.Height + headingFont.Height + detailFont.Height + gap * 2;
            int top = (Height - totalHeight) / 2;

            TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis | TextFormatFlags.Left;

            TextRenderer.DrawText(graphics, Number, numberFont, new Rectangle(left, top, textWidth, numberFont.Height), Theme.Gold, flags);
            top += numberFont.Height + gap;
            TextRenderer.DrawText(graphics, Heading, headingFont, new Rectangle(left, top, textWidth, headingFont.Height), Theme.Navy, flags);
            top += headingFont.Height + gap;
            TextRenderer.DrawText(graphics, Detail, detailFont, new Rectangle(left, top, textWidth, detailFont.Height), Theme.TextMuted, flags);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                numberFont.Dispose();
                headingFont.Dispose();
                detailFont.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
