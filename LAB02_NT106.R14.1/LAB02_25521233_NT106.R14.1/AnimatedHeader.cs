using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Lab02
{
    internal sealed class AnimatedHeader : Panel
    {
        private const int StarCount = 90;

        private readonly Stopwatch clock = Stopwatch.StartNew();
        private readonly System.Windows.Forms.Timer frameTimer = new System.Windows.Forms.Timer { Interval = 16 };
        private readonly Font titleFont = new Font("Palatino Linotype", 22F, FontStyle.Bold);
        private readonly Font subtitleFont = new Font("Segoe UI", 10F);
        private readonly float[] starX = new float[StarCount];
        private readonly float[] starY = new float[StarCount];
        private readonly float[] starSize = new float[StarCount];
        private readonly float[] starSpeed = new float[StarCount];
        private readonly float[] starPhase = new float[StarCount];
        private readonly int[] starAlpha = new int[StarCount];
        private readonly Color[] starTints = { Color.White, Color.FromArgb(170, 230, 255), Color.FromArgb(214, 190, 255) };

        private Bitmap background;
        private Bitmap planetSprite;
        private string title = string.Empty;
        private string subtitle = string.Empty;
        private int buttonsLeft;

        public AnimatedHeader()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            DoubleBuffered = true;

            Random random = new Random(2026);
            for (int i = 0; i < StarCount; i++)
            {
                starX[i] = (float)random.NextDouble();
                starY[i] = (float)random.NextDouble();
                starSize[i] = random.Next(10) < 8 ? 1.6f : 2.8f;
                starSpeed[i] = 0.008f + (float)random.NextDouble() * 0.03f;
                starPhase[i] = (float)(random.NextDouble() * Math.PI * 2);
                starAlpha[i] = random.Next(90, 235);
            }

            frameTimer.Tick += FrameTimer_Tick;
        }

        public string Title
        {
            get { return title; }
            set { title = value; ResetCache(); }
        }

        public string Subtitle
        {
            get { return subtitle; }
            set { subtitle = value; ResetCache(); }
        }

        private void FrameTimer_Tick(object sender, EventArgs e)
        {
            Form owner = FindForm();
            if (owner != null && owner.WindowState == FormWindowState.Minimized)
            {
                return;
            }
            Invalidate();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            frameTimer.Enabled = Visible && !DesignMode;
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            int gap = LogicalToDeviceUnits(8);
            int right = Width - LogicalToDeviceUnits(24);
            for (int i = Controls.Count - 1; i >= 0; i--)
            {
                Control item = Controls[i];
                item.Left = right - item.Width;
                item.Top = (Height - item.Height) / 2;
                right = item.Left - gap;
            }
            buttonsLeft = right + gap;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            ResetCache();
        }

        protected override void OnDpiChangedAfterParent(EventArgs e)
        {
            base.OnDpiChangedAfterParent(e);
            ResetCache();
        }

        private void ResetCache()
        {
            if (background != null)
            {
                background.Dispose();
                background = null;
            }
            if (planetSprite != null)
            {
                planetSprite.Dispose();
                planetSprite = null;
            }
            Invalidate();
        }

        private void BuildBackground()
        {
            float scale = DeviceDpi / 96f;
            background = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
            background.SetResolution(DeviceDpi, DeviceDpi);

            using (Graphics graphics = Graphics.FromImage(background))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle area = new Rectangle(0, 0, Width, Height);

                using (LinearGradientBrush brush = new LinearGradientBrush(area, Color.FromArgb(6, 7, 26), Color.FromArgb(10, 50, 108), LinearGradientMode.Horizontal))
                {
                    ColorBlend blend = new ColorBlend(3);
                    blend.Colors = new[] { Color.FromArgb(6, 7, 26), Color.FromArgb(44, 18, 92), Color.FromArgb(10, 50, 108) };
                    blend.Positions = new[] { 0f, 0.55f, 1f };
                    brush.InterpolationColors = blend;
                    graphics.FillRectangle(brush, area);
                }

                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                using (SolidBrush titleBrush = new SolidBrush(Color.White))
                using (SolidBrush subtitleBrush = new SolidBrush(Color.FromArgb(170, 185, 230)))
                {
                    graphics.DrawString(title, titleFont, titleBrush, 26f * scale, 10f * scale);
                    graphics.DrawString(subtitle, subtitleFont, subtitleBrush, 30f * scale, 56f * scale);
                }

                int lineHeight = Math.Max(3, (int)(3 * scale));
                Rectangle lineArea = new Rectangle(0, Height - lineHeight, Width, lineHeight);
                using (LinearGradientBrush line = new LinearGradientBrush(lineArea, Color.FromArgb(86, 222, 255), Color.FromArgb(255, 86, 170), LinearGradientMode.Horizontal))
                {
                    graphics.FillRectangle(line, lineArea);
                }
            }
        }

        private void BuildPlanetSprite()
        {
            float scale = DeviceDpi / 96f;
            int size = (int)(100 * scale);
            planetSprite = new Bitmap(size, size, PixelFormat.Format32bppPArgb);

            using (Graphics graphics = Graphics.FromImage(planetSprite))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.Clear(Color.Transparent);
                graphics.TranslateTransform(size / 2f, size / 2f);
                graphics.ScaleTransform(scale, scale);
                graphics.RotateTransform(-18f);

                RectangleF ring = new RectangleF(-38f, -9f, 76f, 18f);
                using (Pen ringPen = new Pen(Color.FromArgb(185, 190, 160, 255), 3f))
                {
                    graphics.DrawArc(ringPen, ring, 180f, 180f);

                    using (GraphicsPath body = new GraphicsPath())
                    {
                        body.AddEllipse(-20f, -20f, 40f, 40f);
                        using (PathGradientBrush shade = new PathGradientBrush(body))
                        {
                            shade.CenterPoint = new PointF(-7f, -8f);
                            shade.CenterColor = Color.FromArgb(255, 214, 150);
                            shade.SurroundColors = new[] { Color.FromArgb(150, 62, 170) };
                            graphics.FillPath(shade, body);
                        }
                    }

                    graphics.DrawArc(ringPen, ring, 0f, 180f);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 0 || Height <= 0)
            {
                return;
            }

            if (background == null)
            {
                BuildBackground();
            }
            if (planetSprite == null)
            {
                BuildPlanetSprite();
            }

            Graphics graphics = e.Graphics;
            float scale = DeviceDpi / 96f;
            float time = (float)clock.Elapsed.TotalSeconds;

            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.DrawImageUnscaled(background, 0, 0);
            graphics.CompositingMode = CompositingMode.SourceOver;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            DrawStars(graphics, time);
            DrawShootingStar(graphics, time);

            float centerX = Math.Max(Width * 0.45f, buttonsLeft - 80f * scale);
            float centerY = Height / 2f + 5f * scale * (float)Math.Sin(time * 1.3f);
            float moonAngle = time * 1.1f;
            bool moonInFront = Math.Sin(moonAngle) >= 0;

            if (!moonInFront)
            {
                DrawMoon(graphics, centerX, centerY, scale, moonAngle);
            }

            graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            graphics.PixelOffsetMode = PixelOffsetMode.Half;
            graphics.DrawImage(planetSprite, new RectangleF(centerX - planetSprite.Width / 2f, centerY - planetSprite.Height / 2f, planetSprite.Width, planetSprite.Height));

            if (moonInFront)
            {
                DrawMoon(graphics, centerX, centerY, scale, moonAngle);
            }
        }

        private void DrawStars(Graphics graphics, float time)
        {
            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                for (int i = 0; i < StarCount; i++)
                {
                    float x = (starX[i] - starSpeed[i] * time) % 1f;
                    if (x < 0f)
                    {
                        x += 1f;
                    }

                    float twinkle = 0.65f + 0.35f * (float)Math.Sin(time * 2.2f + starPhase[i]);
                    int alpha = Math.Max(0, Math.Min(255, (int)(starAlpha[i] * twinkle)));
                    brush.Color = Color.FromArgb(alpha, starTints[i % starTints.Length]);
                    graphics.FillEllipse(brush, x * Width, starY[i] * (Height - 4), starSize[i], starSize[i]);
                }
            }
        }

        private void DrawShootingStar(Graphics graphics, float time)
        {
            const float period = 6f;
            const float duration = 0.9f;
            float phase = time % period;
            if (phase > duration)
            {
                return;
            }

            float progress = phase / duration;
            float headX = Width * (0.95f - 0.45f * progress);
            float headY = Height * (0.05f + 0.75f * progress);
            float tailX = headX + Width * 0.08f;
            float tailY = headY - Height * 0.25f;
            int alpha = (int)(255 * Math.Sin(progress * Math.PI));

            using (Pen tail = new Pen(Color.FromArgb(alpha, 190, 230, 255), 2f))
            using (SolidBrush head = new SolidBrush(Color.FromArgb(alpha, Color.White)))
            {
                graphics.DrawLine(tail, tailX, tailY, headX, headY);
                graphics.FillEllipse(head, headX - 2.5f, headY - 2.5f, 5f, 5f);
            }
        }

        private static void DrawMoon(Graphics graphics, float centerX, float centerY, float scale, float angle)
        {
            float moonX = centerX + (float)Math.Cos(angle) * 54f * scale;
            float moonY = centerY + (float)Math.Sin(angle) * 14f * scale - 4f * scale;
            float size = 9f * scale;
            using (SolidBrush moon = new SolidBrush(Color.FromArgb(210, 120, 225, 255)))
            {
                graphics.FillEllipse(moon, moonX - size / 2f, moonY - size / 2f, size, size);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                frameTimer.Stop();
                frameTimer.Dispose();
                titleFont.Dispose();
                subtitleFont.Dispose();
                if (background != null)
                {
                    background.Dispose();
                }
                if (planetSprite != null)
                {
                    planetSprite.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
