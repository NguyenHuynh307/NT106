using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Lab02
{
    public partial class Lab02_Bai01 : Form
    {
        private const string OutputFileName = "output1.txt";

        private string inputFilePath = string.Empty;
        private string originalContent = string.Empty;

        public Lab02_Bai01()
        {
            InitializeComponent();
            Theme.StylePrimary(btnRead);
            Theme.StyleAccent(btnWrite);
            pnlHeader.Paint += Theme.PaintHeaderBackground;
            Theme.StyleSecondary(btnClear);
            Theme.StyleDanger(btnExit);
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Chọn file input1.txt",
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                using FileStream stream = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read);
                using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                originalContent = reader.ReadToEnd();
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show("Không thể đọc file:\r\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            inputFilePath = dialog.FileName;
            rtbOriginal.Text = originalContent;
            rtbUpper.Clear();
            lblStatus.Text = string.Format("Đã đọc: {0}   |   {1} ký tự", Path.GetFileName(inputFilePath), originalContent.Length);
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(inputFilePath))
            {
                MessageBox.Show("Vui lòng đọc file input1.txt trước khi ghi!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string upperContent = originalContent.ToUpper();
            string outputPath = Path.Combine(Path.GetDirectoryName(inputFilePath), OutputFileName);

            try
            {
                using StreamWriter writer = new StreamWriter(outputPath, false, Encoding.UTF8);
                writer.Write(upperContent);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show("Không thể ghi file:\r\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            rtbUpper.Text = upperContent;
            lblStatus.Text = "Đã ghi: " + outputPath;
            MessageBox.Show("Đã ghi nội dung in hoa vào file " + OutputFileName + "!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            inputFilePath = string.Empty;
            originalContent = string.Empty;
            rtbOriginal.Clear();
            rtbUpper.Clear();
            lblStatus.Text = "Chưa chọn file";
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
