using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Lab02
{
    internal sealed class SinhVien
    {
        public string HoTen { get; set; } = string.Empty;
        public int MSSV { get; set; }
        public string DienThoai { get; set; } = string.Empty;
        public float Diem1 { get; set; }
        public float Diem2 { get; set; }
        public float Diem3 { get; set; }
        public float DiemTrungBinh { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? XepLoai { get; set; }
    }

    public partial class Lab02_Bai04 : Form
    {
        private const string OutputFileName = "output4.txt";
        private const string DefaultInputName = "input4.txt";
        private const string EmptyValue = "—";

        private static readonly Color ForestDeep = Color.FromArgb(10, 40, 34);
        private static readonly Color Forest = Color.FromArgb(24, 76, 60);
        private static readonly Color Lotus = Color.FromArgb(178, 58, 96);
        private static readonly Color Ink = Color.FromArgb(24, 38, 34);
        private static readonly Color Muted = Color.FromArgb(92, 110, 102);
        private static readonly Color LineColor = Color.FromArgb(206, 222, 212);
        private static readonly Color FieldNormal = Color.FromArgb(250, 252, 250);
        private static readonly Color FieldInvalid = Color.FromArgb(253, 236, 238);

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        private readonly List<SinhVien> pendingStudents = new List<SinhVien>();
        private List<SinhVien> loadedStudents = new List<SinhVien>();
        private int currentIndex;

        public Lab02_Bai04()
        {
            InitializeComponent();
            pnlHeader.Paint += PaintForestHeader;

            StyleButton(btnAdd, Forest, Color.White, Color.FromArgb(38, 104, 82), false);
            StyleButton(btnResetForm, Color.White, Forest, Color.FromArgb(238, 246, 241), true);
            StyleButton(btnDelete, Color.White, Lotus, Color.FromArgb(252, 240, 244), true);
            StyleButton(btnWrite, Lotus, Color.White, Color.FromArgb(200, 74, 114), false);
            StyleButton(btnRead, Forest, Color.White, Color.FromArgb(38, 104, 82), false);
            StyleButton(btnSearch, Lotus, Color.White, Color.FromArgb(200, 74, 114), false);
            StyleButton(btnBack, Color.White, Forest, Color.FromArgb(238, 246, 241), true);
            StyleButton(btnRefresh, Color.FromArgb(38, 104, 82), Color.White, Color.FromArgb(52, 130, 104), false);
            StyleButton(btnExit, Lotus, Color.White, Color.FromArgb(200, 74, 114), false);
            StyleButton(btnNext, Color.White, Forest, Color.FromArgb(238, 246, 241), true);

            foreach (TextBox box in new[] { txtName, txtId, txtPhone, txtScore1, txtScore2, txtScore3 })
            {
                errorProvider.SetIconPadding(box, -22);
            }

            ConfigureGrid();
            UpdatePreview();
            ShowStudent(-1);
            UpdateStats();
            UpdatePendingCount();
        }

        private void OnInputChanged(object sender, EventArgs e)
        {
            TextBox box = (TextBox)sender;
            string text = box.Text.Trim();
            string? error = text.Length == 0 ? null : GetFieldError(box, text);
            SetFieldError(box, error);
            UpdatePreview();
        }

        private void txtScore3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnAdd_Click(sender, EventArgs.Empty);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string idText = txtId.Text.Trim();
            string phone = txtPhone.Text.Trim();

            string? nameError = CheckName(name);
            string? idError = CheckId(idText);
            string? phoneError = CheckPhone(phone);
            string? score1Error = CheckScore(txtScore1.Text.Trim(), out float score1);
            string? score2Error = CheckScore(txtScore2.Text.Trim(), out float score2);
            string? score3Error = CheckScore(txtScore3.Text.Trim(), out float score3);

            if (idError == null)
            {
                int studentId = int.Parse(idText);
                if (pendingStudents.Exists(student => student.MSSV == studentId))
                {
                    idError = "MSSV đã tồn tại trong danh sách";
                }
            }

            SetFieldError(txtName, nameError);
            SetFieldError(txtId, idError);
            SetFieldError(txtPhone, phoneError);
            SetFieldError(txtScore1, score1Error);
            SetFieldError(txtScore2, score2Error);
            SetFieldError(txtScore3, score3Error);

            List<string> errors = new List<string>();
            TextBox? firstInvalid = null;
            CollectError(errors, ref firstInvalid, txtName, nameError);
            CollectError(errors, ref firstInvalid, txtId, idError);
            CollectError(errors, ref firstInvalid, txtPhone, phoneError);
            CollectError(errors, ref firstInvalid, txtScore1, score1Error is null ? null : "Điểm môn 1: " + score1Error);
            CollectError(errors, ref firstInvalid, txtScore2, score2Error is null ? null : "Điểm môn 2: " + score2Error);
            CollectError(errors, ref firstInvalid, txtScore3, score3Error is null ? null : "Điểm môn 3: " + score3Error);

            if (firstInvalid != null)
            {
                MessageBox.Show("Vui lòng kiểm tra lại:\r\n- " + string.Join("\r\n- ", errors), "Dữ liệu chưa hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                firstInvalid.Focus();
                return;
            }

            SinhVien student = new SinhVien
            {
                HoTen = name,
                MSSV = int.Parse(idText),
                DienThoai = phone,
                Diem1 = score1,
                Diem2 = score2,
                Diem3 = score3
            };

            pendingStudents.Add(student);
            int rowIndex = dgvPending.Rows.Add(idText, name, FormatScore(score1) + " · " + FormatScore(score2) + " · " + FormatScore(score3));
            dgvPending.ClearSelection();
            dgvPending.Rows[rowIndex].Selected = true;
            dgvPending.FirstDisplayedScrollingRowIndex = rowIndex;

            ClearInputs();
            UpdatePendingCount();
            lblStatus.Text = "Đã thêm: " + name + " (" + idText + ")";
        }

        private void btnResetForm_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvPending.SelectedRows.Count == 0)
            {
                lblStatus.Text = "Chọn một dòng trong danh sách để xóa.";
                return;
            }

            int index = dgvPending.SelectedRows[0].Index;
            string name = pendingStudents[index].HoTen;
            pendingStudents.RemoveAt(index);
            dgvPending.Rows.RemoveAt(index);
            UpdatePendingCount();
            lblStatus.Text = "Đã xóa: " + name;
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            if (pendingStudents.Count == 0)
            {
                MessageBox.Show("Danh sách đang trống, hãy thêm ít nhất một sinh viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Ghi danh sách sinh viên",
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                FileName = DefaultInputName
            };

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            if (WriteStudents(dialog.FileName, pendingStudents))
            {
                lblStatus.Text = "Đã ghi " + pendingStudents.Count + " sinh viên: " + dialog.FileName;
                MessageBox.Show("Đã ghi danh sách vào file " + Path.GetFileName(dialog.FileName) + "!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Chọn file input4.txt",
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                CheckFileExists = true
            };

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string json;
            try
            {
                using FileStream stream = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read);
                using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                json = reader.ReadToEnd();
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show("Không thể đọc file:\r\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<SinhVien>? students;
            try
            {
                students = JsonSerializer.Deserialize<List<SinhVien>>(json);
            }
            catch (JsonException)
            {
                MessageBox.Show("File không đúng định dạng danh sách sinh viên!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            students?.RemoveAll(student => student is null);
            if (students == null || students.Count == 0)
            {
                MessageBox.Show("File không có sinh viên nào!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (SinhVien student in students)
            {
                student.DiemTrungBinh = MathF.Round((student.Diem1 + student.Diem2 + student.Diem3) / 3f, 2);
                student.XepLoai = Classify(student.DiemTrungBinh);
            }

            string outputDirectory = Path.GetDirectoryName(dialog.FileName) ?? Environment.CurrentDirectory;
            string outputPath = Path.Combine(outputDirectory, OutputFileName);
            if (!WriteStudents(outputPath, students))
            {
                return;
            }

            loadedStudents = students;
            txtSearch.Clear();
            ShowStudent(0);
            UpdateStats();
            lblStatus.Text = "Đã đọc " + students.Count + " sinh viên, ghi kết quả: " + outputPath;
            MessageBox.Show("Đã tính điểm trung bình và ghi file " + OutputFileName + "!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            ShowStudent(currentIndex - 1);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            ShowStudent(currentIndex + 1);
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SearchStudent();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchStudent();
        }

        private void SearchStudent()
        {
            string keyword = txtSearch.Text.Trim();
            if (loadedStudents.Count == 0)
            {
                lblStatus.Text = "Hãy đọc file trước khi tìm kiếm.";
                return;
            }

            if (keyword.Length == 0)
            {
                lblStatus.Text = "Nhập tên hoặc MSSV cần tìm.";
                return;
            }

            CompareInfo compare = CultureInfo.InvariantCulture.CompareInfo;
            CompareOptions options = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace;

            for (int step = 1; step <= loadedStudents.Count; step++)
            {
                int index = (currentIndex + step) % loadedStudents.Count;
                SinhVien student = loadedStudents[index];
                bool matchId = student.MSSV.ToString().Contains(keyword);
                bool matchName = compare.IndexOf(student.HoTen ?? string.Empty, keyword, options) >= 0;

                if (matchId || matchName)
                {
                    ShowStudent(index);
                    lblStatus.Text = "Tìm thấy: " + student.HoTen + " (Enter để tìm tiếp)";
                    return;
                }
            }

            lblStatus.Text = "Không tìm thấy \"" + keyword + "\".";
        }

        private bool WriteStudents(string path, List<SinhVien> students)
        {
            try
            {
                string json = JsonSerializer.Serialize(students, JsonOptions);
                using FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                using StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write(json);
                return true;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show("Không thể ghi file:\r\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void ShowStudent(int index)
        {
            if (index < 0 || index >= loadedStudents.Count)
            {
                currentIndex = 0;
                lblStuName.Text = "Chưa có dữ liệu";
                lblStuMeta.Text = "Bấm ĐỌC FILE để nạp input4.txt";
                lblStu1Val.Text = EmptyValue;
                lblStu2Val.Text = EmptyValue;
                lblStu3Val.Text = EmptyValue;
                lblAvgValue.Text = EmptyValue;
                lblRank.Visible = false;
                lblPage.Text = "Trang 0 / 0";
                btnBack.Enabled = false;
                btnNext.Enabled = false;
                return;
            }

            currentIndex = index;
            SinhVien student = loadedStudents[index];
            lblStuName.Text = student.HoTen;
            lblStuMeta.Text = "MSSV " + student.MSSV + "   ·   " + student.DienThoai;
            lblStu1Val.Text = FormatScore(student.Diem1);
            lblStu2Val.Text = FormatScore(student.Diem2);
            lblStu3Val.Text = FormatScore(student.Diem3);
            lblAvgValue.Text = student.DiemTrungBinh.ToString("0.00");
            lblRank.Text = student.XepLoai ?? Classify(student.DiemTrungBinh);
            lblRank.BackColor = RankColor(student.DiemTrungBinh);
            lblRank.Visible = true;
            lblPage.Text = "Trang " + (index + 1) + " / " + loadedStudents.Count;
            btnBack.Enabled = index > 0;
            btnNext.Enabled = index < loadedStudents.Count - 1;
        }

        private void UpdateStats()
        {
            if (loadedStudents.Count == 0)
            {
                lblCountValue.Text = EmptyValue;
                lblMeanValue.Text = EmptyValue;
                lblMaxValue.Text = EmptyValue;
                lblMinValue.Text = EmptyValue;
                toolTip.SetToolTip(lblMaxValue, string.Empty);
                toolTip.SetToolTip(lblMinValue, string.Empty);
                return;
            }

            float sum = 0f;
            SinhVien best = loadedStudents[0];
            SinhVien worst = loadedStudents[0];
            foreach (SinhVien student in loadedStudents)
            {
                sum += student.DiemTrungBinh;
                if (student.DiemTrungBinh > best.DiemTrungBinh)
                {
                    best = student;
                }
                if (student.DiemTrungBinh < worst.DiemTrungBinh)
                {
                    worst = student;
                }
            }

            lblCountValue.Text = loadedStudents.Count.ToString();
            lblMeanValue.Text = (sum / loadedStudents.Count).ToString("0.00");
            lblMaxValue.Text = best.DiemTrungBinh.ToString("0.00");
            lblMinValue.Text = worst.DiemTrungBinh.ToString("0.00");
            toolTip.SetToolTip(lblMaxValue, best.HoTen);
            toolTip.SetToolTip(lblMinValue, worst.HoTen);
        }

        private void UpdatePreview()
        {
            string? error1 = CheckScore(txtScore1.Text.Trim(), out float score1);
            string? error2 = CheckScore(txtScore2.Text.Trim(), out float score2);
            string? error3 = CheckScore(txtScore3.Text.Trim(), out float score3);

            if (error1 != null || error2 != null || error3 != null)
            {
                lblPreviewValue.Text = EmptyValue;
                lblPreviewRank.Text = "Nhập đủ 3 điểm";
                lblPreviewRank.ForeColor = Muted;
                return;
            }

            float average = MathF.Round((score1 + score2 + score3) / 3f, 2);
            lblPreviewValue.Text = average.ToString("0.00");
            lblPreviewRank.Text = Classify(average);
            lblPreviewRank.ForeColor = RankColor(average);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            bool hasData = pendingStudents.Count > 0 || loadedStudents.Count > 0;
            if (hasData && MessageBox.Show("Xóa danh sách chờ và dữ liệu đã đọc để bắt đầu lại?", "Làm mới", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            pendingStudents.Clear();
            dgvPending.Rows.Clear();
            loadedStudents = new List<SinhVien>();
            currentIndex = 0;
            txtSearch.Clear();
            ClearInputs();
            ShowStudent(-1);
            UpdateStats();
            UpdatePreview();
            UpdatePendingCount();
            lblStatus.Text = "Đã làm mới. Sẵn sàng nhập thông tin sinh viên.";
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void UpdatePendingCount()
        {
            lblPendingCount.Text = pendingStudents.Count + " sinh viên";
        }

        private void ClearInputs()
        {
            foreach (TextBox box in new[] { txtName, txtId, txtPhone, txtScore1, txtScore2, txtScore3 })
            {
                box.Clear();
                SetFieldError(box, null);
            }
            txtName.Focus();
        }

        private void SetFieldError(TextBox box, string? error)
        {
            errorProvider.SetError(box, error ?? string.Empty);
            box.BackColor = error == null ? FieldNormal : FieldInvalid;
        }

        private static void CollectError(List<string> errors, ref TextBox? firstInvalid, TextBox box, string? error)
        {
            if (error == null)
            {
                return;
            }
            errors.Add(error);
            firstInvalid ??= box;
        }

        private string? GetFieldError(TextBox box, string text)
        {
            if (box == txtName)
            {
                return CheckName(text);
            }
            if (box == txtId)
            {
                return CheckId(text);
            }
            if (box == txtPhone)
            {
                return CheckPhone(text);
            }
            return CheckScore(text, out _);
        }

        private static string? CheckName(string text)
        {
            if (text.Length == 0)
            {
                return "Vui lòng nhập họ và tên";
            }
            if (text.Length > 60)
            {
                return "Họ và tên tối đa 60 ký tự";
            }
            foreach (char c in text)
            {
                if (char.IsDigit(c))
                {
                    return "Họ và tên không được chứa chữ số";
                }
            }
            return null;
        }

        private static string? CheckId(string text)
        {
            return Regex.IsMatch(text, "^[1-9][0-9]{7}$") ? null : "MSSV gồm đúng 8 chữ số";
        }

        private static string? CheckPhone(string text)
        {
            return Regex.IsMatch(text, "^0[0-9]{9}$") ? null : "Điện thoại gồm 10 chữ số, bắt đầu bằng 0";
        }

        private static string? CheckScore(string text, out float score)
        {
            string normalized = text.Replace(',', '.');
            bool parsed = float.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out score);
            if (!parsed)
            {
                return "Điểm phải là một số";
            }
            if (score < 0f || score > 10f)
            {
                return "Điểm phải từ 0 đến 10";
            }
            return null;
        }

        private static string FormatScore(float score)
        {
            return score.ToString("0.##");
        }

        private static string Classify(float average)
        {
            if (average >= 9f)
            {
                return "Xuất sắc";
            }
            if (average >= 8f)
            {
                return "Giỏi";
            }
            if (average >= 6.5f)
            {
                return "Khá";
            }
            if (average >= 5f)
            {
                return "Trung bình";
            }
            return "Yếu";
        }

        private static Color RankColor(float average)
        {
            if (average >= 9f)
            {
                return Color.FromArgb(166, 112, 12);
            }
            if (average >= 8f)
            {
                return Color.FromArgb(28, 112, 80);
            }
            if (average >= 6.5f)
            {
                return Color.FromArgb(40, 106, 160);
            }
            if (average >= 5f)
            {
                return Color.FromArgb(118, 112, 40);
            }
            return Color.FromArgb(176, 52, 58);
        }

        private void ConfigureGrid()
        {
            Color headerColor = Color.FromArgb(232, 242, 236);

            dgvPending.EnableHeadersVisualStyles = false;
            dgvPending.BackgroundColor = Color.White;
            dgvPending.BorderStyle = BorderStyle.None;
            dgvPending.GridColor = LineColor;
            dgvPending.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvPending.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvPending.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvPending.ColumnHeadersHeight = 34;
            dgvPending.RowHeadersVisible = false;
            dgvPending.AllowUserToAddRows = false;
            dgvPending.AllowUserToDeleteRows = false;
            dgvPending.AllowUserToResizeRows = false;
            dgvPending.AllowUserToResizeColumns = false;
            dgvPending.ReadOnly = true;
            dgvPending.MultiSelect = false;
            dgvPending.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPending.RowTemplate.Height = 34;

            dgvPending.ColumnHeadersDefaultCellStyle.BackColor = headerColor;
            dgvPending.ColumnHeadersDefaultCellStyle.ForeColor = Forest;
            dgvPending.ColumnHeadersDefaultCellStyle.SelectionBackColor = headerColor;
            dgvPending.ColumnHeadersDefaultCellStyle.SelectionForeColor = Forest;
            dgvPending.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);

            dgvPending.DefaultCellStyle.BackColor = Color.White;
            dgvPending.DefaultCellStyle.ForeColor = Ink;
            dgvPending.DefaultCellStyle.SelectionBackColor = Color.FromArgb(214, 236, 224);
            dgvPending.DefaultCellStyle.SelectionForeColor = Ink;
            dgvPending.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
        }

        private static void StyleButton(Button button, Color back, Color fore, Color hover, bool outlined)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.Cursor = Cursors.Hand;
            button.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            button.UseVisualStyleBackColor = false;
            button.BackColor = back;
            button.ForeColor = fore;
            button.FlatAppearance.MouseOverBackColor = hover;
            button.FlatAppearance.BorderSize = outlined ? 1 : 0;
            if (outlined)
            {
                button.FlatAppearance.BorderColor = Color.FromArgb(168, 196, 180);
            }
        }

        private void PaintForestHeader(object? sender, PaintEventArgs e)
        {
            Rectangle area = pnlHeader.ClientRectangle;
            if (area.Width <= 0 || area.Height <= 0)
            {
                return;
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (LinearGradientBrush canopy = new LinearGradientBrush(area, ForestDeep, Color.FromArgb(30, 92, 70), LinearGradientMode.Horizontal))
            {
                e.Graphics.FillRectangle(canopy, area);
            }

            float baseX = area.Width - 46;
            float baseY = area.Height + 4;
            DrawLeaf(e.Graphics, baseX, baseY, 170, -168, Color.FromArgb(40, 140, 214, 176));
            DrawLeaf(e.Graphics, baseX, baseY, 140, -142, Color.FromArgb(54, 140, 214, 176));
            DrawLeaf(e.Graphics, baseX, baseY, 112, -116, Color.FromArgb(70, 170, 230, 196));
            DrawLeaf(e.Graphics, baseX, baseY, 90, -88, Color.FromArgb(54, 140, 214, 176));
            DrawLeaf(e.Graphics, baseX, baseY, 70, -62, Color.FromArgb(40, 140, 214, 176));
        }

        private static void DrawLeaf(Graphics graphics, float x, float y, float length, float angle, Color color)
        {
            float half = length * 0.24f;
            using GraphicsPath leaf = new GraphicsPath();
            leaf.AddBezier(0, 0, length * 0.3f, -half, length * 0.7f, -half, length, 0);
            leaf.AddBezier(length, 0, length * 0.7f, half, length * 0.3f, half, 0, 0);
            leaf.CloseFigure();

            GraphicsState state = graphics.Save();
            graphics.TranslateTransform(x, y);
            graphics.RotateTransform(angle);
            using SolidBrush fill = new SolidBrush(color);
            graphics.FillPath(fill, leaf);
            using Pen vein = new Pen(Color.FromArgb(80, 210, 245, 226), 1F);
            graphics.DrawLine(vein, 0, 0, length * 0.92f, 0);
            graphics.Restore(state);
        }
    }
}
