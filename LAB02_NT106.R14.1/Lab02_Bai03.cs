using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Lab02
{
    public partial class Lab02_Bai03 : Form
    {
        private const string OutputFileName = "output3.txt";
        private const string EmptyValue = "—";

        private static readonly Color SkyBlue = Color.FromArgb(143, 176, 255);
        private static readonly Color MoonGold = Color.FromArgb(255, 214, 126);
        private static readonly Color ErrorCoral = Color.FromArgb(255, 138, 128);
        private static readonly Color NightDeep = Color.FromArgb(11, 15, 30);

        private readonly List<string> expressions = new List<string>();
        private string inputFilePath = string.Empty;

        public Lab02_Bai03()
        {
            InitializeComponent();
            pnlTop.Paint += PaintNightSky;
            StyleButton(btnRead, SkyBlue, NightDeep, Color.FromArgb(180, 204, 255), false);
            StyleButton(btnCalc, MoonGold, NightDeep, Color.FromArgb(255, 228, 165), false);
            StyleButton(btnClear, Color.FromArgb(20, 27, 52), Color.FromArgb(205, 214, 240), Color.FromArgb(32, 42, 80), true);
            StyleButton(btnExit, Color.FromArgb(158, 48, 62), Color.White, Color.FromArgb(190, 64, 80), false);
            ConfigureGrid();
            ResetView();
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Chọn file input3.txt",
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                CheckFileExists = true
            };

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            List<string> lines = new List<string>();
            try
            {
                using FileStream stream = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read);
                using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        lines.Add(line.Trim());
                    }
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show("Không thể đọc file:\r\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (lines.Count == 0)
            {
                MessageBox.Show("File không có biểu thức nào để tính!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            inputFilePath = dialog.FileName;
            expressions.Clear();
            expressions.AddRange(lines);

            dgvResult.Rows.Clear();
            for (int index = 0; index < expressions.Count; index++)
            {
                dgvResult.Rows.Add((index + 1).ToString(), expressions[index], EmptyValue);
            }

            lblEmpty.Visible = false;
            lblSummary.Text = "Đã nạp " + expressions.Count + " biểu thức";
            lblStatus.Text = "Đã đọc: " + inputFilePath;
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            if (expressions.Count == 0)
            {
                MessageBox.Show("Vui lòng đọc file input3.txt trước khi tính!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<string> outputLines = new List<string>();
            int successCount = 0;
            int errorCount = 0;

            for (int index = 0; index < expressions.Count; index++)
            {
                string resultText;
                bool isValid;

                try
                {
                    decimal value = ExpressionEvaluator.Evaluate(expressions[index]);
                    resultText = FormatNumber(value);
                    isValid = true;
                    successCount++;
                }
                catch (Exception ex) when (ex is FormatException || ex is ArithmeticException)
                {
                    resultText = "Lỗi: " + ex.Message;
                    isValid = false;
                    errorCount++;
                }

                DataGridViewCell resultCell = dgvResult.Rows[index].Cells[colResult.Index];
                resultCell.Value = resultText;
                resultCell.Style.ForeColor = isValid ? MoonGold : ErrorCoral;
                outputLines.Add(expressions[index] + " = " + resultText);
            }

            string outputDirectory = Path.GetDirectoryName(inputFilePath) ?? Environment.CurrentDirectory;
            string outputPath = Path.Combine(outputDirectory, OutputFileName);

            try
            {
                using StreamWriter writer = new StreamWriter(outputPath, false, new UTF8Encoding(false));
                foreach (string outputLine in outputLines)
                {
                    writer.WriteLine(outputLine);
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show("Không thể ghi file:\r\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lblSummary.Text = "Tổng " + expressions.Count + "   ·   Thành công " + successCount + "   ·   Lỗi " + errorCount;
            lblStatus.Text = "Đã ghi: " + outputPath;
            MessageBox.Show("Đã ghi kết quả vào file " + OutputFileName + "!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ResetView();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ResetView()
        {
            expressions.Clear();
            inputFilePath = string.Empty;
            dgvResult.Rows.Clear();
            lblEmpty.Visible = true;
            lblSummary.Text = "Chưa có dữ liệu";
            lblStatus.Text = "Bấm ĐỌC FILE để nạp input3.txt";
        }

        private static string FormatNumber(decimal value)
        {
            return value.ToString("0.##########", CultureInfo.InvariantCulture);
        }

        private void ConfigureGrid()
        {
            Color panelColor = Color.FromArgb(20, 27, 52);
            Color headerColor = Color.FromArgb(28, 36, 72);

            dgvResult.EnableHeadersVisualStyles = false;
            dgvResult.BackgroundColor = panelColor;
            dgvResult.BorderStyle = BorderStyle.None;
            dgvResult.GridColor = Color.FromArgb(36, 46, 86);
            dgvResult.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvResult.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvResult.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvResult.ColumnHeadersHeight = 46;
            dgvResult.RowHeadersVisible = false;
            dgvResult.AllowUserToAddRows = false;
            dgvResult.AllowUserToDeleteRows = false;
            dgvResult.AllowUserToResizeRows = false;
            dgvResult.AllowUserToResizeColumns = false;
            dgvResult.ReadOnly = true;
            dgvResult.MultiSelect = false;
            dgvResult.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvResult.RowTemplate.Height = 46;

            dgvResult.ColumnHeadersDefaultCellStyle.BackColor = headerColor;
            dgvResult.ColumnHeadersDefaultCellStyle.ForeColor = SkyBlue;
            dgvResult.ColumnHeadersDefaultCellStyle.SelectionBackColor = headerColor;
            dgvResult.ColumnHeadersDefaultCellStyle.SelectionForeColor = SkyBlue;
            dgvResult.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvResult.ColumnHeadersDefaultCellStyle.Padding = new Padding(12, 0, 12, 0);

            dgvResult.DefaultCellStyle.BackColor = panelColor;
            dgvResult.DefaultCellStyle.ForeColor = Color.FromArgb(226, 232, 255);
            dgvResult.DefaultCellStyle.SelectionBackColor = Color.FromArgb(42, 56, 108);
            dgvResult.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvResult.DefaultCellStyle.Font = new Font("Segoe UI", 12F);
            dgvResult.DefaultCellStyle.Padding = new Padding(12, 0, 12, 0);
            dgvResult.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(24, 32, 62);
            dgvResult.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(42, 56, 108);

            colIndex.DefaultCellStyle.ForeColor = Color.FromArgb(130, 146, 196);
            colIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colIndex.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colResult.DefaultCellStyle.ForeColor = MoonGold;
            colResult.DefaultCellStyle.Font = new Font("Palatino Linotype", 14F, FontStyle.Bold);
            colResult.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colResult.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private static void StyleButton(Button button, Color back, Color fore, Color hover, bool outlined)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.Cursor = Cursors.Hand;
            button.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button.UseVisualStyleBackColor = false;
            button.BackColor = back;
            button.ForeColor = fore;
            button.FlatAppearance.MouseOverBackColor = hover;
            button.FlatAppearance.BorderSize = outlined ? 1 : 0;
            if (outlined)
            {
                button.FlatAppearance.BorderColor = Color.FromArgb(70, 84, 140);
            }
        }

        private void PaintNightSky(object? sender, PaintEventArgs e)
        {
            Rectangle area = pnlTop.ClientRectangle;
            if (area.Width <= 0 || area.Height <= 0)
            {
                return;
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (LinearGradientBrush sky = new LinearGradientBrush(area, Color.FromArgb(8, 11, 26), Color.FromArgb(34, 26, 80), LinearGradientMode.Horizontal))
            {
                e.Graphics.FillRectangle(sky, area);
            }

            Random random = new Random(11);
            for (int index = 0; index < 70; index++)
            {
                int x = random.Next(area.Width);
                int y = random.Next(area.Height - 6);
                int size = random.Next(1, 3);
                int alpha = random.Next(70, 220);
                bool insideSubtitle = x < 520 && y > 54 && y < 84;
                if (insideSubtitle)
                {
                    continue;
                }

                using SolidBrush starBrush = new SolidBrush(Color.FromArgb(alpha, 235, 240, 255));
                e.Graphics.FillEllipse(starBrush, x, y, size, size);
            }

            using GraphicsPath disc = new GraphicsPath();
            disc.AddEllipse(28, 24, 40, 40);
            using GraphicsPath bite = new GraphicsPath();
            bite.AddEllipse(41, 17, 38, 38);
            using Region crescent = new Region(disc);
            crescent.Exclude(bite);
            using SolidBrush moonBrush = new SolidBrush(Color.FromArgb(255, 226, 150));
            e.Graphics.FillRegion(moonBrush, crescent);
        }
    }

    internal sealed class ExpressionEvaluator
    {
        private readonly string text;
        private int position;

        private ExpressionEvaluator(string expression)
        {
            text = Normalize(expression);
        }

        public static decimal Evaluate(string expression)
        {
            ExpressionEvaluator evaluator = new ExpressionEvaluator(expression);

            try
            {
                decimal value = evaluator.ParseExpression();
                evaluator.SkipSpaces();
                if (evaluator.position < evaluator.text.Length)
                {
                    throw evaluator.Unexpected();
                }
                return value;
            }
            catch (OverflowException)
            {
                throw new OverflowException("Số quá lớn, vượt giới hạn tính toán");
            }
        }

        private static string Normalize(string expression)
        {
            return expression
                .Replace('–', '-')
                .Replace('—', '-')
                .Replace('−', '-')
                .Replace('×', '*')
                .Replace('÷', '/');
        }

        private decimal ParseExpression()
        {
            decimal value = ParseTerm();
            while (true)
            {
                SkipSpaces();
                if (Match('+'))
                {
                    value += ParseTerm();
                }
                else if (Match('-'))
                {
                    value -= ParseTerm();
                }
                else
                {
                    return value;
                }
            }
        }

        private decimal ParseTerm()
        {
            decimal value = ParseFactor();
            while (true)
            {
                SkipSpaces();
                if (Match('*'))
                {
                    value *= ParseFactor();
                }
                else if (Match('/'))
                {
                    decimal divisor = ParseFactor();
                    if (divisor == 0m)
                    {
                        throw new DivideByZeroException("Không thể chia cho 0");
                    }
                    value /= divisor;
                }
                else
                {
                    return value;
                }
            }
        }

        private decimal ParseFactor()
        {
            SkipSpaces();

            if (Match('-'))
            {
                return -ParseFactor();
            }

            if (Match('+'))
            {
                return ParseFactor();
            }

            if (Match('('))
            {
                decimal inner = ParseExpression();
                SkipSpaces();
                if (!Match(')'))
                {
                    throw new FormatException("Thiếu dấu đóng ngoặc ')'");
                }
                return inner;
            }

            return ParseNumber();
        }

        private decimal ParseNumber()
        {
            int start = position;
            while (position < text.Length && (IsDigit(text[position]) || text[position] == '.'))
            {
                position++;
            }

            if (start == position)
            {
                throw Unexpected();
            }

            string token = text.Substring(start, position - start);
            if (!decimal.TryParse(token, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal number))
            {
                throw new FormatException("Số không hợp lệ '" + token + "'");
            }
            return number;
        }

        private FormatException Unexpected()
        {
            if (position >= text.Length)
            {
                return new FormatException("Biểu thức chưa hoàn chỉnh");
            }
            return new FormatException("Ký tự không hợp lệ '" + text[position] + "' tại vị trí " + (position + 1));
        }

        private bool Match(char expected)
        {
            if (position < text.Length && text[position] == expected)
            {
                position++;
                return true;
            }
            return false;
        }

        private void SkipSpaces()
        {
            while (position < text.Length && char.IsWhiteSpace(text[position]))
            {
                position++;
            }
        }

        private static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }
    }
}
