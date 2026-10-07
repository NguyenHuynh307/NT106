#nullable disable

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Threading;

namespace Lab02.Bai07Browser
{
    internal enum FileContentKind
    {
        Text,
        Image,
        Binary,
        Message
    }

    internal sealed class FileContent : IDisposable
    {
        public FileContentKind Kind { get; set; }

        public string Text { get; set; } = string.Empty;

        public Image Picture { get; set; }

        public string Summary { get; set; } = string.Empty;

        public long SizeBytes { get; set; }

        public void Dispose()
        {
            if (Picture != null)
            {
                Picture.Dispose();
                Picture = null;
            }
        }
    }

    internal static class FileViewer
    {
        private const int MaxTextBytes = 256 * 1024;
        private const int SniffBytes = 4096;
        private const long MaxImageFileBytes = 60L * 1024 * 1024;
        private const int MaxImageEdge = 2400;
        private const int BytesPerHexLine = 16;

        public static FileContent Load(string filePath, CancellationToken token)
        {
            FileInfo info = new FileInfo(filePath);
            if (!info.Exists)
            {
                return CreateMessage("Tệp không còn tồn tại.", 0);
            }

            try
            {
                FileContent content = FileCatalog.IsImage(info.Extension)
                    ? LoadImage(info, token)
                    : LoadTextOrBinary(info, token);
                content.SizeBytes = info.Length;
                return content;
            }
            catch (UnauthorizedAccessException)
            {
                return CreateMessage("Không có quyền đọc tệp này.", info.Length);
            }
            catch (IOException ex)
            {
                return CreateMessage("Không đọc được tệp: " + ex.Message, info.Length);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is OutOfMemoryException || ex is System.Runtime.InteropServices.ExternalException)
            {
                return CreateMessage("Tệp hình ảnh bị hỏng hoặc định dạng không được hỗ trợ.", info.Length);
            }
        }

        private static FileContent CreateMessage(string message, long sizeBytes)
        {
            return new FileContent { Kind = FileContentKind.Message, Summary = message, SizeBytes = sizeBytes };
        }

        private static FileContent LoadImage(FileInfo info, CancellationToken token)
        {
            if (info.Length > MaxImageFileBytes)
            {
                return CreateMessage("Hình ảnh quá lớn (" + FileCatalog.FormatBytes(info.Length) + ") để xem trước.", info.Length);
            }

            using (FileStream stream = new FileStream(info.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (Image original = Image.FromStream(stream))
            {
                token.ThrowIfCancellationRequested();

                int originalWidth = original.Width;
                int originalHeight = original.Height;
                float ratio = Math.Min(1f, (float)MaxImageEdge / Math.Max(originalWidth, originalHeight));
                int targetWidth = Math.Max(1, (int)(originalWidth * ratio));
                int targetHeight = Math.Max(1, (int)(originalHeight * ratio));

                Bitmap picture = new Bitmap(targetWidth, targetHeight);
                using (Graphics graphics = Graphics.FromImage(picture))
                {
                    graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    graphics.DrawImage(original, 0, 0, targetWidth, targetHeight);
                }

                return new FileContent
                {
                    Kind = FileContentKind.Image,
                    Picture = picture,
                    Summary = originalWidth + " × " + originalHeight + " px"
                };
            }
        }

        private static FileContent LoadTextOrBinary(FileInfo info, CancellationToken token)
        {
            if (info.Length == 0)
            {
                return CreateMessage("Tệp rỗng (0 byte).", 0);
            }

            byte[] head = ReadLeadingBytes(info.FullName, SniffBytes);
            token.ThrowIfCancellationRequested();

            bool hasUnicodeMark = HasByteOrderMark(head);
            bool treatAsText = hasUnicodeMark || (!ContainsNul(head) && (FileCatalog.IsText(info.Extension) || LooksLikeText(head)));
            return treatAsText ? LoadText(info) : LoadHexDump(info, head);
        }

        private static FileContent LoadText(FileInfo info)
        {
            using (FileStream stream = new FileStream(info.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8, true))
            {
                char[] buffer = new char[MaxTextBytes];
                int charCount = reader.Read(buffer, 0, buffer.Length);
                bool truncated = reader.Peek() >= 0;

                string text = new string(buffer, 0, charCount).Replace("\r\n", "\n").Replace('\r', '\n');
                int lineCount = 1;
                foreach (char letter in text)
                {
                    if (letter == '\n') lineCount++;
                }

                text = text.Replace("\n", "\r\n");
                if (truncated)
                {
                    text += "\r\n\r\n… (chỉ hiển thị phần đầu tệp, tối đa " + FileCatalog.FormatBytes(MaxTextBytes) + ")";
                }

                return new FileContent
                {
                    Kind = FileContentKind.Text,
                    Text = text,
                    Summary = "Văn bản · " + lineCount.ToString("N0") + " dòng · " + reader.CurrentEncoding.EncodingName
                };
            }
        }

        private static FileContent LoadHexDump(FileInfo info, byte[] head)
        {
            StringBuilder builder = new StringBuilder(head.Length * 4);
            for (int offset = 0; offset < head.Length; offset += BytesPerHexLine)
            {
                builder.Append(offset.ToString("X8")).Append("  ");
                for (int column = 0; column < BytesPerHexLine; column++)
                {
                    int index = offset + column;
                    builder.Append(index < head.Length ? head[index].ToString("X2") + " " : "   ");
                    if (column == 7) builder.Append(' ');
                }

                builder.Append(" |");
                for (int column = 0; column < BytesPerHexLine && offset + column < head.Length; column++)
                {
                    byte value = head[offset + column];
                    builder.Append(value >= 32 && value < 127 ? (char)value : '.');
                }
                builder.AppendLine("|");
            }

            if (info.Length > head.Length)
            {
                builder.AppendLine().Append("… (chỉ hiển thị " + head.Length.ToString("N0") + " byte đầu tiên)");
            }

            return new FileContent
            {
                Kind = FileContentKind.Binary,
                Text = builder.ToString(),
                Summary = "Nhị phân · dạng hex"
            };
        }

        private static byte[] ReadLeadingBytes(string filePath, int count)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] buffer = new byte[count];
                int total = 0;
                while (total < count)
                {
                    int read = stream.Read(buffer, total, count - total);
                    if (read == 0) break;
                    total += read;
                }

                Array.Resize(ref buffer, total);
                return buffer;
            }
        }

        private static bool HasByteOrderMark(byte[] head)
        {
            if (head.Length >= 3 && head[0] == 0xEF && head[1] == 0xBB && head[2] == 0xBF) return true;
            return head.Length >= 2 && ((head[0] == 0xFF && head[1] == 0xFE) || (head[0] == 0xFE && head[1] == 0xFF));
        }

        private static bool ContainsNul(byte[] head)
        {
            foreach (byte value in head)
            {
                if (value == 0) return true;
            }
            return false;
        }

        private static bool LooksLikeText(byte[] head)
        {
            int controlCount = 0;
            foreach (byte value in head)
            {
                if (value < 32 && value != 9 && value != 10 && value != 13 && value != 12) controlCount++;
            }
            return controlCount * 20 < Math.Max(1, head.Length);
        }
    }
}
