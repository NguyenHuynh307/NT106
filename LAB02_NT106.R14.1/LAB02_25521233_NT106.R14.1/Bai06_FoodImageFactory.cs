#nullable disable

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;

namespace Lab02.Bai06Data
{
    internal static class FoodImageFactory
    {
        private enum Kind { Plate, Noodle, Hotpot, Grill, Drink, Dessert, Rice, Bread, Fried, Seafood, Dumpling, Sushi, Pizza, Steak, Salad, Pasta, Fries, Burger }

        private static readonly Color[][] BackgroundPalettes =
        {
            new[] { Color.FromArgb(255, 226, 180), Color.FromArgb(232, 118, 56) },
            new[] { Color.FromArgb(255, 240, 190), Color.FromArgb(220, 156, 36) },
            new[] { Color.FromArgb(255, 214, 200), Color.FromArgb(200, 70, 54) },
            new[] { Color.FromArgb(222, 240, 204), Color.FromArgb(92, 156, 80) },
            new[] { Color.FromArgb(255, 230, 216), Color.FromArgb(180, 94, 82) },
            new[] { Color.FromArgb(214, 236, 240), Color.FromArgb(70, 140, 160) }
        };

        private static readonly Color[] FoodColors =
        {
            Color.FromArgb(232, 110, 50),
            Color.FromArgb(246, 184, 60),
            Color.FromArgb(120, 170, 70),
            Color.FromArgb(210, 60, 48),
            Color.FromArgb(250, 232, 200),
            Color.FromArgb(168, 104, 60)
        };

        private static readonly Color[] DrinkColors =
        {
            Color.FromArgb(214, 140, 56),
            Color.FromArgb(118, 74, 48),
            Color.FromArgb(244, 150, 52),
            Color.FromArgb(120, 190, 96),
            Color.FromArgb(236, 120, 150),
            Color.FromArgb(240, 214, 150)
        };

        public static int StableSeed(string text)
        {
            unchecked
            {
                int hash = (int)2166136261;
                foreach (char letter in text)
                {
                    hash = (hash ^ letter) * 16777619;
                }
                return hash & 0x7FFFFFFF;
            }
        }

        public static void SaveGenerated(string dishName, int seed, string filePath, string category = null)
        {
            using (Bitmap bitmap = CreateGenerated(dishName, seed, category))
            {
                bitmap.Save(filePath, ImageFormat.Png);
            }
        }

        public static Bitmap CreateGenerated(string dishName, int seed, string category = null)
        {
            Random random = new Random(seed);
            Kind kind = Detect(dishName, category);

            Bitmap bitmap = new Bitmap(480, 360, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                DrawBackdrop(graphics, random, kind);

                switch (kind)
                {
                    case Kind.Noodle:
                        DrawNoodleBowl(graphics, random, 240f, 190f);
                        DrawSmoke(graphics, random, 240f, 112f, 9);
                        break;
                    case Kind.Hotpot:
                        DrawHotpot(graphics, random, 240f, 178f);
                        DrawSmoke(graphics, random, 240f, 100f, 11);
                        break;
                    case Kind.Grill:
                        DrawGrill(graphics, random, 240f, 200f);
                        DrawSmoke(graphics, random, 240f, 130f, 8);
                        break;
                    case Kind.Drink:
                        DrawDrink(graphics, random, 240f, 180f);
                        break;
                    case Kind.Dessert:
                        DrawDessert(graphics, random, 240f, 186f);
                        break;
                    case Kind.Rice:
                        DrawRice(graphics, random, 240f, 196f);
                        DrawSmoke(graphics, random, 240f, 120f, 5);
                        break;
                    case Kind.Bread:
                        DrawBread(graphics, random, 240f, 190f);
                        break;
                    case Kind.Fried:
                        DrawFried(graphics, random, 240f, 192f);
                        DrawSmoke(graphics, random, 240f, 128f, 4);
                        break;
                    case Kind.Seafood:
                        DrawSeafood(graphics, random, 240f, 192f);
                        DrawSmoke(graphics, random, 240f, 126f, 5);
                        break;
                    case Kind.Dumpling:
                        DrawDumpling(graphics, random, 240f, 176f);
                        DrawSmoke(graphics, random, 240f, 104f, 9);
                        break;
                    case Kind.Sushi:
                        DrawSushi(graphics, random, 240f, 182f);
                        break;
                    case Kind.Pizza:
                        DrawPizza(graphics, random, 240f, 176f);
                        DrawSmoke(graphics, random, 240f, 128f, 4);
                        break;
                    case Kind.Steak:
                        DrawSteak(graphics, random, 240f, 192f);
                        DrawSmoke(graphics, random, 240f, 126f, 6);
                        break;
                    case Kind.Salad:
                        DrawSalad(graphics, random, 240f, 192f);
                        break;
                    case Kind.Burger:
                        DrawBurger(graphics, random, 240f, 190f);
                        DrawSmoke(graphics, random, 240f, 100f, 4);
                        break;
                    case Kind.Fries:
                        DrawFries(graphics, random, 240f, 192f);
                        DrawSmoke(graphics, random, 240f, 126f, 4);
                        break;
                    case Kind.Pasta:
                        DrawPasta(graphics, random, 240f, 192f);
                        DrawSmoke(graphics, random, 240f, 124f, 5);
                        break;
                    default:
                        DrawGenericPlate(graphics, random, 240f, 190f);
                        DrawSmoke(graphics, random, 240f, 120f, 4);
                        break;
                }

                DrawNameBand(graphics, dishName, bitmap.Width, bitmap.Height);
            }
            return bitmap;
        }

        public static Bitmap CreateCategoryThumb(string category, int width, int height)
        {
            Color top;
            Color bottom;
            switch (category)
            {
                case "Lẩu & nướng":
                    top = Color.FromArgb(255, 205, 170);
                    bottom = Color.FromArgb(214, 84, 52);
                    break;
                case "Ăn vặt":
                    top = Color.FromArgb(255, 236, 178);
                    bottom = Color.FromArgb(232, 164, 40);
                    break;
                case "Đồ uống":
                    top = Color.FromArgb(206, 234, 226);
                    bottom = Color.FromArgb(70, 158, 140);
                    break;
                case "Tráng miệng":
                    top = Color.FromArgb(252, 214, 224);
                    bottom = Color.FromArgb(206, 96, 130);
                    break;
                case "Món Hoa":
                    top = Color.FromArgb(255, 214, 196);
                    bottom = Color.FromArgb(190, 44, 40);
                    break;
                case "Món Âu":
                    top = Color.FromArgb(236, 226, 212);
                    bottom = Color.FromArgb(130, 84, 58);
                    break;
                case "Nhật – Hàn":
                    top = Color.FromArgb(250, 224, 232);
                    bottom = Color.FromArgb(70, 80, 120);
                    break;
                case "Thái – Ấn":
                    top = Color.FromArgb(255, 238, 180);
                    bottom = Color.FromArgb(214, 120, 24);
                    break;
                case "Hải sản":
                    top = Color.FromArgb(210, 236, 244);
                    bottom = Color.FromArgb(40, 120, 168);
                    break;
                case "Món chay":
                    top = Color.FromArgb(226, 244, 210);
                    bottom = Color.FromArgb(70, 140, 70);
                    break;
                default:
                    top = Color.FromArgb(255, 226, 190);
                    bottom = Color.FromArgb(226, 124, 58);
                    break;
            }

            Bitmap thumbnail = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(thumbnail))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                Rectangle area = new Rectangle(0, 0, width, height);
                using (LinearGradientBrush background = new LinearGradientBrush(area, top, bottom, LinearGradientMode.Vertical))
                {
                    graphics.FillRectangle(background, area);
                }

                float diameter = Math.Min(width, height) * 0.62f;
                RectangleF plate = new RectangleF((width - diameter) / 2f, (height - diameter) / 2f, diameter, diameter);
                using (SolidBrush plateBrush = new SolidBrush(Color.FromArgb(235, 255, 252, 246)))
                using (Pen rimPen = new Pen(Color.FromArgb(120, 255, 255, 255), Math.Max(1.5f, diameter * 0.05f)))
                using (SolidBrush foodBrush = new SolidBrush(Color.FromArgb(150, bottom)))
                {
                    graphics.FillEllipse(plateBrush, plate);
                    graphics.DrawEllipse(rimPen, plate);
                    float inner = diameter * 0.5f;
                    graphics.FillEllipse(foodBrush, plate.X + (diameter - inner) / 2f, plate.Y + (diameter - inner) / 2f, inner, inner);
                }
            }
            return thumbnail;
        }

        public static void SaveScaledCopy(Image source, string filePath, int maxWidth, int maxHeight)
        {
            float ratio = Math.Min(1f, Math.Min((float)maxWidth / source.Width, (float)maxHeight / source.Height));
            int width = Math.Max(1, (int)(source.Width * ratio));
            int height = Math.Max(1, (int)(source.Height * ratio));

            using (Bitmap bitmap = new Bitmap(width, height))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(source, 0, 0, width, height);
                bitmap.Save(filePath, ImageFormat.Jpeg);
            }
        }

        public static Image LoadCopy(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return null;
            }

            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (Image original = Image.FromStream(stream))
                {
                    return new Bitmap(original);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Bitmap CreateThumbnail(Image source, int width, int height)
        {
            Bitmap thumbnail = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(thumbnail))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                float ratio = Math.Max((float)width / source.Width, (float)height / source.Height);
                float drawWidth = source.Width * ratio;
                float drawHeight = source.Height * ratio;
                graphics.DrawImage(source, (width - drawWidth) / 2f, (height - drawHeight) / 2f, drawWidth, drawHeight);
            }
            return thumbnail;
        }

        public static Bitmap CreatePlaceholder(int width, int height, string caption = "Chưa có hình ảnh")
        {
            Bitmap placeholder = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(placeholder))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                graphics.Clear(Color.FromArgb(255, 244, 228));

                float diameter = Math.Min(width, height) * 0.5f;
                RectangleF plate = new RectangleF((width - diameter) / 2f, (height - diameter) / 2f - height * 0.05f, diameter, diameter);
                using (Pen platePen = new Pen(Color.FromArgb(226, 196, 160), Math.Max(2f, diameter * 0.04f)))
                {
                    graphics.DrawEllipse(platePen, plate);
                }

                using (Font font = new Font("Segoe UI", Math.Max(8f, height * 0.05f), FontStyle.Regular, GraphicsUnit.Pixel))
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(160, 126, 104)))
                using (StringFormat format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    graphics.DrawString(caption, font, brush, new RectangleF(0, plate.Bottom + height * 0.03f, width, height * 0.12f), format);
                }
            }
            return placeholder;
        }

        private static string Fold(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            string decomposed = text.Normalize(NormalizationForm.FormD);
            StringBuilder builder = new StringBuilder(decomposed.Length);
            foreach (char letter in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark) builder.Append(letter);
            }
            return builder.ToString().ToLowerInvariant().Replace('đ', 'd');
        }

        private static bool HasWord(string[] words, params string[] candidates)
        {
            foreach (string word in words)
            {
                foreach (string candidate in candidates)
                {
                    if (word == candidate) return true;
                }
            }
            return false;
        }

        private static bool Starts(string folded, params string[] heads)
        {
            foreach (string head in heads)
            {
                if (folded == head || folded.StartsWith(head + " ", StringComparison.Ordinal)) return true;
            }
            return false;
        }

        private static bool Has(string padded, params string[] phrases)
        {
            foreach (string phrase in phrases)
            {
                if (padded.Contains(" " + phrase + " ")) return true;
            }
            return false;
        }

        private static Kind Detect(string dishName, string category)
        {
            string folded = string.Join(" ", Fold(dishName).Split(new[] { ' ', ',', '-', '\u2013', '(', ')', '/', '.' }, StringSplitOptions.RemoveEmptyEntries));
            string s = " " + folded + " ";

            if (category == "Đồ uống") return Kind.Drink;
            if (Starts(folded, "tra", "soda", "sinh to", "bia", "ruou", "cappuccino", "latte", "espresso", "americano", "mojito", "lassi", "ca phe", "bac xiu",
                    "nuoc ep", "nuoc mia", "nuoc dua", "nuoc sam", "nuoc chanh", "sua tuoi", "sua dau", "hong tra", "matcha latte", "chocolate da", "chai tra")
                || Has(s, "sua chua danh"))
                return Kind.Drink;

            if (category == "Tráng miệng") return Kind.Dessert;
            if (Starts(folded, "che", "kem", "banh flan", "rau cau", "sua chua", "tiramisu", "panna", "macaron", "brownie", "mousse", "cheesecake", "banh su")
                || (Starts(folded, "xoi") && Has(s, "xoai")))
                return Kind.Dessert;

            if (Starts(folded, "lau", "shabu", "sukiyaki", "nabe") || (category == "Lẩu & nướng" && Has(s, "lau"))) return Kind.Hotpot;

            if (Has(s, "sushi", "sashimi", "chirashi", "onigiri", "kimbap", "gimbap")) return Kind.Sushi;
            if (Has(s, "pizza")) return Kind.Pizza;

            if (Starts(folded, "bit tet", "bo bit tet", "bo ne", "steak", "uc ga") || Has(s, "t-bone", "steak")) return Kind.Steak;

            if (Starts(folded, "ha cao", "sui cao", "xiu mai", "banh bao", "tieu long bao", "hoanh thanh", "gyoza", "ravioli", "gnocchi", "takoyaki", "banh bot loc", "phuong hoang trao"))
                return Kind.Dumpling;

            bool noodleBase = Starts(folded, "mi", "hu tieu", "pho", "bun", "mien", "nui", "banh canh");
            if (Has(s, "spaghetti", "lasagna", "carbonara", "bolognese", "aglio", "alfredo", "pesto", "pad thai", "japchae", "jajangmyeon", "dan dan", "chiet giang", "bun dau", "bun thit nuong", "mi y")
                || (Starts(folded, "bun cha") && !Has(s, "ca"))
                || Has(s, "hu tieu kho", "pho kho", "mi kho", "bun kho")
                || (noodleBase && Has(s, "xao", "tron")))
                return Kind.Pasta;

            if (Starts(folded, "pho", "bun", "mi", "hu tieu", "mien", "banh canh", "banh da", "cao lau", "nui", "ramen", "udon", "soba", "canh", "sup", "chao", "tom yum", "tom kha", "pad thai"))
                return Kind.Noodle;

            if (Starts(folded, "hamburger", "burger") || Has(s, "burger")) return Kind.Burger;
            if (Starts(folded, "banh mi", "hamburger", "burger", "hot dog", "sandwich", "bagel", "croissant", "bruschetta") || Has(s, "banh mi")) return Kind.Bread;

            if (Starts(folded, "com", "xoi", "risotto", "paella", "bibimbap", "biryani") || Has(s, "katsudon", "gyudon", "oyakodon", "unadon", "bento")) return Kind.Rice;

            if (Starts(folded, "goi", "salad", "rau", "bong cai", "cai", "dau que", "bap cai", "nam")) return Kind.Salad;

            if (Starts(folded, "ca vien", "bo vien", "cha gio", "nem ran", "nem chua", "ram cuon", "khoai", "bot chien", "trung cut chien")) return Kind.Fries;
            if (Starts(folded, "dui ga", "canh ga", "ga ran", "ga popcorn")) return Kind.Fried;
            if (Has(s, "nuong", "quay", "bbq", "xien", "kebab", "yakitori", "samgyeopsal", "bulgogi", "galbi", "tandoori", "lui")) return Kind.Grill;
            if (Has(s, "chien", "ran", "tempura", "tonkatsu", "katsu", "popcorn", "gion")) return Kind.Fried;

            if (Starts(folded, "tom", "cua", "ghe", "muc", "bach tuoc", "so", "ngheu", "hau", "oc", "luon", "ech", "ngao", "cha muc")
                || (Starts(folded, "ca") && !Starts(folded, "ca ri", "ca tim", "ca chua", "ca rot")))
                return Kind.Seafood;

            if (category == "Hải sản") return Kind.Seafood;
            if (category == "Lẩu & nướng") return Kind.Grill;
            if (category == "Món chay") return Kind.Salad;
            if (category == "Món Âu") return Kind.Steak;
            return Kind.Plate;
        }

        private static Color Mix(Color a, Color b, float t)
        {
            t = Math.Max(0f, Math.Min(1f, t));
            return Color.FromArgb(
                (int)(a.A + (b.A - a.A) * t),
                (int)(a.R + (b.R - a.R) * t),
                (int)(a.G + (b.G - a.G) * t),
                (int)(a.B + (b.B - a.B) * t));
        }

        private static Color Lighten(Color c, float amount)
        {
            return Mix(c, Color.White, amount);
        }

        private static Color Darken(Color c, float amount)
        {
            return Mix(c, Color.Black, amount);
        }

        private static RectangleF Safe(RectangleF r)
        {
            if (r.Width < 1f) r.Width = 1f;
            if (r.Height < 1f) r.Height = 1f;
            return r;
        }

        private static LinearGradientBrush Vertical(RectangleF r, Color top, Color bottom)
        {
            return new LinearGradientBrush(Safe(r), top, bottom, 90f);
        }

        private static LinearGradientBrush Cylinder(RectangleF r, Color baseColor)
        {
            LinearGradientBrush brush = new LinearGradientBrush(Safe(r), baseColor, baseColor, 0f);
            ColorBlend blend = new ColorBlend(5);
            blend.Colors = new[] { Darken(baseColor, 0.38f), Lighten(baseColor, 0.28f), baseColor, Darken(baseColor, 0.22f), Darken(baseColor, 0.5f) };
            blend.Positions = new[] { 0f, 0.22f, 0.5f, 0.82f, 1f };
            brush.InterpolationColors = blend;
            return brush;
        }

        private static void Sphere(Graphics g, float cx, float cy, float rx, float ry, Color color)
        {
            rx = Math.Max(1f, rx);
            ry = Math.Max(1f, ry);
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(cx - rx, cy - ry, rx * 2f, ry * 2f);
                using (PathGradientBrush brush = new PathGradientBrush(path))
                {
                    brush.CenterPoint = new PointF(cx - rx * 0.32f, cy - ry * 0.38f);
                    brush.CenterColor = Lighten(color, 0.55f);
                    brush.SurroundColors = new[] { Darken(color, 0.32f) };
                    g.FillPath(brush, path);
                }
            }
        }

        private static void Shadow(Graphics g, float cx, float cy, float rx, float ry)
        {
            for (int i = 4; i >= 1; i--)
            {
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(22, 20, 8, 0)))
                {
                    g.FillEllipse(brush, cx - rx - i * 5, cy - ry - i * 1.5f, (rx + i * 5) * 2f, (ry + i * 1.5f) * 2f);
                }
            }
        }

        private static void DrawBackdrop(Graphics g, Random random, Kind kind)
        {
            Color[] palette = BackgroundPalettes[random.Next(BackgroundPalettes.Length)];
            Rectangle area = new Rectangle(0, 0, 480, 360);
            using (LinearGradientBrush wall = Vertical(area, palette[0], palette[1]))
            {
                g.FillRectangle(wall, area);
            }

            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                for (int i = 0; i < 10; i++)
                {
                    int size = random.Next(30, 96);
                    brush.Color = Color.FromArgb(random.Next(22, 52), 255, 255, 255);
                    g.FillEllipse(brush, random.Next(-20, 450), random.Next(-20, 200), size, size);
                }
            }

            RectangleF table = new RectangleF(0, 232, 480, 128);
            using (LinearGradientBrush wood = Vertical(table, Color.FromArgb(150, 92, 54), Color.FromArgb(82, 46, 28)))
            using (Pen grain = new Pen(Color.FromArgb(28, 255, 220, 170), 2f))
            {
                g.FillRectangle(wood, table);
                for (int i = 0; i < 6; i++)
                {
                    float y = table.Y + 18 + i * 20;
                    g.DrawLine(grain, 0, y, 480, y + random.Next(-4, 5));
                }
            }
            using (Pen edge = new Pen(Color.FromArgb(90, 255, 230, 190), 2f))
            {
                g.DrawLine(edge, 0, 232, 480, 232);
            }
        }

        private static void DrawSmoke(Graphics g, Random random, float cx, float baseY, int puffs)
        {
            for (int i = 0; i < puffs * 3; i++)
            {
                float t = i / (float)(puffs * 3);
                float rise = t * (70f + puffs * 4f);
                float sway = (float)Math.Sin(t * 7.0 + random.NextDouble() * 2.0) * (14f + 30f * t);
                float radius = 9f + 26f * t + random.Next(0, 6);
                int alpha = (int)(62f * (1f - t) + 8f);
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, 255, 255, 255)))
                {
                    g.FillEllipse(brush, cx + sway + random.Next(-30, 31) - radius, baseY - rise - radius, radius * 2f, radius * 1.7f);
                }
            }
        }

        private static void DrawNoodleBowl(Graphics g, Random random, float cx, float cy)
        {
            Color[] soups =
            {
                Color.FromArgb(232, 150, 52),
                Color.FromArgb(214, 96, 40),
                Color.FromArgb(236, 188, 90),
                Color.FromArgb(198, 120, 56)
            };
            Color soup = soups[random.Next(soups.Length)];

            Shadow(g, cx, cy + 108, 150, 14);

            RectangleF bodyRect = new RectangleF(cx - 150, cy - 65 - 170f, 300, 340f);
            using (GraphicsPath body = new GraphicsPath())
            {
                body.AddArc(bodyRect, 0, 180);
                body.CloseFigure();
                using (LinearGradientBrush brush = Cylinder(new RectangleF(bodyRect.X, bodyRect.Y, bodyRect.Width, bodyRect.Height), Color.FromArgb(252, 248, 238)))
                {
                    g.FillPath(brush, body);
                }
            }
            using (Pen band = new Pen(Color.FromArgb(206, 60, 42), 5f))
            {
                g.DrawArc(band, cx - 110, cy - 60, 220, 120, 20, 140);
                g.DrawArc(band, cx - 92, cy - 48, 184, 96, 28, 124);
            }
            using (SolidBrush foot = new SolidBrush(Color.FromArgb(210, 188, 158)))
            {
                g.FillEllipse(foot, cx - 54, cy + 98, 108, 16);
            }

            RectangleF rim = new RectangleF(cx - 150, cy - 96, 300, 62);
            using (LinearGradientBrush rimBrush = Vertical(rim, Color.White, Color.FromArgb(232, 220, 198)))
            using (Pen rimPen = new Pen(Color.FromArgb(214, 196, 166), 2f))
            {
                g.FillEllipse(rimBrush, rim);
                g.DrawEllipse(rimPen, rim);
            }

            RectangleF broth = new RectangleF(rim.X + 12, rim.Y + 8, rim.Width - 24, rim.Height - 16);
            using (LinearGradientBrush brothBrush = Vertical(broth, Darken(soup, 0.18f), Lighten(soup, 0.12f)))
            {
                g.FillEllipse(brothBrush, broth);
            }

            using (GraphicsPath clip = new GraphicsPath())
            {
                clip.AddEllipse(broth);
                GraphicsState state = g.Save();
                g.SetClip(clip);

                using (Pen noodle = new Pen(Color.FromArgb(236, 255, 246, 218), 3f))
                {
                    noodle.StartCap = LineCap.Round;
                    noodle.EndCap = LineCap.Round;
                    for (int i = 0; i < 9; i++)
                    {
                        g.DrawArc(noodle, broth.X + 24 + i * 8, broth.Y + 4 + i * 1.6f, broth.Width * 0.58f, broth.Height * 0.8f, 170 + i * 11, 160);
                    }
                }

                for (int i = 0; i < 4; i++)
                {
                    float bx = broth.X + broth.Width * (0.22f + 0.2f * i) + random.Next(-8, 9);
                    float by = broth.Y + broth.Height * (0.34f + 0.12f * (i % 2));
                    Sphere(g, bx, by, 24, 9, Color.FromArgb(156, 84, 62));
                }
                for (int i = 0; i < 14; i++)
                {
                    float hx = broth.X + 28 + random.Next((int)broth.Width - 56);
                    float hy = broth.Y + 8 + random.Next((int)broth.Height - 16);
                    Sphere(g, hx, hy, 5, 2.6f, Color.FromArgb(96, 176, 80));
                }
                Sphere(g, broth.Right - 62, broth.Y + broth.Height * 0.5f, 7, 7, Color.FromArgb(226, 56, 38));
                Sphere(g, broth.Right - 84, broth.Y + broth.Height * 0.36f, 5, 5, Color.FromArgb(226, 56, 38));
                Sphere(g, broth.X + 66, broth.Y + broth.Height * 0.62f, 16, 11, Color.FromArgb(255, 226, 120));
                g.Restore(state);
            }

            using (Pen shine = new Pen(Color.FromArgb(150, 255, 255, 255), 4f))
            {
                shine.StartCap = LineCap.Round;
                shine.EndCap = LineCap.Round;
                g.DrawArc(shine, rim.X + 10, rim.Y + 4, rim.Width - 20, rim.Height - 8, 196, 56);
            }

            GraphicsState chopState = g.Save();
            g.TranslateTransform(cx + 58, cy - 58);
            g.RotateTransform(-34f);
            using (Pen chopA = new Pen(Color.FromArgb(236, 188, 108), 6f))
            using (Pen chopB = new Pen(Color.FromArgb(200, 142, 76), 6f))
            {
                chopA.StartCap = LineCap.Round;
                chopA.EndCap = LineCap.Round;
                chopB.StartCap = LineCap.Round;
                chopB.EndCap = LineCap.Round;
                g.DrawLine(chopA, 0, 0, 0, -130);
                g.DrawLine(chopB, 14, 4, 14, -130);
            }
            g.Restore(chopState);
        }

        private static void DrawHotpot(Graphics g, Random random, float cx, float cy)
        {
            Color[] broths =
            {
                Color.FromArgb(210, 56, 36),
                Color.FromArgb(232, 132, 40),
                Color.FromArgb(240, 206, 120),
                Color.FromArgb(188, 44, 40)
            };
            Color broth = broths[random.Next(broths.Length)];

            Shadow(g, cx, cy + 108, 170, 16);

            using (SolidBrush stove = new SolidBrush(Color.FromArgb(44, 36, 34)))
            using (SolidBrush stoveTop = new SolidBrush(Color.FromArgb(72, 62, 58)))
            {
                g.FillEllipse(stove, cx - 120, cy + 84, 240, 36);
                g.FillEllipse(stoveTop, cx - 108, cy + 76, 216, 28);
            }
            for (int i = 0; i < 7; i++)
            {
                float fx = cx - 78 + i * 26;
                using (GraphicsPath flame = new GraphicsPath())
                {
                    flame.AddBezier(fx, cy + 98, fx - 12, cy + 80, fx + 4, cy + 70, fx, cy + 54);
                    flame.AddBezier(fx, cy + 54, fx + 4, cy + 72, fx + 18, cy + 80, fx, cy + 98);
                    using (LinearGradientBrush brush = Vertical(new RectangleF(fx - 14, cy + 54, 30, 46), Color.FromArgb(255, 236, 120), Color.FromArgb(240, 96, 30)))
                    {
                        g.FillPath(brush, flame);
                    }
                }
            }

            RectangleF bodyRect = new RectangleF(cx - 140, cy - 34, 280, 130);
            using (GraphicsPath body = new GraphicsPath())
            {
                body.AddArc(bodyRect.X, bodyRect.Y, bodyRect.Width, 80, 180, 180);
                body.AddArc(bodyRect.X + 30, bodyRect.Bottom - 70, bodyRect.Width - 60, 70, 0, 180);
                body.CloseFigure();
                using (LinearGradientBrush brush = Cylinder(bodyRect, Color.FromArgb(176, 178, 186)))
                {
                    g.FillPath(brush, body);
                }
            }

            using (SolidBrush handle = new SolidBrush(Color.FromArgb(64, 52, 48)))
            {
                g.FillEllipse(handle, cx - 168, cy - 36, 40, 20);
                g.FillEllipse(handle, cx + 128, cy - 36, 40, 20);
            }

            RectangleF rim = new RectangleF(cx - 146, cy - 76, 292, 80);
            using (LinearGradientBrush rimBrush = Vertical(rim, Color.FromArgb(236, 238, 244), Color.FromArgb(150, 152, 162)))
            {
                g.FillEllipse(rimBrush, rim);
            }
            RectangleF liquid = new RectangleF(rim.X + 12, rim.Y + 10, rim.Width - 24, rim.Height - 20);
            using (LinearGradientBrush liquidBrush = Vertical(liquid, Darken(broth, 0.22f), Lighten(broth, 0.1f)))
            {
                g.FillEllipse(liquidBrush, liquid);
            }

            using (GraphicsPath clip = new GraphicsPath())
            {
                clip.AddEllipse(liquid);
                GraphicsState state = g.Save();
                g.SetClip(clip);
                Color[] items =
                {
                    Color.FromArgb(236, 214, 170),
                    Color.FromArgb(224, 62, 44),
                    Color.FromArgb(104, 178, 84),
                    Color.FromArgb(240, 150, 150),
                    Color.FromArgb(255, 238, 200),
                    Color.FromArgb(168, 108, 64)
                };
                for (int i = 0; i < 16; i++)
                {
                    float ix = liquid.X + 24 + random.Next((int)liquid.Width - 48);
                    float iy = liquid.Y + 10 + random.Next((int)liquid.Height - 20);
                    Sphere(g, ix, iy, random.Next(9, 17), random.Next(6, 10), items[random.Next(items.Length)]);
                }
                g.Restore(state);
            }

            using (Pen shine = new Pen(Color.FromArgb(170, 255, 255, 255), 4f))
            {
                shine.StartCap = LineCap.Round;
                shine.EndCap = LineCap.Round;
                g.DrawArc(shine, rim.X + 8, rim.Y + 4, rim.Width - 16, rim.Height - 8, 192, 60);
            }
        }

        private static void DrawGrill(Graphics g, Random random, float cx, float cy)
        {
            Shadow(g, cx, cy + 74, 190, 20);

            RectangleF slab = new RectangleF(cx - 190, cy + 20, 380, 100);
            using (LinearGradientBrush brush = Vertical(slab, Color.FromArgb(70, 62, 60), Color.FromArgb(28, 24, 24)))
            {
                g.FillEllipse(brush, slab);
            }
            RectangleF glow = new RectangleF(cx - 150, cy + 38, 300, 60);
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(glow);
                using (PathGradientBrush brush = new PathGradientBrush(path))
                {
                    brush.CenterColor = Color.FromArgb(230, 255, 150, 40);
                    brush.SurroundColors = new[] { Color.FromArgb(0, 255, 90, 20) };
                    g.FillPath(brush, path);
                }
            }
            using (Pen grate = new Pen(Color.FromArgb(150, 20, 18, 18), 3f))
            {
                for (int i = 0; i < 9; i++)
                {
                    float gx = cx - 140 + i * 35;
                    g.DrawLine(grate, gx, cy + 34, gx - 18, cy + 104);
                }
            }

            int skewers = 5;
            for (int s = 0; s < skewers; s++)
            {
                float baseX = cx - 150 + s * 74;
                float topY = cy - 70 - (s % 2) * 10;
                float botY = cy + 74;
                float angle = -0.2f;
                using (Pen stick = new Pen(Color.FromArgb(226, 190, 128), 5f))
                {
                    stick.StartCap = LineCap.Round;
                    stick.EndCap = LineCap.Round;
                    g.DrawLine(stick, baseX + 34, topY - 30, baseX - 10, botY + 14);
                }
                for (int m = 0; m < 4; m++)
                {
                    float t = 0.1f + m * 0.2f;
                    float mx = baseX + 28 - (28 + 10) * (t + 0.12f) + 4;
                    float my = topY + (botY - topY) * t + 8;
                    Color meat = m % 2 == 0 ? Color.FromArgb(176, 92, 48) : Color.FromArgb(206, 128, 62);
                    Sphere(g, mx + angle * 10, my, 22, 15, meat);
                    using (Pen charLine = new Pen(Color.FromArgb(120, 52, 24, 12), 2f))
                    {
                        g.DrawArc(charLine, mx - 14, my - 9, 28, 18, 200, 120);
                    }
                }
            }

            Sphere(g, cx - 164, cy + 62, 16, 10, Color.FromArgb(226, 62, 42));
            Sphere(g, cx + 172, cy + 58, 18, 12, Color.FromArgb(104, 178, 84));
        }

        private static void DrawDrink(Graphics g, Random random, float cx, float cy)
        {
            Color liquid = DrinkColors[random.Next(DrinkColors.Length)];
            Shadow(g, cx, cy + 112, 84, 12);

            PointF[] glassPoints =
            {
                new PointF(cx - 72, cy - 108),
                new PointF(cx + 72, cy - 108),
                new PointF(cx + 56, cy + 104),
                new PointF(cx - 56, cy + 104)
            };
            using (GraphicsPath glass = new GraphicsPath())
            {
                glass.AddPolygon(glassPoints);
                using (SolidBrush back = new SolidBrush(Color.FromArgb(70, 255, 255, 255)))
                {
                    g.FillPath(back, glass);
                }

                GraphicsState state = g.Save();
                g.SetClip(glass);
                RectangleF fluid = new RectangleF(cx - 80, cy - 76, 160, 190);
                using (LinearGradientBrush brush = Cylinder(fluid, liquid))
                {
                    g.FillRectangle(brush, fluid);
                }
                using (LinearGradientBrush top = Vertical(new RectangleF(cx - 80, cy - 76, 160, 70), Color.FromArgb(120, 255, 255, 255), Color.FromArgb(0, 255, 255, 255)))
                {
                    g.FillRectangle(top, cx - 80, cy - 76, 160, 70);
                }
                for (int i = 0; i < 5; i++)
                {
                    GraphicsState cube = g.Save();
                    g.TranslateTransform(cx - 44 + random.Next(0, 88), cy - 50 + random.Next(0, 130));
                    g.RotateTransform(random.Next(-35, 35));
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        path.AddRectangle(new RectangleF(-17, -17, 34, 34));
                        using (SolidBrush fill = new SolidBrush(Color.FromArgb(96, 255, 255, 255)))
                        using (Pen edge = new Pen(Color.FromArgb(170, 255, 255, 255), 2f))
                        {
                            g.FillPath(fill, path);
                            g.DrawPath(edge, path);
                        }
                    }
                    g.Restore(cube);
                }
                g.Restore(state);

                using (Pen outline = new Pen(Color.FromArgb(200, 255, 255, 255), 3f))
                {
                    g.DrawPath(outline, glass);
                }
            }

            RectangleF mouth = new RectangleF(cx - 72, cy - 120, 144, 24);
            using (Pen rim = new Pen(Color.FromArgb(220, 255, 255, 255), 3f))
            using (SolidBrush inside = new SolidBrush(Color.FromArgb(120, Lighten(liquid, 0.4f))))
            {
                g.FillEllipse(inside, mouth);
                g.DrawEllipse(rim, mouth);
            }

            using (SolidBrush shine = new SolidBrush(Color.FromArgb(110, 255, 255, 255)))
            using (GraphicsPath path = B6Round(new RectangleF(cx - 56, cy - 84, 12, 150), 6f))
            {
                g.FillPath(shine, path);
            }

            GraphicsState strawState = g.Save();
            g.TranslateTransform(cx + 12, cy + 70);
            g.RotateTransform(16f);
            using (Pen strawA = new Pen(Color.FromArgb(226, 60, 50), 9f))
            using (Pen strawB = new Pen(Color.White, 9f))
            {
                strawA.StartCap = LineCap.Round;
                strawA.EndCap = LineCap.Round;
                strawB.DashPattern = new[] { 1.2f, 1.2f };
                g.DrawLine(strawA, 0, 0, 0, -214);
                g.DrawLine(strawB, 0, 0, 0, -214);
            }
            g.Restore(strawState);

            float sx = cx + 58;
            float sy = cy - 108;
            using (SolidBrush rind = new SolidBrush(Color.FromArgb(244, 200, 60)))
            using (SolidBrush flesh = new SolidBrush(Color.FromArgb(255, 240, 150)))
            using (Pen seg = new Pen(Color.FromArgb(240, 220, 120), 1.5f))
            {
                g.FillEllipse(rind, sx - 24, sy - 24, 48, 48);
                g.FillEllipse(flesh, sx - 19, sy - 19, 38, 38);
                for (int i = 0; i < 4; i++)
                {
                    double a = i * Math.PI / 4;
                    g.DrawLine(seg, sx - (float)Math.Cos(a) * 19, sy - (float)Math.Sin(a) * 19, sx + (float)Math.Cos(a) * 19, sy + (float)Math.Sin(a) * 19);
                }
            }
        }

        private static GraphicsPath B6Round(RectangleF r, float radius)
        {
            float d = Math.Max(1f, Math.Min(radius * 2f, Math.Min(r.Width, r.Height)));
            GraphicsPath path = new GraphicsPath();
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static void DrawDessert(Graphics g, Random random, float cx, float cy)
        {
            Color[][] layers =
            {
                new[] { Color.FromArgb(120, 190, 96), Color.FromArgb(244, 232, 214), Color.FromArgb(150, 82, 52), Color.FromArgb(236, 120, 150) },
                new[] { Color.FromArgb(246, 200, 100), Color.FromArgb(255, 246, 232), Color.FromArgb(210, 96, 70), Color.FromArgb(120, 74, 48) },
                new[] { Color.FromArgb(240, 150, 190), Color.FromArgb(255, 240, 240), Color.FromArgb(176, 120, 220), Color.FromArgb(250, 214, 120) }
            };
            Color[] stack = layers[random.Next(layers.Length)];

            Shadow(g, cx, cy + 112, 100, 13);

            PointF[] cupPoints =
            {
                new PointF(cx - 110, cy - 40),
                new PointF(cx + 110, cy - 40),
                new PointF(cx + 52, cy + 104),
                new PointF(cx - 52, cy + 104)
            };
            using (GraphicsPath cup = new GraphicsPath())
            {
                cup.AddPolygon(cupPoints);
                GraphicsState state = g.Save();
                g.SetClip(cup);
                for (int i = 0; i < 4; i++)
                {
                    RectangleF band = new RectangleF(cx - 120, cy - 44 + i * 38, 240, 40);
                    using (LinearGradientBrush brush = Cylinder(band, stack[i]))
                    {
                        g.FillRectangle(brush, band);
                    }
                }
                for (int i = 0; i < 12; i++)
                {
                    Sphere(g, cx - 70 + random.Next(140), cy - 30 + random.Next(120), 6, 6, i % 2 == 0 ? Color.FromArgb(255, 250, 240) : stack[random.Next(4)]);
                }
                g.Restore(state);
                using (Pen outline = new Pen(Color.FromArgb(190, 255, 255, 255), 3f))
                {
                    g.DrawPath(outline, cup);
                }
            }

            RectangleF mouth = new RectangleF(cx - 110, cy - 52, 220, 26);
            using (SolidBrush inside = new SolidBrush(Color.FromArgb(150, 255, 255, 255)))
            using (Pen rim = new Pen(Color.FromArgb(220, 255, 255, 255), 3f))
            {
                g.FillEllipse(inside, mouth);
                g.DrawEllipse(rim, mouth);
            }

            Sphere(g, cx - 40, cy - 66, 36, 30, Color.FromArgb(255, 246, 232));
            Sphere(g, cx + 38, cy - 70, 34, 28, stack[3]);
            Sphere(g, cx, cy - 104, 15, 15, Color.FromArgb(206, 40, 52));
            using (Pen stem = new Pen(Color.FromArgb(96, 150, 70), 3f))
            {
                g.DrawArc(stem, cx - 2, cy - 140, 26, 30, 180, 90);
            }

            GraphicsState spoon = g.Save();
            g.TranslateTransform(cx + 118, cy + 60);
            g.RotateTransform(22f);
            using (Pen handle = new Pen(Color.FromArgb(208, 210, 218), 7f))
            using (SolidBrush bowl = new SolidBrush(Color.FromArgb(226, 228, 236)))
            {
                handle.StartCap = LineCap.Round;
                handle.EndCap = LineCap.Round;
                g.DrawLine(handle, 0, 0, 0, -120);
                g.FillEllipse(bowl, -14, -150, 28, 40);
            }
            g.Restore(spoon);
        }

        private static void DrawRice(Graphics g, Random random, float cx, float cy)
        {
            Shadow(g, cx, cy + 92, 180, 18);

            RectangleF plate = new RectangleF(cx - 180, cy - 20, 360, 120);
            RectangleF inner = new RectangleF(cx - 150, cy - 6, 300, 92);
            using (LinearGradientBrush brush = Vertical(plate, Color.White, Color.FromArgb(226, 214, 196)))
            using (Pen rim = new Pen(Color.FromArgb(214, 196, 168), 2.5f))
            using (SolidBrush well = new SolidBrush(Color.FromArgb(246, 238, 224)))
            {
                g.FillEllipse(brush, plate);
                g.DrawEllipse(rim, plate);
                g.FillEllipse(well, inner);
            }

            RectangleF dome = new RectangleF(cx - 100, cy - 78, 150, 120);
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(dome, 180, 180);
                path.AddArc(new RectangleF(dome.X, dome.Bottom - 26, dome.Width, 52), 0, 180);
                path.CloseFigure();
                using (PathGradientBrush brush = new PathGradientBrush(path))
                {
                    brush.CenterPoint = new PointF(dome.X + dome.Width * 0.35f, dome.Y + dome.Height * 0.3f);
                    brush.CenterColor = Color.White;
                    brush.SurroundColors = new[] { Color.FromArgb(222, 214, 198) };
                    g.FillPath(brush, path);
                }
            }
            using (SolidBrush grain = new SolidBrush(Color.FromArgb(150, 255, 255, 255)))
            {
                for (int i = 0; i < 40; i++)
                {
                    g.FillEllipse(grain, dome.X + 14 + random.Next((int)dome.Width - 28), dome.Y + 10 + random.Next((int)dome.Height - 20), 6, 3);
                }
            }

            Sphere(g, cx + 92, cy + 22, 54, 28, Color.FromArgb(176, 96, 52));
            using (Pen charLine = new Pen(Color.FromArgb(130, 70, 34, 16), 3f))
            {
                for (int i = 0; i < 4; i++)
                {
                    g.DrawLine(charLine, cx + 60 + i * 16, cy + 6, cx + 50 + i * 16, cy + 38);
                }
            }

            Sphere(g, cx - 72, cy + 54, 44, 22, Color.White);
            Sphere(g, cx - 70, cy + 50, 14, 13, Color.FromArgb(255, 190, 40));

            Sphere(g, cx + 6, cy + 66, 15, 9, Color.FromArgb(226, 62, 44));
            Sphere(g, cx + 34, cy + 70, 15, 9, Color.FromArgb(226, 62, 44));
            Sphere(g, cx - 12, cy + 74, 14, 8, Color.FromArgb(120, 190, 96));
        }

        private static void DrawBread(Graphics g, Random random, float cx, float cy)
        {
            Shadow(g, cx, cy + 88, 190, 16);

            RectangleF board = new RectangleF(cx - 200, cy + 30, 400, 70);
            using (LinearGradientBrush wood = Vertical(board, Color.FromArgb(214, 164, 104), Color.FromArgb(160, 108, 62)))
            {
                g.FillEllipse(wood, board);
            }

            GraphicsState state = g.Save();
            g.TranslateTransform(cx, cy + 10);
            g.RotateTransform(-9f);

            RectangleF loaf = new RectangleF(-176, -50, 352, 110);
            using (GraphicsPath path = B6Round(loaf, 54f))
            {
                using (LinearGradientBrush brush = Vertical(loaf, Color.FromArgb(244, 196, 102), Color.FromArgb(196, 126, 48)))
                {
                    g.FillPath(brush, path);
                }

                GraphicsState inside = g.Save();
                g.SetClip(path);
                using (Pen slit = new Pen(Color.FromArgb(150, 160, 90, 30), 4f))
                {
                    slit.StartCap = LineCap.Round;
                    slit.EndCap = LineCap.Round;
                    for (int i = 0; i < 6; i++)
                    {
                        g.DrawLine(slit, -130 + i * 50, -34, -108 + i * 50, 4);
                    }
                }
                g.Restore(inside);
            }

            RectangleF fill = new RectangleF(-168, 4, 336, 26);
            using (LinearGradientBrush layer = Vertical(fill, Color.FromArgb(120, 190, 96), Color.FromArgb(70, 140, 70)))
            {
                g.FillEllipse(layer, fill);
            }
            for (int i = 0; i < 9; i++)
            {
                Sphere(g, -140 + i * 34, 14 + (i % 2) * 6, 14, 8, i % 3 == 0 ? Color.FromArgb(236, 130, 40) : (i % 3 == 1 ? Color.FromArgb(176, 92, 56) : Color.FromArgb(250, 240, 220)));
            }
            using (Pen shine = new Pen(Color.FromArgb(120, 255, 244, 200), 5f))
            {
                shine.StartCap = LineCap.Round;
                shine.EndCap = LineCap.Round;
                g.DrawArc(shine, -150, -46, 300, 60, 200, 100);
            }
            g.Restore(state);
        }

        private static void DrawGenericPlate(Graphics g, Random random, float cx, float cy)
        {
            Shadow(g, cx, cy + 90, 190, 18);

            RectangleF plate = new RectangleF(cx - 190, cy - 40, 380, 140);
            RectangleF inner = new RectangleF(cx - 156, cy - 22, 312, 106);
            using (LinearGradientBrush brush = Vertical(plate, Color.White, Color.FromArgb(226, 212, 190)))
            using (Pen rim = new Pen(Color.FromArgb(214, 196, 168), 3f))
            using (SolidBrush well = new SolidBrush(Color.FromArgb(250, 240, 224)))
            {
                g.FillEllipse(brush, plate);
                g.DrawEllipse(rim, plate);
                g.FillEllipse(well, inner);
            }

            for (int i = 0; i < 14; i++)
            {
                double angle = random.NextDouble() * Math.PI * 2;
                double distance = Math.Sqrt(random.NextDouble()) * 0.7;
                float x = inner.X + inner.Width / 2f + (float)(Math.Cos(angle) * distance * inner.Width / 2f);
                float y = inner.Y + inner.Height / 2f + (float)(Math.Sin(angle) * distance * inner.Height / 2f);
                Sphere(g, x, y, random.Next(14, 26), random.Next(10, 18), FoodColors[random.Next(FoodColors.Length)]);
            }
        }

        private static RectangleF DrawPlateBase(Graphics g, float cx, float cy, float width)
        {
            float h = width * 0.37f;
            Shadow(g, cx, cy + h * 0.64f, width / 2f - 8f, 18f);
            RectangleF plate = new RectangleF(cx - width / 2f, cy - h * 0.29f, width, h);
            RectangleF inner = new RectangleF(cx - width * 0.41f, cy - h * 0.16f, width * 0.82f, h * 0.76f);
            using (LinearGradientBrush brush = Vertical(plate, Color.White, Color.FromArgb(226, 212, 190)))
            using (Pen rim = new Pen(Color.FromArgb(214, 196, 168), 3f))
            using (SolidBrush well = new SolidBrush(Color.FromArgb(250, 242, 228)))
            {
                g.FillEllipse(brush, plate);
                g.DrawEllipse(rim, plate);
                g.FillEllipse(well, inner);
            }
            return inner;
        }

        private static void Crumbs(Graphics g, Random random, float cx, float cy, float rx, float ry, int count, Color color)
        {
            using (SolidBrush brush = new SolidBrush(color))
            {
                for (int i = 0; i < count; i++)
                {
                    double a = random.NextDouble() * Math.PI * 2;
                    double d = Math.Sqrt(random.NextDouble());
                    float x = cx + (float)(Math.Cos(a) * d * rx);
                    float y = cy + (float)(Math.Sin(a) * d * ry);
                    float s = random.Next(2, 6);
                    g.FillEllipse(brush, x, y, s, s * 0.8f);
                }
            }
        }

        private static void DrawFried(Graphics g, Random random, float cx, float cy)
        {
            RectangleF inner = DrawPlateBase(g, cx, cy, 390f);
            float mx = inner.X + inner.Width / 2f;
            float my = inner.Y + inner.Height / 2f;

            for (int k = 0; k < 7; k++)
            {
                float fx = mx + 70 + k * 15f;
                float fy = my + 18 + (k % 3) * 9f;
                using (SolidBrush fry = new SolidBrush(Color.FromArgb(244, 200 - k * 4, 70)))
                {
                    GraphicsState st = g.Save();
                    g.TranslateTransform(fx, fy);
                    g.RotateTransform(-24f + k * 9f);
                    g.FillRectangle(fry, -4, -34, 9, 62);
                    g.Restore(st);
                }
            }

            for (int i = 0; i < 2; i++)
            {
                GraphicsState state = g.Save();
                g.TranslateTransform(mx - 70 + i * 72, my - 4 + i * 12);
                g.RotateTransform(-32f + i * 20f);

                using (Pen bone = new Pen(Color.FromArgb(246, 238, 220), 13f))
                {
                    bone.StartCap = LineCap.Round;
                    bone.EndCap = LineCap.Round;
                    g.DrawLine(bone, 30, 0, 96, 0);
                }
                Sphere(g, 100, -8, 10, 9, Color.FromArgb(246, 238, 220));
                Sphere(g, 100, 9, 10, 9, Color.FromArgb(246, 238, 220));
                Sphere(g, 0, 0, 62, 42, Color.FromArgb(214, 140, 44));
                Crumbs(g, random, 0, 0, 52, 32, 46, Color.FromArgb(150, 150, 84, 22));
                Crumbs(g, random, -8, -10, 36, 18, 22, Color.FromArgb(120, 255, 226, 140));
                using (Pen shine = new Pen(Color.FromArgb(110, 255, 244, 190), 5f))
                {
                    shine.StartCap = LineCap.Round;
                    shine.EndCap = LineCap.Round;
                    g.DrawArc(shine, -44, -34, 76, 40, 200, 90);
                }
                g.Restore(state);
            }

            Sphere(g, mx + 150, my + 40, 30, 14, Color.FromArgb(252, 252, 250));
            Sphere(g, mx + 150, my + 38, 22, 9, Color.FromArgb(150, 40, 28));
            Sphere(g, mx - 150, my + 36, 14, 8, Color.FromArgb(250, 110, 60));
            Sphere(g, mx - 128, my + 44, 12, 7, Color.FromArgb(120, 190, 96));
            Sphere(g, mx - 168, my + 46, 12, 7, Color.FromArgb(120, 190, 96));
        }

        private static void DrawShrimp(Graphics g, float x, float y, float size, float angle, Color body)
        {
            GraphicsState st = g.Save();
            g.TranslateTransform(x, y);
            g.RotateTransform(angle);
            RectangleF arc = new RectangleF(-size, -size, size * 2f, size * 2f);
            using (Pen shell = new Pen(body, size * 0.46f))
            using (Pen light = new Pen(Lighten(body, 0.4f), size * 0.14f))
            {
                shell.StartCap = LineCap.Round;
                shell.EndCap = LineCap.Triangle;
                light.StartCap = LineCap.Round;
                light.EndCap = LineCap.Round;
                g.DrawArc(shell, arc, 200f, 230f);
                g.DrawArc(light, new RectangleF(-size, -size - size * 0.1f, size * 2f, size * 2f), 215f, 150f);
            }
            using (Pen seg = new Pen(Darken(body, 0.35f), 2f))
            {
                for (int a = 215; a < 425; a += 18)
                {
                    double r = a * Math.PI / 180.0;
                    float px = (float)Math.Cos(r) * size;
                    float py = (float)Math.Sin(r) * size;
                    float nx = (float)Math.Cos(r) * size * 0.2f;
                    float ny = (float)Math.Sin(r) * size * 0.2f;
                    g.DrawLine(seg, px - nx, py - ny, px + nx, py + ny);
                }
            }
            Sphere(g, (float)Math.Cos(200 * Math.PI / 180.0) * size, (float)Math.Sin(200 * Math.PI / 180.0) * size, size * 0.17f, size * 0.17f, Color.FromArgb(40, 24, 20));
            g.Restore(st);
        }

        private static void DrawSeafood(Graphics g, Random random, float cx, float cy)
        {
            RectangleF inner = DrawPlateBase(g, cx, cy, 400f);
            float mx = inner.X + inner.Width / 2f;
            float my = inner.Y + inner.Height / 2f;

            for (int i = 0; i < 9; i++)
            {
                Sphere(g, mx - 150 + i * 36, my + 34 + (i % 2) * 8, 20, 9, Color.FromArgb(96 + random.Next(40), 170, 90));
            }

            DrawShrimp(g, mx - 82, my + 28, 52, -8f, Color.FromArgb(236, 98, 56));
            DrawShrimp(g, mx + 4, my + 38, 58, 14f, Color.FromArgb(242, 112, 62));
            DrawShrimp(g, mx + 88, my + 26, 50, -20f, Color.FromArgb(230, 90, 50));

            RectangleF wedge = new RectangleF(mx + 120, my - 6, 76, 76);
            using (SolidBrush rind = new SolidBrush(Color.FromArgb(244, 214, 60)))
            using (SolidBrush flesh = new SolidBrush(Color.FromArgb(252, 240, 140)))
            {
                g.FillPie(rind, wedge.X, wedge.Y, wedge.Width, wedge.Height, 20, 110);
                RectangleF pulp = new RectangleF(wedge.X + 8, wedge.Y + 8, wedge.Width - 16, wedge.Height - 16);
                g.FillPie(flesh, pulp.X, pulp.Y, pulp.Width, pulp.Height, 22, 106);
            }
            Sphere(g, mx - 150, my - 4, 24, 14, Color.FromArgb(246, 186, 60));
            Sphere(g, mx - 148, my - 8, 14, 8, Color.FromArgb(120, 28, 24));
        }

        private static void DrawDumpling(Graphics g, Random random, float cx, float cy)
        {
            Shadow(g, cx, cy + 96, 170, 16);

            RectangleF basket = new RectangleF(cx - 150, cy - 10, 300, 108);
            using (LinearGradientBrush wood = Cylinder(basket, Color.FromArgb(204, 150, 88)))
            using (Pen band = new Pen(Color.FromArgb(120, 90, 50, 20), 3f))
            {
                g.FillRectangle(wood, basket.X, basket.Y + 36, basket.Width, 58);
                g.FillEllipse(wood, new RectangleF(basket.X, basket.Bottom - 56, basket.Width, 38));
                g.DrawLine(band, basket.X, basket.Y + 62, basket.Right, basket.Y + 62);
            }
            RectangleF topRim = new RectangleF(cx - 150, cy - 10, 300, 74);
            using (SolidBrush inside = new SolidBrush(Color.FromArgb(196, 140, 78)))
            using (Pen rim = new Pen(Color.FromArgb(224, 176, 110), 6f))
            using (SolidBrush paper = new SolidBrush(Color.FromArgb(240, 226, 196)))
            {
                g.FillEllipse(inside, topRim);
                g.DrawEllipse(rim, topRim);
                g.FillEllipse(paper, topRim.X + 20, topRim.Y + 12, topRim.Width - 40, topRim.Height - 26);
            }

            float[] xs = { cx - 78, cx + 6, cx + 84, cx - 34, cx + 46 };
            float[] ys = { cy + 10, cy + 6, cy + 14, cy + 30, cy + 28 };
            for (int i = 0; i < xs.Length; i++)
            {
                Sphere(g, xs[i], ys[i], 40, 25, Color.FromArgb(252, 244, 232));
                using (Pen pleat = new Pen(Color.FromArgb(150, 200, 170, 140), 2f))
                {
                    for (int p = -3; p <= 3; p++)
                    {
                        g.DrawLine(pleat, xs[i] + p * 9, ys[i] - 20, xs[i] + p * 10, ys[i] - 8);
                    }
                }
                Sphere(g, xs[i] + 2, ys[i] - 2, 11, 6, Color.FromArgb(250, 190, 140));
            }

            Sphere(g, cx + 176, cy + 80, 36, 15, Color.White);
            Sphere(g, cx + 176, cy + 78, 26, 10, Color.FromArgb(96, 42, 22));
            Sphere(g, cx + 164, cy + 77, 5, 3, Color.FromArgb(220, 60, 40));
        }

        private static void DrawSushi(Graphics g, Random random, float cx, float cy)
        {
            Shadow(g, cx, cy + 92, 200, 16);

            RectangleF board = new RectangleF(cx - 210, cy - 26, 420, 130);
            using (GraphicsPath path = B6Round(board, 30f))
            using (LinearGradientBrush wood = Vertical(board, Color.FromArgb(196, 140, 84), Color.FromArgb(132, 84, 48)))
            using (Pen edge = new Pen(Color.FromArgb(110, 255, 220, 170), 2f))
            {
                g.FillPath(wood, path);
                g.DrawPath(edge, path);
            }

            Color[] topping = { Color.FromArgb(246, 128, 84), Color.FromArgb(214, 62, 66), Color.FromArgb(250, 150, 98), Color.FromArgb(244, 214, 120) };
            for (int i = 0; i < 4; i++)
            {
                float x = cx - 190 + i * 62;
                float y = cy + 18 + (i % 2) * 14;
                RectangleF rice = new RectangleF(x, y + 6, 54, 30);
                using (GraphicsPath rp = B6Round(rice, 12f))
                using (LinearGradientBrush rb = Vertical(rice, Color.White, Color.FromArgb(226, 220, 206)))
                {
                    g.FillPath(rb, rp);
                }
                RectangleF fish = new RectangleF(x - 4, y - 8, 62, 26);
                using (GraphicsPath fp = B6Round(fish, 12f))
                using (LinearGradientBrush fb = Vertical(fish, Lighten(topping[i], 0.22f), Darken(topping[i], 0.1f)))
                using (Pen stripe = new Pen(Color.FromArgb(120, 255, 255, 255), 2f))
                {
                    g.FillPath(fb, fp);
                    for (int s = 0; s < 4; s++)
                    {
                        g.DrawLine(stripe, x + 8 + s * 14, y - 4, x + 14 + s * 14, y + 14);
                    }
                }
            }

            for (int i = 0; i < 3; i++)
            {
                float x = cx + 62 + i * 52;
                float y = cy + 24 + (i % 2) * 12;
                Sphere(g, x, y, 26, 18, Color.FromArgb(34, 40, 30));
                Sphere(g, x, y - 3, 20, 12, Color.White);
                Sphere(g, x, y - 4, 8, 5, i == 1 ? Color.FromArgb(246, 128, 84) : Color.FromArgb(240, 190, 50));
            }

            Sphere(g, cx - 28, cy + 82, 18, 10, Color.FromArgb(150, 196, 90));
            Sphere(g, cx + 20, cy + 84, 24, 10, Color.FromArgb(246, 168, 176));
            using (Pen stick = new Pen(Color.FromArgb(70, 40, 24), 5f))
            {
                stick.StartCap = LineCap.Round;
                stick.EndCap = LineCap.Round;
                g.DrawLine(stick, cx + 110, cy + 96, cx + 220, cy + 54);
                g.DrawLine(stick, cx + 120, cy + 104, cx + 230, cy + 64);
            }
        }

        private static void DrawPizza(Graphics g, Random random, float cx, float cy)
        {
            Shadow(g, cx, cy + 76, 200, 16);
            GraphicsState st = g.Save();
            g.TranslateTransform(cx, cy + 34);
            g.ScaleTransform(1f, 0.44f);

            using (SolidBrush crust = new SolidBrush(Color.FromArgb(210, 148, 70)))
            using (SolidBrush crustDark = new SolidBrush(Color.FromArgb(160, 100, 42)))
            using (SolidBrush sauce = new SolidBrush(Color.FromArgb(206, 70, 44)))
            using (SolidBrush cheese = new SolidBrush(Color.FromArgb(250, 214, 96)))
            {
                g.FillEllipse(crustDark, -178, -166, 356, 356);
                g.FillEllipse(crust, -178, -176, 356, 356);
                g.FillEllipse(sauce, -150, -148, 300, 300);
                g.FillEllipse(cheese, -140, -140, 280, 280);
            }
            using (SolidBrush melt = new SolidBrush(Color.FromArgb(120, 255, 238, 160)))
            {
                for (int i = 0; i < 22; i++)
                {
                    double a = random.NextDouble() * Math.PI * 2;
                    double d = Math.Sqrt(random.NextDouble()) * 120;
                    g.FillEllipse(melt, (float)(Math.Cos(a) * d) - 14, (float)(Math.Sin(a) * d) - 12, 30, 26);
                }
            }
            for (int i = 0; i < 9; i++)
            {
                double a = i * 0.7 + random.NextDouble() * 0.3;
                double d = 40 + (i % 3) * 36;
                float x = (float)(Math.Cos(a) * d);
                float y = (float)(Math.Sin(a) * d);
                using (SolidBrush pep = new SolidBrush(Color.FromArgb(188, 44, 38)))
                using (SolidBrush pepDark = new SolidBrush(Color.FromArgb(130, 24, 22)))
                {
                    g.FillEllipse(pepDark, x - 20, y - 18, 40, 40);
                    g.FillEllipse(pep, x - 19, y - 20, 38, 38);
                }
            }
            for (int i = 0; i < 6; i++)
            {
                double a = i * 1.1 + 0.4;
                float x = (float)(Math.Cos(a) * 92);
                float y = (float)(Math.Sin(a) * 92);
                using (SolidBrush leaf = new SolidBrush(Color.FromArgb(70, 150, 70)))
                {
                    g.FillEllipse(leaf, x - 12, y - 8, 28, 16);
                }
            }
            using (Pen cut = new Pen(Color.FromArgb(150, 150, 96, 30), 3f))
            {
                for (int i = 0; i < 4; i++)
                {
                    double a = i * Math.PI / 4.0;
                    g.DrawLine(cut, (float)(Math.Cos(a) * -170), (float)(Math.Sin(a) * -170), (float)(Math.Cos(a) * 170), (float)(Math.Sin(a) * 170));
                }
            }
            g.Restore(st);
        }

        private static void DrawSteak(Graphics g, Random random, float cx, float cy)
        {
            RectangleF inner = DrawPlateBase(g, cx, cy, 410f);
            float mx = inner.X + inner.Width / 2f;
            float my = inner.Y + inner.Height / 2f;

            Sphere(g, mx - 6, my + 24, 120, 30, Color.FromArgb(120, 52, 24));

            RectangleF meat = new RectangleF(mx - 112, my - 34, 190, 82);
            using (GraphicsPath path = B6Round(meat, 36f))
            using (LinearGradientBrush brush = Vertical(meat, Color.FromArgb(150, 80, 44), Color.FromArgb(82, 38, 20)))
            {
                g.FillPath(brush, path);
                GraphicsState inside = g.Save();
                g.SetClip(path);
                using (Pen mark = new Pen(Color.FromArgb(170, 36, 16, 8), 6f))
                {
                    for (int i = 0; i < 6; i++)
                    {
                        g.DrawLine(mark, meat.X + 14 + i * 32, meat.Y - 6, meat.X + 56 + i * 32, meat.Bottom + 6);
                    }
                }
                g.Restore(inside);
            }
            using (Pen shine = new Pen(Color.FromArgb(100, 255, 214, 170), 4f))
            {
                shine.StartCap = LineCap.Round;
                shine.EndCap = LineCap.Round;
                g.DrawArc(shine, meat.X + 20, meat.Y + 4, meat.Width - 40, 40, 200, 100);
            }

            Sphere(g, mx + 116, my + 6, 38, 28, Color.FromArgb(252, 240, 200));
            Sphere(g, mx + 116, my - 6, 14, 8, Color.FromArgb(250, 210, 60));
            using (Pen bean = new Pen(Color.FromArgb(80, 150, 60), 6f))
            {
                bean.StartCap = LineCap.Round;
                bean.EndCap = LineCap.Round;
                for (int i = 0; i < 5; i++)
                {
                    g.DrawLine(bean, mx - 140 + i * 6, my + 44 + i * 3, mx - 100 + i * 6, my + 56 + i * 3);
                }
            }
            Sphere(g, mx - 70, my + 58, 14, 10, Color.FromArgb(214, 52, 44));
            Sphere(g, mx - 44, my + 62, 13, 9, Color.FromArgb(220, 60, 46));
            Sphere(g, mx + 170, my + 36, 12, 8, Color.FromArgb(120, 190, 96));
        }

        private static void DrawBurger(Graphics g, Random random, float cx, float cy)
        {
            Shadow(g, cx, cy + 92, 150, 14);
            RectangleF board = new RectangleF(cx - 170, cy + 52, 340, 50);
            using (LinearGradientBrush wood = Vertical(board, Color.FromArgb(214, 164, 104), Color.FromArgb(160, 108, 62)))
            {
                g.FillEllipse(wood, board);
            }

            RectangleF bottom = new RectangleF(cx - 112, cy + 38, 224, 40);
            using (GraphicsPath bp = B6Round(bottom, 18f))
            using (LinearGradientBrush bb = Vertical(bottom, Color.FromArgb(226, 164, 80), Color.FromArgb(180, 114, 46)))
            {
                g.FillPath(bb, bp);
            }

            RectangleF patty = new RectangleF(cx - 118, cy + 12, 236, 34);
            using (GraphicsPath pp = B6Round(patty, 16f))
            using (LinearGradientBrush pb = Vertical(patty, Color.FromArgb(130, 68, 36), Color.FromArgb(78, 38, 20)))
            {
                g.FillPath(pb, pp);
            }

            Color[] leaf = { Color.FromArgb(110, 184, 76), Color.FromArgb(78, 150, 64) };
            for (int i = 0; i < 9; i++)
            {
                Sphere(g, cx - 112 + i * 28, cy + 8 + (i % 2) * 5, 20, 8, leaf[i % 2]);
            }

            PointF[] cheese = { new PointF(cx - 120, cy + 4), new PointF(cx + 120, cy + 4), new PointF(cx + 98, cy + 34), new PointF(cx + 70, cy + 14), new PointF(cx + 20, cy + 40), new PointF(cx - 24, cy + 14), new PointF(cx - 84, cy + 30) };
            using (SolidBrush cb = new SolidBrush(Color.FromArgb(250, 196, 50)))
            {
                g.FillPolygon(cb, cheese);
            }

            Sphere(g, cx - 60, cy - 6, 26, 8, Color.FromArgb(214, 52, 44));
            Sphere(g, cx + 54, cy - 6, 26, 8, Color.FromArgb(220, 60, 46));

            RectangleF top = new RectangleF(cx - 124, cy - 90, 248, 118);
            using (GraphicsPath tp = new GraphicsPath())
            {
                tp.AddArc(top, 180, 180);
                tp.AddLine(top.Right, top.Y + top.Height / 2f, top.X, top.Y + top.Height / 2f);
                tp.CloseFigure();
                using (PathGradientBrush tb = new PathGradientBrush(tp))
                {
                    tb.CenterPoint = new PointF(cx - 40, cy - 60);
                    tb.CenterColor = Color.FromArgb(250, 196, 104);
                    tb.SurroundColors = new[] { Color.FromArgb(176, 106, 40) };
                    g.FillPath(tb, tp);
                }
            }
            using (SolidBrush seed = new SolidBrush(Color.FromArgb(250, 244, 220)))
            {
                for (int i = 0; i < 16; i++)
                {
                    GraphicsState st = g.Save();
                    g.TranslateTransform(cx - 80 + random.Next(160), cy - 74 + random.Next(40));
                    g.RotateTransform(random.Next(0, 180));
                    g.FillEllipse(seed, -4, -2, 8, 4);
                    g.Restore(st);
                }
            }
        }

        private static void DrawFries(Graphics g, Random random, float cx, float cy)
        {
            RectangleF inner = DrawPlateBase(g, cx, cy, 400f);
            float mx = inner.X + inner.Width / 2f;
            float my = inner.Y + inner.Height / 2f;

            Color[] golds = { Color.FromArgb(246, 200, 70), Color.FromArgb(240, 178, 52), Color.FromArgb(252, 214, 100) };
            for (int i = 0; i < 46; i++)
            {
                float x = mx - 70 + random.Next(150);
                float y = my + 18 - random.Next(70);
                GraphicsState st = g.Save();
                g.TranslateTransform(x, y);
                g.RotateTransform(-70f + random.Next(140));
                using (SolidBrush fry = new SolidBrush(golds[random.Next(golds.Length)]))
                using (Pen edge = new Pen(Color.FromArgb(90, 150, 90, 20), 1f))
                {
                    g.FillRectangle(fry, -4, -34, 9, 66);
                    g.DrawRectangle(edge, -4, -34, 9, 66);
                }
                g.Restore(st);
            }

            for (int i = 0; i < 3; i++)
            {
                Sphere(g, mx - 150 + i * 6, my + 4 + i * 20, 20, 15, Color.FromArgb(210, 140, 52));
                Crumbs(g, random, mx - 150 + i * 6, my + 4 + i * 20, 15, 10, 9, Color.FromArgb(140, 150, 84, 22));
            }
            Sphere(g, mx + 140, my + 6, 36, 14, Color.FromArgb(252, 252, 250));
            Sphere(g, mx + 140, my + 4, 26, 9, Color.FromArgb(214, 60, 40));
            Crumbs(g, random, mx, my - 10, 70, 30, 24, Color.FromArgb(220, 255, 214, 90));
        }

        private static void DrawPasta(Graphics g, Random random, float cx, float cy)
        {
            RectangleF inner = DrawPlateBase(g, cx, cy, 400f);
            float mx = inner.X + inner.Width / 2f;
            float my = inner.Y + inner.Height / 2f;

            Color sauce = random.Next(2) == 0 ? Color.FromArgb(204, 78, 44) : Color.FromArgb(150, 92, 56);
            Color strand = Color.FromArgb(246, 208, 112);

            Sphere(g, mx, my + 6, 118, 50, strand);
            RectangleF dome = new RectangleF(mx - 118, my - 44, 236, 100);
            using (GraphicsPath clip = new GraphicsPath())
            {
                clip.AddEllipse(dome);
                GraphicsState state = g.Save();
                g.SetClip(clip);
                using (Pen dark = new Pen(Color.FromArgb(150, Darken(strand, 0.2f)), 3f))
                using (Pen light = new Pen(Color.FromArgb(170, 255, 244, 190), 3f))
                {
                    for (int i = 0; i < 18; i++)
                    {
                        float y = dome.Y + 4 + i * 5.6f;
                        PointF a = new PointF(dome.X - 8, y + random.Next(-8, 8));
                        PointF c1 = new PointF(mx - 60, y + random.Next(-26, 26));
                        PointF c2 = new PointF(mx + 60, y + random.Next(-26, 26));
                        PointF b = new PointF(dome.Right + 8, y + random.Next(-8, 8));
                        g.DrawBezier(i % 2 == 0 ? dark : light, a, c1, c2, b);
                    }
                }
                g.Restore(state);
            }

            using (SolidBrush cap = new SolidBrush(Color.FromArgb(235, sauce)))
            using (SolidBrush capLight = new SolidBrush(Color.FromArgb(90, 255, 255, 255)))
            {
                g.FillEllipse(cap, mx - 62, my - 40, 124, 44);
                g.FillEllipse(capLight, mx - 40, my - 36, 60, 12);
            }
            for (int i = 0; i < 5; i++)
            {
                Sphere(g, mx - 40 + i * 20, my - 22 + (i % 2) * 5, 11, 7, i % 2 == 0 ? Color.FromArgb(150, 78, 58) : Color.FromArgb(214, 60, 46));
            }
            for (int i = 0; i < 10; i++)
            {
                Sphere(g, mx - 44 + random.Next(88), my - 34 + random.Next(24), 4, 2.6f, Color.FromArgb(70, 150, 70));
            }
            Sphere(g, mx + 150, my + 40, 15, 8, Color.FromArgb(120, 190, 96));
            Sphere(g, mx - 150, my + 42, 15, 8, Color.FromArgb(214, 52, 44));
            Sphere(g, mx + 130, my - 6, 20, 9, Color.FromArgb(252, 248, 236));
        }

        private static void DrawSalad(Graphics g, Random random, float cx, float cy)
        {
            RectangleF inner = DrawPlateBase(g, cx, cy, 400f);
            float mx = inner.X + inner.Width / 2f;
            float my = inner.Y + inner.Height / 2f;

            Color[] greens =
            {
                Color.FromArgb(96, 170, 70), Color.FromArgb(130, 196, 90), Color.FromArgb(62, 134, 66), Color.FromArgb(168, 214, 110)
            };
            for (int i = 0; i < 26; i++)
            {
                double a = random.NextDouble() * Math.PI * 2;
                double d = Math.Sqrt(random.NextDouble());
                float x = mx + (float)(Math.Cos(a) * d * 130);
                float y = my + (float)(Math.Sin(a) * d * 40);
                GraphicsState st = g.Save();
                g.TranslateTransform(x, y);
                g.RotateTransform(random.Next(-60, 60));
                Sphere(g, 0, 0, random.Next(22, 36), random.Next(9, 15), greens[random.Next(greens.Length)]);
                g.Restore(st);
            }
            for (int i = 0; i < 5; i++)
            {
                float x = mx - 100 + i * 50 + random.Next(-10, 10);
                float y = my + 6 + (i % 2) * 22;
                Sphere(g, x, y, 15, 12, Color.FromArgb(220, 56, 44));
            }
            for (int i = 0; i < 4; i++)
            {
                float x = mx - 70 + i * 48;
                float y = my + 26 - (i % 2) * 18;
                using (SolidBrush skin = new SolidBrush(Color.FromArgb(110, 170, 80)))
                using (SolidBrush pale = new SolidBrush(Color.FromArgb(214, 236, 170)))
                {
                    g.FillEllipse(skin, x - 17, y - 10, 34, 20);
                    g.FillEllipse(pale, x - 13, y - 7, 26, 14);
                }
            }
            Crumbs(g, random, mx, my + 6, 120, 30, 22, Color.FromArgb(240, 214, 70));
            Sphere(g, mx + 130, my + 14, 22, 15, Color.FromArgb(252, 248, 236));
            Sphere(g, mx + 130, my + 12, 10, 7, Color.FromArgb(250, 190, 40));
        }

        private static void DrawNameBand(Graphics graphics, string dishName, int width, int height)
        {
            Rectangle band = new Rectangle(0, height - 70, width, 70);
            using (SolidBrush bandBrush = new SolidBrush(Color.FromArgb(150, 43, 22, 16)))
            using (Font font = new Font("Segoe UI", 24f, FontStyle.Bold, GraphicsUnit.Pixel))
            using (SolidBrush textBrush = new SolidBrush(Color.FromArgb(255, 246, 230)))
            using (StringFormat format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            })
            {
                graphics.FillRectangle(bandBrush, band);
                graphics.DrawString(dishName, font, textBrush, new RectangleF(16, band.Y, width - 32, band.Height), format);
            }
        }
    }
}
