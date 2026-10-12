#nullable disable

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace Lab02.Bai07Browser
{
    internal static class FileCatalog
    {
        public const string DriveIcon = "drive";
        public const string FolderIcon = "folder";
        public const string LockedIcon = "locked";
        public const string DocumentIcon = "document";
        public const string ImageIcon = "image";
        public const string TextIcon = "text";
        public const string ArchiveIcon = "archive";
        public const string BinaryIcon = "binary";

        private static readonly CultureInfo Vietnamese = new CultureInfo("vi-VN");

        private static readonly HashSet<string> ImageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".ico", ".tif", ".tiff"
        };

        private static readonly HashSet<string> TextExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".txt", ".md", ".log", ".csv", ".tsv", ".json", ".xml", ".yml", ".yaml", ".ini", ".cfg", ".config", ".conf",
            ".cs", ".vb", ".c", ".h", ".cpp", ".hpp", ".java", ".py", ".js", ".ts", ".html", ".htm", ".css", ".sql",
            ".bat", ".cmd", ".ps1", ".sh", ".sln", ".csproj", ".resx", ".props", ".targets", ".gitignore", ".rtf"
        };

        private static readonly HashSet<string> ArchiveExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".zip", ".rar", ".7z", ".tar", ".gz", ".bz2", ".iso"
        };

        private static readonly HashSet<string> BinaryExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".exe", ".dll", ".msi", ".sys", ".bin", ".dat", ".db", ".sqlite", ".pdb", ".obj"
        };

        private static readonly Dictionary<string, string> Descriptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { ".txt", "Tệp văn bản" }, { ".md", "Tài liệu Markdown" }, { ".log", "Tệp nhật ký" },
            { ".csv", "Bảng dữ liệu CSV" }, { ".json", "Dữ liệu JSON" }, { ".xml", "Dữ liệu XML" },
            { ".cs", "Mã nguồn C#" }, { ".html", "Trang web HTML" }, { ".htm", "Trang web HTML" },
            { ".pdf", "Tài liệu PDF" }, { ".docx", "Tài liệu Word" }, { ".xlsx", "Bảng tính Excel" },
            { ".pptx", "Bản trình chiếu PowerPoint" }, { ".zip", "Tệp nén ZIP" }, { ".rar", "Tệp nén RAR" },
            { ".exe", "Chương trình" }, { ".dll", "Thư viện liên kết động" }, { ".db", "Cơ sở dữ liệu" },
            { ".sqlite", "Cơ sở dữ liệu SQLite" }, { ".mp3", "Âm thanh MP3" }, { ".mp4", "Video MP4" }
        };

        public static bool IsImage(string extension)
        {
            return ImageExtensions.Contains(extension ?? string.Empty);
        }

        public static bool IsText(string extension)
        {
            return TextExtensions.Contains(extension ?? string.Empty);
        }

        public static string IconKeyFor(string extension)
        {
            extension = extension ?? string.Empty;
            if (ImageExtensions.Contains(extension)) return ImageIcon;
            if (TextExtensions.Contains(extension)) return TextIcon;
            if (ArchiveExtensions.Contains(extension)) return ArchiveIcon;
            if (BinaryExtensions.Contains(extension)) return BinaryIcon;
            return DocumentIcon;
        }

        public static string Describe(string extension, bool isDirectory, bool isDriveRoot)
        {
            if (isDriveRoot) return "Ổ đĩa";
            if (isDirectory) return "Thư mục";

            extension = extension ?? string.Empty;
            string description;
            if (Descriptions.TryGetValue(extension, out description)) return description;
            if (ImageExtensions.Contains(extension)) return "Hình ảnh " + extension.TrimStart('.').ToUpperInvariant();
            return extension.Length == 0 ? "Tệp" : "Tệp " + extension.TrimStart('.').ToUpperInvariant();
        }

        public static string FormatBytes(long bytes)
        {
            if (bytes < 1024)
            {
                return bytes.ToString("N0", Vietnamese) + " byte";
            }

            string[] units = { "KB", "MB", "GB", "TB" };
            double value = bytes;
            int unitIndex = -1;
            do
            {
                value /= 1024;
                unitIndex++;
            }
            while (value >= 1024 && unitIndex < units.Length - 1);

            return value.ToString("0.##", Vietnamese) + " " + units[unitIndex];
        }

        public static ImageList BuildImageList(int size)
        {
            ImageList list = new ImageList { ColorDepth = ColorDepth.Depth32Bit, ImageSize = new Size(size, size) };
            list.Images.Add(DriveIcon, DrawIcon(size, DrawDrive));
            list.Images.Add(FolderIcon, DrawIcon(size, DrawFolder));
            list.Images.Add(LockedIcon, DrawIcon(size, DrawLockedFolder));
            list.Images.Add(DocumentIcon, DrawIcon(size, (g, u) => DrawPage(g, u, Color.FromArgb(120, 130, 150))));
            list.Images.Add(TextIcon, DrawIcon(size, (g, u) => DrawPage(g, u, Color.FromArgb(34, 54, 94))));
            list.Images.Add(BinaryIcon, DrawIcon(size, (g, u) => DrawPage(g, u, Color.FromArgb(96, 96, 104))));
            list.Images.Add(ImageIcon, DrawIcon(size, DrawPicture));
            list.Images.Add(ArchiveIcon, DrawIcon(size, DrawArchive));
            return list;
        }

        private static Bitmap DrawIcon(int size, Action<Graphics, float> draw)
        {
            Bitmap bitmap = new Bitmap(size, size);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                draw(graphics, size / 16f);
            }
            return bitmap;
        }

        private static void DrawFolder(Graphics g, float u)
        {
            using (SolidBrush tab = new SolidBrush(Color.FromArgb(176, 138, 25)))
            using (LinearGradientBrush body = new LinearGradientBrush(new RectangleF(u, 5 * u, 14 * u, 9 * u), Color.FromArgb(238, 204, 96), Color.FromArgb(201, 162, 39), 90f))
            {
                g.FillRectangle(tab, u, 3 * u, 6 * u, 3 * u);
                g.FillRectangle(body, u, 5 * u, 14 * u, 9 * u);
            }
        }

        private static void DrawLockedFolder(Graphics g, float u)
        {
            using (SolidBrush tab = new SolidBrush(Color.FromArgb(120, 120, 126)))
            using (SolidBrush body = new SolidBrush(Color.FromArgb(160, 160, 166)))
            using (Pen cross = new Pen(Color.FromArgb(160, 30, 30), Math.Max(1.5f, 1.6f * u)))
            {
                g.FillRectangle(tab, u, 3 * u, 6 * u, 3 * u);
                g.FillRectangle(body, u, 5 * u, 14 * u, 9 * u);
                g.DrawLine(cross, 4 * u, 7 * u, 12 * u, 13 * u);
                g.DrawLine(cross, 12 * u, 7 * u, 4 * u, 13 * u);
            }
        }

        private static void DrawDrive(Graphics g, float u)
        {
            using (LinearGradientBrush body = new LinearGradientBrush(new RectangleF(u, 5 * u, 14 * u, 8 * u), Color.FromArgb(52, 74, 120), Color.FromArgb(22, 36, 66), 90f))
            using (SolidBrush led = new SolidBrush(Color.FromArgb(201, 162, 39)))
            using (Pen slot = new Pen(Color.FromArgb(120, 140, 180), Math.Max(1f, u)))
            {
                g.FillRectangle(body, u, 5 * u, 14 * u, 8 * u);
                g.FillEllipse(led, 11.5f * u, 8.2f * u, 2 * u, 2 * u);
                g.DrawLine(slot, 3 * u, 9 * u, 9 * u, 9 * u);
            }
        }

        private static void DrawPage(Graphics g, float u, Color accent)
        {
            PointF[] outline =
            {
                new PointF(3 * u, u), new PointF(10 * u, u), new PointF(13 * u, 4 * u),
                new PointF(13 * u, 15 * u), new PointF(3 * u, 15 * u)
            };

            using (SolidBrush paper = new SolidBrush(Color.FromArgb(252, 249, 240)))
            using (Pen border = new Pen(accent, Math.Max(1f, u)))
            using (SolidBrush fold = new SolidBrush(Color.FromArgb(220, 214, 196)))
            using (Pen line = new Pen(accent, Math.Max(1f, 0.9f * u)))
            {
                g.FillPolygon(paper, outline);
                g.DrawPolygon(border, outline);
                g.FillPolygon(fold, new[] { new PointF(10 * u, u), new PointF(10 * u, 4 * u), new PointF(13 * u, 4 * u) });
                g.DrawLine(line, 5 * u, 7 * u, 11 * u, 7 * u);
                g.DrawLine(line, 5 * u, 9.5f * u, 11 * u, 9.5f * u);
                g.DrawLine(line, 5 * u, 12 * u, 9 * u, 12 * u);
            }
        }

        private static void DrawPicture(Graphics g, float u)
        {
            using (LinearGradientBrush sky = new LinearGradientBrush(new RectangleF(u, 3 * u, 14 * u, 10 * u), Color.FromArgb(170, 214, 232), Color.FromArgb(222, 240, 246), 90f))
            using (SolidBrush sun = new SolidBrush(Color.FromArgb(232, 190, 60)))
            using (SolidBrush hills = new SolidBrush(Color.FromArgb(70, 130, 84)))
            using (Pen frame = new Pen(Color.FromArgb(34, 54, 94), Math.Max(1f, u)))
            {
                g.FillRectangle(sky, u, 3 * u, 14 * u, 10 * u);
                g.FillEllipse(sun, 10 * u, 5 * u, 3 * u, 3 * u);
                g.FillPolygon(hills, new[]
                {
                    new PointF(u, 13 * u), new PointF(5 * u, 8 * u), new PointF(8 * u, 11 * u),
                    new PointF(11 * u, 8.5f * u), new PointF(15 * u, 13 * u)
                });
                g.DrawRectangle(frame, u, 3 * u, 14 * u, 10 * u);
            }
        }

        private static void DrawArchive(Graphics g, float u)
        {
            using (SolidBrush body = new SolidBrush(Color.FromArgb(127, 29, 29)))
            using (Pen zipper = new Pen(Color.FromArgb(240, 226, 190), Math.Max(1f, u)))
            {
                g.FillRectangle(body, 3 * u, 2 * u, 10 * u, 13 * u);
                zipper.DashPattern = new[] { 1f, 1f };
                g.DrawLine(zipper, 8 * u, 2 * u, 8 * u, 11 * u);
                g.DrawRectangle(zipper, 6.5f * u, 11 * u, 3 * u, 2.5f * u);
            }
        }
    }
}
