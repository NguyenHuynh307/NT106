using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Lab02
{
    public partial class Lab02_Bai02 : Form
    {
        private const string EmptyValue = "—";
        private const string ReadyMessage = "Sẵn sàng. Bấm ĐỌC FILE hoặc kéo thả một file .txt vào cửa sổ.";

        public Lab02_Bai02()
        {
            InitializeComponent();
            pnlHeader.Paint += Theme.PaintHeaderBackground;
            Theme.StyleAccent(btnRead);
            Theme.StyleSecondary(btnClear);
            Theme.StyleDanger(btnExit);
            EnableFileDrop(this);
            ResetInfo();
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Chọn file .txt cần đọc",
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                CheckFileExists = true
            };

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            LoadFile(dialog.FileName);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ResetInfo();
            rtbContent.Clear();
            lblStatus.Text = ReadyMessage;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LoadFile(string path)
        {
            if (!string.Equals(Path.GetExtension(path), ".txt", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Vui lòng chọn một file có đuôi .txt!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string fileName;
            string fileUrl;
            long fileSize;
            string content;

            try
            {
                using FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using StreamReader reader = new StreamReader(stream, Encoding.UTF8, true);
                fileUrl = stream.Name;
                fileName = Path.GetFileName(fileUrl);
                fileSize = stream.Length;
                content = reader.ReadToEnd();
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show("Không thể đọc file:\r\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lblFileName.Text = fileName;
            lblPathValue.Text = fileUrl;
            lblSizeValue.Text = FormatSize(fileSize);
            lblLinesValue.Text = CountLines(content).ToString("N0");
            lblWordsValue.Text = CountWords(content).ToString("N0");
            lblCharsValue.Text = content.Length.ToString("N0");
            rtbContent.Text = content;

            toolTip.SetToolTip(lblFileName, fileName);
            toolTip.SetToolTip(lblPathValue, fileUrl);
            toolTip.SetToolTip(lblSizeValue, fileSize.ToString("N0") + " bytes");
            lblStatus.Text = "Đã đọc: " + fileUrl;
        }

        private void ResetInfo()
        {
            lblFileName.Text = "Chưa chọn file";
            lblPathValue.Text = EmptyValue;
            lblSizeValue.Text = EmptyValue;
            lblLinesValue.Text = EmptyValue;
            lblWordsValue.Text = EmptyValue;
            lblCharsValue.Text = EmptyValue;
            toolTip.SetToolTip(lblFileName, string.Empty);
            toolTip.SetToolTip(lblPathValue, string.Empty);
            toolTip.SetToolTip(lblSizeValue, string.Empty);
        }

        private void EnableFileDrop(Control control)
        {
            control.AllowDrop = true;
            control.DragEnter += OnFileDragEnter;
            control.DragDrop += OnFileDragDrop;

            foreach (Control child in control.Controls)
            {
                EnableFileDrop(child);
            }
        }

        private void OnFileDragEnter(object? sender, DragEventArgs e)
        {
            bool hasFile = e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop);
            e.Effect = hasFile ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void OnFileDragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
            {
                LoadFile(files[0]);
            }
        }

        private static int CountLines(string content)
        {
            int lineCount = 0;
            using StringReader lineReader = new StringReader(content);
            while (lineReader.ReadLine() != null)
            {
                lineCount++;
            }
            return lineCount;
        }

        private static int CountWords(string content)
        {
            return content.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries).Length;
        }

        private static string FormatSize(long bytes)
        {
            if (bytes < 1024)
            {
                return bytes.ToString("N0") + " bytes";
            }

            double kilobytes = bytes / 1024.0;
            if (kilobytes < 1024)
            {
                return kilobytes.ToString("N2") + " KB";
            }

            return (kilobytes / 1024.0).ToString("N2") + " MB";
        }
    }
}
