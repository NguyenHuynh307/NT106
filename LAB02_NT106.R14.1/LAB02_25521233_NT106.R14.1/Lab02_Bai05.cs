#nullable disable

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab02
{
    internal sealed class Bai05Movie
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public List<int> Rooms { get; set; } = new List<int>();
    }

    internal sealed class Bai05Stat
    {
        public string Name { get; set; } = string.Empty;
        public int Sold { get; set; }
        public int Capacity { get; set; }
        public decimal Revenue { get; set; }
        public int Rank { get; set; }
        public int Remaining { get { return Capacity - Sold; } }
        public double SoldRate { get { return Capacity == 0 ? 0 : (double)Sold / Capacity; } }
    }

    internal sealed class Bai05Showtime
    {
        public Bai05Showtime(string time, decimal rate, string promo, string notice)
        {
            Time = time;
            Rate = rate;
            Promo = promo;
            Notice = notice;
        }

        public string Time { get; }
        public decimal Rate { get; }
        public string Promo { get; }
        public string Notice { get; }
        public string Display { get { return Time + "  ·  " + Promo; } }
    }

    internal sealed class StarField
    {
        public float[] X;
        public float[] Y;
        public float[] Size;
        public int[] Alpha;
        public float[] Speed;
        public float[] Phase;
    }

    internal sealed class StarProgressBar : ProgressBar
    {
        public StarProgressBar()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle track = new Rectangle(0, 0, Width - 1, Height - 1);
            using (SolidBrush trackBrush = new SolidBrush(Color.FromArgb(8, 10, 30)))
            {
                graphics.FillRectangle(trackBrush, track);
            }

            double ratio = Maximum > Minimum ? (double)(Value - Minimum) / (Maximum - Minimum) : 0;
            int fillWidth = (int)((Width - 4) * ratio);
            if (fillWidth > 0 && Width > 6 && Height > 6)
            {
                Rectangle gradientArea = new Rectangle(2, 2, Width - 4, Height - 5);
                using (LinearGradientBrush fillBrush = new LinearGradientBrush(gradientArea, Color.FromArgb(86, 222, 255), Color.FromArgb(255, 86, 170), LinearGradientMode.Horizontal))
                {
                    graphics.FillRectangle(fillBrush, 2, 2, fillWidth, Height - 5);
                }
            }

            using (Pen borderPen = new Pen(Color.FromArgb(70, 78, 160)))
            {
                graphics.DrawRectangle(borderPen, track);
            }

            string percentText = ((int)(ratio * 100)).ToString() + "%";
            TextRenderer.DrawText(graphics, percentText, Font, ClientRectangle, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }
    }

    public partial class Lab02_Bai05 : Form
    {
        private const string DefaultInputName = "input5.txt";
        private const string DefaultOutputName = "output5.txt";
        private const int MaxTicketsPerOrder = 2;
        private const int SeatsPerRoom = 15;
        private const int ExportStepDelay = 160;

        private static readonly CultureInfo ViCulture = new CultureInfo("vi-VN");
        private static readonly CultureInfo FileCulture = CultureInfo.InvariantCulture;

        private static readonly Color SpaceDeep = Color.FromArgb(5, 6, 20);
        private static readonly Color PanelBorder = Color.FromArgb(52, 58, 120);
        private static readonly Color FieldBack = Color.FromArgb(8, 10, 30);
        private static readonly Color TextMain = Color.FromArgb(226, 232, 255);
        private static readonly Color Cyan = Color.FromArgb(86, 222, 255);
        private static readonly Color Magenta = Color.FromArgb(255, 86, 170);
        private static readonly Color Violet = Color.FromArgb(122, 92, 240);
        private static readonly Color Gold = Color.FromArgb(255, 206, 100);
        private static readonly Color Silver = Color.FromArgb(205, 214, 235);
        private static readonly Color Bronze = Color.FromArgb(222, 150, 96);

        private static readonly Color SeatNormal = Color.FromArgb(176, 190, 255);
        private static readonly Color SeatVip = Color.FromArgb(255, 200, 92);
        private static readonly Color SeatEdge = Color.FromArgb(88, 80, 156);
        private static readonly Color SeatPicked = Color.FromArgb(255, 76, 160);
        private static readonly Color SeatSold = Color.FromArgb(22, 24, 52);

        private static readonly Bai05Showtime[] Showtimes =
        {
            new Bai05Showtime("09:15", 0.80m, "Early Bird (-20%)", "Suất sáng sớm  ·  giảm 20% giá vé"),
            new Bai05Showtime("11:30", 0.80m, "Early Bird (-20%)", "Suất trưa  ·  giảm 20% giá vé"),
            new Bai05Showtime("14:00", 1.00m, "Standard (giá gốc)", "Suất chiều  ·  áp dụng nguyên giá vé"),
            new Bai05Showtime("16:30", 1.00m, "Standard (giá gốc)", "Suất chiều  ·  áp dụng nguyên giá vé"),
            new Bai05Showtime("18:45", 1.15m, "Prime Time (+15%)", "Giờ vàng  ·  phụ thu 15% giá vé"),
            new Bai05Showtime("20:15", 1.15m, "Prime Time (+15%)", "Giờ vàng  ·  phụ thu 15% giá vé"),
            new Bai05Showtime("21:45", 0.85m, "Night Owl (-15%)", "Suất khuya  ·  giảm 15% giá vé"),
            new Bai05Showtime("23:15", 0.85m, "Night Owl (-15%)", "Suất khuya  ·  giảm 15% giá vé")
        };

        private static readonly Bai05Showtime NoShowtime = new Bai05Showtime("Không chọn", 1.00m, "giá vé chuẩn", "Chưa chọn suất chiếu  ·  áp dụng nguyên giá vé");

        private readonly List<Bai05Movie> movies = new List<Bai05Movie>();
        private readonly HashSet<string> soldSeats = new HashSet<string>();
        private readonly HashSet<string> selectedSeats = new HashSet<string>();
        private readonly Dictionary<string, int> soldByMovie = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, decimal> revenueByMovie = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Button> seatButtons = new Dictionary<string, Button>();
        private readonly Dictionary<Control, StarField> starFields = new Dictionary<Control, StarField>();
        private readonly ToolTip seatTip = new ToolTip();
        private readonly Random starRandom = new Random(2026);

        private string inputFolder = string.Empty;
        private bool hasUnsavedMovies;
        private bool isLoading;
        private bool isExporting;

        public Lab02_Bai05()
        {
            InitializeComponent();
            ApplyTheme();
            BuildSeatMatrix();
            BuildLegend();
            FillShowtimes();
            HookDragDrop();
            RefreshAll();
            SetStatus("Sẵn sàng. Đọc input5.txt hoặc thêm phim ở bước 1.");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Rectangle area = Screen.FromControl(this).WorkingArea;
            int width = Math.Min(Width, area.Width);
            int height = Math.Min(Height, area.Height);
            if (width != Width || height != Height)
            {
                Size = new Size(width, height);
            }
            Location = new Point(area.Left + (area.Width - Width) / 2, area.Top + (area.Height - Height) / 2);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (isExporting)
            {
                e.Cancel = true;
                return;
            }

            if (hasUnsavedMovies && movies.Count > 0)
            {
                DialogResult answer = MessageBox.Show("Danh sách phim chưa được ghi vào input5.txt. Vẫn thoát?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (answer != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }

            base.OnFormClosing(e);
        }

        private void ApplyTheme()
        {
            foreach (Control control in new Control[] { pnlHeader, pnlLeft, pnlMid, pnlRight, pnlSeatMatrix, dgvMovies, dgvStats })
            {
                EnableDoubleBuffer(control);
            }

            foreach (Panel panel in new[] { pnlLeft, pnlMid, pnlRight })
            {
                panel.Paint += PaintGlassPanel;
            }

            StyleButton(btnReadFile, Cyan, SpaceDeep, Color.FromArgb(150, 238, 255), false);
            StyleButton(btnExport, Color.FromArgb(236, 72, 160), Color.White, Color.FromArgb(255, 110, 186), false);
            StyleButton(btnReset, Color.FromArgb(20, 24, 60), TextMain, Color.FromArgb(34, 40, 92), true);
            StyleButton(btnExit, Color.FromArgb(170, 40, 70), Color.White, Color.FromArgb(205, 60, 94), false);
            StyleButton(btnAddMovie, Violet, Color.White, Color.FromArgb(150, 122, 255), false);
            StyleButton(btnSaveFile, Gold, SpaceDeep, Color.FromArgb(255, 224, 150), false);
            StyleButton(btnRemoveMovie, Color.FromArgb(20, 24, 60), Magenta, Color.FromArgb(40, 30, 74), true);
            StyleButton(btnBook, Cyan, SpaceDeep, Color.FromArgb(150, 238, 255), false);

            foreach (TextBox box in new[] { txtMovieName, txtPrice, txtRooms, txtCustomer })
            {
                box.BackColor = FieldBack;
                box.ForeColor = TextMain;
                box.BorderStyle = BorderStyle.FixedSingle;
            }

            foreach (ComboBox combo in new[] { cboMovie, cboRoom, cboShowtime })
            {
                combo.DropDownStyle = ComboBoxStyle.DropDownList;
                combo.FlatStyle = FlatStyle.Flat;
                combo.BackColor = FieldBack;
                combo.ForeColor = TextMain;
            }

            rtbInvoice.BackColor = FieldBack;
            rtbInvoice.ForeColor = Cyan;
            rtbInvoice.BorderStyle = BorderStyle.None;

            StyleGrid(dgvMovies);
            StyleGrid(dgvStats);
            dgvMovies.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvMovies.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            foreach (int index in new[] { 0, 2, 3, 4 })
            {
                dgvStats.Columns[index].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            dgvStats.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            txtRooms.KeyDown += txtRooms_KeyDown;
        }

        private static void EnableDoubleBuffer(Control control)
        {
            PropertyInfo property = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            if (property != null)
            {
                property.SetValue(control, true, null);
            }
        }

        private static void StyleButton(Button button, Color back, Color fore, Color hover, bool outlined)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.Cursor = Cursors.Hand;
            button.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button.UseVisualStyleBackColor = false;
            button.BackColor = back;
            button.ForeColor = fore;
            button.FlatAppearance.BorderSize = outlined ? 1 : 0;
            button.FlatAppearance.BorderColor = outlined ? Color.FromArgb(90, 80, 190) : back;
            button.FlatAppearance.MouseOverBackColor = hover;
        }

        private static void StyleGrid(DataGridView grid)
        {
            grid.BackgroundColor = Color.FromArgb(12, 14, 36);
            grid.BorderStyle = BorderStyle.None;
            grid.GridColor = Color.FromArgb(34, 40, 86);
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.EnableHeadersVisualStyles = false;
            grid.RowHeadersVisible = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.ReadOnly = true;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = grid.LogicalToDeviceUnits(32);
            grid.RowTemplate.Height = grid.LogicalToDeviceUnits(28);
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(28, 24, 70);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(190, 170, 255);
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(28, 24, 70);
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.DefaultCellStyle.BackColor = Color.FromArgb(12, 14, 36);
            grid.DefaultCellStyle.ForeColor = Color.FromArgb(220, 226, 250);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(74, 42, 140);
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(17, 19, 46);
        }

        private StarField GetStarField(Control control, int areaPerStar)
        {
            StarField field;
            if (starFields.TryGetValue(control, out field))
            {
                return field;
            }

            int count = Math.Max(10, control.Width * control.Height / areaPerStar);
            field = new StarField
            {
                X = new float[count],
                Y = new float[count],
                Size = new float[count],
                Alpha = new int[count],
                Speed = new float[count],
                Phase = new float[count]
            };

            for (int i = 0; i < count; i++)
            {
                field.X[i] = (float)starRandom.NextDouble();
                field.Y[i] = (float)starRandom.NextDouble();
                field.Size[i] = starRandom.Next(10) < 8 ? 1.6f : 2.8f;
                field.Alpha[i] = starRandom.Next(60, 230);
                field.Speed[i] = 0.008f + (float)starRandom.NextDouble() * 0.03f;
                field.Phase[i] = (float)(starRandom.NextDouble() * Math.PI * 2);
            }

            starFields[control] = field;
            return field;
        }

        private void DrawStars(Graphics graphics, Control control, int areaPerStar, int alphaLimit, float time = 0f)
        {
            StarField field = GetStarField(control, areaPerStar);
            Color[] tints = { Color.White, Color.FromArgb(170, 230, 255), Color.FromArgb(214, 190, 255) };
            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                for (int i = 0; i < field.X.Length; i++)
                {
                    float x = field.X[i];
                    float twinkle = 1f;
                    if (time > 0f)
                    {
                        x = (x - field.Speed[i] * time) % 1f;
                        if (x < 0f)
                        {
                            x += 1f;
                        }
                        twinkle = 0.65f + 0.35f * (float)Math.Sin(time * 2.2f + field.Phase[i]);
                    }

                    int alpha = (int)(Math.Min(field.Alpha[i], alphaLimit) * twinkle);
                    brush.Color = Color.FromArgb(Math.Max(0, Math.Min(255, alpha)), tints[i % tints.Length]);
                    graphics.FillEllipse(brush, x * control.Width, field.Y[i] * control.Height, field.Size[i], field.Size[i]);
                }
            }
        }

        private void PaintGlassPanel(object sender, PaintEventArgs e)
        {
            Control panel = (Control)sender;
            if (panel.Width <= 2 || panel.Height <= 2)
            {
                return;
            }

            DrawStars(e.Graphics, panel, 9000, 120);
            using (Pen border = new Pen(PanelBorder))
            {
                e.Graphics.DrawRectangle(border, 0, 0, panel.Width - 1, panel.Height - 1);
            }

            using (LinearGradientBrush top = new LinearGradientBrush(new Rectangle(0, 0, panel.Width, 3), Cyan, Magenta, LinearGradientMode.Horizontal))
            {
                e.Graphics.FillRectangle(top, 0, 0, panel.Width, 3);
            }
        }

        private void BuildSeatMatrix()
        {
            string[] rows = { "A", "B", "C" };
            pnlSeatMatrix.Controls.Clear();
            seatButtons.Clear();

            TableLayoutPanel grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                ColumnCount = 6,
                RowCount = rows.Length,
                Margin = Padding.Empty
            };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8f));
            for (int c = 0; c < 5; c++)
            {
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18.4f));
            }
            for (int r = 0; r < rows.Length; r++)
            {
                grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / rows.Length));
            }

            int gap = LogicalToDeviceUnits(4);
            for (int r = 0; r < rows.Length; r++)
            {
                Label rowLabel = new Label
                {
                    Text = rows[r],
                    Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                    ForeColor = Cyan,
                    BackColor = Color.Transparent,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                grid.Controls.Add(rowLabel, 0, r);

                for (int c = 1; c <= 5; c++)
                {
                    string code = rows[r] + c.ToString();
                    Button seat = new Button
                    {
                        Name = "btnSeat_" + code,
                        Tag = code,
                        Text = c.ToString(),
                        Dock = DockStyle.Fill,
                        Margin = new Padding(gap),
                        Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand,
                        UseVisualStyleBackColor = false
                    };
                    seat.FlatAppearance.BorderSize = 1;
                    seat.Click += SeatButton_Click;
                    grid.Controls.Add(seat, c, r);
                    seatButtons.Add(code, seat);
                }
            }

            pnlSeatMatrix.Controls.Add(grid);
        }

        private void BuildLegend()
        {
            string[] captions = { "● Thường", "● VIP x2", "● Rìa x0.25", "● Đang chọn", "● Đã bán" };
            Color[] colors = { SeatNormal, SeatVip, Color.FromArgb(150, 140, 220), SeatPicked, Color.FromArgb(110, 116, 160) };
            flowLegend.Controls.Clear();

            for (int i = 0; i < captions.Length; i++)
            {
                Label item = new Label
                {
                    Text = captions[i],
                    Font = new Font("Segoe UI", 8F),
                    ForeColor = colors[i],
                    BackColor = Color.Transparent,
                    AutoSize = true,
                    Margin = new Padding(0, 0, LogicalToDeviceUnits(12), LogicalToDeviceUnits(2))
                };
                flowLegend.Controls.Add(item);
            }
        }

        private void FillShowtimes()
        {
            isLoading = true;
            cboShowtime.Items.Clear();
            cboShowtime.Items.Add(NoShowtime.Display);
            foreach (Bai05Showtime showtime in Showtimes)
            {
                cboShowtime.Items.Add(showtime.Display);
            }
            cboShowtime.SelectedIndex = 0;
            isLoading = false;
        }

        private void HookDragDrop()
        {
            foreach (Control control in new Control[] { this, pnlHeader, pnlLeft, pnlMid, pnlRight })
            {
                control.AllowDrop = true;
                control.DragEnter += OnFileDragEnter;
                control.DragDrop += OnFileDragDrop;
            }
        }

        private void OnFileDragEnter(object sender, DragEventArgs e)
        {
            bool isFile = e.Data.GetDataPresent(DataFormats.FileDrop);
            e.Effect = isFile ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void OnFileDragDrop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files == null || files.Length == 0)
            {
                return;
            }

            if (!string.Equals(Path.GetExtension(files[0]), ".txt", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Chỉ nhận file .txt!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LoadMoviesFromFile(files[0]);
        }

        private void txtRooms_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnAddMovie_Click(sender, EventArgs.Empty);
            }
        }

        private void btnReadFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Chọn file input5.txt";
                dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                dialog.CheckFileExists = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    LoadMoviesFromFile(dialog.FileName);
                }
            }
        }

        private void LoadMoviesFromFile(string path)
        {
            List<string> lines = new List<string>();
            List<int> lineNumbers = new List<int>();

            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string line;
                    int number = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        number++;
                        line = line.Trim();
                        if (line.Length > 0)
                        {
                            lines.Add(line);
                            lineNumbers.Add(number);
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show("Không thể đọc file:\r\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show("Không thể đọc file:\r\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (lines.Count == 0 || lines.Count % 3 != 0)
            {
                MessageBox.Show("File phải gồm từng nhóm 3 dòng: tên phim, giá vé chuẩn, phòng chiếu.\r\nSố dòng có nội dung hiện tại: " + lines.Count, "File không đúng cấu trúc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<Bai05Movie> parsed = new List<Bai05Movie>();
            for (int i = 0; i < lines.Count; i += 3)
            {
                decimal price;
                List<int> rooms;

                if (!TryParsePrice(lines[i + 1], out price))
                {
                    MessageBox.Show("Giá vé chuẩn không hợp lệ ở dòng " + lineNumbers[i + 1] + ":\r\n" + lines[i + 1], "File không đúng cấu trúc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!TryParseRooms(lines[i + 2], out rooms))
                {
                    MessageBox.Show("Phòng chiếu không hợp lệ ở dòng " + lineNumbers[i + 2] + ":\r\n" + lines[i + 2], "File không đúng cấu trúc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string name = lines[i];
                Bai05Movie existing = parsed.FirstOrDefault(m => string.Equals(m.Name, name, StringComparison.OrdinalIgnoreCase));
                if (existing == null)
                {
                    parsed.Add(new Bai05Movie { Name = name, Price = price, Rooms = rooms });
                }
                else
                {
                    existing.Rooms = existing.Rooms.Union(rooms).OrderBy(room => room).ToList();
                }
            }

            if (soldSeats.Count > 0)
            {
                DialogResult answer = MessageBox.Show("Nạp file mới sẽ xóa các vé đã bán trong phiên này. Tiếp tục?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (answer != DialogResult.Yes)
                {
                    return;
                }
            }

            movies.Clear();
            movies.AddRange(parsed);
            ResetBookings();
            inputFolder = Path.GetDirectoryName(path);
            hasUnsavedMovies = false;
            RefreshAll();
            SetStatus("Đã đọc " + movies.Count + " phim từ " + path);
        }

        private static bool TryParsePrice(string text, out decimal price)
        {
            price = 0;
            string digits = Regex.Replace(text, "[^0-9]", string.Empty);
            if (digits.Length == 0 || digits.Length > 9)
            {
                return false;
            }

            price = decimal.Parse(digits, FileCulture);
            return price > 0;
        }

        private static bool TryParseRooms(string text, out List<int> rooms)
        {
            rooms = new List<int>();
            foreach (Match match in Regex.Matches(text, "[0-9]+"))
            {
                int room;
                if (match.Value.Length > 2 || !int.TryParse(match.Value, out room) || room < 1)
                {
                    return false;
                }
                if (!rooms.Contains(room))
                {
                    rooms.Add(room);
                }
            }

            rooms.Sort();
            return rooms.Count > 0;
        }

        private void btnAddMovie_Click(object sender, EventArgs e)
        {
            string name = txtMovieName.Text.Trim();
            decimal price;
            List<int> rooms;

            if (name.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập tên phim!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMovieName.Focus();
                return;
            }

            if (movies.Any(m => string.Equals(m.Name, name, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Phim này đã có trong danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMovieName.Focus();
                return;
            }

            if (!TryParsePrice(txtPrice.Text, out price))
            {
                MessageBox.Show("Giá vé chuẩn phải là số dương, ví dụ: 45000", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrice.Focus();
                return;
            }

            if (!TryParseRooms(txtRooms.Text, out rooms))
            {
                MessageBox.Show("Nhập ít nhất một phòng chiếu (số từ 1 đến 99), cách nhau dấu phẩy. Ví dụ: 1, 2, 3", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRooms.Focus();
                return;
            }

            movies.Add(new Bai05Movie { Name = name, Price = price, Rooms = rooms });
            hasUnsavedMovies = true;
            txtMovieName.Clear();
            txtPrice.Clear();
            txtRooms.Clear();
            txtMovieName.Focus();
            RefreshAll();
            SetStatus("Đã thêm phim: " + name + ". Bấm GHI để lưu vào input5.txt.");
        }

        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            if (movies.Count == 0)
            {
                MessageBox.Show("Chưa có phim nào để ghi!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string initialFolder = Directory.Exists(inputFolder) ? inputFolder : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "Ghi file input5.txt";
                dialog.Filter = "Text files (*.txt)|*.txt";
                dialog.FileName = DefaultInputName;
                dialog.InitialDirectory = initialFolder;

                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                if (WriteMovies(dialog.FileName))
                {
                    inputFolder = Path.GetDirectoryName(dialog.FileName);
                    hasUnsavedMovies = false;
                    SetStatus("Đã ghi " + movies.Count + " phim: " + dialog.FileName);
                    MessageBox.Show("Đã ghi " + movies.Count + " phim vào " + Path.GetFileName(dialog.FileName) + "!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private bool WriteMovies(string path)
        {
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                using (StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false)))
                {
                    for (int i = 0; i < movies.Count; i++)
                    {
                        writer.WriteLine(movies[i].Name);
                        writer.WriteLine(movies[i].Price.ToString("0", FileCulture));
                        writer.WriteLine(string.Join(", ", movies[i].Rooms));
                        if (i < movies.Count - 1)
                        {
                            writer.WriteLine();
                        }
                    }
                }
                return true;
            }
            catch (IOException ex)
            {
                MessageBox.Show("Không thể ghi file:\r\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show("Không thể ghi file:\r\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void btnRemoveMovie_Click(object sender, EventArgs e)
        {
            if (dgvMovies.SelectedRows.Count == 0)
            {
                SetStatus("Chọn một phim trong danh sách để xóa.");
                return;
            }

            Bai05Movie movie = movies[dgvMovies.SelectedRows[0].Index];
            int sold;
            soldByMovie.TryGetValue(movie.Name, out sold);

            if (sold > 0)
            {
                DialogResult answer = MessageBox.Show("Phim \"" + movie.Name + "\" đã bán " + sold + " vé. Xóa phim sẽ mất thống kê của phim này. Tiếp tục?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (answer != DialogResult.Yes)
                {
                    return;
                }
            }

            string prefix = movie.Name + "|";
            soldSeats.RemoveWhere(key => key.StartsWith(prefix, StringComparison.Ordinal));
            soldByMovie.Remove(movie.Name);
            revenueByMovie.Remove(movie.Name);
            movies.Remove(movie);
            hasUnsavedMovies = true;
            selectedSeats.Clear();
            RefreshAll();
            SetStatus("Đã xóa phim: " + movie.Name);
        }

        private void RefreshAll()
        {
            RefreshMovieGrid();
            RefreshMovieCombo();
            RefreshStats();
        }

        private void RefreshMovieGrid()
        {
            dgvMovies.Rows.Clear();
            foreach (Bai05Movie movie in movies)
            {
                dgvMovies.Rows.Add(movie.Name, FormatMoney(movie.Price, ViCulture), string.Join(", ", movie.Rooms));
            }
            dgvMovies.ClearSelection();
            lblMovieCount.Text = movies.Count + " phim";
        }

        private void RefreshMovieCombo()
        {
            string previous = cboMovie.SelectedItem as string;
            isLoading = true;
            cboMovie.Items.Clear();
            foreach (Bai05Movie movie in movies)
            {
                cboMovie.Items.Add(movie.Name);
            }

            int index = previous == null ? -1 : cboMovie.Items.IndexOf(previous);
            if (index >= 0)
            {
                cboMovie.SelectedIndex = index;
            }
            else if (cboMovie.Items.Count > 0)
            {
                cboMovie.SelectedIndex = 0;
            }
            isLoading = false;

            LoadRooms();
        }

        private void LoadRooms()
        {
            string previous = cboRoom.SelectedItem as string;
            isLoading = true;
            cboRoom.Items.Clear();

            Bai05Movie movie = CurrentMovie();
            if (movie != null)
            {
                foreach (int room in movie.Rooms)
                {
                    cboRoom.Items.Add("Phòng " + room);
                }
            }

            int index = previous == null ? -1 : cboRoom.Items.IndexOf(previous);
            if (index >= 0)
            {
                cboRoom.SelectedIndex = index;
            }
            else if (cboRoom.Items.Count > 0)
            {
                cboRoom.SelectedIndex = 0;
            }
            isLoading = false;

            selectedSeats.Clear();
            UpdateBookingView();
        }

        private Bai05Movie CurrentMovie()
        {
            string name = cboMovie.SelectedItem as string;
            return name == null ? null : movies.FirstOrDefault(m => m.Name == name);
        }

        private Bai05Showtime CurrentShowtime()
        {
            int index = cboShowtime.SelectedIndex;
            return index <= 0 ? NoShowtime : Showtimes[index - 1];
        }

        private void cboMovie_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoading)
            {
                LoadRooms();
            }
        }

        private void cboRoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoading)
            {
                selectedSeats.Clear();
                UpdateBookingView();
            }
        }

        private void cboShowtime_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoading)
            {
                selectedSeats.Clear();
                UpdateBookingView();
            }
        }

        private void UpdateBookingView()
        {
            Bai05Movie movie = CurrentMovie();
            Bai05Showtime showtime = CurrentShowtime();

            if (movie == null)
            {
                lblPriceTag.Text = "Chưa có phim để đặt vé.";
            }
            else
            {
                lblPriceTag.Text = "Giá vé: " + FormatMoney(movie.Price * showtime.Rate, ViCulture) + " đ  (ghế thường)";
            }

            lblNotice.Text = showtime.Notice;
            btnBook.Enabled = movie != null && cboRoom.SelectedItem != null;
            RefreshSeats();
        }

        private static decimal SeatMultiplier(string code)
        {
            if (code == "A1" || code == "A5" || code == "C1" || code == "C5")
            {
                return 0.25m;
            }
            if (code == "B2" || code == "B3" || code == "B4")
            {
                return 2m;
            }
            return 1m;
        }

        private string SeatKey(Bai05Movie movie, string room, Bai05Showtime showtime, string code)
        {
            return movie.Name + "|" + room + "|" + showtime.Time + "|" + code;
        }

        private void RefreshSeats()
        {
            Bai05Movie movie = CurrentMovie();
            Bai05Showtime showtime = CurrentShowtime();
            string room = cboRoom.SelectedItem as string;
            bool active = movie != null && room != null;

            foreach (KeyValuePair<string, Button> pair in seatButtons)
            {
                string code = pair.Key;
                Button seat = pair.Value;
                decimal multiplier = SeatMultiplier(code);
                bool isSold = active && soldSeats.Contains(SeatKey(movie, room, showtime, code));

                seat.Text = isSold ? "×" : code.Substring(1);

                if (!active || isSold)
                {
                    seat.BackColor = SeatSold;
                    seat.ForeColor = Color.FromArgb(90, 96, 140);
                    seat.FlatAppearance.BorderColor = Color.FromArgb(40, 44, 90);
                    seat.Enabled = false;
                    seatTip.SetToolTip(seat, isSold ? "Ghế " + code + " đã bán" : string.Empty);
                    continue;
                }

                seat.Enabled = true;
                if (selectedSeats.Contains(code))
                {
                    seat.BackColor = SeatPicked;
                    seat.ForeColor = Color.White;
                    seat.FlatAppearance.BorderColor = Color.FromArgb(255, 150, 205);
                }
                else if (multiplier > 1m)
                {
                    seat.BackColor = SeatVip;
                    seat.ForeColor = Color.FromArgb(40, 24, 0);
                    seat.FlatAppearance.BorderColor = Color.FromArgb(255, 226, 150);
                }
                else if (multiplier < 1m)
                {
                    seat.BackColor = SeatEdge;
                    seat.ForeColor = Color.FromArgb(235, 232, 255);
                    seat.FlatAppearance.BorderColor = Color.FromArgb(130, 120, 210);
                }
                else
                {
                    seat.BackColor = SeatNormal;
                    seat.ForeColor = Color.FromArgb(10, 12, 40);
                    seat.FlatAppearance.BorderColor = Color.FromArgb(220, 228, 255);
                }

                decimal seatPrice = movie.Price * showtime.Rate * multiplier;
                seatTip.SetToolTip(seat, "Ghế " + code + "  ·  x" + multiplier.ToString("0.##", ViCulture) + "  ·  " + FormatMoney(seatPrice, ViCulture) + " đ");
            }
        }

        private void SeatButton_Click(object sender, EventArgs e)
        {
            Button seat = (Button)sender;
            string code = (string)seat.Tag;

            if (selectedSeats.Contains(code))
            {
                selectedSeats.Remove(code);
            }
            else
            {
                if (selectedSeats.Count >= MaxTicketsPerOrder)
                {
                    MessageBox.Show("Không thể chọn nhiều hơn " + MaxTicketsPerOrder + " vé trong một lần đặt!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                selectedSeats.Add(code);
            }

            RefreshSeats();
        }

        private void btnBook_Click(object sender, EventArgs e)
        {
            string customer = txtCustomer.Text.Trim();
            Bai05Movie movie = CurrentMovie();
            string room = cboRoom.SelectedItem as string;

            if (customer.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập họ và tên khách hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCustomer.Focus();
                return;
            }

            if (movie == null || room == null)
            {
                MessageBox.Show("Vui lòng chọn phim và phòng chiếu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (selectedSeats.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ghế trên sơ đồ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Bai05Showtime showtime = CurrentShowtime();
            List<string> seats = selectedSeats.ToList();
            seats.Sort(StringComparer.Ordinal);

            decimal total = 0;
            foreach (string code in seats)
            {
                soldSeats.Add(SeatKey(movie, room, showtime, code));
                total += movie.Price * showtime.Rate * SeatMultiplier(code);
            }

            int soldBefore;
            soldByMovie.TryGetValue(movie.Name, out soldBefore);
            soldByMovie[movie.Name] = soldBefore + seats.Count;

            decimal revenueBefore;
            revenueByMovie.TryGetValue(movie.Name, out revenueBefore);
            revenueByMovie[movie.Name] = revenueBefore + total;

            string divider = "  ──────────────────────────────\r\n";
            StringBuilder invoice = new StringBuilder();
            invoice.Append("        HÓA ĐƠN ĐẶT VÉ\r\n");
            invoice.Append(divider);
            invoice.Append("  Khách : " + customer + "\r\n");
            invoice.Append("  Phim  : " + movie.Name + "\r\n");
            invoice.Append("  Suất  : " + showtime.Time + "  ·  " + showtime.Promo + "\r\n");
            invoice.Append("  Phòng : " + room + "\r\n");
            invoice.Append("  Ghế   : " + string.Join(", ", seats) + "  (" + seats.Count + " vé)\r\n");
            invoice.Append(divider);
            invoice.Append("  TỔNG  : " + FormatMoney(total, ViCulture) + " VNĐ\r\n");
            invoice.Append(divider);
            invoice.Append("   Cảm ơn quý khách. Chúc xem phim vui vẻ!");
            rtbInvoice.Text = invoice.ToString();

            selectedSeats.Clear();
            RefreshSeats();
            RefreshStats();
            SetStatus("Đã bán " + seats.Count + " vé phim " + movie.Name + " (" + room + ", " + showtime.Time + ").");
            MessageBox.Show("Giao dịch đặt vé thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private List<Bai05Stat> BuildStats()
        {
            List<Bai05Stat> list = new List<Bai05Stat>();
            foreach (Bai05Movie movie in movies)
            {
                int sold;
                decimal revenue;
                soldByMovie.TryGetValue(movie.Name, out sold);
                revenueByMovie.TryGetValue(movie.Name, out revenue);

                list.Add(new Bai05Stat
                {
                    Name = movie.Name,
                    Sold = sold,
                    Capacity = movie.Rooms.Count * (Showtimes.Length + 1) * SeatsPerRoom,
                    Revenue = revenue
                });
            }

            foreach (Bai05Stat stat in list)
            {
                stat.Rank = 1 + list.Count(other => other.Revenue > stat.Revenue);
            }

            list.Sort((left, right) =>
            {
                int byRank = left.Rank.CompareTo(right.Rank);
                return byRank != 0 ? byRank : string.Compare(left.Name, right.Name, StringComparison.CurrentCultureIgnoreCase);
            });

            return list;
        }

        private void RefreshStats()
        {
            List<Bai05Stat> stats = BuildStats();
            dgvStats.Rows.Clear();

            foreach (Bai05Stat stat in stats)
            {
                int rowIndex = dgvStats.Rows.Add(stat.Rank, stat.Name, stat.Sold, stat.Remaining, FormatRate(stat.SoldRate, ViCulture), FormatMoney(stat.Revenue, ViCulture));
                Color rankColor = stat.Rank == 1 ? Gold : stat.Rank == 2 ? Silver : stat.Rank == 3 ? Bronze : TextMain;
                DataGridViewCell rankCell = dgvStats.Rows[rowIndex].Cells[0];
                rankCell.Style.ForeColor = rankColor;
                rankCell.Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            }
            dgvStats.ClearSelection();

            int totalSold = stats.Sum(s => s.Sold);
            decimal totalRevenue = stats.Sum(s => s.Revenue);
            lblTotal.Text = "Tổng: " + totalSold + " vé  ·  " + FormatMoney(totalRevenue, ViCulture) + " đ";
        }

        private static string FormatMoney(decimal value, CultureInfo culture)
        {
            return Math.Round(value, MidpointRounding.AwayFromZero).ToString("#,##0", culture);
        }

        private static string FormatRate(double rate, CultureInfo culture)
        {
            return (rate * 100).ToString("0.0", culture) + "%";
        }

        private string ResolveOutputPath()
        {
            if (!string.IsNullOrEmpty(inputFolder) && Directory.Exists(inputFolder))
            {
                return Path.Combine(inputFolder, DefaultOutputName);
            }

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "Lưu file output5.txt";
                dialog.Filter = "Text files (*.txt)|*.txt";
                dialog.FileName = DefaultOutputName;
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
            }
        }

        private void SetBusy(bool busy)
        {
            foreach (Control control in new Control[] { btnReadFile, btnExport, btnReset, btnBook, btnAddMovie, btnSaveFile, btnRemoveMovie })
            {
                control.Enabled = !busy;
            }
        }

        private async void btnExport_Click(object sender, EventArgs e)
        {
            if (isExporting)
            {
                return;
            }

            if (movies.Count == 0)
            {
                MessageBox.Show("Chưa có phim nào để thống kê!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string outputPath = ResolveOutputPath();
            if (outputPath == null)
            {
                return;
            }

            List<Bai05Stat> stats = BuildStats();
            isExporting = true;
            SetBusy(true);
            prgExport.Value = 0;
            lblProgressCap.Text = "ĐANG XUẤT output5.txt ...";

            try
            {
                using (FileStream stream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                using (StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false)))
                {
                    await writer.WriteLineAsync("THỐNG KÊ PHÒNG VÉ");
                    await writer.WriteLineAsync("Ngày xuất: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm", FileCulture));
                    await writer.WriteLineAsync("==============================");

                    for (int i = 0; i < stats.Count; i++)
                    {
                        Bai05Stat stat = stats[i];
                        await writer.WriteLineAsync("Tên phim: " + stat.Name);
                        await writer.WriteLineAsync("Số lượng vé bán ra: " + stat.Sold);
                        await writer.WriteLineAsync("Số lượng vé tồn: " + stat.Remaining);
                        await writer.WriteLineAsync("Tỉ lệ vé bán ra: " + (stat.SoldRate * 100).ToString("0.00", FileCulture) + "%");
                        await writer.WriteLineAsync("Doanh thu: " + FormatMoney(stat.Revenue, FileCulture) + " VNĐ");
                        await writer.WriteLineAsync("Xếp hạng doanh thu: " + stat.Rank);
                        await writer.WriteLineAsync("------------------------------");

                        prgExport.Value = (i + 1) * 100 / stats.Count;
                        SetStatus("Đang ghi phim " + (i + 1) + "/" + stats.Count + ": " + stat.Name);
                        await Task.Delay(ExportStepDelay);
                    }

                    await writer.WriteLineAsync("Tổng vé bán ra: " + stats.Sum(s => s.Sold));
                    await writer.WriteLineAsync("Tổng doanh thu: " + FormatMoney(stats.Sum(s => s.Revenue), FileCulture) + " VNĐ");
                }

                prgExport.Value = 100;
                lblProgressCap.Text = "ĐÃ XUẤT XONG output5.txt";
                SetStatus("Đã xuất thống kê: " + outputPath);
                MessageBox.Show("Đã xuất thống kê ra file:\r\n" + outputPath, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (IOException ex)
            {
                lblProgressCap.Text = "XUẤT FILE THẤT BẠI";
                MessageBox.Show("Không thể ghi file:\r\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException ex)
            {
                lblProgressCap.Text = "XUẤT FILE THẤT BẠI";
                MessageBox.Show("Không thể ghi file:\r\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isExporting = false;
                SetBusy(false);
                btnBook.Enabled = CurrentMovie() != null && cboRoom.SelectedItem != null;
            }
        }

        private void ResetBookings()
        {
            soldSeats.Clear();
            selectedSeats.Clear();
            soldByMovie.Clear();
            revenueByMovie.Clear();
            rtbInvoice.Clear();
            prgExport.Value = 0;
            lblProgressCap.Text = "TIẾN ĐỘ XUẤT output5.txt";
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (soldSeats.Count > 0)
            {
                DialogResult answer = MessageBox.Show("Làm mới sẽ xóa toàn bộ vé đã bán trong phiên này (danh sách phim được giữ lại). Tiếp tục?", "Làm mới", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (answer != DialogResult.Yes)
                {
                    return;
                }
            }

            ResetBookings();
            txtCustomer.Clear();
            cboShowtime.SelectedIndex = 0;
            RefreshStats();
            UpdateBookingView();
            SetStatus("Đã làm mới. Danh sách phim được giữ nguyên.");
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SetStatus(string text)
        {
            lblStatus.Text = text;
        }
    }
}
