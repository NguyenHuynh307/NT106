#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lab02.Bai06Data;
using Timer = System.Windows.Forms.Timer;

namespace Lab02
{
    public partial class Lab02_Bai06 : Form
    {
        private const string MessageTitle = "Hôm nay ăn gì?";
        private const string Input6FileName = "input6.txt";
        private const int SpinSteps = 18;
        private const int MenuPageSize = 8;

        private static readonly string[] PriceLabels = { "Mọi mức giá", "Dưới 30.000đ", "Dưới 50.000đ", "Dưới 80.000đ", "Dưới 120.000đ", "Dưới 200.000đ", "Dưới 350.000đ", "Dưới 500.000đ", "Dưới 1.000.000đ", "Dưới 2.000.000đ", "Dưới 5.000.000đ", "Dưới 10.000.000đ" };
        private static readonly int[] PriceLimits = { 0, 30000, 50000, 80000, 120000, 200000, 350000, 500000, 1000000, 2000000, 5000000, 10000000 };
        private static readonly string[] RoleChoices = { "Quản trị viên", "Thành viên", "Khách" };

        private readonly FoodDatabase database = new FoodDatabase();
        private readonly Random random = new Random();
        private readonly B6Images dishImages;
        private readonly ImageList smallImages;
        private readonly ImageList largeImages;
        private readonly Dictionary<string, int> thumbIndex = new Dictionary<string, int>();
        private readonly Timer spinTimer = new Timer { Interval = 40 };
        private readonly Timer clockTimer = new Timer { Interval = 15000 };

        private List<Contributor> contributors = new List<Contributor>();
        private List<Dish> allDishes = new List<Dish>();
        private HashSet<int> favoriteIds = new HashSet<int>();
        private readonly List<int> dishOwnerIds = new List<int>();
        private readonly List<int> filterOwnerIds = new List<int>();
        private bool loading;

        private B6Hero hero;
        private Panel bodyHost;
        private B6Fade fade;
        private Panel statusBar;
        private Label statusLabel;
        private Panel basicRoot;
        private Panel chefRoot;
        private bool chefMode;

        private B6Input txtContributorName;
        private B6Input txtDishName;
        private B6Input txtImagePath;
        private B6Input txtSearch;
        private B6Combo cboRole;
        private B6Segments segLeftTabs;
        private Panel pnlContribPage;
        private Panel pnlDishPage;
        private B6Combo cboDishCategory;
        private B6Input txtDishPrice;
        private Label lblContribHint;
        private Label lblContribStats;
        private ContextMenuStrip contributorMenu;
        private B6Combo cboDishOwner;
        private B6Combo cboFilterOwner;
        private B6Segments segListMeal;
        private B6Segments segPickMeal;
        private ListView lvContributors;
        private ListView lvDishes;
        private B6Stage stagePreview;
        private B6Stage stageResult;
        private Label lblPickNote;
        private Label lblResultName;
        private Label lblResultOwner;
        private Label lblResultMeta;
        private Label lblResultDesc;
        private Label lblListStats;
        private B6Button btnPick;
        private B6Button btnToggleView;
        private bool thumbnailMode;
        private List<Dish> spinPool = new List<Dish>();
        private Dish spinResult;
        private int spinStep;

        public Lab02_Bai06()
        {
            InitializeComponent();

            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }

            dishImages = new B6Images(database);
            smallImages = new ImageList { ColorDepth = ColorDepth.Depth32Bit, ImageSize = new Size(U(48), U(36)) };
            largeImages = new ImageList { ColorDepth = ColorDepth.Depth32Bit, ImageSize = new Size(U(112), U(84)) };

            BuildShell();
            BuildBasicMode();
            BuildChefMode();

            spinTimer.Tick += SpinTimer_Tick;
            clockTimer.Tick += ClockTimer_Tick;
            clockTimer.Start();

            ApplyMode(false);
        }

        private int U(int value)
        {
            return LogicalToDeviceUnits(value);
        }

        private void BuildShell()
        {
            SuspendLayout();

            bodyHost = new Panel { Dock = DockStyle.Fill, BackColor = B6Ui.Paper, AutoScroll = true };
            bodyHost.AutoScrollMinSize = new Size(U(1180), U(700));

            statusBar = new Panel { Dock = DockStyle.Bottom, Height = U(34), BackColor = B6Ui.Night };
            statusLabel = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = B6Ui.Cream,
                Font = new Font("Segoe UI", 9F),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(U(18), 0, U(18), 0),
                AutoEllipsis = true
            };
            statusBar.Controls.Add(statusLabel);

            hero = new B6Hero
            {
                Dock = DockStyle.Top,
                Height = U(132),
                Title = "Bài 06 · Hôm Nay Ăn Gì?",
                Subtitle = "SQLite · MonAn / NguoiDung · quay ngẫu nhiên món ngon theo từng buổi"
            };
            hero.AddAction("BẾP TRƯỞNG  ✦", true, () => ApplyMode(!chefMode, true));
            hero.AddAction("NHẬP input6", false, ImportClicked);
            hero.AddAction("TẢI LẠI", false, ReloadClicked);
            hero.AddAction("THOÁT", false, () => Close());

            fade = new B6Fade();

            Controls.Add(bodyHost);
            Controls.Add(statusBar);
            Controls.Add(hero);

            ResumeLayout(true);
        }

        private void ApplyMode(bool chef, bool animate = false)
        {
            Action change = () =>
            {
                chefMode = chef;
                basicRoot.Visible = !chef;
                chefRoot.Visible = chef;
                hero.Style = chef ? B6Hero.Look.Chef : B6Hero.Look.Street;
                hero.Title = chef ? "Bếp Trưởng · Gợi Ý Theo Giờ" : "Bài 06 · Hôm Nay Ăn Gì?";
                hero.Subtitle = chef
                    ? "Lắc quẻ · quán gần bạn · thực đơn 1.000+ món · yêu thích & thống kê"
                    : "SQLite · MonAn / NguoiDung · quay ngẫu nhiên món ngon theo từng buổi";
                hero.SetActionText(0, chef ? "←  QUẦY GỌI MÓN" : "BẾP TRƯỞNG  ✦");
                bodyHost.BackColor = chef ? B6Ui.Night : B6Ui.Paper;
                if (chef)
                {
                    RefreshChefAll();
                }
            };

            if (animate)
            {
                fade.Run(bodyHost, change);
            }
            else
            {
                change();
            }
        }

        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            UpdateClock();
            if (chefMode && chefAutoMeal)
            {
                int index = B6Ui.MealIndexForNow();
                if (segChefMeal.SelectedIndex != index)
                {
                    segChefMeal.SelectSilently(index);
                    OnChefMealChanged();
                }
            }
        }

        private void Lab02_Bai06_Load(object sender, EventArgs e)
        {
            try
            {
                loading = true;
                database.Initialize();
                bool seeded = false;
                if (database.CountContributors() == 0 && database.CountDishes() == 0)
                {
                    database.SeedSampleData();
                    seeded = true;
                }

                int removed = database.CleanupAutoContributors();
                int added = database.EnsureCatalog() + database.EnsureExtraCatalog() + database.EnsureWorldCatalog();
                loading = false;

                ReloadAll();
                if (added > 0 || removed > 0)
                {
                    SetStatus("Đã nạp thêm " + added + " món (tổng " + allDishes.Count + ")" + (removed > 0 ? " · đã dọn " + removed + " người đóng góp do chương trình tự tạo trước đây" : string.Empty));
                }
                else
                {
                    SetStatus(seeded ? "Đã tạo cơ sở dữ liệu SQLite và nạp dữ liệu mẫu" : "Đã kết nối cơ sở dữ liệu: " + database.DatabasePath);
                }
            }
            catch (Exception ex)
            {
                loading = false;
                ShowError(ex);
            }
        }

        private void ReloadAll()
        {
            loading = true;
            try
            {
                contributors = database.GetContributors();
                allDishes = database.GetDishes(0);
                favoriteIds = database.GetFavoriteIds();

                FillContributorList();
                FillOwnerCombos();
                RebuildDishList();
                UpdateListStats();

                List<string> ticker = new List<string>();
                if (allDishes.Count > 0)
                {
                    for (int i = 0; i < 24; i++)
                    {
                        ticker.Add(allDishes[random.Next(allDishes.Count)].Name);
                    }
                }
                hero.SetTicker(ticker);
            }
            finally
            {
                loading = false;
            }

            if (chefMode)
            {
                RefreshChefAll();
            }
        }

        private void ReloadClicked()
        {
            try
            {
                ReloadAll();
                SetStatus("Đã tải lại dữ liệu từ SQLite · " + allDishes.Count + " món · " + PeopleCount() + " người đóng góp", database.LastSql);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private static string RoleGlyph(string role)
        {
            switch (role)
            {
                case Roles.Admin: return "★ ";
                case Roles.Member: return "● ";
                case Roles.Guest: return "○ ";
                default: return "× ";
            }
        }

        private int CountAdmins()
        {
            int count = 0;
            foreach (Contributor c in contributors)
            {
                if (c.Role == Roles.Admin) count++;
            }
            return count;
        }

        private void FillContributorList()
        {
            lvContributors.BeginUpdate();
            lvContributors.Items.Clear();
            int people = 0;
            foreach (Contributor c in contributors)
            {
                if (c.IsSystem) continue;
                people++;
                ListViewItem item = new ListViewItem(c.FullName) { Tag = c };
                item.SubItems.Add(RoleGlyph(c.Role) + c.Role);
                item.SubItems.Add("•••");
                lvContributors.Items.Add(item);
            }
            lvContributors.EndUpdate();

            lblContribStats.Text = people + " người đóng góp · " + CountAdmins() + " quản trị viên";
            lblContribHint.Text = people == 0
                ? "Danh sách đang trống. Nhập họ tên rồi bấm THÊM — người đầu tiên sẽ tự là Quản trị viên."
                : "Nhấp chuột phải vào một người (hoặc bấm ••• ở cuối dòng) để cấp, thu hồi hoặc xóa quyền.";
        }

        private void FillOwnerCombos()
        {
            int ownerSelected = cboDishOwner.SelectedIndex >= 0 && cboDishOwner.SelectedIndex < dishOwnerIds.Count ? dishOwnerIds[cboDishOwner.SelectedIndex] : 0;
            int filterSelected = cboFilterOwner.SelectedIndex >= 0 && cboFilterOwner.SelectedIndex < filterOwnerIds.Count ? filterOwnerIds[cboFilterOwner.SelectedIndex] : 0;

            dishOwnerIds.Clear();
            filterOwnerIds.Clear();
            List<string> ownerNames = new List<string>();
            List<string> filterNames = new List<string> { "Tất cả người đóng góp" };
            filterOwnerIds.Add(0);

            foreach (Contributor c in contributors)
            {
                if (c.CanContribute)
                {
                    dishOwnerIds.Add(c.Id);
                    ownerNames.Add(c.FullName);
                }
                filterOwnerIds.Add(c.Id);
                filterNames.Add(c.FullName);
            }

            cboDishOwner.SetItems(ownerNames, Math.Max(0, dishOwnerIds.IndexOf(ownerSelected)));
            cboFilterOwner.SetItems(filterNames, Math.Max(0, filterOwnerIds.IndexOf(filterSelected)));
        }

        private readonly ToolTip sqlTip = new ToolTip();

        private void SetStatus(string message, string sql = null)
        {
            statusLabel.Text = message;
            sqlTip.SetToolTip(statusLabel, string.IsNullOrEmpty(sql) ? string.Empty : "SQL: " + sql);
        }

        private void ShowInfo(string message)
        {
            MessageBox.Show(this, message, MessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowWarning(string message)
        {
            MessageBox.Show(this, message, MessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ShowError(Exception ex)
        {
            MessageBox.Show(this, "Có lỗi xảy ra:\n" + ex.Message, MessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            SetStatus("Lỗi: " + ex.Message);
        }

        private TableLayoutPanel Grid(Color back, float[] cols, params float[] rows)
        {
            TableLayoutPanel grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = back,
                ColumnCount = cols.Length,
                RowCount = rows.Length,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            foreach (float c in cols) grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, c));
            foreach (float r in rows)
            {
                if (r >= 1f) grid.RowStyles.Add(new RowStyle(SizeType.Absolute, U((int)r)));
                else if (r < 0f) grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                else grid.RowStyles.Add(new RowStyle(SizeType.Percent, r == 0f ? 100f : r * 100f));
            }
            B6Ui.EnableDoubleBuffer(grid);
            return grid;
        }

        private void Put(TableLayoutPanel grid, Control control, int col, int row, int span = 1)
        {
            control.Dock = DockStyle.Fill;
            control.Margin = new Padding(U(2), U(3), U(2), U(3));
            grid.Controls.Add(control, col, row);
            if (span > 1) grid.SetColumnSpan(control, span);
        }

        private B6Card NewCard(Color accent, bool dark)
        {
            B6Card card = new B6Card
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(U(6)),
                Padding = new Padding(U(18), U(20), U(24), U(26)),
                BackColor = dark ? B6Ui.NightCard : Color.White
            };
            if (dark)
            {
                card.Shadow = false;
                card.BorderColor = B6Ui.NightLine;
                card.Padding = new Padding(U(16), U(14), U(18), U(16));
            }
            else
            {
                card.AccentColor = accent;
            }
            return card;
        }

        private Label SectionTitle(string text, bool dark)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Palatino Linotype", 14.5F, FontStyle.Bold),
                ForeColor = dark ? B6Ui.Gold : B6Ui.ChiliDark,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };
        }

        private Label SmallLabel(string text, bool dark, FontStyle style = FontStyle.Regular, float size = 9.5F)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", size, style),
                ForeColor = dark ? B6Ui.CreamMuted : B6Ui.InkSoft,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };
        }

        private B6Button MakeButton(string text, Color back, Color fore, Color hover, Color border, Color surface, EventHandler click)
        {
            B6Button button = new B6Button
            {
                Text = text,
                BackColor = back,
                ForeColor = fore,
                HoverColor = hover,
                BorderColor = border,
                SurfaceColor = surface,
                Radius = 14
            };
            if (click != null) button.Click += click;
            return button;
        }

        private B6Button PrimaryButton(string text, EventHandler click)
        {
            return MakeButton(text, B6Ui.Chili, Color.White, B6Ui.ChiliDark, Color.Empty, Color.White, click);
        }

        private B6Button GreenButton(string text, EventHandler click)
        {
            return MakeButton(text, B6Ui.Basil, Color.White, B6Ui.BasilDark, Color.Empty, Color.White, click);
        }

        private B6Button GhostButton(string text, EventHandler click)
        {
            return MakeButton(text, Color.White, B6Ui.ChiliDark, B6Ui.Paper, B6Ui.Line, Color.White, click);
        }

        private B6Button GoldButton(string text, EventHandler click)
        {
            return MakeButton(text, B6Ui.Gold, Color.FromArgb(48, 30, 12), B6Ui.GoldDark, Color.Empty, B6Ui.NightCard, click);
        }

        private B6Button DarkButton(string text, EventHandler click)
        {
            return MakeButton(text, B6Ui.NightCard2, B6Ui.Cream, B6Ui.NightLine, B6Ui.NightLine, B6Ui.NightCard, click);
        }

        private static void FitColumns(ListView list, params float[] weights)
        {
            int total = Math.Max(100, list.ClientSize.Width - SystemInformation.VerticalScrollBarWidth);
            float sum = 0;
            foreach (float w in weights) sum += w;
            for (int i = 0; i < list.Columns.Count && i < weights.Length; i++)
            {
                list.Columns[i].Width = (int)(total * weights[i] / sum);
            }
        }

        private static ListView NewList(bool dark, params string[] headers)
        {
            ListView list = new ListView { Dock = DockStyle.Fill };
            foreach (string header in headers) list.Columns.Add(header, 100);
            B6Ui.StyleList(list, dark);
            return list;
        }

        private void RememberPick(Dish dish, string source)
        {
            try
            {
                database.AddHistory(dish.Id, source);
                string sql = database.LastSql;
                SetStatus("Đã chọn \"" + dish.Name + "\" (" + source + ") · lưu vào lịch sử", sql);
                if (chefMode && chefViewIndex == 4)
                {
                    RefreshFavoritesView();
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private B6StatStrip statStrip;

        private void BuildBasicMode()
        {
            SuspendLayout();
            basicRoot = new Panel { Dock = DockStyle.Fill, BackColor = B6Ui.Paper, Padding = new Padding(U(12), U(10), U(12), U(6)) };
            TableLayoutPanel columns = Grid(B6Ui.Paper, new float[] { 31, 38, 31 }, 0);

            B6Card leftCard = NewCard(B6Ui.Basil, false);
            TableLayoutPanel leftGrid = Grid(Color.White, new float[] { 100 }, 44, 0);
            segLeftTabs = new B6Segments { Items = new[] { "Người đóng góp", "Thêm món ăn" }, ThumbColor = B6Ui.Basil };
            segLeftTabs.SelectedIndexChanged += (s, e) => ShowLeftPage(segLeftTabs.SelectedIndex);
            Put(leftGrid, segLeftTabs, 0, 0);

            Panel leftHost = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            pnlContribPage = BuildContributorPage();
            pnlDishPage = BuildAddDishPage();
            pnlDishPage.Visible = false;
            leftHost.Controls.Add(pnlContribPage);
            leftHost.Controls.Add(pnlDishPage);
            Put(leftGrid, leftHost, 0, 1);
            leftCard.Controls.Add(leftGrid);
            columns.Controls.Add(leftCard, 0, 0);

            B6Card listCard = NewCard(B6Ui.Saffron, false);
            TableLayoutPanel lg = Grid(Color.White, new float[] { 64, 36 }, 34, 44, 44, 42, 0, 26, 46);
            Put(lg, SectionTitle("3 · Danh sách món ăn", false), 0, 0, 2);
            txtSearch = new B6Input(false) { PlaceholderText = "Tìm món (không cần gõ dấu)" };
            txtSearch.TextChanged += (s, e) => { if (!loading) RebuildDishList(); };
            Put(lg, txtSearch, 0, 1, 2);
            cboFilterOwner = new B6Combo(false);
            cboFilterOwner.SelectedIndexChanged += (s, e) => { if (!loading) RebuildDishList(); };
            Put(lg, cboFilterOwner, 0, 2);
            btnToggleView = GhostButton("DẠNG ẢNH", btnToggleView_Click);
            Put(lg, btnToggleView, 1, 2);
            segListMeal = new B6Segments { Items = new[] { "Tất cả", "Sáng", "Trưa", "Chiều", "Tối" } };
            segListMeal.SelectedIndexChanged += (s, e) => { if (!loading) RebuildDishList(); };
            Put(lg, segListMeal, 0, 3, 2);

            lvDishes = NewList(false, "Tên món", "Mã", "Người đóng góp", "Loại", "Giá");
            lvDishes.SmallImageList = smallImages;
            lvDishes.LargeImageList = largeImages;
            lvDishes.SizeChanged += (s, e) => FitColumns(lvDishes, 4.4f, 0.9f, 3f, 1.7f, 1.4f);
            lvDishes.ItemActivate += lvDishes_ItemActivate;
            Put(lg, lvDishes, 0, 4, 2);
            lblListStats = SmallLabel(string.Empty, false);
            Put(lg, lblListStats, 0, 5, 2);

            TableLayoutPanel listButtons = Grid(Color.White, new float[] { 40, 60 }, 0);
            Put(listButtons, GhostButton("BỎ LỌC", btnResetFilter_Click), 0, 0);
            Put(listButtons, MakeButton("XÓA MÓN ĐANG CHỌN", Color.FromArgb(150, 36, 34), Color.White, Color.FromArgb(120, 26, 26), Color.Empty, Color.White, btnDeleteDish_Click), 1, 0);
            Put(lg, listButtons, 0, 6, 2);
            listCard.Controls.Add(lg);
            columns.Controls.Add(listCard, 1, 0);

            B6Card resultCard = NewCard(B6Ui.Chili, false);
            TableLayoutPanel rg = Grid(Color.White, new float[] { 100 }, 34, 40, 46, 0, 78, 24, 24, 74, 56);
            Put(rg, SectionTitle("4 · Hôm nay ăn gì?", false), 0, 0);
            segPickMeal = new B6Segments { Items = B6Ui.Meals, ThumbColor = B6Ui.Basil };
            segPickMeal.SelectSilently(B6Ui.MealIndexForNow());
            segPickMeal.SelectedIndexChanged += (s, e) => lblPickNote.Text = B6Ui.MealNote(segPickMeal.SelectedText);
            Put(rg, segPickMeal, 0, 1);
            lblPickNote = SmallLabel(B6Ui.MealNote(segPickMeal.SelectedText), false, FontStyle.Italic, 9F);
            lblPickNote.TextAlign = ContentAlignment.TopLeft;
            lblPickNote.AutoEllipsis = false;
            Put(rg, lblPickNote, 0, 2);
            stageResult = new B6Stage { Radius = 18 };
            stageResult.Clear("Bấm \"QUAY MÓN HÔM NAY\" để chọn món\ncho đúng buổi bạn đang đói");
            Put(rg, stageResult, 0, 3);
            lblResultName = new Label
            {
                Text = "Sẵn sàng quay",
                Font = new Font("Palatino Linotype", 17F, FontStyle.Bold),
                ForeColor = B6Ui.Chili,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                UseMnemonic = false,
                AutoEllipsis = false
            };
            Put(rg, lblResultName, 0, 4);
            lblResultOwner = SmallLabel(string.Empty, false, FontStyle.Italic, 10.5F);
            lblResultOwner.ForeColor = B6Ui.Basil;
            lblResultOwner.TextAlign = ContentAlignment.MiddleCenter;
            Put(rg, lblResultOwner, 0, 5);
            lblResultMeta = SmallLabel(string.Empty, false, FontStyle.Bold, 9.5F);
            lblResultMeta.ForeColor = B6Ui.SaffronDark;
            lblResultMeta.TextAlign = ContentAlignment.MiddleCenter;
            Put(rg, lblResultMeta, 0, 6);
            lblResultDesc = SmallLabel(string.Empty, false, FontStyle.Regular, 10F);
            lblResultDesc.TextAlign = ContentAlignment.TopCenter;
            lblResultDesc.AutoEllipsis = false;
            Put(rg, lblResultDesc, 0, 7);
            btnPick = MakeButton("QUAY MÓN HÔM NAY", B6Ui.Saffron, Color.FromArgb(54, 30, 12), B6Ui.SaffronDark, Color.Empty, Color.White, btnPick_Click);
            btnPick.Font = new Font("Segoe UI", 12.5F, FontStyle.Bold);
            btnPick.Radius = 18;
            Put(rg, btnPick, 0, 8);
            resultCard.Controls.Add(rg);
            columns.Controls.Add(resultCard, 2, 0);

            statStrip = new B6StatStrip { Dock = DockStyle.Top, Height = U(70) };
            basicRoot.Controls.Add(columns);
            basicRoot.Controls.Add(statStrip);
            bodyHost.Controls.Add(basicRoot);
            ResumeLayout(true);
        }

        private int ThumbIndexFor(Dish dish)
        {
            string key = dish.HasCustomImage ? "dish:" + dish.Id + ":" + dish.ImagePath : "cat:" + dish.Category;
            int index;
            if (thumbIndex.TryGetValue(key, out index))
            {
                return index;
            }

            Bitmap small;
            Bitmap large;
            if (dish.HasCustomImage)
            {
                using (Image source = dishImages.LoadSource(dish))
                {
                    small = FoodImageFactory.CreateThumbnail(source, smallImages.ImageSize.Width, smallImages.ImageSize.Height);
                    large = FoodImageFactory.CreateThumbnail(source, largeImages.ImageSize.Width, largeImages.ImageSize.Height);
                }
            }
            else
            {
                small = FoodImageFactory.CreateCategoryThumb(dish.Category, smallImages.ImageSize.Width, smallImages.ImageSize.Height);
                large = FoodImageFactory.CreateCategoryThumb(dish.Category, largeImages.ImageSize.Width, largeImages.ImageSize.Height);
            }

            smallImages.Images.Add(small);
            largeImages.Images.Add(large);
            small.Dispose();
            large.Dispose();
            index = smallImages.Images.Count - 1;
            thumbIndex[key] = index;
            return index;
        }

        private void RebuildDishList()
        {
            if (lvDishes == null) return;

            string keyword = B6Ui.Fold(txtSearch.Text.Trim());
            int ownerId = cboFilterOwner.SelectedIndex >= 0 && cboFilterOwner.SelectedIndex < filterOwnerIds.Count ? filterOwnerIds[cboFilterOwner.SelectedIndex] : 0;
            string meal = segListMeal.SelectedIndex > 0 ? segListMeal.SelectedText : string.Empty;

            List<Dish> candidates = allDishes.FindAll(d =>
                (ownerId == 0 || d.ContributorId == ownerId) &&
                (meal.Length == 0 || d.SuitsMeal(meal)));
            if (keyword.Length > 0)
            {
                candidates = RankByKeyword(candidates, keyword);
            }

            lvDishes.BeginUpdate();
            lvDishes.Items.Clear();
            int shown = 0;
            foreach (Dish dish in candidates)
            {

                ListViewItem item = new ListViewItem(dish.Name) { Tag = dish, ImageIndex = ThumbIndexFor(dish) };
                item.SubItems.Add(dish.Id.ToString());
                item.SubItems.Add(dish.ContributorName);
                item.SubItems.Add(dish.Category);
                item.SubItems.Add(B6Ui.FormatPrice(dish.Price));
                lvDishes.Items.Add(item);
                shown++;
            }
            lvDishes.EndUpdate();
            FitColumns(lvDishes, 4.4f, 0.9f, 3f, 1.7f, 1.4f);
            UpdateListStats(shown);
        }

        private void UpdateListStats(int shown = -1)
        {
            if (lblListStats == null) return;
            if (shown < 0) shown = lvDishes.Items.Count;

            if (statStrip != null)
            {
                HashSet<string> groups = new HashSet<string>();
                foreach (Dish dish in allDishes) groups.Add(dish.Category);
                statStrip.SetValues(allDishes.Count, PeopleCount(), groups.Count);
            }

            string top = string.Empty;
            try
            {
                List<ContributorStat> stats = database.GetContributorStats();
                if (stats.Count > 0 && stats[0].DishCount > 0)
                {
                    top = "  ·  Nhiều nhất: " + stats[0].Name + " (" + stats[0].DishCount + " món)";
                }
            }
            catch (Exception)
            {
            }
            lblListStats.Text = "Hiển thị " + shown + "/" + allDishes.Count + " món  ·  " + PeopleCount() + " người đóng góp" + top;
        }

        private Dish SelectedDish()
        {
            return lvDishes.SelectedItems.Count > 0 ? lvDishes.SelectedItems[0].Tag as Dish : null;
        }

        private void lvDishes_ItemActivate(object sender, EventArgs e)
        {
            Dish dish = SelectedDish();
            if (dish != null)
            {
                ShowPickResult(dish, false);
            }
        }

        private void btnToggleView_Click(object sender, EventArgs e)
        {
            thumbnailMode = !thumbnailMode;
            lvDishes.View = thumbnailMode ? View.LargeIcon : View.Details;
            btnToggleView.Text = thumbnailMode ? "DẠNG DANH SÁCH" : "DẠNG ẢNH";
            if (!thumbnailMode) FitColumns(lvDishes, 4.4f, 0.9f, 3f, 1.7f, 1.4f);
        }

        private void btnResetFilter_Click(object sender, EventArgs e)
        {
            loading = true;
            txtSearch.Text = string.Empty;
            cboFilterOwner.SelectedIndex = 0;
            segListMeal.SelectSilently(0);
            loading = false;
            RebuildDishList();
            SetStatus("Đã bỏ bộ lọc, hiển thị tất cả món ăn");
        }

        private Panel BuildContributorPage()
        {
            Panel page = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            TableLayoutPanel cg = Grid(Color.White, new float[] { 64, 36 }, 24, 42, 44, 40, 0, 46);

            lblContribStats = SmallLabel(string.Empty, false, FontStyle.Bold, 9.5F);
            lblContribStats.ForeColor = B6Ui.Basil;
            Put(cg, lblContribStats, 0, 0, 2);

            txtContributorName = new B6Input(false) { PlaceholderText = "Nhập họ và tên người đóng góp" };
            Put(cg, txtContributorName, 0, 1, 2);

            cboRole = new B6Combo(false);
            cboRole.SetItems(RoleChoices, 1);
            Put(cg, cboRole, 0, 2);
            Put(cg, GreenButton("THÊM", btnAddContributor_Click), 1, 2);

            lblContribHint = SmallLabel(string.Empty, false, FontStyle.Italic, 9F);
            lblContribHint.AutoEllipsis = false;
            lblContribHint.TextAlign = ContentAlignment.TopLeft;
            Put(cg, lblContribHint, 0, 3, 2);

            lvContributors = NewList(false, "Họ và tên", "Quyền hạn", string.Empty);
            lvContributors.SizeChanged += (s, e) => FitColumns(lvContributors, 5.2f, 3.2f, 0.9f);
            lvContributors.MouseUp += lvContributors_MouseUp;
            lvContributors.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Delete) { DeleteContributorAction(SelectedContributor()); }
                else if (e.KeyCode == Keys.Apps || (e.Shift && e.KeyCode == Keys.F10)) { ShowSelectedContributorMenu(); }
            };
            Put(cg, lvContributors, 0, 4, 2);

            TableLayoutPanel actions = Grid(Color.White, new float[] { 36, 32, 32 }, 0);
            B6Button grant = GhostButton("QUYỀN  ▾", (s, e) => ShowSelectedContributorMenu());
            B6Button revoke = MakeButton("THU HỒI", Color.FromArgb(150, 36, 34), Color.White, Color.FromArgb(120, 26, 26), Color.Empty, Color.White, (s, e) => RevokeAction(SelectedContributor()));
            B6Button remove = GhostButton("XÓA", (s, e) => DeleteContributorAction(SelectedContributor()));
            grant.Font = revoke.Font = remove.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            Put(actions, grant, 0, 0);
            Put(actions, revoke, 1, 0);
            Put(actions, remove, 2, 0);
            Put(cg, actions, 0, 5, 2);

            page.Controls.Add(cg);
            return page;
        }

        private Panel BuildAddDishPage()
        {
            Panel page = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, AutoScroll = true };
            TableLayoutPanel ag = Grid(Color.White, new float[] { 56, 44 }, 22, 42, 22, 42, 22, 42, 22, 42, 150, 48);
            ag.Dock = DockStyle.Top;
            ag.Height = U(22 * 4 + 42 * 4 + 150 + 48 + 16);

            Put(ag, SmallLabel("Tên món ăn", false, FontStyle.Bold), 0, 0, 2);
            txtDishName = new B6Input(false) { PlaceholderText = "Ví dụ: Bún bò Huế" };
            Put(ag, txtDishName, 0, 1, 2);

            Put(ag, SmallLabel("Người đóng góp (chỉ người còn quyền)", false, FontStyle.Bold), 0, 2, 2);
            cboDishOwner = new B6Combo(false);
            Put(ag, cboDishOwner, 0, 3, 2);

            Put(ag, SmallLabel("Loại món · Giá (đồng)", false, FontStyle.Bold), 0, 4, 2);
            cboDishCategory = new B6Combo(false);
            cboDishCategory.SetItems(FoodCatalog.Categories, 0);
            Put(ag, cboDishCategory, 0, 5);
            txtDishPrice = new B6Input(false) { PlaceholderText = "50000" };
            Put(ag, txtDishPrice, 1, 5);

            Put(ag, SmallLabel("Hình ảnh (bỏ trống để vẽ ảnh theo tên món)", false, FontStyle.Bold), 0, 6, 2);
            txtImagePath = new B6Input(false) { PlaceholderText = "Chưa chọn ảnh" };
            txtImagePath.Inner.ReadOnly = true;
            Put(ag, txtImagePath, 0, 7);
            Put(ag, GhostButton("CHỌN ẢNH", btnBrowse_Click), 1, 7);

            stagePreview = new B6Stage { Radius = 12 };
            stagePreview.Clear("Xem trước ảnh");
            Put(ag, stagePreview, 0, 8, 2);
            Put(ag, PrimaryButton("THÊM MÓN ĂN", btnAddDish_Click), 0, 9, 2);

            page.Controls.Add(ag);
            return page;
        }

        private void ShowLeftPage(int index)
        {
            pnlContribPage.Visible = index == 0;
            pnlDishPage.Visible = index == 1;
        }

        private Contributor SelectedContributor()
        {
            return lvContributors.SelectedItems.Count == 0 ? null : (Contributor)lvContributors.SelectedItems[0].Tag;
        }

        private int PeopleCount()
        {
            int count = 0;
            foreach (Contributor c in contributors)
            {
                if (!c.IsSystem) count++;
            }
            return count;
        }

        private void lvContributors_MouseUp(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo hit = lvContributors.HitTest(e.Location);
            if (hit.Item == null) return;

            bool dots = e.Button == MouseButtons.Left && hit.SubItem != null && hit.Item.SubItems.IndexOf(hit.SubItem) == 2;
            if (e.Button != MouseButtons.Right && !dots) return;

            hit.Item.Selected = true;
            ShowContributorMenu((Contributor)hit.Item.Tag, lvContributors, e.Location);
        }

        private void ShowSelectedContributorMenu()
        {
            ListViewItem item = lvContributors.SelectedItems.Count > 0 ? lvContributors.SelectedItems[0] : null;
            if (item == null)
            {
                ShowInfo("Hãy chọn một người trong danh sách trước (hoặc nhấp chuột phải vào tên họ).");
                return;
            }
            Rectangle r = item.Bounds;
            ShowContributorMenu((Contributor)item.Tag, lvContributors, new Point(Math.Min(r.Left + U(40), lvContributors.ClientSize.Width - U(20)), r.Bottom));
        }

        private void ShowContributorMenu(Contributor c, Control host, Point at)
        {
            ContextMenuStrip menu = new ContextMenuStrip { Font = new Font("Segoe UI", 10F), ShowImageMargin = false };
            menu.Closed += (s, e) => BeginInvoke((Action)(() => menu.Dispose()));

            ToolStripMenuItem header = new ToolStripMenuItem(c.FullName + "  ·  " + RoleGlyph(c.Role) + c.Role) { Enabled = false };
            menu.Items.Add(header);
            menu.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem grant = new ToolStripMenuItem("Cấp quyền");
            foreach (string role in RoleChoices)
            {
                string target = role;
                ToolStripMenuItem item = new ToolStripMenuItem(RoleGlyph(role) + role) { Checked = c.Role == role, Enabled = c.Role != role };
                item.Click += (s, e) => SetContributorRole(c, target, "Đã cấp quyền \"" + target + "\" cho " + c.FullName);
                grant.DropDownItems.Add(item);
            }
            menu.Items.Add(grant);

            ToolStripMenuItem revoke = new ToolStripMenuItem("Thu hồi quyền (hạ xuống Khách)") { Enabled = c.Role == Roles.Admin || c.Role == Roles.Member };
            revoke.Click += (s, e) => RevokeAction(c);
            menu.Items.Add(revoke);

            ToolStripMenuItem clear = new ToolStripMenuItem("Xóa quyền (không còn quyền nào)") { Enabled = c.Role != Roles.None };
            clear.Click += (s, e) => ClearRoleAction(c);
            menu.Items.Add(clear);

            menu.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem filter = new ToolStripMenuItem("Xem món của người này");
            filter.Click += (s, e) => ShowDishesOf(c);
            menu.Items.Add(filter);

            ToolStripMenuItem delete = new ToolStripMenuItem("Xóa người đóng góp…");
            delete.Click += (s, e) => DeleteContributorAction(c);
            menu.Items.Add(delete);

            menu.Show(host, at);
        }

        private void ShowDishesOf(Contributor c)
        {
            int index = filterOwnerIds.IndexOf(c.Id);
            if (index >= 0)
            {
                cboFilterOwner.SelectedIndex = index;
                SetStatus("Đang lọc danh sách món của " + c.FullName);
            }
        }

        private void btnAddContributor_Click(object sender, EventArgs e)
        {
            string fullName = Regex.Replace(txtContributorName.Text.Trim(), "\\s+", " ");
            if (fullName.Length == 0)
            {
                ShowWarning("Vui lòng nhập họ và tên người đóng góp.");
                txtContributorName.Inner.Focus();
                return;
            }

            foreach (Contributor existing in contributors)
            {
                if (string.Equals(existing.FullName, fullName, StringComparison.OrdinalIgnoreCase))
                {
                    ShowInfo("\"" + fullName + "\" đã có trong danh sách (quyền hiện tại: " + existing.Role + ").");
                    return;
                }
            }

            try
            {
                bool first = PeopleCount() == 0;
                string role = first ? Roles.Admin : (cboRole.SelectedItem != null ? cboRole.SelectedItem.ToString() : Roles.Member);
                int newId = database.AddContributor(fullName, role);
                string sql = database.LastSql;
                txtContributorName.Text = string.Empty;
                ReloadAll();
                SetStatus("Đã thêm " + fullName + " với quyền \"" + role + "\"" + (first ? " (người đầu tiên tự là Quản trị viên)" : string.Empty) + " · mã #" + newId, sql);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void SetContributorRole(Contributor c, string role, string message)
        {
            if (c == null)
            {
                ShowInfo("Hãy chọn một người trong danh sách trước.");
                return;
            }
            if (c.Role == role)
            {
                ShowInfo(c.FullName + " đang có quyền \"" + role + "\" rồi.");
                return;
            }
            if (c.Role == Roles.Admin && role != Roles.Admin && CountAdmins() <= 1)
            {
                ShowWarning("Đây là Quản trị viên duy nhất. Hãy cấp quyền Quản trị viên cho người khác trước khi hạ quyền của " + c.FullName + ".");
                return;
            }

            try
            {
                database.UpdateContributorRole(c.Id, role);
                string sql = database.LastSql;
                ReloadAll();
                SetStatus(message, sql);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void RevokeAction(Contributor c)
        {
            if (c == null)
            {
                ShowInfo("Hãy chọn người cần thu hồi quyền (nhấp chuột phải vào tên họ).");
                return;
            }
            if (c.Role != Roles.Admin && c.Role != Roles.Member)
            {
                ShowInfo(c.FullName + " hiện là \"" + c.Role + "\", không còn quyền nào để thu hồi.");
                return;
            }
            if (MessageBox.Show(this, "Thu hồi quyền \"" + c.Role + "\" của " + c.FullName + "?\nNgười này sẽ trở thành \"Khách\" (không thể đứng tên món mới).", MessageTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            SetContributorRole(c, Roles.Guest, "Đã thu hồi quyền \"" + c.Role + "\" của " + c.FullName);
        }

        private void ClearRoleAction(Contributor c)
        {
            if (c == null)
            {
                ShowInfo("Hãy chọn một người trong danh sách trước.");
                return;
            }
            if (MessageBox.Show(this, "Xóa toàn bộ quyền của " + c.FullName + "?\nNgười này vẫn nằm trong danh sách nhưng ở mức \"" + Roles.None + "\".", MessageTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            SetContributorRole(c, Roles.None, "Đã xóa quyền của " + c.FullName);
        }

        private void DeleteContributorAction(Contributor c)
        {
            if (c == null)
            {
                ShowInfo("Hãy chọn một người đóng góp trong danh sách để xóa.");
                return;
            }
            if (c.Role == Roles.Admin && CountAdmins() <= 1)
            {
                ShowWarning("Đây là Quản trị viên duy nhất nên chưa thể xóa. Hãy cấp quyền Quản trị viên cho người khác trước.");
                return;
            }

            try
            {
                int dishCount = database.CountDishesOf(c.Id);
                string question = "Xóa người đóng góp \"" + c.FullName + "\"?";
                if (dishCount > 0)
                {
                    question += "\n\nNgười này đang đứng tên " + dishCount + " món. Các món sẽ được chuyển sang \"Thư viện món ăn\" (không bị mất).";
                }
                if (MessageBox.Show(this, question, MessageTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                database.DeleteContributor(c.Id, true);
                string sql = database.LastSql;
                ReloadAll();
                SetStatus("Đã xóa người đóng góp: " + c.FullName + (dishCount > 0 ? " · " + dishCount + " món chuyển về Thư viện món ăn" : string.Empty), sql);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Chọn hình ảnh món ăn";
                dialog.Filter = "Hình ảnh|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Tất cả tệp|*.*";
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                using (Image preview = FoodImageFactory.LoadCopy(dialog.FileName))
                {
                    if (preview == null)
                    {
                        ShowWarning("Không đọc được tệp ảnh đã chọn.");
                        return;
                    }
                    txtImagePath.Text = dialog.FileName;
                    stagePreview.Reveal(preview, "Xem trước", false);
                }
            }
        }

        private void btnAddDish_Click(object sender, EventArgs e)
        {
            string dishName = Regex.Replace(txtDishName.Text.Trim(), "\\s+", " ");
            if (dishName.Length == 0)
            {
                ShowWarning("Vui lòng nhập tên món ăn.");
                txtDishName.Inner.Focus();
                return;
            }

            if (cboDishOwner.SelectedIndex < 0 || cboDishOwner.SelectedIndex >= dishOwnerIds.Count)
            {
                ShowWarning("Chưa có người đóng góp nào còn quyền.\nHãy sang thẻ \"Người đóng góp\" để thêm người hoặc cấp quyền trước.");
                segLeftTabs.SelectedIndex = 0;
                return;
            }

            int price = 0;
            string priceText = new string(txtDishPrice.Text.Where(char.IsDigit).ToArray());
            if (priceText.Length > 0 && !int.TryParse(priceText, NumberStyles.Integer, CultureInfo.InvariantCulture, out price))
            {
                ShowWarning("Giá món không hợp lệ.");
                return;
            }

            if (database.FindDishByName(dishName) != null)
            {
                ShowInfo("Món \"" + dishName + "\" đã có trong cơ sở dữ liệu.");
                return;
            }

            try
            {
                string category = cboDishCategory.SelectedItem != null ? cboDishCategory.SelectedItem.ToString() : "Món chính";
                string storedPath = SaveDishImage();
                int newId = database.AddDish(dishName, storedPath, dishOwnerIds[cboDishOwner.SelectedIndex]);
                string sql = database.LastSql;
                database.SaveDishInfo(newId, category, "Sáng,Trưa,Chiều,Tối", price, string.Empty, "Món do người dùng thêm vào.");

                txtDishName.Text = string.Empty;
                txtDishPrice.Text = string.Empty;
                txtImagePath.Text = string.Empty;
                stagePreview.Clear("Xem trước ảnh");
                ReloadAll();
                SetStatus("Đã thêm món \"" + dishName + "\" (IDMA = " + newId + ")", sql);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private string SaveDishImage()
        {
            using (Image source = FoodImageFactory.LoadCopy(txtImagePath.Text.Trim()))
            {
                if (source == null)
                {
                    return string.Empty;
                }

                string fileName = "dish_" + DateTime.Now.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture) + ".jpg";
                FoodImageFactory.SaveScaledCopy(source, Path.Combine(database.ImageDirectory, fileName), 960, 720);
                return database.RelativeImagePath(fileName);
            }
        }

        private void btnDeleteDish_Click(object sender, EventArgs e)
        {
            Dish dish = SelectedDish();
            if (dish == null)
            {
                ShowInfo("Hãy chọn một món ăn trong danh sách để xóa.");
                return;
            }

            if (MessageBox.Show(this, "Xóa món \"" + dish.Name + "\" khỏi cơ sở dữ liệu?", MessageTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                database.DeleteDish(dish.Id);
                string sql = database.LastSql;
                ReloadAll();
                SetStatus("Đã xóa món \"" + dish.Name + "\"", sql);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void btnPick_Click(object sender, EventArgs e)
        {
            if (spinTimer.Enabled) return;

            string meal = segPickMeal.SelectedText;
            spinPool = allDishes.FindAll(d => d.SuitsMeal(meal));
            if (spinPool.Count == 0)
            {
                ShowInfo("Chưa có món nào hợp với buổi " + meal.ToLowerInvariant() + ".");
                return;
            }

            spinResult = spinPool[random.Next(spinPool.Count)];
            spinStep = 0;
            btnPick.Enabled = false;
            lblResultOwner.Text = "Đang quay các món buổi " + meal.ToLowerInvariant() + "…";
            lblResultMeta.Text = string.Empty;
            lblResultDesc.Text = string.Empty;
            spinTimer.Interval = 40;
            spinTimer.Start();
        }

        private void SpinTimer_Tick(object sender, EventArgs e)
        {
            if (spinStep < SpinSteps)
            {
                Dish preview = spinPool[random.Next(spinPool.Count)];
                lblResultName.Text = preview.Name;
                stageResult.Preview(dishImages.Get(preview, 480, 360), preview.Name);
                spinTimer.Interval = 40 + spinStep * 14;
                spinStep++;
                return;
            }

            spinTimer.Stop();
            btnPick.Enabled = true;
            ShowPickResult(spinResult, true);
            RememberPick(spinResult, "Quay món");
        }

        private void ShowPickResult(Dish dish, bool celebrate)
        {
            lblResultName.Text = dish.Name;
            lblResultOwner.Text = "Đóng góp bởi " + dish.ContributorName;
            lblResultMeta.Text = (B6Ui.FormatPrice(dish.Price) + "  ·  " + dish.Category + (string.IsNullOrEmpty(dish.Region) ? string.Empty : "  ·  " + dish.Region)).ToUpperInvariant();
            lblResultDesc.Text = string.IsNullOrWhiteSpace(dish.Description) ? string.Empty : dish.Description;
            stageResult.Reveal(dishImages.Get(dish, 480, 360), string.Empty, celebrate);
        }

        private static readonly string[] NavTitles =
        {
            "✦  Gợi ý hôm nay", "☯  Lắc quẻ", "⌖  Quán gần bạn", "▦  Thực đơn", "♥  Yêu thích", "▤  Thống kê"
        };

        private Panel chefContent;
        private Panel viewHost;
        private B6Detail detail;
        private Panel[] chefViews;
        private B6Button[] navButtons;
        private B6Segments segChefMeal;
        private B6Button btnAutoMeal;
        private Label lblChefClock;
        private Label lblChefNote;
        private bool chefAutoMeal = true;
        private int chefViewIndex = -1;

        private B6Combo cboSuggestCategory;
        private B6Combo cboSuggestPrice;
        private FlowLayoutPanel suggestFlow;

        private B6FortuneStage fortuneStage;
        private B6Combo cboFortuneCategory;
        private B6Combo cboFortunePrice;
        private Label lblFortuneMeal;
        private B6Button btnFortune;
        private B6Stage stageFortuneDish;
        private Label lblFortuneDishName;
        private Label lblFortuneDishDesc;
        private B6Button btnFortuneNearby;
        private Dish fortuneDish;

        private void BuildChefMode()
        {
            SuspendLayout();
            chefRoot = new Panel { Dock = DockStyle.Fill, BackColor = B6Ui.Night, Visible = false, Padding = new Padding(U(14), U(12), U(14), U(10)) };

            chefContent = new Panel { Dock = DockStyle.Fill, BackColor = B6Ui.Night, Padding = new Padding(U(12), 0, 0, 0) };
            viewHost = new Panel { Dock = DockStyle.Fill, BackColor = B6Ui.Night };
            detail = new B6Detail { Dock = DockStyle.Right, Width = U(318), Margin = Padding.Empty };
            detail.FavoriteClicked += (s, e) => { if (detail.Current != null) ToggleFavorite(detail.Current); };
            detail.NearbyClicked += (s, e) => { if (detail.Current != null) GoNearby(detail.Current); };
            detail.MapsClicked += (s, e) => { if (detail.Current != null) OpenMapsForDish(detail.Current); };
            chefContent.Controls.Add(viewHost);
            chefContent.Controls.Add(detail);

            chefViews = new Panel[6];
            for (int i = 0; i < chefViews.Length; i++)
            {
                chefViews[i] = new Panel { Dock = DockStyle.Fill, BackColor = B6Ui.Night, Visible = false };
                B6Ui.EnableDoubleBuffer(chefViews[i]);
                viewHost.Controls.Add(chefViews[i]);
            }

            Panel nav = BuildNavRail();

            BuildSuggestView(chefViews[0]);
            BuildFortuneView(chefViews[1]);
            BuildNearbyView(chefViews[2]);
            BuildMenuView(chefViews[3]);
            BuildFavoritesView(chefViews[4]);
            BuildStatsView(chefViews[5]);

            chefRoot.Controls.Add(chefContent);
            chefRoot.Controls.Add(nav);
            bodyHost.Controls.Add(chefRoot);
            ResumeLayout(true);
        }

        private Panel BuildNavRail()
        {
            B6Card rail = new B6Card
            {
                Dock = DockStyle.Left,
                Width = U(244),
                BackColor = B6Ui.NightCard,
                BorderColor = B6Ui.NightLine,
                Shadow = false,
                Radius = 22,
                Padding = new Padding(U(12), U(8), U(14), U(12))
            };

            TableLayoutPanel g = Grid(B6Ui.NightCard, new float[] { 100 }, 36, 22, 34, 30, 70, 2, 38, 38, 38, 38, 38, 38, 0, 46);

            Label brand = new Label
            {
                Text = "BẾP TRƯỞNG",
                Font = new Font("Palatino Linotype", 17F, FontStyle.Bold),
                ForeColor = B6Ui.Gold,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft
            };
            Put(g, brand, 0, 0);

            lblChefClock = SmallLabel(string.Empty, true, FontStyle.Bold, 10F);
            lblChefClock.ForeColor = B6Ui.Cream;
            Put(g, lblChefClock, 0, 1);

            segChefMeal = new B6Segments
            {
                Items = B6Ui.Meals,
                TrackColor = B6Ui.NightCard2,
                ThumbColor = B6Ui.Gold,
                ThumbTextColor = Color.FromArgb(48, 30, 12),
                TextColor = B6Ui.CreamMuted,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold)
            };
            segChefMeal.SelectSilently(B6Ui.MealIndexForNow());
            segChefMeal.SelectedIndexChanged += (s, e) =>
            {
                chefAutoMeal = false;
                btnAutoMeal.Text = "THEO GIỜ: TẮT";
                OnChefMealChanged();
            };
            Put(g, segChefMeal, 0, 2);

            btnAutoMeal = DarkButton("THEO GIỜ: BẬT", (s, e) =>
            {
                chefAutoMeal = !chefAutoMeal;
                btnAutoMeal.Text = chefAutoMeal ? "THEO GIỜ: BẬT" : "THEO GIỜ: TẮT";
                if (chefAutoMeal)
                {
                    segChefMeal.SelectSilently(B6Ui.MealIndexForNow());
                    OnChefMealChanged();
                }
            });
            btnAutoMeal.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btnAutoMeal.SurfaceColor = B6Ui.NightCard;
            Put(g, btnAutoMeal, 0, 3);

            lblChefNote = SmallLabel(B6Ui.MealNote(segChefMeal.SelectedText), true, FontStyle.Italic, 8.5F);
            lblChefNote.AutoEllipsis = false;
            lblChefNote.TextAlign = ContentAlignment.TopLeft;
            Put(g, lblChefNote, 0, 4);

            navButtons = new B6Button[NavTitles.Length];
            for (int i = 0; i < NavTitles.Length; i++)
            {
                int index = i;
                B6Button b = DarkButton(NavTitles[i], (s, e) => SetChefView(index, true));
                b.Radius = 15;
                b.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
                b.BorderColor = Color.Empty;
                b.BackColor = B6Ui.NightCard;
                navButtons[i] = b;
                Put(g, b, 0, 6 + i);
            }

            B6Button back = MakeButton("←  VỀ QUẦY GỌI MÓN", B6Ui.NightCard2, B6Ui.Gold, B6Ui.NightLine, B6Ui.NightLine, B6Ui.NightCard, (s, e) => ApplyMode(false, true));
            back.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            back.Text = "←  QUẦY GỌI MÓN";
            Put(g, back, 0, 13);

            rail.Controls.Add(g);
            UpdateClock();
            return rail;
        }

        private void UpdateClock()
        {
            if (lblChefClock != null)
            {
                lblChefClock.Text = DateTime.Now.ToString("dddd, dd/MM  ·  HH:mm", new CultureInfo("vi-VN"));
            }
        }

        private void OnChefMealChanged()
        {
            lblChefNote.Text = B6Ui.MealNote(segChefMeal.SelectedText);
            UpdateFortuneGuard();
            if (chefViewIndex == 0) RefreshSuggestions();
        }

        private void RefreshChefAll()
        {
            if (chefViews == null) return;
            if (chefAutoMeal) segChefMeal.SelectSilently(B6Ui.MealIndexForNow());
            lblChefNote.Text = B6Ui.MealNote(segChefMeal.SelectedText);
            UpdateFortuneGuard();
            UpdateClock();
            SetChefView(chefViewIndex < 0 ? 0 : chefViewIndex, false);
        }

        private void SetChefView(int index, bool animate)
        {
            if (fortuneStage != null && fortuneStage.IsBusy) return;

            Action change = () =>
            {
                chefViewIndex = index;
                for (int i = 0; i < chefViews.Length; i++)
                {
                    chefViews[i].Visible = i == index;
                    bool selected = i == index;
                    navButtons[i].BackColor = selected ? B6Ui.Gold : B6Ui.NightCard;
                    navButtons[i].ForeColor = selected ? Color.FromArgb(48, 30, 12) : B6Ui.Cream;
                    navButtons[i].HoverColor = selected ? B6Ui.GoldDark : B6Ui.NightCard2;
                    navButtons[i].Invalidate();
                }

                bool showDetail = index == 0 || index == 3 || index == 4;
                detail.Visible = showDetail;
                viewHost.Padding = new Padding(0, 0, showDetail ? U(12) : 0, 0);

                switch (index)
                {
                    case 0: RefreshSuggestions(); break;
                    case 1: break;
                    case 2: RefreshSavedShopsIfShown(); break;
                    case 3: RebuildMenu(true); break;
                    case 4: RefreshFavoritesView(); break;
                    case 5: RefreshStats(); break;
                }
            };

            if (animate && chefMode) fade.Run(bodyHost, change); else change();
        }

        private void ShowDetail(Dish dish)
        {
            detail.ShowDish(dish, dishImages.Get(dish, 560, 380), favoriteIds.Contains(dish.Id));
            foreach (FlowLayoutPanel flow in new[] { suggestFlow, menuFlow })
            {
                if (flow == null) continue;
                foreach (Control c in flow.Controls)
                {
                    B6DishCard card = c as B6DishCard;
                    if (card != null)
                    {
                        bool now = card.Dish.Id == dish.Id;
                        if (card.Selected != now) { card.Selected = now; card.Invalidate(); }
                    }
                }
            }
        }

        private void ToggleFavorite(Dish dish)
        {
            try
            {
                bool nowFavorite = !favoriteIds.Contains(dish.Id);
                database.SetFavorite(dish.Id, nowFavorite);
                string sql = database.LastSql;
                if (nowFavorite) favoriteIds.Add(dish.Id); else favoriteIds.Remove(dish.Id);

                foreach (FlowLayoutPanel flow in new[] { suggestFlow, menuFlow })
                {
                    if (flow == null) continue;
                    foreach (Control c in flow.Controls)
                    {
                        B6DishCard card = c as B6DishCard;
                        if (card != null && card.Dish.Id == dish.Id)
                        {
                            card.Favorite = nowFavorite;
                            card.Invalidate();
                        }
                    }
                }

                if (detail.Current != null && detail.Current.Id == dish.Id) detail.SetFavorite(nowFavorite);
                if (chefViewIndex == 4) RefreshFavoritesView();
                SetStatus((nowFavorite ? "Đã thêm vào yêu thích: " : "Đã bỏ yêu thích: ") + dish.Name, sql);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private FlowLayoutPanel NewCardFlow()
        {
            FlowLayoutPanel flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = B6Ui.Night,
                WrapContents = true,
                Padding = new Padding(0, U(4), 0, U(4))
            };
            flow.AutoScroll = false;
            flow.HorizontalScroll.Enabled = false;
            flow.HorizontalScroll.Visible = false;
            flow.HorizontalScroll.Maximum = 0;
            flow.AutoScroll = true;
            B6Ui.DarkScroll(flow);
            B6Ui.EnableDoubleBuffer(flow);
            return flow;
        }

        private void FillCards(FlowLayoutPanel flow, List<Dish> dishes)
        {
            flow.SuspendLayout();
            List<Control> old = new List<Control>();
            foreach (Control c in flow.Controls) old.Add(c);
            flow.Controls.Clear();
            foreach (Control c in old) c.Dispose();

            int order = 0;
            foreach (Dish dish in dishes)
            {
                B6DishCard card = new B6DishCard(dish, dishImages.Get(dish, 440, 300))
                {
                    Size = new Size(U(214), U(300)),
                    Margin = new Padding(U(5)),
                    Favorite = favoriteIds.Contains(dish.Id),
                    Selected = detail.Current != null && detail.Current.Id == dish.Id
                };
                Dish captured = dish;
                card.CardClicked += (s, e) => ShowDetail(captured);
                card.FavoriteClicked += (s, e) => ToggleFavorite(captured);
                flow.Controls.Add(card);
                card.StartAppear(order * 55);
                order++;
            }
            flow.ResumeLayout(true);
        }

        private void BuildSuggestView(Panel view)
        {
            TableLayoutPanel bar = Grid(B6Ui.Night, new float[] { 30, 26, 44 }, 50);
            bar.Dock = DockStyle.Top;
            bar.Height = U(54);

            cboSuggestCategory = new B6Combo(true);
            List<string> categories = new List<string> { "Mọi loại món" };
            categories.AddRange(FoodCatalog.Categories);
            cboSuggestCategory.SetItems(categories, 0);
            cboSuggestCategory.SelectedIndexChanged += (s, e) => { if (!loading && chefViewIndex == 0) RefreshSuggestions(); };
            Put(bar, cboSuggestCategory, 0, 0);

            cboSuggestPrice = new B6Combo(true);
            cboSuggestPrice.SetItems(PriceLabels, 0);
            cboSuggestPrice.SelectedIndexChanged += (s, e) => { if (!loading && chefViewIndex == 0) RefreshSuggestions(); };
            Put(bar, cboSuggestPrice, 1, 0);

            Put(bar, GoldButton("✦  GỢI Ý MỚI CHO TÔI", (s, e) => RefreshSuggestions()), 2, 0);

            suggestFlow = NewCardFlow();
            lblSuggestWarn = new Label
            {
                Dock = DockStyle.Top,
                Height = U(34),
                ForeColor = Color.FromArgb(255, 138, 112),
                BackColor = B6Ui.Night,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(U(8), 0, 0, 0),
                Visible = false
            };
            view.Controls.Add(suggestFlow);
            view.Controls.Add(lblSuggestWarn);
            view.Controls.Add(bar);
        }

        private void RefreshSuggestions()
        {
            if (suggestFlow == null || allDishes.Count == 0) return;
            string category = cboSuggestCategory.SelectedIndex > 0 ? cboSuggestCategory.SelectedItem.ToString() : string.Empty;
            int maxPrice = PriceLimits[Math.Max(0, cboSuggestPrice.SelectedIndex)];
            string meal = segChefMeal.SelectedText;

            try
            {
                string violation = BudgetViolation(category, maxPrice);
                lblSuggestWarn.Text = violation;
                lblSuggestWarn.Visible = violation.Length > 0;
                if (violation.Length > 0)
                {
                    FillCards(suggestFlow, new List<Dish>());
                    SetStatus(violation);
                    return;
                }

                List<Dish> picks = database.GetSuggestions(category, meal, maxPrice, 8);
                string relaxed = string.Empty;
                if (picks.Count == 0 && category.Length > 0)
                {
                    picks = database.GetSuggestions(category, string.Empty, maxPrice, 8);
                    relaxed = " (ngoài khung giờ " + meal.ToLowerInvariant() + ")";
                }
                FillCards(suggestFlow, picks);
                SetStatus("Gợi ý " + picks.Count + " món hợp buổi " + meal.ToLowerInvariant() + (category.Length > 0 ? " · " + category : string.Empty) + relaxed, database.LastSql);
                if (picks.Count > 0 && (detail.Current == null || !picks.Exists(d => d.Id == detail.Current.Id)))
                {
                    ShowDetail(picks[0]);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void BuildFortuneView(Panel view)
        {
            TableLayoutPanel g = Grid(B6Ui.Night, new float[] { 62, 38 }, 0);

            B6Card stageCard = NewCard(Color.Empty, true);
            stageCard.Padding = new Padding(U(6));
            fortuneStage = new B6FortuneStage { Dock = DockStyle.Fill, BackColor = B6Ui.NightCard };
            fortuneStage.Completed += FortuneStage_Completed;
            stageCard.Controls.Add(fortuneStage);
            g.Controls.Add(stageCard, 0, 0);

            B6Card panel = NewCard(Color.Empty, true);
            TableLayoutPanel p = Grid(B6Ui.NightCard, new float[] { 100 }, 36, 22, 42, 22, 42, 46, 54, 0.4f, 44, 0.22f, 44);
            Put(p, SectionTitle("Rút một quẻ lộc", true), 0, 0);
            Put(p, SmallLabel("Loại món muốn ăn", true, FontStyle.Bold, 8.5F), 0, 1);
            cboFortuneCategory = new B6Combo(true);
            List<string> options = new List<string> { "✦ Theo mệnh của quẻ", "Mọi loại món" };
            options.AddRange(FoodCatalog.Categories);
            cboFortuneCategory.SetItems(options, 0);
            cboFortuneCategory.SelectedIndexChanged += (s, e) => UpdateFortuneGuard();
            Put(p, cboFortuneCategory, 0, 2);
            Put(p, SmallLabel("Ngân sách", true, FontStyle.Bold, 8.5F), 0, 3);
            cboFortunePrice = new B6Combo(true);
            cboFortunePrice.SetItems(PriceLabels, 0);
            cboFortunePrice.SelectedIndexChanged += (s, e) => UpdateFortuneGuard();
            Put(p, cboFortunePrice, 0, 4);
            lblFortuneMeal = SmallLabel("Buổi " + segChefMeal.SelectedText.ToLowerInvariant(), true, FontStyle.Italic, 9.5F);
            lblFortuneMeal.ForeColor = B6Ui.Gold;
            lblFortuneMeal.AutoEllipsis = false;
            Put(p, lblFortuneMeal, 0, 5);
            btnFortune = GoldButton("☯  LẮC QUẺ NGAY", btnFortune_Click);
            btnFortune.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnFortune.Radius = 18;
            Put(p, btnFortune, 0, 6);

            stageFortuneDish = new B6Stage { Radius = 16 };
            stageFortuneDish.Clear("Món lộc sẽ hiện ở đây");
            Put(p, stageFortuneDish, 0, 7);
            lblFortuneDishName = new Label
            {
                Font = new Font("Palatino Linotype", 15F, FontStyle.Bold),
                ForeColor = B6Ui.Cream,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoEllipsis = true
            };
            Put(p, lblFortuneDishName, 0, 8);
            lblFortuneDishDesc = SmallLabel(string.Empty, true, FontStyle.Regular, 9.5F);
            lblFortuneDishDesc.TextAlign = ContentAlignment.TopCenter;
            lblFortuneDishDesc.AutoEllipsis = false;
            Put(p, lblFortuneDishDesc, 0, 9);
            btnFortuneNearby = DarkButton("TÌM QUÁN GẦN ĐÂY", (s, e) => { if (fortuneDish != null) GoNearby(fortuneDish); });
            btnFortuneNearby.Enabled = false;
            Put(p, btnFortuneNearby, 0, 10);
            panel.Controls.Add(p);
            g.Controls.Add(panel, 1, 0);

            view.Controls.Add(g);
        }

        private void btnFortune_Click(object sender, EventArgs e)
        {
            if (fortuneStage.IsBusy) return;

            B6Fortune card = B6Fortunes.Draw();
            string meal = segChefMeal.SelectedText;
            int maxPrice = PriceLimits[Math.Max(0, cboFortunePrice.SelectedIndex)];
            string category;
            if (cboFortuneCategory.SelectedIndex == 0) category = card.PreferredCategory;
            else if (cboFortuneCategory.SelectedIndex == 1) category = string.Empty;
            else category = cboFortuneCategory.SelectedItem.ToString();

            string violation = FortuneViolation();
            if (violation.Length > 0)
            {
                UpdateFortuneGuard();
                ShowWarning(violation);
                return;
            }

            try
            {
                bool strict = cboFortuneCategory.SelectedIndex >= 2;
                string note;
                Dish dish = PickFortuneDish(category, meal, maxPrice, strict, out note);
                fortuneNote = note;
                if (dish != null && maxPrice > 0 && (dish.Price <= 0 || dish.Price > maxPrice))
                {
                    dish = null;
                }
                if (dish == null)
                {
                    ShowInfo("Không có món nào trong mức giá đã chọn. Hãy chọn mức giá cao hơn nhé.");
                    return;
                }

                fortuneDish = dish;
                btnFortune.Enabled = false;
                btnFortuneNearby.Enabled = false;
                stageFortuneDish.Clear("Đang xin quẻ…");
                lblFortuneDishName.Text = string.Empty;
                lblFortuneDishDesc.Text = string.Empty;
                fortuneStage.Begin(card, dish.Name);
                SetStatus("Đang lắc ống xăm… " + (cboFortuneCategory.SelectedIndex == 0 ? "(quẻ " + card.ElementText + " → " + card.PreferredCategory + ")" : string.Empty));
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private string fortuneNote = string.Empty;
        private bool fortuneBlocked;
        private Label lblSuggestWarn;

        private string BudgetViolation(string category, int maxPrice)
        {
            if (maxPrice <= 0) return string.Empty;
            try
            {
                int cheapest = database.GetMinPrice(category, string.Empty);
                if (cheapest <= 0)
                {
                    return category.Length > 0 ? "Nhóm \"" + category + "\" chưa có món nào có giá để chọn." : string.Empty;
                }
                if (cheapest <= maxPrice) return string.Empty;

                string target = PriceLabels[PriceLabels.Length - 1];
                for (int i = 1; i < PriceLimits.Length; i++)
                {
                    if (PriceLimits[i] >= cheapest)
                    {
                        target = PriceLabels[i];
                        break;
                    }
                }

                string scope = category.Length > 0 ? "nhóm \"" + category + "\"" : "thực đơn";
                return "Mức giá không hợp lệ: " + scope + " rẻ nhất " + B6Ui.FormatPrice(cheapest) + ". Hãy chọn từ \"" + target + "\" trở lên.";
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private string FortuneViolation()
        {
            int maxPrice = PriceLimits[Math.Max(0, cboFortunePrice.SelectedIndex)];
            string category = cboFortuneCategory.SelectedIndex >= 2 ? cboFortuneCategory.SelectedItem.ToString() : string.Empty;
            return BudgetViolation(category, maxPrice);
        }

        private void UpdateFortuneGuard()
        {
            if (lblFortuneMeal == null || cboFortunePrice == null || cboFortuneCategory == null || segChefMeal == null) return;
            string violation = FortuneViolation();
            fortuneBlocked = violation.Length > 0;
            lblFortuneMeal.ForeColor = fortuneBlocked ? Color.FromArgb(255, 138, 112) : B6Ui.Gold;
            lblFortuneMeal.Text = fortuneBlocked ? violation : "Buổi " + segChefMeal.SelectedText.ToLowerInvariant();
        }

        private Dish PickFortuneDish(string category, string meal, int maxPrice, bool strict, out string note)
        {
            note = string.Empty;
            Dish dish = database.GetRandomDish(category, meal, maxPrice);
            if (dish != null) return dish;

            if (category.Length > 0 && strict)
            {
                dish = database.GetRandomDish(category, string.Empty, maxPrice);
                if (dish != null) note = "Ngoài khung giờ " + meal.ToLowerInvariant() + " nhưng đúng ý bạn chọn";
                return dish;
            }

            dish = database.GetRandomDish(string.Empty, meal, maxPrice);
            if (dish != null)
            {
                if (category.Length > 0) note = "Quẻ nghiêng về \"" + category + "\" nhưng chưa có món hợp mức giá này";
                return dish;
            }

            dish = database.GetRandomDish(string.Empty, string.Empty, maxPrice);
            if (dish != null) note = "Ít món hợp buổi " + meal.ToLowerInvariant() + " trong mức giá này";
            return dish;
        }

        private void FortuneStage_Completed(object sender, EventArgs e)
        {
            btnFortune.Enabled = true;
            if (fortuneDish == null) return;

            stageFortuneDish.Reveal(dishImages.Get(fortuneDish, 480, 360), string.Empty, true);
            lblFortuneDishName.Text = fortuneDish.Name;
            lblFortuneDishDesc.Text = (B6Ui.FormatPrice(fortuneDish.Price) + " · " + fortuneDish.Region + "\n" + fortuneDish.Description).Trim();
            if (fortuneNote.Length > 0)
            {
                lblFortuneMeal.ForeColor = B6Ui.Gold;
                lblFortuneMeal.Text = fortuneNote;
                SetStatus(fortuneNote);
            }
            else
            {
                UpdateFortuneGuard();
            }
            btnFortuneNearby.Enabled = true;
            RememberPick(fortuneDish, "Lắc quẻ");
        }

        private B6Input txtNearQuery;
        private B6Combo cboRadius;
        private B6Segments segNearSource;
        private ListView lvPlaces;
        private Label lblLocation;
        private B6Button btnLocate;
        private B6Button btnFindNearby;
        private B6Geo myLocation;
        private List<B6Place> foundPlaces = new List<B6Place>();
        private List<ShopInfo> savedShops = new List<ShopInfo>();
        private Dish nearbyDish;
        private bool nearbyBusy;

        private static readonly int[] RadiusMeters = { 1000, 2000, 3000, 5000, 10000, 20000, 30000, 50000 };
        private static readonly string[] RadiusLabels =
        {
            "Bán kính 1 km", "Bán kính 2 km", "Bán kính 3 km", "Bán kính 5 km",
            "Bán kính 10 km", "Bán kính 20 km", "Bán kính 30 km", "Bán kính 50 km"
        };

        private void BuildNearbyView(Panel view)
        {
            B6Card card = NewCard(Color.Empty, true);
            TableLayoutPanel g = Grid(B6Ui.NightCard, new float[] { 21, 17, 33, 29 }, 36, 46, 26, 40, 0, 46, 24);
            Put(g, SectionTitle("Quán ăn gần bạn", true), 0, 0, 4);

            btnLocate = DarkButton("⌖  ĐỊNH VỊ TÔI", (s, e) => LocateClicked());
            Put(g, btnLocate, 0, 1);
            cboRadius = new B6Combo(true);
            cboRadius.SetItems(RadiusLabels, 3);
            Put(g, cboRadius, 1, 1);
            txtNearQuery = new B6Input(true) { PlaceholderText = "Nhập món cần tìm quán (vd: bánh tằm cay)" };
            Put(g, txtNearQuery, 2, 1);
            btnFindNearby = GoldButton("TÌM ĐỊA CHỈ QUÁN ĂN", (s, e) => FindNearbyClicked());
            Put(g, btnFindNearby, 3, 1);

            lblLocation = SmallLabel("Chưa định vị. Bấm \"ĐỊNH VỊ TÔI\" để dùng GPS / Wi-Fi của Windows.", true, FontStyle.Italic, 9.5F);
            Put(g, lblLocation, 0, 2, 4);

            segNearSource = new B6Segments
            {
                Items = new[] { "Kết quả tìm kiếm", "Quán đã lưu" },
                TrackColor = B6Ui.NightCard2,
                ThumbColor = B6Ui.Gold,
                ThumbTextColor = Color.FromArgb(48, 30, 12),
                TextColor = B6Ui.CreamMuted
            };
            segNearSource.SelectedIndexChanged += (s, e) => FillPlaceList();
            Put(g, segNearSource, 0, 3, 2);

            lvPlaces = NewList(true, "Tên quán", "Loại", "Cách bạn", "Địa chỉ");
            lvPlaces.SizeChanged += (s, e) => FitColumns(lvPlaces, 3.2f, 2f, 1.1f, 3.6f);
            lvPlaces.DoubleClick += (s, e) => OpenSelectedPlace(false);
            Put(g, lvPlaces, 0, 4, 4);

            TableLayoutPanel actions = Grid(B6Ui.NightCard, new float[] { 28, 24, 24, 24 }, 0);
            Put(actions, GoldButton("↗  MỞ GOOGLE MAPS", (s, e) => OpenSelectedPlace(false)), 0, 0);
            Put(actions, DarkButton("CHỈ ĐƯỜNG", (s, e) => OpenSelectedPlace(true)), 1, 0);
            Put(actions, DarkButton("♡  LƯU QUÁN", (s, e) => SaveSelectedPlace()), 2, 0);
            Put(actions, DarkButton("XÓA QUÁN ĐÃ LƯU", (s, e) => DeleteSavedShop()), 3, 0);
            Put(g, actions, 0, 5, 4);

            Label note = SmallLabel("MỞ GOOGLE MAPS: mở quán đang chọn (hoặc tìm món này quanh bạn) trên Google Maps bằng trình duyệt  ·  CHỈ ĐƯỜNG: vẽ lộ trình từ vị trí của bạn.", true, FontStyle.Italic, 8.5F);
            Put(g, note, 0, 6, 4);

            card.Controls.Add(g);
            view.Controls.Add(card);
        }

        private void GoNearby(Dish dish)
        {
            nearbyDish = dish;
            SetChefView(2, true);
            txtNearQuery.Text = dish.Name;
            SetStatus("Tìm quán bán \"" + dish.Name + "\" — bấm TÌM ĐỊA CHỈ QUÁN ĂN (hoặc MỞ GOOGLE MAPS)");
            if (myLocation != null) FindNearbyClicked();
        }

        private async void LocateClicked()
        {
            if (nearbyBusy) return;
            nearbyBusy = true;
            btnLocate.Enabled = false;
            lblLocation.Text = "Đang xác định vị trí… (có thể mất vài giây)";
            try
            {
                B6Geo point = await B6Locator.LocateAsync();
                if (IsDisposed) return;
                if (point == null)
                {
                    myLocation = null;
                    lblLocation.Text = "Không xác định được vị trí. Hãy bật Dịch vụ vị trí của Windows, hoặc dùng nút MỞ GOOGLE MAPS.";
                    return;
                }

                myLocation = point;
                lblLocation.Text = point.Source + string.Format(CultureInfo.InvariantCulture, " · {0:0.0000}, {1:0.0000}", point.Latitude, point.Longitude);
                string address = await B6Locator.ReverseGeocodeAsync(point);
                if (!IsDisposed && !string.IsNullOrEmpty(address))
                {
                    lblLocation.Text = "📍 " + address;
                }
                SetStatus("Đã định vị: " + point.Source);
            }
            finally
            {
                nearbyBusy = false;
                if (!IsDisposed) btnLocate.Enabled = true;
            }
        }

        private async void FindNearbyClicked()
        {
            if (nearbyBusy) return;

            string query = txtNearQuery.Text.Trim();
            if (query.Length == 0 && nearbyDish != null)
            {
                query = nearbyDish.Name;
                txtNearQuery.Text = query;
            }
            if (query.Length == 0)
            {
                ShowInfo("Hãy nhập tên món cần tìm quán (ví dụ: bánh tằm cay, lẩu Thái, phở bò).");
                txtNearQuery.Inner.Focus();
                return;
            }

            if (myLocation == null)
            {
                LocateClicked();
                while (nearbyBusy) await System.Threading.Tasks.Task.Delay(150);
                if (myLocation == null) return;
            }

            try
            {
                Dish matched = database.FindDishByName(query);
                if (matched != null) nearbyDish = matched;
            }
            catch (Exception)
            {
            }

            int radius = RadiusMeters[Math.Max(0, Math.Min(RadiusMeters.Length - 1, cboRadius.SelectedIndex))];
            nearbyBusy = true;
            btnFindNearby.Enabled = false;
            string previous = lblLocation.Text;
            lblLocation.Text = "Đang tìm quán bán \"" + query + "\" trong bán kính " + (radius / 1000) + " km… (có thể mất tới 1 phút)";
            try
            {
                foundPlaces = await B6Locator.FindDishPlacesAsync(myLocation, radius, query);
                if (IsDisposed) return;
                segNearSource.SelectSilently(0);
                FillPlaceList();
                lblLocation.Text = previous;
                SetStatus("Tìm thấy " + foundPlaces.Count + " quán có tên liên quan \"" + query + "\" (dữ liệu OpenStreetMap)");
                if (foundPlaces.Count == 0)
                {
                    ShowInfo("Không thấy quán nào có tên liên quan \"" + query + "\" trong bán kính " + (radius / 1000) + " km theo dữ liệu OpenStreetMap.\n\nDữ liệu này chưa đầy đủ — hãy tăng bán kính hoặc bấm MỞ GOOGLE MAPS để tìm trên Google.");
                }
            }
            catch (Exception ex)
            {
                lblLocation.Text = previous;
                ShowWarning("Không tải được danh sách quán (kiểm tra mạng Internet).\n" + ex.Message + "\n\nBạn vẫn có thể bấm MỞ GOOGLE MAPS để tìm trực tiếp.");
            }
            finally
            {
                nearbyBusy = false;
                if (!IsDisposed) btnFindNearby.Enabled = true;
            }
        }

        private void FillPlaceList()
        {
            if (lvPlaces == null) return;
            lvPlaces.BeginUpdate();
            lvPlaces.Items.Clear();
            if (segNearSource.SelectedIndex == 0)
            {
                foreach (B6Place place in foundPlaces)
                {
                    ListViewItem item = new ListViewItem(place.Name) { Tag = place };
                    item.SubItems.Add(string.IsNullOrEmpty(place.Cuisine) ? place.Kind : place.Kind + " · " + place.Cuisine);
                    item.SubItems.Add(place.DistanceText);
                    item.SubItems.Add(place.Address);
                    lvPlaces.Items.Add(item);
                }
            }
            else
            {
                try
                {
                    savedShops = database.GetShops();
                }
                catch (Exception ex)
                {
                    ShowError(ex);
                }

                foreach (ShopInfo shop in savedShops)
                {
                    ListViewItem item = new ListViewItem(shop.Name) { Tag = shop };
                    item.SubItems.Add(string.IsNullOrEmpty(shop.DishName) ? "Quán đã lưu" : shop.DishName);
                    item.SubItems.Add("—");
                    item.SubItems.Add(shop.Address);
                    lvPlaces.Items.Add(item);
                }
            }
            lvPlaces.EndUpdate();
            FitColumns(lvPlaces, 3.2f, 2f, 1.1f, 3.6f);
        }

        private void RefreshSavedShopsIfShown()
        {
            if (segNearSource != null && segNearSource.SelectedIndex == 1) FillPlaceList();
        }

        private void OpenSelectedPlace(bool directions)
        {
            if (lvPlaces.SelectedItems.Count == 0)
            {
                if (directions)
                {
                    ShowInfo("Hãy chọn một quán trong danh sách để chỉ đường.");
                }
                else
                {
                    OpenMapsSearch();
                }
                return;
            }

            object tag = lvPlaces.SelectedItems[0].Tag;
            B6Place place = tag as B6Place;
            if (place != null)
            {
                B6Ui.OpenUrl(directions ? B6Locator.MapsDirectionsUrl(place, myLocation) : B6Locator.MapsSearchUrl(place.Name, new B6Geo { Latitude = place.Latitude, Longitude = place.Longitude }));
                return;
            }

            ShopInfo shop = tag as ShopInfo;
            if (shop != null)
            {
                string full = shop.Name + " " + shop.Address;
                B6Ui.OpenUrl(directions ? B6Locator.MapsDirectionsToAddress(full) : B6Locator.MapsSearchUrl(full, null));
            }
        }

        private void SaveSelectedPlace()
        {
            if (lvPlaces.SelectedItems.Count == 0 || !(lvPlaces.SelectedItems[0].Tag is B6Place))
            {
                ShowInfo("Hãy chọn một quán ở danh sách \"Quanh đây\" để lưu.");
                return;
            }

            B6Place place = (B6Place)lvPlaces.SelectedItems[0].Tag;
            try
            {
                string address = string.IsNullOrEmpty(place.Address)
                    ? place.Latitude.ToString("0.00000", CultureInfo.InvariantCulture) + ", " + place.Longitude.ToString("0.00000", CultureInfo.InvariantCulture)
                    : place.Address;
                database.AddShop(place.Name, address, nearbyDish != null ? nearbyDish.Id : 0, place.Kind + " · cách " + place.DistanceText);
                SetStatus("Đã lưu quán \"" + place.Name + "\" vào SQLite", database.LastSql);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void DeleteSavedShop()
        {
            if (segNearSource.SelectedIndex != 1 || lvPlaces.SelectedItems.Count == 0 || !(lvPlaces.SelectedItems[0].Tag is ShopInfo))
            {
                ShowInfo("Chuyển sang \"Quán đã lưu\" và chọn quán cần xóa.");
                return;
            }

            ShopInfo shop = (ShopInfo)lvPlaces.SelectedItems[0].Tag;
            try
            {
                database.DeleteShop(shop.Id);
                SetStatus("Đã xóa quán \"" + shop.Name + "\"", database.LastSql);
                FillPlaceList();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void OpenMapsSearch()
        {
            string query = txtNearQuery.Text.Trim();
            if (query.Length == 0) query = "quán ăn";
            OpenMapsQuery(query);
        }

        private void OpenMapsForDish(Dish dish)
        {
            OpenMapsQuery(dish.Name);
        }

        private void OpenMapsQuery(string query)
        {
            if (myLocation != null)
            {
                B6Ui.OpenUrl(B6Locator.MapsSearchUrl(query, myLocation));
            }
            else
            {

                B6Ui.OpenUrl("https://www.google.com/maps/search/?api=1&query=" + Uri.EscapeDataString(query + " gần tôi"));
            }
        }

        private B6Input txtMenuSearch;
        private B6Combo cboMenuCategory;
        private B6Combo cboMenuMeal;
        private B6Combo cboMenuPrice;
        private FlowLayoutPanel menuFlow;
        private Label lblMenuPage;
        private List<Dish> menuFiltered = new List<Dish>();
        private int menuPage;

        private void BuildMenuView(Panel view)
        {
            TableLayoutPanel bar = Grid(B6Ui.Night, new float[] { 34, 22, 20, 24 }, 50);
            bar.Dock = DockStyle.Top;
            bar.Height = U(54);

            txtMenuSearch = new B6Input(true) { PlaceholderText = "Tìm trong 1.000+ món (không cần dấu)" };
            txtMenuSearch.TextChanged += (s, e) => { if (!loading && chefViewIndex == 3) RebuildMenu(true); };
            Put(bar, txtMenuSearch, 0, 0);

            cboMenuCategory = new B6Combo(true);
            List<string> cats = new List<string> { "Mọi loại món" };
            cats.AddRange(FoodCatalog.Categories);
            cboMenuCategory.SetItems(cats, 0);
            cboMenuCategory.SelectedIndexChanged += (s, e) => { if (!loading && chefViewIndex == 3) RebuildMenu(true); };
            Put(bar, cboMenuCategory, 1, 0);

            cboMenuMeal = new B6Combo(true);
            cboMenuMeal.SetItems(new[] { "Mọi buổi", "Sáng", "Trưa", "Chiều", "Tối" }, 0);
            cboMenuMeal.SelectedIndexChanged += (s, e) => { if (!loading && chefViewIndex == 3) RebuildMenu(true); };
            Put(bar, cboMenuMeal, 2, 0);

            cboMenuPrice = new B6Combo(true);
            cboMenuPrice.SetItems(PriceLabels, 0);
            cboMenuPrice.SelectedIndexChanged += (s, e) => { if (!loading && chefViewIndex == 3) RebuildMenu(true); };
            Put(bar, cboMenuPrice, 3, 0);

            TableLayoutPanel pager = Grid(B6Ui.Night, new float[] { 22, 56, 22 }, 46);
            pager.Dock = DockStyle.Bottom;
            pager.Height = U(50);
            Put(pager, DarkButton("◀  TRANG TRƯỚC", (s, e) => { if (menuPage > 0) { menuPage--; ShowMenuPage(); } }), 0, 0);
            lblMenuPage = SmallLabel(string.Empty, true, FontStyle.Bold, 10F);
            lblMenuPage.TextAlign = ContentAlignment.MiddleCenter;
            lblMenuPage.ForeColor = B6Ui.Cream;
            Put(pager, lblMenuPage, 1, 0);
            Put(pager, DarkButton("TRANG SAU  ▶", (s, e) => { if ((menuPage + 1) * MenuPageSize < menuFiltered.Count) { menuPage++; ShowMenuPage(); } }), 2, 0);

            menuFlow = NewCardFlow();
            view.Controls.Add(menuFlow);
            view.Controls.Add(pager);
            view.Controls.Add(bar);
        }

        private static List<Dish> RankByKeyword(List<Dish> source, string keyword)
        {
            string[] tokens = keyword.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<KeyValuePair<int, Dish>> scored = new List<KeyValuePair<int, Dish>>();
            foreach (Dish dish in source)
            {
                string name = B6Ui.Fold(dish.Name);
                string extra = B6Ui.Fold(dish.Category + " " + dish.Region + " " + dish.Description);
                int score = 0;
                bool matchesAll = true;
                foreach (string token in tokens)
                {
                    if (name.StartsWith(token, StringComparison.Ordinal) || name.Contains(" " + token)) score += 30;
                    else if (name.Contains(token)) score += 18;
                    else if (extra.Contains(token)) score += 3;
                    else
                    {
                        matchesAll = false;
                        break;
                    }
                }

                if (!matchesAll) continue;
                if (name == keyword) score += 200;
                else if (name.StartsWith(keyword, StringComparison.Ordinal)) score += 120;
                else if (name.Contains(keyword)) score += 80;
                score -= name.Length / 8;
                scored.Add(new KeyValuePair<int, Dish>(score, dish));
            }

            scored.Sort((a, b) => b.Key.CompareTo(a.Key));
            return scored.ConvertAll(pair => pair.Value);
        }

        private void RebuildMenu(bool resetPage)
        {
            if (menuFlow == null) return;
            string keyword = B6Ui.Fold(txtMenuSearch.Text.Trim());
            string category = cboMenuCategory.SelectedIndex > 0 ? cboMenuCategory.SelectedItem.ToString() : string.Empty;
            string meal = cboMenuMeal.SelectedIndex > 0 ? cboMenuMeal.SelectedItem.ToString() : string.Empty;
            int maxPrice = PriceLimits[Math.Max(0, cboMenuPrice.SelectedIndex)];

            menuFiltered = allDishes.FindAll(d =>
                (category.Length == 0 || d.Category == category) &&
                (meal.Length == 0 || d.SuitsMeal(meal)) &&
                (maxPrice == 0 || (d.Price > 0 && d.Price <= maxPrice)));
            if (keyword.Length > 0)
            {
                menuFiltered = RankByKeyword(menuFiltered, keyword);
            }
            if (resetPage) menuPage = 0;
            ShowMenuPage();
        }

        private void ShowMenuPage()
        {
            int pages = Math.Max(1, (menuFiltered.Count + MenuPageSize - 1) / MenuPageSize);
            menuPage = Math.Max(0, Math.Min(menuPage, pages - 1));
            List<Dish> slice = menuFiltered.GetRange(menuPage * MenuPageSize, Math.Min(MenuPageSize, menuFiltered.Count - menuPage * MenuPageSize));
            FillCards(menuFlow, slice);
            int nextStart = (menuPage + 1) * MenuPageSize;
            if (nextStart < menuFiltered.Count)
            {
                dishImages.Prefetch(menuFiltered.GetRange(nextStart, Math.Min(MenuPageSize * 2, menuFiltered.Count - nextStart)), 440, 300);
            }
            lblMenuPage.Text = "Trang " + (menuPage + 1) + "/" + pages + "  ·  " + menuFiltered.Count + " món phù hợp";
            if (slice.Count > 0 && (detail.Current == null || !slice.Exists(d => d.Id == detail.Current.Id)))
            {
                ShowDetail(slice[0]);
            }
        }

        private ListView lvFavorites;
        private ListView lvHistory;

        private void BuildFavoritesView(Panel view)
        {
            TableLayoutPanel g = Grid(B6Ui.Night, new float[] { 50, 50 }, 0);

            B6Card favCard = NewCard(Color.Empty, true);
            TableLayoutPanel fg = Grid(B6Ui.NightCard, new float[] { 50, 50 }, 36, 0, 46);
            Put(fg, SectionTitle("Món yêu thích", true), 0, 0, 2);
            lvFavorites = NewList(true, "Tên món", "Loại", "Giá");
            lvFavorites.SizeChanged += (s, e) => FitColumns(lvFavorites, 4f, 2f, 1.5f);
            lvFavorites.SelectedIndexChanged += (s, e) =>
            {
                if (lvFavorites.SelectedItems.Count > 0) ShowDetail((Dish)lvFavorites.SelectedItems[0].Tag);
            };
            Put(fg, lvFavorites, 0, 1, 2);
            Put(fg, GoldButton("✦  CHỌN NGẪU NHIÊN", (s, e) => PickRandomFavorite()), 0, 2);
            Put(fg, DarkButton("BỎ YÊU THÍCH", (s, e) =>
            {
                if (lvFavorites.SelectedItems.Count > 0) ToggleFavorite((Dish)lvFavorites.SelectedItems[0].Tag);
                else ShowInfo("Hãy chọn một món trong danh sách yêu thích.");
            }), 1, 2);
            favCard.Controls.Add(fg);
            g.Controls.Add(favCard, 0, 0);

            B6Card histCard = NewCard(Color.Empty, true);
            TableLayoutPanel hg = Grid(B6Ui.NightCard, new float[] { 100 }, 36, 0, 46);
            Put(hg, SectionTitle("Lịch sử quay & lắc quẻ", true), 0, 0);
            lvHistory = NewList(true, "Thời gian", "Món ăn", "Nguồn");
            lvHistory.SizeChanged += (s, e) => FitColumns(lvHistory, 2.6f, 4f, 1.6f);
            Put(hg, lvHistory, 0, 1);
            Put(hg, DarkButton("XÓA LỊCH SỬ", (s, e) =>
            {
                if (MessageBox.Show(this, "Xóa toàn bộ lịch sử quay món?", MessageTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                database.ClearHistory();
                SetStatus("Đã xóa lịch sử", database.LastSql);
                RefreshFavoritesView();
            }), 0, 2);
            histCard.Controls.Add(hg);
            g.Controls.Add(histCard, 1, 0);

            view.Controls.Add(g);
        }

        private void RefreshFavoritesView()
        {
            if (lvFavorites == null) return;
            try
            {
                lvFavorites.BeginUpdate();
                lvFavorites.Items.Clear();
                foreach (Dish dish in database.GetFavorites())
                {
                    ListViewItem item = new ListViewItem(dish.Name) { Tag = dish };
                    item.SubItems.Add(dish.Category);
                    item.SubItems.Add(B6Ui.FormatPrice(dish.Price));
                    lvFavorites.Items.Add(item);
                }
                lvFavorites.EndUpdate();

                lvHistory.BeginUpdate();
                lvHistory.Items.Clear();
                foreach (HistoryEntry entry in database.GetHistory(100))
                {
                    ListViewItem item = new ListViewItem(entry.Time);
                    item.SubItems.Add(entry.DishName);
                    item.SubItems.Add(entry.Source);
                    lvHistory.Items.Add(item);
                }
                lvHistory.EndUpdate();
                FitColumns(lvFavorites, 4f, 2f, 1.5f);
                FitColumns(lvHistory, 2.6f, 4f, 1.6f);

                if (lvFavorites.Items.Count == 0)
                {
                    detail.ShowEmpty();
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void PickRandomFavorite()
        {
            if (lvFavorites.Items.Count == 0)
            {
                ShowInfo("Bạn chưa có món yêu thích nào. Bấm biểu tượng trái tim trên thẻ món để thêm.");
                return;
            }
            ListViewItem item = lvFavorites.Items[random.Next(lvFavorites.Items.Count)];
            item.Selected = true;
            item.EnsureVisible();
            Dish dish = (Dish)item.Tag;
            ShowDetail(dish);
            RememberPick(dish, "Yêu thích");
        }

        private Label[] statNumbers;
        private B6Bars chartContributors;
        private B6Bars chartCategories;
        private B6Bars chartMeals;

        private void BuildStatsView(Panel view)
        {
            TableLayoutPanel g = Grid(B6Ui.Night, new float[] { 25, 25, 25, 25 }, 128, 0);
            string[] captions = { "MÓN ĂN", "NGƯỜI ĐÓNG GÓP", "YÊU THÍCH", "LẦN QUAY / LẮC" };
            statNumbers = new Label[4];
            for (int i = 0; i < 4; i++)
            {
                B6Card tile = NewCard(Color.Empty, true);
                TableLayoutPanel tg = Grid(B6Ui.NightCard, new float[] { 100 }, 0, 30);
                statNumbers[i] = new Label
                {
                    Font = new Font("Palatino Linotype", 22F, FontStyle.Bold),
                    ForeColor = B6Ui.Gold,
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.BottomLeft
                };
                Put(tg, statNumbers[i], 0, 0);
                Put(tg, SmallLabel(captions[i], true, FontStyle.Bold, 8.5F), 0, 1);
                tile.Controls.Add(tg);
                g.Controls.Add(tile, i, 0);
            }

            TableLayoutPanel charts = Grid(B6Ui.Night, new float[] { 40, 30, 30 }, 0);
            chartContributors = new B6Bars { Dock = DockStyle.Fill, BackColor = B6Ui.NightCard };
            chartCategories = new B6Bars { Dock = DockStyle.Fill, BackColor = B6Ui.NightCard, BarColor = B6Ui.Chili };
            chartMeals = new B6Bars { Dock = DockStyle.Fill, BackColor = B6Ui.NightCard, BarColor = B6Ui.Basil };
            string[] titles = { "Người đóng góp nhiều món nhất", "Số món theo loại", "Số món theo buổi" };
            B6Bars[] bars = { chartContributors, chartCategories, chartMeals };
            for (int i = 0; i < 3; i++)
            {
                B6Card card = NewCard(Color.Empty, true);
                TableLayoutPanel cg = Grid(B6Ui.NightCard, new float[] { 100 }, 36, 0);
                Put(cg, SectionTitle(titles[i], true), 0, 0);
                Put(cg, bars[i], 0, 1);
                card.Controls.Add(cg);
                charts.Controls.Add(card, i, 0);
            }
            g.Controls.Add(charts, 0, 1);
            g.SetColumnSpan(charts, 4);
            view.Controls.Add(g);
        }

        private void RefreshStats()
        {
            if (statNumbers == null) return;
            try
            {
                statNumbers[0].Text = allDishes.Count.ToString();
                statNumbers[1].Text = PeopleCount().ToString();
                statNumbers[2].Text = favoriteIds.Count.ToString();
                statNumbers[3].Text = database.GetHistory(100000).Count.ToString();

                List<KeyValuePair<string, int>> top = new List<KeyValuePair<string, int>>();
                foreach (ContributorStat stat in database.GetContributorStats())
                {
                    if (stat.DishCount > 0 && top.Count < 9) top.Add(new KeyValuePair<string, int>(stat.Name, stat.DishCount));
                }
                chartContributors.SetData(top);

                List<KeyValuePair<string, int>> cats = new List<KeyValuePair<string, int>>();
                foreach (CategoryStat stat in database.GetCategoryStats()) cats.Add(new KeyValuePair<string, int>(stat.Category, stat.DishCount));
                chartCategories.SetData(cats);

                List<KeyValuePair<string, int>> meals = new List<KeyValuePair<string, int>>();
                foreach (string meal in B6Ui.Meals) meals.Add(new KeyValuePair<string, int>(meal, database.CountDishesByMeal(meal)));
                chartMeals.SetData(meals);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void ImportClicked()
        {
            string defaultPath = Path.Combine(AppContext.BaseDirectory, Input6FileName);
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Nhập dữ liệu Bài 06 từ input6.txt";
                dialog.Filter = "Dữ liệu Bài 06|input6.txt;*.txt|Tất cả tệp|*.*";
                dialog.FileName = File.Exists(defaultPath) ? Input6FileName : string.Empty;
                if (File.Exists(defaultPath))
                {
                    dialog.InitialDirectory = Path.GetDirectoryName(defaultPath);
                }

                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    ImportInput6File(dialog.FileName);
                }
                catch (Exception ex)
                {
                    ShowError(ex);
                }
            }
        }

        private void ImportInput6File(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Không tìm thấy file input6.txt.", filePath);
            }

            string[] lines = File.ReadAllLines(filePath, new UTF8Encoding(false));
            string section = string.Empty;
            Dictionary<int, int> contributorMap = new Dictionary<int, int>();
            List<Contributor> currentContributors = database.GetContributors();
            int addedContributors = 0;
            int addedDishes = 0;
            int skippedDishes = 0;

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();
                if (line.Length == 0 || line.StartsWith("#", StringComparison.Ordinal)) continue;

                if (line.Equals("[NguoiDung]", StringComparison.OrdinalIgnoreCase)) { section = "NguoiDung"; continue; }
                if (line.Equals("[MonAn]", StringComparison.OrdinalIgnoreCase)) { section = "MonAn"; continue; }

                string[] fields = line.Split('|');
                if (section == "NguoiDung")
                {
                    if (fields.Length != 3 || !int.TryParse(fields[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int sourceId))
                    {
                        throw new FormatException("Dòng NguoiDung không hợp lệ: " + line);
                    }

                    string fullName = fields[1].Trim();
                    string role = fields[2].Trim();
                    if (fullName.Length == 0 || role.Length == 0)
                    {
                        throw new FormatException("Dòng NguoiDung thiếu dữ liệu: " + line);
                    }
                    if (Array.IndexOf(Roles.Assignable, role) < 0)
                    {
                        role = Roles.Member;
                    }

                    Contributor existing = currentContributors.Find(c => string.Equals(c.FullName, fullName, StringComparison.OrdinalIgnoreCase));
                    int databaseId;
                    if (existing != null)
                    {
                        databaseId = existing.Id;
                    }
                    else
                    {
                        databaseId = database.AddContributor(fullName, role);
                        currentContributors.Add(new Contributor { Id = databaseId, FullName = fullName, Role = role });
                        addedContributors++;
                    }
                    contributorMap[sourceId] = databaseId;
                    continue;
                }

                if (section == "MonAn")
                {
                    if (fields.Length != 9 || !int.TryParse(fields[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int sourceDishId))
                    {
                        throw new FormatException("Dòng MonAn không hợp lệ: " + line);
                    }

                    string dishName = fields[1].Trim();
                    string imageValue = fields[2].Trim();
                    if (!int.TryParse(fields[3].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int contributorSourceId) ||
                        !contributorMap.TryGetValue(contributorSourceId, out int contributorId))
                    {
                        throw new FormatException("Không tìm thấy người đóng góp cho món có mã " + sourceDishId + ".");
                    }

                    string category = fields[4].Trim();
                    string meals = fields[5].Trim();
                    if (!int.TryParse(fields[6].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int price) || price < 0)
                    {
                        throw new FormatException("Giá món không hợp lệ: " + line);
                    }

                    string region = fields[7].Trim();
                    string description = fields[8].Trim();
                    if (dishName.Length == 0 || category.Length == 0 || meals.Length == 0)
                    {
                        throw new FormatException("Dòng MonAn thiếu dữ liệu: " + line);
                    }

                    if (database.FindDishByName(dishName) != null)
                    {
                        skippedDishes++;
                        continue;
                    }

                    string storedImagePath = string.Empty;
                    if (imageValue.Length > 0 && !imageValue.Equals("AUTO", StringComparison.OrdinalIgnoreCase))
                    {
                        string sourceImagePath = Path.IsPathRooted(imageValue) ? imageValue : Path.Combine(Path.GetDirectoryName(filePath), imageValue);
                        if (File.Exists(sourceImagePath))
                        {
                            string extension = Path.GetExtension(sourceImagePath);
                            if (string.IsNullOrWhiteSpace(extension)) extension = ".png";
                            string safeName = "import_" + sourceDishId.ToString(CultureInfo.InvariantCulture) + "_" + Guid.NewGuid().ToString("N") + extension;
                            string destination = Path.Combine(database.ImageDirectory, safeName);
                            using (Image source = FoodImageFactory.LoadCopy(sourceImagePath))
                            {
                                if (source != null)
                                {
                                    FoodImageFactory.SaveScaledCopy(source, destination, 960, 720);
                                    storedImagePath = database.RelativeImagePath(safeName);
                                }
                            }
                        }
                    }

                    if (storedImagePath.Length == 0)
                    {
                        string safeName = "input6_" + sourceDishId.ToString(CultureInfo.InvariantCulture) + ".png";
                        FoodImageFactory.SaveGenerated(dishName, FoodImageFactory.StableSeed(dishName), Path.Combine(database.ImageDirectory, safeName), category);
                        storedImagePath = database.RelativeImagePath(safeName);
                    }

                    int dishId = database.AddDish(dishName, storedImagePath, contributorId);
                    database.SaveDishInfo(dishId, category, meals, price, region, description);
                    addedDishes++;
                }
            }

            ReloadAll();
            ApplyMode(false);
            string message = "Đã nhập input6.txt · thêm " + addedContributors + " người · thêm " + addedDishes + " món";
            if (skippedDishes > 0) message += " · bỏ qua " + skippedDishes + " món đã tồn tại";
            SetStatus(message);
            MessageBox.Show(this, message, MessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            spinTimer.Stop();
            clockTimer.Stop();
            smallImages.Dispose();
            largeImages.Dispose();
            base.OnFormClosed(e);
        }
    }

    internal static class B6Ui
    {

        public static readonly Color Chili = Color.FromArgb(214, 62, 38);
        public static readonly Color ChiliDark = Color.FromArgb(176, 44, 26);
        public static readonly Color Saffron = Color.FromArgb(247, 168, 32);
        public static readonly Color SaffronDark = Color.FromArgb(222, 140, 10);
        public static readonly Color Basil = Color.FromArgb(70, 150, 84);
        public static readonly Color BasilDark = Color.FromArgb(52, 122, 66);
        public static readonly Color Paper = Color.FromArgb(255, 246, 232);
        public static readonly Color Ink = Color.FromArgb(52, 34, 26);
        public static readonly Color InkSoft = Color.FromArgb(138, 112, 96);
        public static readonly Color Line = Color.FromArgb(238, 224, 205);
        public static readonly Color Field = Color.FromArgb(255, 251, 243);

        public static readonly Color Night = Color.FromArgb(21, 14, 12);
        public static readonly Color NightCard = Color.FromArgb(37, 26, 22);
        public static readonly Color NightCard2 = Color.FromArgb(52, 38, 32);
        public static readonly Color NightLine = Color.FromArgb(76, 57, 48);
        public static readonly Color Gold = Color.FromArgb(232, 184, 82);
        public static readonly Color GoldDark = Color.FromArgb(205, 156, 52);
        public static readonly Color Cream = Color.FromArgb(250, 238, 218);
        public static readonly Color CreamMuted = Color.FromArgb(184, 164, 144);
        public static readonly Color Ruby = Color.FromArgb(196, 58, 62);

        public static readonly string[] Meals = { "Sáng", "Trưa", "Chiều", "Tối" };

        public static Color SurfaceOf(Control control)
        {
            for (Control parent = control.Parent; parent != null; parent = parent.Parent)
            {
                if (parent.BackColor.A == 255)
                {
                    return parent.BackColor;
                }
            }
            return Color.White;
        }

        public static int Dp(this Control control, int value)
        {
            return control.LogicalToDeviceUnits(value);
        }

        public static float Scale(Control control)
        {
            return control.DeviceDpi / 96f;
        }

        public static LinearGradientBrush Gradient(RectangleF area, Color from, Color to, float angle)
        {
            if (area.Width < 1f) area.Width = 1f;
            if (area.Height < 1f) area.Height = 1f;
            return new LinearGradientBrush(area, from, to, angle);
        }

        public static LinearGradientBrush Gradient(Rectangle area, Color from, Color to, float angle)
        {
            return Gradient((RectangleF)area, from, to, angle);
        }

        public static LinearGradientBrush Gradient(RectangleF area, Color from, Color to, LinearGradientMode mode)
        {
            if (area.Width < 1f) area.Width = 1f;
            if (area.Height < 1f) area.Height = 1f;
            return new LinearGradientBrush(area, from, to, mode);
        }

        public static LinearGradientBrush Gradient(Rectangle area, Color from, Color to, LinearGradientMode mode)
        {
            return Gradient((RectangleF)area, from, to, mode);
        }

        public static GraphicsPath RoundRect(RectangleF r, float radius)
        {
            float d = Math.Max(0.1f, Math.Min(radius * 2f, Math.Min(r.Width, r.Height)));
            GraphicsPath path = new GraphicsPath();
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        public static Color Lerp(Color a, Color b, float t)
        {
            t = Math.Max(0f, Math.Min(1f, t));
            return Color.FromArgb(
                (int)(a.A + (b.A - a.A) * t),
                (int)(a.R + (b.R - a.R) * t),
                (int)(a.G + (b.G - a.G) * t),
                (int)(a.B + (b.B - a.B) * t));
        }

        public static float EaseOutCubic(float t)
        {
            t = Math.Max(0f, Math.Min(1f, t));
            float u = 1f - t;
            return 1f - u * u * u;
        }

        public static float EaseOutBack(float t)
        {
            t = Math.Max(0f, Math.Min(1f, t));
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            float u = t - 1f;
            return 1f + c3 * u * u * u + c1 * u * u;
        }

        public static void EnableDoubleBuffer(Control control)
        {
            typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(control, true, null);
        }

        public static string Fold(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            string decomposed = text.Normalize(NormalizationForm.FormD);
            StringBuilder builder = new StringBuilder(decomposed.Length);
            foreach (char letter in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(letter);
                }
            }
            return builder.ToString().ToLowerInvariant().Replace('đ', 'd');
        }

        public static string FormatPrice(int price)
        {
            if (price <= 0)
            {
                return "—";
            }
            return price.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) + "đ";
        }

        public static int MealIndexForNow()
        {
            int hour = DateTime.Now.Hour;
            if (hour >= 4 && hour < 10) return 0;
            if (hour >= 10 && hour < 14) return 1;
            if (hour >= 14 && hour < 18) return 2;
            return 3;
        }

        public static string MealForNow()
        {
            return Meals[MealIndexForNow()];
        }

        public static string MealNote(string meal)
        {
            switch (meal)
            {
                case "Sáng": return "Buổi sáng nhẹ bụng: phở, bún, xôi, bánh mì, cháo. Lẩu & nướng nghỉ đến trưa nhé.";
                case "Trưa": return "Buổi trưa cần no và đủ năng lượng: cơm, bún, mì, kèm canh mát. Lẩu bắt đầu mở cửa.";
                case "Chiều": return "Buổi chiều vừa phải: ăn vặt, chè, trà, bánh trái để lấy lại tinh thần.";
                case "Tối": return "Buổi tối sum họp: lẩu, nướng, món nóng; chừa chỗ cho tráng miệng.";
                default: return "Chọn một buổi để xem gợi ý phù hợp.";
            }
        }

        public static void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không mở được liên kết:\n" + url + "\n\n" + ex.Message, "Bài 06", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        [System.Runtime.InteropServices.DllImport("uxtheme.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string subAppName, string subIdList);

        public static void DarkScroll(Control control)
        {
            Action apply = () =>
            {
                try
                {
                    SetWindowTheme(control.Handle, "DarkMode_Explorer", null);
                }
                catch (Exception)
                {
                }
            };

            if (control.IsHandleCreated) apply(); else control.HandleCreated += (s, e) => apply();
        }

        public static void StyleList(ListView list, bool dark)
        {
            Color back = dark ? NightCard : Color.White;
            Color fore = dark ? Cream : Ink;
            Color headerBack = dark ? NightCard2 : Color.FromArgb(252, 240, 220);
            Color headerFore = dark ? Gold : ChiliDark;

            list.BorderStyle = BorderStyle.None;
            list.BackColor = back;
            list.ForeColor = fore;
            list.FullRowSelect = true;
            list.HideSelection = false;
            list.MultiSelect = false;
            list.View = View.Details;
            list.OwnerDraw = true;
            list.Font = new Font("Segoe UI", 10F);
            EnableDoubleBuffer(list);
            if (dark) DarkScroll(list);

            Font headerFont = new Font("Segoe UI", 9F, FontStyle.Bold);
            list.Disposed += (sender, e) => headerFont.Dispose();
            list.DrawColumnHeader += (sender, e) =>
            {
                using (SolidBrush brush = new SolidBrush(headerBack))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }
                TextRenderer.DrawText(e.Graphics, e.Header.Text, headerFont,
                    new Rectangle(e.Bounds.X + 8, e.Bounds.Y, e.Bounds.Width - 12, e.Bounds.Height), headerFore,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            };
            list.DrawItem += (sender, e) =>
            {
                if (list.View != View.Details)
                {
                    e.DrawDefault = true;
                }
            };
            list.DrawSubItem += (sender, e) =>
            {
                bool selected = e.Item.Selected;
                Color rowBack = selected ? (dark ? Color.FromArgb(94, 66, 36) : Color.FromArgb(255, 226, 190)) : back;
                Color rowFore = selected ? (dark ? Gold : ChiliDark) : (e.ColumnIndex == 0 ? fore : (dark ? CreamMuted : InkSoft));
                using (SolidBrush brush = new SolidBrush(rowBack))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }

                int textLeft = e.Bounds.X + 8;
                if (e.ColumnIndex == 0 && list.SmallImageList != null && e.Item.ImageIndex >= 0 && e.Item.ImageIndex < list.SmallImageList.Images.Count)
                {
                    Image image = list.SmallImageList.Images[e.Item.ImageIndex];
                    int top = e.Bounds.Y + (e.Bounds.Height - image.Height) / 2;
                    e.Graphics.DrawImage(image, e.Bounds.X + 6, top, image.Width, image.Height);
                    textLeft = e.Bounds.X + image.Width + 14;
                }

                TextRenderer.DrawText(e.Graphics, e.SubItem.Text, list.Font,
                    new Rectangle(textLeft, e.Bounds.Y, Math.Max(10, e.Bounds.Right - textLeft - 4), e.Bounds.Height), rowFore,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            };
        }
    }

    internal sealed class B6Hover : IDisposable
    {
        private readonly Control owner;
        private readonly Timer timer = new Timer { Interval = 15 };
        private bool over;

        public float T { get; private set; }
        public bool Down { get; private set; }

        public B6Hover(Control owner)
        {
            this.owner = owner;
            owner.MouseEnter += (s, e) => { over = true; timer.Start(); };
            owner.MouseLeave += (s, e) => { over = false; Down = false; timer.Start(); };
            owner.MouseDown += (s, e) => { Down = true; owner.Invalidate(); };
            owner.MouseUp += (s, e) => { Down = false; owner.Invalidate(); };
            timer.Tick += (s, e) =>
            {
                float target = over ? 1f : 0f;
                T += (target - T) * 0.35f;
                if (Math.Abs(target - T) < 0.02f)
                {
                    T = target;
                    timer.Stop();
                }
                owner.Invalidate();
            };
        }

        public void Dispose()
        {
            timer.Dispose();
        }
    }

    [System.ComponentModel.DesignerCategory("Code")]
    [System.ComponentModel.ToolboxItem(false)]
    internal sealed class B6Button : Button
    {
        private readonly B6Hover hover;

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int Radius { get; set; } = 12;
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Color HoverColor { get; set; } = Color.Empty;
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Color BorderColor { get; set; } = Color.Empty;
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Color SurfaceColor { get; set; } = Color.Empty;

        public B6Button()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            hover = new B6Hover(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                hover.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width < 4 || Height < 4)
            {
                return;
            }

            Graphics g = e.Graphics;
            Color surface = SurfaceColor != Color.Empty ? SurfaceColor : (B6Ui.SurfaceOf(this));
            g.Clear(surface);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color hoverTarget = HoverColor != Color.Empty ? HoverColor : BackColor;
            Color fill = B6Ui.Lerp(BackColor, hoverTarget, hover.T);
            if (!Enabled)
            {
                fill = B6Ui.Lerp(fill, surface, 0.6f);
            }

            RectangleF rect = new RectangleF(0.5f, 0.5f, Width - 2f, Height - 2f);
            if (hover.Down)
            {
                rect.Inflate(-1.5f, -1.5f);
            }

            using (GraphicsPath path = B6Ui.RoundRect(rect, Radius))
            using (SolidBrush brush = new SolidBrush(fill))
            {
                g.FillPath(brush, path);
                if (BorderColor != Color.Empty)
                {
                    using (Pen pen = new Pen(B6Ui.Lerp(BorderColor, hoverTarget == BackColor ? BorderColor : hoverTarget, hover.T), 1.2f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            Color text = Enabled ? ForeColor : B6Ui.Lerp(ForeColor, fill, 0.55f);
            TextRenderer.DrawText(g, Text, Font, new Rectangle(4, 0, Width - 8, Height), text,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
        }
    }

    [System.ComponentModel.DesignerCategory("Code")]
    [System.ComponentModel.ToolboxItem(false)]
    internal class B6Card : Panel
    {
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int Radius { get; set; } = 18;
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Color SurfaceColor { get; set; } = Color.Empty;
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Color BorderColor { get; set; } = Color.Empty;
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Color AccentColor { get; set; } = Color.Empty;
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool Shadow { get; set; } = true;

        public B6Card()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = Color.White;
            Padding = new Padding(18, 18, 24, 24);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width < 4 || Height < 4)
            {
                return;
            }

            Graphics g = e.Graphics;
            Color surface = SurfaceColor != Color.Empty ? SurfaceColor : (B6Ui.SurfaceOf(this));
            g.Clear(surface);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            float inset = Shadow ? 6f : 1f;
            RectangleF body = new RectangleF(1f, 1f, Width - inset - 1f, Height - inset - 1f);

            if (Shadow)
            {
                for (int i = 5; i >= 1; i--)
                {
                    RectangleF layer = new RectangleF(body.X + 1f, body.Y + 2f + i * 0.4f, body.Width, body.Height);
                    layer.Inflate(i * 0.6f, i * 0.6f);
                    using (GraphicsPath path = B6Ui.RoundRect(layer, Radius + i))
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(9, 60, 30, 10)))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }

            using (GraphicsPath path = B6Ui.RoundRect(body, Radius))
            using (SolidBrush brush = new SolidBrush(BackColor))
            {
                g.FillPath(brush, path);
                if (BorderColor != Color.Empty)
                {
                    using (Pen pen = new Pen(BorderColor, 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }

                if (AccentColor != Color.Empty)
                {
                    GraphicsState state = g.Save();
                    g.SetClip(path);
                    using (SolidBrush accent = new SolidBrush(AccentColor))
                    {
                        g.FillRectangle(accent, body.X, body.Y, body.Width, 5f);
                    }
                    g.Restore(state);
                }
            }
        }
    }

    [System.ComponentModel.DesignerCategory("Code")]
    [System.ComponentModel.ToolboxItem(false)]
    internal sealed class B6Input : Panel
    {
        private bool focused;

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public TextBox Inner { get; private set; }
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Color BorderColor { get; set; } = B6Ui.Line;
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Color FocusColor { get; set; } = B6Ui.Saffron;
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Color SurfaceColor { get; set; } = Color.White;

        public B6Input(bool dark)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Height = 38;
            Inner = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10.5F)
            };
            if (dark)
            {
                BackColor = B6Ui.NightCard2;
                BorderColor = B6Ui.NightLine;
                FocusColor = B6Ui.Gold;
                SurfaceColor = B6Ui.NightCard;
                Inner.BackColor = B6Ui.NightCard2;
                Inner.ForeColor = B6Ui.Cream;
            }
            else
            {
                BackColor = B6Ui.Field;
                Inner.BackColor = B6Ui.Field;
                Inner.ForeColor = B6Ui.Ink;
            }

            MouseDown += (s, e) => Inner.Focus();
            Inner.GotFocus += (s, e) => { focused = true; Invalidate(); };
            Inner.LostFocus += (s, e) => { focused = false; Invalidate(); };
            Inner.TextChanged += (s, e) => OnTextChanged(EventArgs.Empty);
            Controls.Add(Inner);
        }

        public override string Text
        {
            get { return Inner == null ? string.Empty : Inner.Text; }
            set { if (Inner != null) Inner.Text = value; }
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public string PlaceholderText
        {
            get { return Inner.PlaceholderText; }
            set { Inner.PlaceholderText = value; }
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            if (Inner != null)
            {
                int pad = Math.Max(10, Height / 3);
                Inner.SetBounds(pad, (Height - Inner.Height) / 2, Math.Max(10, Width - pad * 2), Inner.Height);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width < 4 || Height < 4)
            {
                return;
            }

            Graphics g = e.Graphics;
            Color surface = B6Ui.SurfaceOf(this);
            g.Clear(surface);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            RectangleF rect = new RectangleF(0.5f, 0.5f, Width - 2f, Height - 2f);
            using (GraphicsPath path = B6Ui.RoundRect(rect, Height / 2.6f))
            using (SolidBrush brush = new SolidBrush(BackColor))
            using (Pen pen = new Pen(focused ? FocusColor : BorderColor, focused ? 1.8f : 1.2f))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }
        }
    }

    [System.ComponentModel.DesignerCategory("Code")]
    [System.ComponentModel.ToolboxItem(false)]
    internal sealed class B6Combo : Control
    {
        private readonly bool dark;
        private readonly List<string> entries = new List<string>();
        private int selectedIndex = -1;
        private bool hover;
        private bool opened;
        private int lastClosed;

        public event EventHandler SelectedIndexChanged;

        public B6Combo(bool dark)
        {
            this.dark = dark;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
            TabStop = true;
            Height = 38;
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI", 10.5F);
            BackColor = dark ? B6Ui.NightCard2 : B6Ui.Field;
            ForeColor = dark ? B6Ui.Cream : B6Ui.Ink;
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public IList<string> Items
        {
            get { return entries; }
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { SetIndex(value, true); }
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public object SelectedItem
        {
            get { return selectedIndex >= 0 && selectedIndex < entries.Count ? entries[selectedIndex] : null; }
            set
            {
                int index = entries.IndexOf(value as string);
                if (index >= 0) SetIndex(index, true);
            }
        }

        public void SetItems(IEnumerable<string> items, int index)
        {
            entries.Clear();
            entries.AddRange(items);
            selectedIndex = -1;
            if (entries.Count > 0)
            {
                SetIndex(Math.Max(0, Math.Min(index, entries.Count - 1)), true);
            }
            Invalidate();
        }

        private void SetIndex(int value, bool raise)
        {
            if (value < -1 || value >= entries.Count || value == selectedIndex)
            {
                return;
            }

            selectedIndex = value;
            Invalidate();
            if (raise)
            {
                EventHandler handler = SelectedIndexChanged;
                if (handler != null) handler(this, EventArgs.Empty);
            }
        }

        private void OpenPopup()
        {
            if (entries.Count == 0 || opened)
            {
                return;
            }

            float k = B6Ui.Scale(this);
            int itemHeight = (int)Math.Round(34 * k);
            int rows = Math.Min(entries.Count, 9);
            Color back = dark ? B6Ui.NightCard : Color.White;
            Color fore = dark ? B6Ui.Cream : B6Ui.Ink;
            Color pick = dark ? Color.FromArgb(94, 66, 36) : Color.FromArgb(255, 226, 190);
            Color mark = dark ? B6Ui.Gold : B6Ui.ChiliDark;

            ListBox list = new ListBox
            {
                BorderStyle = BorderStyle.None,
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = itemHeight,
                IntegralHeight = false,
                BackColor = back,
                ForeColor = fore,
                Font = Font,
                Size = new Size(Math.Max(Width, (int)(150 * k)), itemHeight * rows + 6)
            };
            list.Items.AddRange(entries.ToArray());
            if (selectedIndex >= 0) list.SelectedIndex = selectedIndex;

            list.DrawItem += (s, e) =>
            {
                if (e.Index < 0 || e.Index >= entries.Count) return;
                bool active = e.Index == list.SelectedIndex;
                using (SolidBrush fill = new SolidBrush(active ? pick : back))
                {
                    e.Graphics.FillRectangle(fill, e.Bounds);
                }
                TextRenderer.DrawText(e.Graphics, entries[e.Index], Font,
                    new Rectangle(e.Bounds.X + 12, e.Bounds.Y, e.Bounds.Width - 20, e.Bounds.Height), active ? mark : fore,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            };

            ToolStripControlHost host = new ToolStripControlHost(list)
            {
                AutoSize = false,
                Size = list.Size,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            ToolStripDropDown popup = new ToolStripDropDown
            {
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                BackColor = back,
                DropShadowEnabled = true
            };
            popup.Items.Add(host);

            list.MouseMove += (s, e) =>
            {
                int index = list.IndexFromPoint(e.Location);
                if (index >= 0 && index != list.SelectedIndex) list.SelectedIndex = index;
            };
            list.SelectedIndexChanged += (s, e) => list.Invalidate();
            list.MouseClick += (s, e) =>
            {
                int index = list.IndexFromPoint(e.Location);
                if (index >= 0)
                {
                    popup.Close();
                    SetIndex(index, true);
                }
            };
            list.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && list.SelectedIndex >= 0)
                {
                    int index = list.SelectedIndex;
                    popup.Close();
                    SetIndex(index, true);
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    popup.Close();
                }
            };
            popup.Closed += (s, e) =>
            {
                opened = false;
                lastClosed = Environment.TickCount;
                Invalidate();
                if (IsHandleCreated && !IsDisposed)
                {
                    BeginInvoke(new Action(() => popup.Dispose()));
                }
            };

            opened = true;
            Invalidate();
            popup.Show(this, new Point(0, Height + 2));
            list.Focus();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            Focus();
            if (Environment.TickCount - lastClosed > 250)
            {
                OpenPopup();
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return keyData == Keys.Down || keyData == Keys.Up || base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.F4 || e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                OpenPopup();
                e.Handled = true;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            hover = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            hover = false;
            Invalidate();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width < 4 || Height < 4)
            {
                return;
            }

            Graphics g = e.Graphics;
            g.Clear(B6Ui.SurfaceOf(this));
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color border = opened || Focused
                ? (dark ? B6Ui.Gold : B6Ui.Saffron)
                : (hover ? (dark ? B6Ui.GoldDark : B6Ui.Saffron) : (dark ? B6Ui.NightLine : B6Ui.Line));
            Color fill = hover ? B6Ui.Lerp(BackColor, dark ? Color.White : Color.Black, 0.05f) : BackColor;

            RectangleF rect = new RectangleF(0.5f, 0.5f, Width - 2f, Height - 2f);
            using (GraphicsPath path = B6Ui.RoundRect(rect, Height / 2.6f))
            using (SolidBrush brush = new SolidBrush(fill))
            using (Pen pen = new Pen(border, opened || Focused ? 1.8f : 1.2f))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }

            int arrowWidth = (int)(Height * 0.8f);
            int left = (int)(Height * 0.4f);
            string text = selectedIndex >= 0 && selectedIndex < entries.Count ? entries[selectedIndex] : string.Empty;
            TextRenderer.DrawText(g, text, Font, new Rectangle(left, 0, Math.Max(10, Width - arrowWidth - left), Height), ForeColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);

            float cx = Width - arrowWidth / 2f - 2f;
            float cy = Height / 2f;
            float size = Height * 0.12f;
            float flip = opened ? -1f : 1f;
            using (Pen chevron = new Pen(dark ? B6Ui.Gold : B6Ui.ChiliDark, 2f))
            {
                chevron.StartCap = LineCap.Round;
                chevron.EndCap = LineCap.Round;
                chevron.LineJoin = LineJoin.Round;
                g.DrawLines(chevron, new[]
                {
                    new PointF(cx - size, cy - size * 0.5f * flip),
                    new PointF(cx, cy + size * 0.5f * flip),
                    new PointF(cx + size, cy - size * 0.5f * flip)
                });
            }
        }
    }

    [System.ComponentModel.DesignerCategory("Code")]
    [System.ComponentModel.ToolboxItem(false)]
    internal sealed class B6Segments : Control
    {
        private readonly Timer slide = new Timer { Interval = 15 };
        private readonly HashSet<int> disabled = new HashSet<int>();
        private string[] items = new string[0];
        private int selected;
        private float thumbPosition;
        private int hoverIndex = -1;

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Color TrackColor { get; set; } = Color.FromArgb(251, 237, 214);
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Color ThumbColor { get; set; } = B6Ui.Chili;
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Color ThumbTextColor { get; set; } = Color.White;
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Color TextColor { get; set; } = B6Ui.InkSoft;
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Color DisabledTextColor { get; set; } = Color.FromArgb(206, 190, 172);
        public event EventHandler SelectedIndexChanged;

        public B6Segments()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Height = 38;
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            slide.Tick += (s, e) =>
            {
                thumbPosition += (selected - thumbPosition) * 0.32f;
                if (Math.Abs(selected - thumbPosition) < 0.01f)
                {
                    thumbPosition = selected;
                    slide.Stop();
                }
                Invalidate();
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                slide.Dispose();
            }
            base.Dispose(disposing);
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public string[] Items
        {
            get { return items; }
            set
            {
                items = value ?? new string[0];
                if (selected >= items.Length) selected = 0;
                thumbPosition = selected;
                Invalidate();
            }
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex
        {
            get { return selected; }
            set { Select(value, true); }
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public string SelectedText
        {
            get { return selected >= 0 && selected < items.Length ? items[selected] : string.Empty; }
        }

        public void SelectSilently(int index)
        {
            Select(index, false);
        }

        public void SetDisabled(int index, bool value)
        {
            if (value) disabled.Add(index); else disabled.Remove(index);
            Invalidate();
        }

        public bool IsDisabled(int index)
        {
            return disabled.Contains(index);
        }

        private void Select(int index, bool raise)
        {
            if (index < 0 || index >= items.Length || index == selected)
            {
                return;
            }
            selected = index;
            slide.Start();
            Invalidate();
            if (raise)
            {
                EventHandler handler = SelectedIndexChanged;
                if (handler != null) handler(this, EventArgs.Empty);
            }
        }

        private int IndexAt(int x)
        {
            if (items.Length == 0) return -1;
            int index = (int)(x / (Width / (float)items.Length));
            return Math.Max(0, Math.Min(items.Length - 1, index));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int index = IndexAt(e.X);
            if (index != hoverIndex)
            {
                hoverIndex = index;
                Cursor = disabled.Contains(index) ? Cursors.No : Cursors.Hand;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            hoverIndex = -1;
            Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            int index = IndexAt(e.X);
            if (index >= 0 && !disabled.Contains(index))
            {
                Select(index, true);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width < 4 || Height < 4)
            {
                return;
            }

            Graphics g = e.Graphics;
            g.Clear(B6Ui.SurfaceOf(this));
            g.SmoothingMode = SmoothingMode.AntiAlias;
            if (items.Length == 0) return;

            RectangleF track = new RectangleF(0.5f, 0.5f, Width - 2f, Height - 2f);
            using (GraphicsPath path = B6Ui.RoundRect(track, Height / 2f))
            using (SolidBrush brush = new SolidBrush(TrackColor))
            {
                g.FillPath(brush, path);
            }

            float segment = Width / (float)items.Length;
            RectangleF thumb = new RectangleF(thumbPosition * segment + 3f, 3.5f, segment - 6f, Height - 7f);
            using (GraphicsPath path = B6Ui.RoundRect(thumb, thumb.Height / 2f))
            using (SolidBrush brush = new SolidBrush(ThumbColor))
            {
                g.FillPath(brush, path);
            }

            for (int i = 0; i < items.Length; i++)
            {
                Color color;
                if (disabled.Contains(i)) color = DisabledTextColor;
                else if (Math.Abs(i - thumbPosition) < 0.5f) color = ThumbTextColor;
                else color = i == hoverIndex ? B6Ui.Lerp(TextColor, ThumbColor, 0.5f) : TextColor;

                TextRenderer.DrawText(g, items[i], Font, new Rectangle((int)(i * segment), 0, (int)segment, Height), color,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            }
        }
    }

    [System.ComponentModel.DesignerCategory("Code")]
    [System.ComponentModel.ToolboxItem(false)]
    internal sealed class B6Hero : Control
    {
        public enum Look { Street, Chef }

        private sealed class Particle
        {
            public int Kind;
            public float X, Y, Speed, Rotation, RotationSpeed, Size, Phase, Alpha;
        }

        private sealed class HeroAction
        {
            public string Text;
            public bool Filled;
            public Action Click;
            public RectangleF Bounds;
            public float Hover;
            public bool Over;
        }

        private static readonly Color[][] SkyPalettes =
        {
            new[] { Color.FromArgb(150, 62, 34), Color.FromArgb(247, 156, 56) },
            new[] { Color.FromArgb(188, 50, 30), Color.FromArgb(250, 124, 30) },
            new[] { Color.FromArgb(118, 34, 66), Color.FromArgb(238, 104, 52) },
            new[] { Color.FromArgb(24, 20, 50), Color.FromArgb(112, 36, 56) }
        };

        private readonly Timer timer = new Timer { Interval = 33 };
        private readonly List<Particle> particles = new List<Particle>();
        private readonly List<HeroAction> actions = new List<HeroAction>();
        private readonly Random random = new Random(11);
        private readonly PointF[] stars = new PointF[22];
        private float time;
        private float tickerOffset;
        private string tickerText = string.Empty;
        private float tickerWidth;
        private int period;
        private int lastPeriodCheck;

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public string Title { get; set; } = string.Empty;
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public string Subtitle { get; set; } = string.Empty;
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Look Style { get; set; } = Look.Street;

        public B6Hero()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Height = 132;
            period = B6Ui.MealIndexForNow();

            for (int i = 0; i < stars.Length; i++)
            {
                stars[i] = new PointF((float)random.NextDouble(), (float)random.NextDouble() * 0.7f);
            }
            for (int i = 0; i < 14; i++)
            {
                particles.Add(NewParticle(true));
            }

            timer.Tick += OnTick;
            timer.Start();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                timer.Stop();
                timer.Dispose();
                if (tickerBitmap != null) tickerBitmap.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible && !DesignMode) timer.Start(); else timer.Stop();
        }

        public void AddAction(string text, bool filled, Action click)
        {
            actions.Add(new HeroAction { Text = text, Filled = filled, Click = click });
            Invalidate();
        }

        public void SetActionText(int index, string text)
        {
            if (index >= 0 && index < actions.Count)
            {
                actions[index].Text = text;
            }
        }

        public void SetTicker(IEnumerable<string> names)
        {
            List<string> list = new List<string>(names);
            Random pick = new Random();
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = pick.Next(i + 1);
                string swap = list[i];
                list[i] = list[j];
                list[j] = swap;
            }
            if (list.Count > 30)
            {
                list.RemoveRange(30, list.Count - 30);
            }

            tickerText = "   ✦   " + string.Join("   ✦   ", list) + "   ✦   ";
            tickerWidth = 0;
            if (tickerBitmap != null)
            {
                tickerBitmap.Dispose();
                tickerBitmap = null;
            }
            Invalidate();
        }

        private Particle NewParticle(bool anywhere)
        {
            float k = B6Ui.Scale(this);
            return new Particle
            {
                Kind = random.Next(5),
                X = (float)random.NextDouble() * Math.Max(400, Width),
                Y = anywhere ? (float)random.NextDouble() * Math.Max(150, Height) : Height + 20 * k,
                Speed = 0.25f + (float)random.NextDouble() * 0.6f,
                Rotation = random.Next(360),
                RotationSpeed = ((float)random.NextDouble() - 0.5f) * 3f,
                Size = 8 + random.Next(9),
                Phase = (float)random.NextDouble() * 6.28f,
                Alpha = 70 + random.Next(80)
            };
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (DesignMode || !Visible) return;
            Form owner = FindForm();
            if (owner != null && owner.WindowState == FormWindowState.Minimized) return;

            time += 0.033f;
            float k = B6Ui.Scale(this);
            tickerOffset += 0.9f * k;
            if (tickerWidth > 0 && tickerOffset > tickerWidth) tickerOffset -= tickerWidth;

            for (int i = 0; i < particles.Count; i++)
            {
                Particle p = particles[i];
                p.Y -= p.Speed * k;
                p.X += (float)Math.Sin(time * 1.2f + p.Phase) * 0.35f * k;
                p.Rotation += p.RotationSpeed;
                if (p.Y < -30 * k)
                {
                    particles[i] = NewParticle(false);
                }
            }

            UpdateSmoke(k);

            foreach (HeroAction action in actions)
            {
                float target = action.Over ? 1f : 0f;
                action.Hover += (target - action.Hover) * 0.3f;
            }

            if (Environment.TickCount - lastPeriodCheck > 30000)
            {
                lastPeriodCheck = Environment.TickCount;
                period = B6Ui.MealIndexForNow();
            }

            Invalidate(ClientRectangle, false);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            bool any = false;
            foreach (HeroAction action in actions)
            {
                action.Over = action.Bounds.Contains(e.Location);
                any |= action.Over;
            }
            Cursor = any ? Cursors.Hand : Cursors.Default;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            foreach (HeroAction action in actions) action.Over = false;
            Cursor = Cursors.Default;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            foreach (HeroAction action in actions)
            {
                if (action.Bounds.Contains(e.Location) && action.Click != null)
                {
                    action.Click();
                    return;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width < 4 || Height < 4)
            {
                return;
            }

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            float k = B6Ui.Scale(this);
            float w = Width;
            float h = Height;
            if (w < 10 || h < 10) return;

            Color top = SkyPalettes[period][0];
            Color bottom = SkyPalettes[period][1];
            if (Style == Look.Chef)
            {
                top = B6Ui.Lerp(top, Color.Black, 0.62f);
                bottom = B6Ui.Lerp(bottom, Color.Black, 0.5f);
            }

            using (LinearGradientBrush sky = B6Ui.Gradient(new RectangleF(0, 0, w, h), top, bottom, 18f))
            {
                g.FillRectangle(sky, 0, 0, w, h);
            }

            float actionsWidth = LayoutActions(g, k);
            float sceneX = Math.Min(w * 0.58f, w - actionsWidth - 150 * k);
            sceneX = Math.Max(sceneX, 420 * k);

            if (period == 3) DrawStars(g, w, h);
            DrawOrb(g, sceneX + 125 * k, h * 0.34f, k);

            foreach (Particle p in particles) DrawParticle(g, p, k);

            smokeSource = new PointF(sceneX, h - 34 * k - 44 * k);
            DrawBowl(g, sceneX, h - 34 * k, k * 0.86f);
            DrawSmoke(g);
            DrawTitle(g, k, h, Math.Max(220f * k, sceneX - 60f * k));
            DrawTicker(g, k, w, h);
            DrawActions(g, k);

            using (SolidBrush line = new SolidBrush(Style == Look.Chef ? B6Ui.Gold : B6Ui.Saffron))
            {
                g.FillRectangle(line, 0, h - 3 * k, w, 3 * k);
            }
        }

        private sealed class Puff
        {
            public float X, Y, Radius, Grow, Rise, Drift, Phase, Age, Life, Strength;
        }

        private readonly List<Puff> puffs = new List<Puff>();
        private PointF smokeSource = new PointF(-1000f, -1000f);

        private void UpdateSmoke(float k)
        {
            if (smokeSource.X > -500f && puffs.Count < 56)
            {
                for (int n = 0; n < 2; n++)
                {
                    puffs.Add(new Puff
                    {
                        X = smokeSource.X + ((float)random.NextDouble() - 0.5f) * 80f * k,
                        Y = smokeSource.Y,
                        Radius = (6f + (float)random.NextDouble() * 6f) * k,
                        Grow = (0.3f + (float)random.NextDouble() * 0.35f) * k,
                        Rise = (0.7f + (float)random.NextDouble() * 0.9f) * k,
                        Drift = ((float)random.NextDouble() - 0.35f) * 0.5f * k,
                        Phase = (float)random.NextDouble() * 6.28f,
                        Life = 70f + random.Next(60),
                        Strength = 0.6f + (float)random.NextDouble() * 0.4f
                    });
                }
            }

            for (int i = puffs.Count - 1; i >= 0; i--)
            {
                Puff puff = puffs[i];
                puff.Age += 1f;
                puff.Y -= puff.Rise;
                puff.X += puff.Drift + (float)Math.Sin(time * 1.8f + puff.Phase) * 0.55f * k;
                puff.Radius += puff.Grow;
                if (puff.Age >= puff.Life)
                {
                    puffs.RemoveAt(i);
                }
            }
        }

        private void DrawSmoke(Graphics g)
        {
            foreach (Puff puff in puffs)
            {
                float t = puff.Age / puff.Life;
                int alpha = (int)(80f * puff.Strength * (1f - t) * Math.Min(1f, t * 8f));
                if (alpha <= 1) continue;
                using (SolidBrush outer = new SolidBrush(Color.FromArgb(alpha / 2, 255, 250, 242)))
                using (SolidBrush inner = new SolidBrush(Color.FromArgb(alpha, 255, 252, 246)))
                {
                    g.FillEllipse(outer, puff.X - puff.Radius, puff.Y - puff.Radius * 0.85f, puff.Radius * 2f, puff.Radius * 1.7f);
                    float core = puff.Radius * 0.62f;
                    g.FillEllipse(inner, puff.X - core, puff.Y - core * 0.85f, core * 2f, core * 1.7f);
                }
            }
        }

        private void DrawTitle(Graphics g, float k, float h, float maxWidth)
        {
            using (Font titleFont = new Font("Palatino Linotype", 27 * k, FontStyle.Bold, GraphicsUnit.Pixel))
            using (Font subFont = new Font("Segoe UI", 13 * k, FontStyle.Regular, GraphicsUnit.Pixel))
            using (Font chipFont = new Font("Segoe UI", 11 * k, FontStyle.Bold, GraphicsUnit.Pixel))
            using (SolidBrush shadow = new SolidBrush(Color.FromArgb(70, 0, 0, 0)))
            using (SolidBrush white = new SolidBrush(Color.FromArgb(255, 250, 240)))
            using (SolidBrush soft = new SolidBrush(Color.FromArgb(235, 255, 226, 192)))
            {
                float x = 28 * k;
                float y = 12 * k;
                using (StringFormat clip = new StringFormat { Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap })
                {
                    g.DrawString(Title, titleFont, shadow, new RectangleF(x + 2 * k, y + 2 * k, maxWidth, 36 * k), clip);
                    g.DrawString(Title, titleFont, white, new RectangleF(x, y, maxWidth, 36 * k), clip);
                    g.DrawString(Subtitle, subFont, soft, new RectangleF(x + 2 * k, y + 38 * k, maxWidth, 22 * k), clip);
                }

                string chip = "BUỔI " + B6Ui.Meals[period].ToUpperInvariant() + "  ·  " + DateTime.Now.ToString("HH:mm");
                SizeF size = g.MeasureString(chip, chipFont);
                RectangleF chipRect = new RectangleF(x + 2 * k, y + 64 * k, size.Width + 24 * k, 24 * k);
                using (GraphicsPath path = B6Ui.RoundRect(chipRect, 12 * k))
                using (SolidBrush back = new SolidBrush(Color.FromArgb(70, 255, 255, 255)))
                using (Pen border = new Pen(Color.FromArgb(120, 255, 226, 160), 1f))
                using (SolidBrush text = new SolidBrush(Style == Look.Chef ? B6Ui.Gold : Color.FromArgb(255, 236, 190)))
                using (SolidBrush dot = new SolidBrush(B6Ui.Saffron))
                {
                    g.FillPath(back, path);
                    g.DrawPath(border, path);
                    float pulse = 4 + (float)Math.Sin(time * 4) * 1.2f;
                    g.FillEllipse(dot, chipRect.X + 9 * k - pulse * k / 2, chipRect.Y + 12 * k - pulse * k / 2, pulse * k, pulse * k);
                    g.DrawString(chip, chipFont, text, chipRect.X + 18 * k, chipRect.Y + 5 * k);
                }
            }
        }

        private float LayoutActions(Graphics g, float k)
        {
            if (actions.Count == 0) return 0;

            float h = 40 * k;
            float gap = 10 * k;
            float right = Width - 24 * k;
            float y = (Height - 3 * k - 26 * k - h) / 2f + 4 * k;
            float total = 0;

            using (Font font = new Font("Segoe UI", 12 * k, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                for (int i = actions.Count - 1; i >= 0; i--)
                {
                    HeroAction action = actions[i];
                    float width = g.MeasureString(action.Text, font).Width + 36 * k;
                    right -= width;
                    action.Bounds = new RectangleF(right, y, width, h);
                    right -= gap;
                    total += width + gap;
                }
            }
            return total + 24 * k;
        }

        private void DrawActions(Graphics g, float k)
        {
            using (Font font = new Font("Segoe UI", 12 * k, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                foreach (HeroAction action in actions)
                {
                    RectangleF r = action.Bounds;
                    r.Offset(0, -action.Hover * 1.5f * k);
                    Color fill = action.Filled
                        ? B6Ui.Lerp(Style == Look.Chef ? B6Ui.Gold : B6Ui.Saffron, Color.White, action.Hover * 0.25f)
                        : Color.FromArgb((int)(55 + action.Hover * 60), 255, 255, 255);
                    Color text = action.Filled ? Color.FromArgb(54, 30, 12) : Color.White;

                    using (GraphicsPath path = B6Ui.RoundRect(r, r.Height / 2f))
                    using (SolidBrush brush = new SolidBrush(fill))
                    using (Pen border = new Pen(Color.FromArgb(action.Filled ? 0 : 130, 255, 255, 255), 1.2f))
                    using (SolidBrush shadow = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
                    using (SolidBrush textBrush = new SolidBrush(text))
                    using (StringFormat format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        RectangleF shadowRect = r;
                        shadowRect.Offset(0, 2.5f * k);
                        using (GraphicsPath shadowPath = B6Ui.RoundRect(shadowRect, r.Height / 2f))
                        {
                            g.FillPath(shadow, shadowPath);
                        }
                        g.FillPath(brush, path);
                        g.DrawPath(border, path);
                        g.DrawString(action.Text, font, textBrush, r, format);
                    }
                }
            }
        }

        private Bitmap tickerBitmap;
        private float tickerScale;

        private void BuildTickerBitmap(Graphics g, float k)
        {
            if (tickerBitmap != null)
            {
                tickerBitmap.Dispose();
                tickerBitmap = null;
            }

            using (Font font = new Font("Segoe UI", 12 * k, FontStyle.Italic, GraphicsUnit.Pixel))
            {
                float measured = Math.Max(10f, g.MeasureString(tickerText, font).Width);
                int width = (int)Math.Min(16000f, Math.Ceiling(measured));
                int height = Math.Max(4, (int)Math.Ceiling(26 * k));
                Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
                using (Graphics bitmapGraphics = Graphics.FromImage(bitmap))
                using (SolidBrush text = new SolidBrush(Color.FromArgb(230, 255, 232, 200)))
                {
                    bitmapGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                    bitmapGraphics.DrawString(tickerText, font, text, 0, 4 * k);
                }

                tickerBitmap = bitmap;
                tickerWidth = Math.Max(10f, width - 14 * k);
                tickerScale = k;
            }
        }

        private void DrawTicker(Graphics g, float k, float w, float h)
        {
            if (string.IsNullOrEmpty(tickerText)) return;
            float bandHeight = 26 * k;
            RectangleF band = new RectangleF(0, h - bandHeight - 3 * k, w, bandHeight);
            using (SolidBrush back = new SolidBrush(Color.FromArgb(70, 0, 0, 0)))
            {
                g.FillRectangle(back, band);
            }

            if (tickerBitmap == null || Math.Abs(tickerScale - k) > 0.001f)
            {
                BuildTickerBitmap(g, k);
            }

            GraphicsState state = g.Save();
            g.SetClip(band);
            float x = -tickerOffset;
            while (x < w)
            {
                g.DrawImageUnscaled(tickerBitmap, (int)x, (int)band.Y);
                x += tickerWidth;
            }
            g.Restore(state);
        }

        private void DrawStars(Graphics g, float w, float h)
        {
            for (int i = 0; i < stars.Length; i++)
            {
                float twinkle = 0.5f + 0.5f * (float)Math.Sin(time * 2.2f + i * 1.7f);
                using (SolidBrush brush = new SolidBrush(Color.FromArgb((int)(60 + 150 * twinkle), 255, 244, 214)))
                {
                    float size = 1.5f + twinkle * 1.8f;
                    g.FillEllipse(brush, stars[i].X * w, stars[i].Y * h, size, size);
                }
            }
        }

        private static void DrawGlow(Graphics g, float x, float y, float r, Color c, int alpha)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(x - r, y - r, r * 2, r * 2);
                using (PathGradientBrush brush = new PathGradientBrush(path))
                {
                    brush.CenterColor = Color.FromArgb(alpha, c);
                    brush.SurroundColors = new[] { Color.FromArgb(0, c) };
                    g.FillPath(brush, path);
                }
            }
        }

        private void DrawOrb(Graphics g, float x, float y, float k)
        {
            float pulse = 1f + 0.04f * (float)Math.Sin(time * 1.6f);
            if (period == 3)
            {
                DrawGlow(g, x, y, 54 * k * pulse, Color.FromArgb(255, 240, 200), 90);
                float r = 17 * k;
                using (SolidBrush moon = new SolidBrush(Color.FromArgb(255, 246, 222)))
                using (SolidBrush cut = new SolidBrush(SkyPalettes[3][0]))
                {
                    g.FillEllipse(moon, x - r, y - r, r * 2, r * 2);
                    g.FillEllipse(cut, x - r * 0.35f, y - r * 1.05f, r * 1.8f, r * 1.8f);
                }
            }
            else
            {
                Color sun = period == 2 ? Color.FromArgb(255, 150, 80) : Color.FromArgb(255, 226, 120);
                DrawGlow(g, x, y, 66 * k * pulse, sun, 120);
                float r = (period == 1 ? 21 : 17) * k;
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 244, 190)))
                {
                    g.FillEllipse(brush, x - r, y - r, r * 2, r * 2);
                }
            }
        }

        private void DrawBowl(Graphics g, float cx, float baseY, float k)
        {
            float rimY = baseY - 46 * k;
            float bob = (float)Math.Sin(time * 1.4f) * 1.2f * k;

            using (SolidBrush shadow = new SolidBrush(Color.FromArgb(55, 0, 0, 0)))
            {
                g.FillEllipse(shadow, cx - 70 * k, baseY - 6 * k, 140 * k, 14 * k);
            }

            for (int i = 0; i < 5; i++)
            {
                float sx = cx + (i - 2) * 24 * k;
                float phase = time * 1.4f + i * 1.3f;
                float height = (62 + 12 * (i % 2)) * k;
                PointF previous = new PointF(sx, rimY - 4 * k);
                const int steps = 14;
                for (int s = 1; s <= steps; s++)
                {
                    float t = s / (float)steps;
                    float drift = (float)Math.Sin(phase + t * 5.2f) * 9 * k * t;
                    PointF next = new PointF(sx + drift, rimY - 4 * k - t * height);
                    int alpha = (int)(120 * (1f - t) * (0.6f + 0.4f * Math.Sin(phase)));
                    using (Pen pen = new Pen(Color.FromArgb(Math.Max(0, alpha), 255, 255, 255), (5 - 3 * t) * k))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;
                        g.DrawLine(pen, previous, next);
                    }
                    previous = next;
                }
            }

            GraphicsState chopState = g.Save();
            g.TranslateTransform(cx + 22 * k, rimY - 6 * k + bob);
            g.RotateTransform(-38f + (float)Math.Sin(time * 1.1f) * 2f);
            using (Pen chop = new Pen(Color.FromArgb(226, 176, 96), 4 * k))
            using (Pen chop2 = new Pen(Color.FromArgb(196, 140, 72), 4 * k))
            {
                chop.StartCap = LineCap.Round;
                chop.EndCap = LineCap.Round;
                chop2.StartCap = LineCap.Round;
                chop2.EndCap = LineCap.Round;
                g.DrawLine(chop, 0, 0, 0, -88 * k);
                g.DrawLine(chop2, 9 * k, 0, 9 * k, -88 * k);
            }
            g.Restore(chopState);

            RectangleF bodyRect = new RectangleF(cx - 72 * k, rimY - 54 * k, 144 * k, 100 * k);
            using (GraphicsPath body = new GraphicsPath())
            {
                body.AddArc(bodyRect, 0, 180);
                body.CloseFigure();
                using (LinearGradientBrush brush = B6Ui.Gradient(
                    new RectangleF(bodyRect.X, rimY, bodyRect.Width, 50 * k), Color.FromArgb(255, 253, 246), Color.FromArgb(236, 220, 196), 90f))
                {
                    g.FillPath(brush, body);
                }
            }
            using (Pen band = new Pen(Color.FromArgb(214, 62, 38), 3 * k))
            {
                g.DrawArc(band, cx - 58 * k, rimY - 40 * k, 116 * k, 78 * k, 28, 124);
                g.DrawArc(band, cx - 50 * k, rimY - 34 * k, 100 * k, 66 * k, 36, 108);
            }
            using (SolidBrush foot = new SolidBrush(Color.FromArgb(214, 190, 160)))
            {
                g.FillEllipse(foot, cx - 26 * k, baseY - 7 * k, 52 * k, 9 * k);
            }

            RectangleF rim = new RectangleF(cx - 72 * k, rimY - 14 * k, 144 * k, 28 * k);
            using (SolidBrush rimBrush = new SolidBrush(Color.FromArgb(255, 248, 232)))
            using (Pen rimPen = new Pen(Color.FromArgb(226, 206, 176), 1.5f * k))
            {
                g.FillEllipse(rimBrush, rim);
                g.DrawEllipse(rimPen, rim);
            }
            RectangleF broth = new RectangleF(rim.X + 7 * k, rim.Y + 3.5f * k, rim.Width - 14 * k, rim.Height - 7 * k);
            using (LinearGradientBrush brothBrush = B6Ui.Gradient(broth, Color.FromArgb(240, 168, 64), Color.FromArgb(206, 112, 36), 90f))
            {
                g.FillEllipse(brothBrush, broth);
            }

            using (Pen noodle = new Pen(Color.FromArgb(240, 255, 244, 214), 2f * k))
            {
                for (int i = 0; i < 4; i++)
                {
                    g.DrawArc(noodle, broth.X + 14 * k + i * 6 * k, broth.Y + 3 * k + i * 1.2f * k, broth.Width * 0.55f, broth.Height * 0.7f, 190 + i * 14, 150);
                }
            }
            using (SolidBrush herb = new SolidBrush(Color.FromArgb(94, 168, 80)))
            using (SolidBrush chili = new SolidBrush(Color.FromArgb(226, 60, 40)))
            using (SolidBrush beef = new SolidBrush(Color.FromArgb(150, 82, 60)))
            {
                g.FillEllipse(beef, broth.X + broth.Width * 0.58f, broth.Y + broth.Height * 0.25f, 24 * k, 10 * k);
                g.FillEllipse(beef, broth.X + broth.Width * 0.46f, broth.Y + broth.Height * 0.5f, 22 * k, 9 * k);
                for (int i = 0; i < 7; i++)
                {
                    g.FillEllipse(herb, broth.X + 22 * k + i * 11 * k, broth.Y + 5 * k + (i % 3) * 4 * k, 6 * k, 3.4f * k);
                }
                g.FillEllipse(chili, broth.X + broth.Width * 0.74f, broth.Y + broth.Height * 0.5f, 5 * k, 5 * k);
                g.FillEllipse(chili, broth.X + broth.Width * 0.8f, broth.Y + broth.Height * 0.36f, 4 * k, 4 * k);
            }
        }

        private void DrawParticle(Graphics g, Particle p, float k)
        {
            float s = p.Size * k;
            int a = (int)p.Alpha;
            GraphicsState state = g.Save();
            g.TranslateTransform(p.X, p.Y);
            g.RotateTransform(p.Rotation);

            switch (p.Kind)
            {
                case 0:
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        path.AddBezier(-s, 0, -s * 0.4f, -s * 0.5f, s * 0.5f, -s * 0.4f, s, s * 0.25f);
                        path.AddBezier(s, s * 0.25f, s * 0.3f, s * 0.25f, -s * 0.4f, s * 0.4f, -s, 0);
                        using (SolidBrush brush = new SolidBrush(Color.FromArgb(a, 226, 60, 40)))
                        using (Pen stem = new Pen(Color.FromArgb(a, 84, 160, 70), 2f * k))
                        {
                            g.FillPath(brush, path);
                            g.DrawLine(stem, -s, 0, -s - s * 0.3f, -s * 0.2f);
                        }
                    }
                    break;
                case 1:
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(a, 96, 176, 84)))
                    using (Pen vein = new Pen(Color.FromArgb(a, 220, 255, 210), 1f))
                    {
                        g.FillEllipse(brush, -s, -s * 0.42f, s * 2, s * 0.84f);
                        g.DrawLine(vein, -s * 0.9f, 0, s * 0.9f, 0);
                    }
                    break;
                case 2:
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(a, 230, 72, 52)))
                    using (SolidBrush leaf = new SolidBrush(Color.FromArgb(a, 90, 160, 70)))
                    {
                        g.FillEllipse(brush, -s * 0.65f, -s * 0.6f, s * 1.3f, s * 1.2f);
                        g.FillEllipse(leaf, -s * 0.3f, -s * 0.72f, s * 0.6f, s * 0.3f);
                    }
                    break;
                case 3:
                    using (SolidBrush rind = new SolidBrush(Color.FromArgb(a, 150, 200, 90)))
                    using (SolidBrush flesh = new SolidBrush(Color.FromArgb(a, 226, 240, 160)))
                    using (Pen seg = new Pen(Color.FromArgb(a, 170, 210, 100), 1f))
                    {
                        g.FillEllipse(rind, -s * 0.7f, -s * 0.7f, s * 1.4f, s * 1.4f);
                        g.FillEllipse(flesh, -s * 0.55f, -s * 0.55f, s * 1.1f, s * 1.1f);
                        g.DrawLine(seg, -s * 0.55f, 0, s * 0.55f, 0);
                        g.DrawLine(seg, 0, -s * 0.55f, 0, s * 0.55f);
                    }
                    break;
                default:
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(a, 255, 238, 200)))
                    {
                        g.FillEllipse(brush, -s * 0.5f, -s * 0.25f, s * 0.7f, s * 0.38f);
                        g.FillEllipse(brush, s * 0.1f, s * 0.15f, s * 0.7f, s * 0.38f);
                        g.FillEllipse(brush, -s * 0.2f, s * 0.45f, s * 0.7f, s * 0.38f);
                    }
                    break;
            }

            g.Restore(state);
        }
    }

    internal sealed class B6Images
    {
        private readonly FoodDatabase database;
        private readonly Dictionary<string, Bitmap> cache = new Dictionary<string, Bitmap>();
        private readonly Dictionary<int, Bitmap> sources = new Dictionary<int, Bitmap>();

        public B6Images(FoodDatabase database)
        {
            this.database = database;
        }

        private static string KeyOf(Dish dish, int width, int height)
        {
            return dish.Id + "|" + dish.ImagePath + "|" + width + "x" + height;
        }

        public Bitmap Get(Dish dish, int width, int height)
        {
            string key = KeyOf(dish, width, height);
            lock (cache)
            {
                Bitmap cached;
                if (cache.TryGetValue(key, out cached))
                {
                    return cached;
                }
            }

            Image source = null;
            bool shared = false;
            Bitmap thumb;
            try
            {
                source = AcquireSource(dish, out shared);
                thumb = FoodImageFactory.CreateThumbnail(source, Math.Max(1, width), Math.Max(1, height));
            }
            finally
            {
                if (source != null && !shared)
                {
                    source.Dispose();
                }
            }

            lock (cache)
            {
                if (cache.Count > 400)
                {
                    cache.Clear();
                }
                cache[key] = thumb;
            }
            return thumb;
        }

        private Image AcquireSource(Dish dish, out bool shared)
        {
            shared = false;
            if (dish.HasCustomImage)
            {
                Image custom = FoodImageFactory.LoadCopy(database.ResolveImagePath(dish.ImagePath));
                if (custom != null)
                {
                    return custom;
                }
            }

            lock (sources)
            {
                Bitmap held;
                if (sources.TryGetValue(dish.Id, out held))
                {
                    shared = true;
                    return held;
                }
            }

            Bitmap generated = FoodImageFactory.CreateGenerated(dish.Name, FoodImageFactory.StableSeed(dish.Name), dish.Category);
            lock (sources)
            {
                if (sources.Count > 40)
                {
                    sources.Clear();
                }
                sources[dish.Id] = generated;
            }
            shared = true;
            return generated;
        }

        public void Prefetch(IEnumerable<Dish> dishes, int width, int height)
        {
            List<Dish> list = new List<Dish>(dishes);
            Task.Run(() =>
            {
                foreach (Dish dish in list)
                {
                    if (dish.HasCustomImage) continue;
                    string key = KeyOf(dish, width, height);
                    bool known;
                    lock (cache)
                    {
                        known = cache.ContainsKey(key);
                    }
                    if (known) continue;

                    try
                    {
                        Bitmap thumb;
                        using (Bitmap generated = FoodImageFactory.CreateGenerated(dish.Name, FoodImageFactory.StableSeed(dish.Name), dish.Category))
                        {
                            thumb = FoodImageFactory.CreateThumbnail(generated, Math.Max(1, width), Math.Max(1, height));
                        }

                        lock (cache)
                        {
                            if (cache.ContainsKey(key)) thumb.Dispose(); else cache[key] = thumb;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            });
        }

        public Image LoadSource(Dish dish)
        {
            if (dish.HasCustomImage)
            {
                Image custom = FoodImageFactory.LoadCopy(database.ResolveImagePath(dish.ImagePath));
                if (custom != null)
                {
                    return custom;
                }
            }
            return FoodImageFactory.CreateGenerated(dish.Name, FoodImageFactory.StableSeed(dish.Name), dish.Category);
        }
    }

    [System.ComponentModel.DesignerCategory("Code")]
    [System.ComponentModel.ToolboxItem(false)]
    internal sealed class B6Stage : Control
    {
        private sealed class Confetti
        {
            public float X, Y, Vx, Vy, Size, Spin, Angle;
            public Color Color;
        }

        private static readonly Color[] ConfettiColors =
        {
            Color.FromArgb(214, 62, 38), Color.FromArgb(247, 168, 32), Color.FromArgb(70, 150, 84),
            Color.FromArgb(255, 214, 120), Color.FromArgb(240, 120, 150)
        };

        private readonly Timer timer = new Timer { Interval = 16 };
        private readonly Timer steamTimer = new Timer { Interval = 50 };
        private float steamTime;
        private readonly Stopwatch clock = new Stopwatch();
        private readonly List<Confetti> confetti = new List<Confetti>();
        private readonly Random random = new Random();
        private Image image;
        private string caption = string.Empty;
        private string hint = "Bấm \"QUAY MÓN\" để chọn món cho hôm nay";
        private float reveal = 1f;
        private float lastSeconds;
        private Color tint = B6Ui.Saffron;

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int Radius { get; set; } = 18;

        public B6Stage()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            timer.Tick += OnTick;
            steamTimer.Tick += (s, e) =>
            {
                steamTime += 0.05f;
                if (image != null && Visible && reveal >= 1f) Invalidate();
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                timer.Dispose();
                steamTimer.Dispose();
                if (image != null) image.Dispose();
            }
            base.Dispose(disposing);
        }

        public void Clear(string hintText)
        {
            SetImage(null);
            caption = string.Empty;
            hint = hintText;
            reveal = 1f;
            Invalidate();
        }

        public void Preview(Image newImage, string text)
        {
            SetImage(newImage);
            caption = text;
            reveal = 1f;
            Invalidate();
        }

        public void Reveal(Image newImage, string text, bool celebrate)
        {
            SetImage(newImage);
            caption = text;
            reveal = 0f;
            confetti.Clear();
            if (celebrate)
            {
                for (int i = 0; i < 46; i++)
                {
                    confetti.Add(new Confetti
                    {
                        X = Width / 2f,
                        Y = Height * 0.42f,
                        Vx = ((float)random.NextDouble() - 0.5f) * 11f,
                        Vy = -3f - (float)random.NextDouble() * 9f,
                        Size = 5 + random.Next(6),
                        Spin = ((float)random.NextDouble() - 0.5f) * 20f,
                        Angle = random.Next(360),
                        Color = ConfettiColors[random.Next(ConfettiColors.Length)]
                    });
                }
            }
            lastSeconds = 0;
            clock.Restart();
            timer.Start();
        }

        private void SetImage(Image newImage)
        {
            Image old = image;
            image = newImage == null ? null : new Bitmap(newImage);
            if (old != null) old.Dispose();
            if (image != null) steamTimer.Start(); else steamTimer.Stop();
        }

        private void OnTick(object sender, EventArgs e)
        {
            float seconds = (float)clock.Elapsed.TotalSeconds;
            float dt = Math.Min(0.05f, seconds - lastSeconds) * 60f;
            lastSeconds = seconds;

            reveal = Math.Min(1f, seconds / 0.65f);
            for (int i = confetti.Count - 1; i >= 0; i--)
            {
                Confetti c = confetti[i];
                c.X += c.Vx * dt;
                c.Y += c.Vy * dt;
                c.Vy += 0.35f * dt;
                c.Angle += c.Spin * dt;
                if (c.Y > Height + 20 || seconds > 2.6f)
                {
                    confetti.RemoveAt(i);
                }
            }

            if (reveal >= 1f && confetti.Count == 0)
            {
                timer.Stop();
                clock.Stop();
            }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width < 4 || Height < 4)
            {
                return;
            }

            Graphics g = e.Graphics;
            g.Clear(B6Ui.SurfaceOf(this));
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            RectangleF frame = new RectangleF(0, 0, Width - 1, Height - 1);
            float scale = 0.84f + 0.16f * B6Ui.EaseOutBack(reveal);
            float cx = frame.X + frame.Width / 2f;
            float cy = frame.Y + frame.Height / 2f;
            float flip = 0.08f + 0.92f * B6Ui.EaseOutCubic(reveal);
            RectangleF shown = new RectangleF(cx - frame.Width * scale * flip / 2f, cy - frame.Height * scale / 2f, frame.Width * scale * flip, frame.Height * scale);

            using (GraphicsPath path = B6Ui.RoundRect(shown, Radius))
            {
                GraphicsState state = g.Save();
                g.SetClip(path);

                if (image != null)
                {
                    float ratio = Math.Max(frame.Width / image.Width, frame.Height / image.Height) * scale;
                    float dw = image.Width * ratio * flip;
                    float dh = image.Height * ratio;
                    g.DrawImage(image, cx - dw / 2f, cy - dh / 2f, dw, dh);

                    if (!string.IsNullOrEmpty(caption))
                    {
                        RectangleF band = new RectangleF(shown.X, shown.Bottom - 46, shown.Width, 46);
                        using (LinearGradientBrush shade = B6Ui.Gradient(band, Color.FromArgb(0, 30, 14, 8), Color.FromArgb(190, 30, 14, 8), 90f))
                        using (Font font = new Font("Segoe UI", 13.5F, FontStyle.Bold))
                        using (StringFormat format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap })
                        {
                            g.FillRectangle(shade, band);
                            g.DrawString(caption, font, Brushes.White, new RectangleF(band.X + 10, band.Y + 6, band.Width - 20, band.Height - 6), format);
                        }
                    }
                }
                else
                {
                    using (LinearGradientBrush back = B6Ui.Gradient(shown, Color.FromArgb(255, 240, 214), Color.FromArgb(252, 214, 170), 90f))
                    {
                        g.FillRectangle(back, shown);
                    }
                    float d = Math.Min(shown.Width, shown.Height) * 0.42f;
                    RectangleF plate = new RectangleF(cx - d / 2f, cy - d / 2f - 14, d, d);
                    using (Pen pen = new Pen(Color.FromArgb(120, 214, 62, 38), 3f))
                    using (Pen inner = new Pen(Color.FromArgb(80, 214, 62, 38), 1.5f))
                    using (Font font = new Font("Segoe UI", 10.5F, FontStyle.Italic))
                    using (SolidBrush text = new SolidBrush(B6Ui.InkSoft))
                    using (StringFormat format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near })
                    {
                        g.DrawEllipse(pen, plate);
                        g.DrawEllipse(inner, plate.X + d * 0.12f, plate.Y + d * 0.12f, d * 0.76f, d * 0.76f);
                        g.DrawString(hint, font, text, new RectangleF(shown.X + 12, plate.Bottom + 12, shown.Width - 24, 50), format);
                    }
                }
                if (image != null && reveal >= 1f)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        float phase = (steamTime * 0.32f + i / 9f) % 1f;
                        float sway = (float)Math.Sin(phase * 6.0f + i * 1.7f) * frame.Width * (0.03f + 0.06f * phase);
                        float px = cx + (i - 4) * frame.Width * 0.035f + sway;
                        float py = cy + frame.Height * 0.12f - phase * frame.Height * 0.55f;
                        float pr = frame.Width * (0.03f + 0.06f * phase);
                        int pa = (int)(78f * (1f - phase) * Math.Min(1f, phase * 5f));
                        if (pa < 2) continue;
                        using (SolidBrush puff = new SolidBrush(Color.FromArgb(pa, 255, 252, 246)))
                        {
                            g.FillEllipse(puff, px - pr, py - pr * 0.85f, pr * 2f, pr * 1.7f);
                        }
                    }
                }
                g.Restore(state);
            }

            using (GraphicsPath edge = B6Ui.RoundRect(shown, Radius))
            using (Pen pen = new Pen(Color.FromArgb(70, 120, 60, 30), 1f))
            {
                g.DrawPath(pen, edge);
            }

            foreach (Confetti c in confetti)
            {
                GraphicsState s = g.Save();
                g.TranslateTransform(c.X, c.Y);
                g.RotateTransform(c.Angle);
                using (SolidBrush brush = new SolidBrush(c.Color))
                {
                    g.FillRectangle(brush, -c.Size / 2f, -c.Size / 4f, c.Size, c.Size / 2f);
                }
                g.Restore(s);
            }
        }
    }

    [System.ComponentModel.DesignerCategory("Code")]
    [System.ComponentModel.ToolboxItem(false)]
    internal sealed class B6DishCard : Control
    {
        private readonly B6Hover hover;
        private readonly Timer appearTimer = new Timer { Interval = 16 };
        private readonly Stopwatch appearClock = new Stopwatch();
        private Rectangle heartBounds;
        private Bitmap picture;
        private float appear = 1f;
        private int appearDelay;

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Dish Dish { get; private set; }
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool Favorite { get; set; }
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool Selected { get; set; }
        public event EventHandler CardClicked;
        public event EventHandler FavoriteClicked;

        public B6DishCard(Dish dish, Bitmap picture)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Dish = dish;
            this.picture = picture;
            Cursor = Cursors.Hand;
            hover = new B6Hover(this);
            appearTimer.Tick += (s, e) =>
            {
                float t = (appearClock.ElapsedMilliseconds - appearDelay) / 360f;
                appear = Math.Max(0f, Math.Min(1f, t));
                if (appear >= 1f)
                {
                    appearTimer.Stop();
                }
                Invalidate();
            };
        }

        public void StartAppear(int delayMilliseconds)
        {
            appear = 0f;
            appearDelay = delayMilliseconds;
            appearClock.Restart();
            appearTimer.Start();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                hover.Dispose();
                appearTimer.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            EventHandler handler = heartBounds.Contains(e.Location) ? FavoriteClicked : CardClicked;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width < 4 || Height < 4)
            {
                return;
            }

            Graphics g = e.Graphics;
            float k = B6Ui.Scale(this);
            g.Clear(B6Ui.SurfaceOf(this));
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            if (appear <= 0f)
            {
                return;
            }

            float lift = hover.T * 4f * k;
            float slide = (1f - B6Ui.EaseOutCubic(appear)) * 28f * k;
            RectangleF card = new RectangleF(2 * k, 6 * k - lift + slide, Width - 4 * k, Height - 10 * k - slide);

            using (GraphicsPath shadow = B6Ui.RoundRect(new RectangleF(card.X, card.Y + 4 * k + lift, card.Width, card.Height), 16 * k))
            using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb((int)(40 + 30 * hover.T), 0, 0, 0)))
            {
                g.FillPath(shadowBrush, shadow);
            }

            using (GraphicsPath path = B6Ui.RoundRect(card, 16 * k))
            using (SolidBrush back = new SolidBrush(B6Ui.NightCard2))
            using (Pen border = new Pen(Selected ? B6Ui.Gold : B6Ui.Lerp(B6Ui.NightLine, B6Ui.GoldDark, hover.T), Selected ? 2f : 1.2f))
            {
                g.FillPath(back, path);

                GraphicsState state = g.Save();
                g.SetClip(path);
                RectangleF pic = new RectangleF(card.X, card.Y, card.Width, card.Height * 0.52f);
                if (picture != null)
                {
                    float ratio = Math.Max(pic.Width / picture.Width, pic.Height / picture.Height) * (1f + 0.04f * hover.T);
                    float dw = picture.Width * ratio;
                    float dh = picture.Height * ratio;
                    g.DrawImage(picture, pic.X + (pic.Width - dw) / 2f, pic.Y + (pic.Height - dh) / 2f, dw, dh);
                }
                using (LinearGradientBrush fade = B6Ui.Gradient(new RectangleF(pic.X, pic.Bottom - 40 * k, pic.Width, 41 * k), Color.FromArgb(0, B6Ui.NightCard2), B6Ui.NightCard2, 90f))
                {
                    g.FillRectangle(fade, pic.X, pic.Bottom - 40 * k, pic.Width, 41 * k);
                }
                g.Restore(state);
                g.DrawPath(border, path);

                using (Font chipFont = new Font("Segoe UI", 10 * k, FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    SizeF size = g.MeasureString(Dish.Category, chipFont);
                    RectangleF chip = new RectangleF(card.X + 10 * k, pic.Bottom - 26 * k, size.Width + 14 * k, 20 * k);
                    using (GraphicsPath chipPath = B6Ui.RoundRect(chip, 10 * k))
                    using (SolidBrush chipBack = new SolidBrush(Color.FromArgb(220, B6Ui.Night)))
                    using (SolidBrush chipText = new SolidBrush(B6Ui.Gold))
                    {
                        g.FillPath(chipBack, chipPath);
                        g.DrawString(Dish.Category, chipFont, chipText, chip.X + 7 * k, chip.Y + 3 * k);
                    }
                }

                float heartSize = 30 * k;
                RectangleF heart = new RectangleF(card.Right - heartSize - 8 * k, card.Y + 8 * k, heartSize, heartSize);
                heartBounds = Rectangle.Round(heart);
                using (SolidBrush circle = new SolidBrush(Color.FromArgb(170, B6Ui.Night)))
                {
                    g.FillEllipse(circle, heart);
                }
                DrawHeart(g, heart.X + heart.Width / 2f, heart.Y + heart.Height / 2f + 1 * k, 9 * k, Favorite);

                using (Font nameFont = new Font("Segoe UI", 13 * k, FontStyle.Bold, GraphicsUnit.Pixel))
                using (SolidBrush nameBrush = new SolidBrush(B6Ui.Cream))
                using (StringFormat format = new StringFormat { Trimming = StringTrimming.EllipsisWord })
                {
                    g.DrawString(Dish.Name, nameFont, nameBrush, new RectangleF(card.X + 12 * k, pic.Bottom + 4 * k, card.Width - 24 * k, 38 * k), format);
                }

                using (Font infoFont = new Font("Segoe UI", 11.5f * k, FontStyle.Regular, GraphicsUnit.Pixel))
                using (Font priceFont = new Font("Segoe UI", 13 * k, FontStyle.Bold, GraphicsUnit.Pixel))
                using (SolidBrush priceBrush = new SolidBrush(B6Ui.Gold))
                using (SolidBrush muted = new SolidBrush(B6Ui.CreamMuted))
                {
                    float y = card.Bottom - 46 * k;
                    g.DrawString(B6Ui.FormatPrice(Dish.Price), priceFont, priceBrush, card.X + 12 * k, y);
                    string region = string.IsNullOrEmpty(Dish.Region) ? string.Empty : Dish.Region;
                    SizeF regionSize = g.MeasureString(region, infoFont);
                    g.DrawString(region, infoFont, muted, card.Right - regionSize.Width - 12 * k, y + 1.5f * k);
                }

                using (Font dotFont = new Font("Segoe UI", 9 * k, FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    string[] letters = { "S", "T", "C", "T" };
                    for (int i = 0; i < 4; i++)
                    {
                        bool ok = Dish.SuitsMeal(B6Ui.Meals[i]);
                        RectangleF dot = new RectangleF(card.X + 12 * k + i * 22 * k, card.Bottom - 22 * k, 18 * k, 14 * k);
                        using (GraphicsPath dotPath = B6Ui.RoundRect(dot, 7 * k))
                        using (SolidBrush dotBack = new SolidBrush(ok ? Color.FromArgb(70, B6Ui.Gold) : Color.FromArgb(40, 255, 255, 255)))
                        using (SolidBrush dotText = new SolidBrush(ok ? B6Ui.Gold : B6Ui.CreamMuted))
                        using (StringFormat center = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            g.FillPath(dotBack, dotPath);
                            g.DrawString(letters[i], dotFont, dotText, dot, center);
                        }
                    }
                }
            }
        }

        private static void DrawHeart(Graphics g, float cx, float cy, float size, bool filled)
        {
            using (GraphicsPath heart = new GraphicsPath())
            {
                heart.AddBezier(cx, cy + size, cx - size * 1.7f, cy - size * 0.1f, cx - size * 0.9f, cy - size * 1.5f, cx, cy - size * 0.55f);
                heart.AddBezier(cx, cy - size * 0.55f, cx + size * 0.9f, cy - size * 1.5f, cx + size * 1.7f, cy - size * 0.1f, cx, cy + size);
                if (filled)
                {
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(236, 72, 84)))
                    {
                        g.FillPath(brush, heart);
                    }
                }
                else
                {
                    using (Pen pen = new Pen(Color.FromArgb(230, 250, 238, 218), 1.6f))
                    {
                        g.DrawPath(pen, heart);
                    }
                }
            }
        }
    }

    [System.ComponentModel.DesignerCategory("Code")]
    [System.ComponentModel.ToolboxItem(false)]
    internal sealed class B6Bars : Control
    {
        private readonly Timer timer = new Timer { Interval = 16 };
        private readonly List<KeyValuePair<string, int>> data = new List<KeyValuePair<string, int>>();
        private float progress = 1f;

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Color TextColor { get; set; } = B6Ui.Cream;
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Color BarColor { get; set; } = B6Ui.Gold;
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Color TrackColor { get; set; } = B6Ui.NightCard2;

        public B6Bars()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            timer.Tick += (s, e) =>
            {
                progress = Math.Min(1f, progress + 0.06f);
                if (progress >= 1f) timer.Stop();
                Invalidate();
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) timer.Dispose();
            base.Dispose(disposing);
        }

        public void SetData(IEnumerable<KeyValuePair<string, int>> items)
        {
            data.Clear();
            data.AddRange(items);
            progress = 0f;
            timer.Start();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width < 4 || Height < 4)
            {
                return;
            }

            Graphics g = e.Graphics;
            g.Clear(B6Ui.SurfaceOf(this));
            g.SmoothingMode = SmoothingMode.AntiAlias;
            if (data.Count == 0) return;

            float k = B6Ui.Scale(this);
            int max = 1;
            foreach (KeyValuePair<string, int> item in data) max = Math.Max(max, item.Value);

            float rowHeight = Math.Min(34 * k, (Height - 4) / (float)data.Count);
            float labelWidth = Width * 0.34f;
            using (Font font = new Font("Segoe UI", 11.5f * k, FontStyle.Regular, GraphicsUnit.Pixel))
            using (SolidBrush textBrush = new SolidBrush(TextColor))
            using (StringFormat format = new StringFormat { Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap, LineAlignment = StringAlignment.Center })
            {
                for (int i = 0; i < data.Count; i++)
                {
                    float y = 2 + i * rowHeight;
                    g.DrawString(data[i].Key, font, textBrush, new RectangleF(0, y, labelWidth - 8 * k, rowHeight), format);

                    RectangleF track = new RectangleF(labelWidth, y + rowHeight * 0.28f, Width - labelWidth - 44 * k, rowHeight * 0.44f);
                    using (GraphicsPath trackPath = B6Ui.RoundRect(track, track.Height / 2f))
                    using (SolidBrush trackBrush = new SolidBrush(TrackColor))
                    {
                        g.FillPath(trackBrush, trackPath);
                    }

                    float fill = track.Width * (data[i].Value / (float)max) * B6Ui.EaseOutCubic(progress);
                    if (fill > 1f)
                    {
                        RectangleF bar = new RectangleF(track.X, track.Y, Math.Max(track.Height, fill), track.Height);
                        using (GraphicsPath barPath = B6Ui.RoundRect(bar, bar.Height / 2f))
                        using (SolidBrush barBrush = new SolidBrush(BarColor))
                        {
                            g.FillPath(barBrush, barPath);
                        }
                    }

                    using (StringFormat right = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(data[i].Value.ToString(), font, textBrush, new RectangleF(Width - 40 * k, y, 40 * k, rowHeight), right);
                    }
                }
            }
        }
    }

    internal sealed class B6Fortune
    {
        public int Number { get; set; }
        public string Title { get; set; }
        public string Level { get; set; }
        public string Element { get; set; }
        public string PoemLine1 { get; set; }
        public string PoemLine2 { get; set; }
        public string Blessing { get; set; }

        public string PreferredCategory
        {
            get
            {
                switch (Element)
                {
                    case "Hỏa": return "Lẩu & nướng";
                    case "Thủy": return "Đồ uống";
                    case "Kim": return "Ăn vặt";
                    case "Thổ": return "Tráng miệng";
                    default: return "Món chính";
                }
            }
        }

        public string ElementText
        {
            get { return "Mệnh " + Element; }
        }
    }

    internal static class B6Fortunes
    {
        private static readonly Random Dice = new Random();

        private static B6Fortune Make(string title, string level, string element, string line1, string line2, string blessing)
        {
            return new B6Fortune { Title = title, Level = level, Element = element, PoemLine1 = line1, PoemLine2 = line2, Blessing = blessing };
        }

        public static readonly B6Fortune[] All = Number(new[]
        {
            Make("Quẻ Hương Khói", "Đại cát", "Hỏa", "Bếp hồng đỏ lửa canh khuya,", "Mùi thơm gọi bạn sum vầy bên nhau.", "Hôm nay lộc đến đầy mâm, ăn ngon vui khỏe, mọi sự hanh thông."),
            Make("Quẻ Bát Đầy", "Đại cát", "Mộc", "Bát đầy hương lúa mới thơm,", "Một ngày no ấm, niềm vui đong đầy.", "Chúc bạn cơm no lòng nhẹ, việc gì cũng suôn sẻ như sợi bún mềm."),
            Make("Quẻ Trăng Chợ Đêm", "Đại cát", "Kim", "Chợ đêm đèn sáng lung linh,", "Rong chơi một vòng, lộc lành theo chân.", "Chúc bạn gặp món ngon bất ngờ và niềm vui ghé đến mỗi góc phố."),
            Make("Quẻ Lửa Hồng", "Đại cát", "Hỏa", "Than hồng nướng thịt thơm lừng,", "Quây quần bè bạn tưng bừng đón xuân.", "Chúc bạn có người bên cạnh sẻ chia, bữa ăn nào cũng rộn tiếng cười."),
            Make("Quẻ Mâm Cỗ Vàng", "Đại cát", "Thổ", "Mâm vàng cỗ đẹp bày ra,", "Ngọt bùi hội tụ, cả nhà bình an.", "Hôm nay tài lộc nở hoa, ăn gì cũng thấy ngon và đúng ý."),
            Make("Quẻ Suối Nguồn", "Đại cát", "Thủy", "Ly trong mát lạnh đầu ngày,", "Tinh thần sảng khoái, gió bay lo phiền.", "Chúc bạn đầu óc sáng trong, quyết định nào cũng chín như trái cây mùa nắng."),

            Make("Quẻ Giọt Mát", "Trung cát", "Thủy", "Ly trong đá lạnh tan dần,", "Nắng trưa dịu lại, tâm thần nhẹ tênh.", "Chúc bạn tâm an đầu mát, mọi lo toan tan như viên đá."),
            Make("Quẻ Ngọt Lành", "Trung cát", "Thổ", "Chén chè ngọt dịu vừa trao,", "Nhân duyên may mắn ngọt ngào đến nơi.", "Chúc bạn một ngày ngọt ngào, gặp người tử tế, nhận tin vui."),
            Make("Quẻ Gió Xuân", "Trung cát", "Mộc", "Rau xanh mướt lá đầu mùa,", "Canh thanh vị nhẹ, lòng vừa bình yên.", "Chúc bạn sức khỏe dồi dào, tinh thần tươi như rau mới hái."),
            Make("Quẻ Giòn Tan", "Trung cát", "Kim", "Miếng ngon giòn rụm bên đường,", "Niềm vui nhỏ bé cũng thương vô vàn.", "Chúc bạn trân trọng niềm vui nhỏ, vì chính chúng làm nên ngày rạng rỡ."),
            Make("Quẻ Khói Bếp Quê", "Trung cát", "Hỏa", "Khói chiều nhè nhẹ mái tranh,", "Nồi canh dở bữa, tình thân đậm đà.", "Chúc bạn sớm về nhà, gặp bữa cơm ấm và những lời hỏi han."),
            Make("Quẻ Hạt Gạo Thơm", "Trung cát", "Mộc", "Hạt cơm dẻo, hạt gạo thơm,", "Từng hạt chắt chiu, nên cơm nên người.", "Chúc bạn kiên trì từng chút, công sức hôm nay sẽ chín muồi."),

            Make("Quẻ Sợi Bún Dài", "Tiểu cát", "Mộc", "Sợi bún mảnh nhưng dài lâu,", "Chậm mà chắc bước, đường sau thênh thang.", "Chúc bạn đừng vội, mọi điều tốt đẹp đều đang trên đường tới."),
            Make("Quẻ Nước Dùng Trong", "Tiểu cát", "Thủy", "Nước dùng ninh suốt canh dài,", "Kiên nhẫn mới có ngày ngọt thanh.", "Chúc bạn đủ bình tĩnh để chờ điều hay, và đủ ấm để chia cho người khác."),
            Make("Quẻ Nắng Quán Nhỏ", "Tiểu cát", "Kim", "Quán nhỏ bên phố quen quen,", "Một dĩa bình dị mà thương vô cùng.", "Chúc bạn tìm được góc ngồi yên tĩnh và một món ăn làm lòng dịu lại."),
            Make("Quẻ Mật Ong", "Tiểu cát", "Thổ", "Ngọt từ hoa nắng gom về,", "Chút ngọt nhỏ đủ vỗ về ngày xa.", "Chúc bạn có một khoảnh khắc ngọt ngào ngay hôm nay, dù chỉ nhỏ thôi."),
            Make("Quẻ Bếp Than Đỏ", "Tiểu cát", "Hỏa", "Lửa reo tí tách trong đêm,", "Thịt thơm cháy cạnh, bạn hiền bên nhau.", "Chúc bạn đêm nay vui vẻ, hẹn hò bạn cũ, chuyện cũ kể hoài chưa hết."),
            Make("Quẻ Gió Thoảng", "Tiểu cát", "Thủy", "Gió lùa qua ly trà xanh,", "Hương nhài thoang thoảng, lòng thanh nhẹ nhàng.", "Chúc bạn một buổi thư thả, bớt vội, bớt lo và nhiều tiếng cười."),

            Make("Quẻ Bình An", "Bình an", "Mộc", "Ngày thường cũng quý biết bao,", "Cơm canh đạm bạc, tâm hồn ấm êm.", "Chúc bạn bình an giản dị, đủ đầy trong những điều rất nhỏ."),
            Make("Quẻ Đũa Sẻ Chia", "Bình an", "Kim", "Đôi đũa gắp miếng cho nhau,", "Chẳng cần mỹ vị, lòng vui tự nhiên.", "Chúc bạn có người cùng gắp thức ăn, cùng sẻ buồn vui."),
            Make("Quẻ Ly Trà Chiều", "Bình an", "Thủy", "Chiều xuống một tách trà thơm,", "Thong dong nhìn phố, lòng thêm dịu dàng.", "Chúc bạn gác bớt bộn bề, dành vài phút cho chính mình."),
            Make("Quẻ Chén Chè Nhỏ", "Bình an", "Thổ", "Chén chè nho nhỏ cuối ngày,", "Ngọt vừa đủ để mai này thêm vui.", "Chúc bạn khép ngày nhẹ nhõm, mai thức dậy với nụ cười."),
            Make("Quẻ Lửa Ấm", "Bình an", "Hỏa", "Nồi lẩu nghi ngút khói bay,", "Mùa đông cũng ấm vì ngày có nhau.", "Chúc bạn luôn có một nơi để về, một mâm cơm để ngồi."),
            Make("Quẻ Nắng Mới", "Bình an", "Mộc", "Nắng lên bát cháo hành thơm,", "Ngày mới mở cửa, niềm vui đầy lòng.", "Chúc bạn khởi đầu nhẹ nhàng, ngày mới thuận chiều gió xuôi.")
        });

        private static B6Fortune[] Number(B6Fortune[] cards)
        {

            int[] numbers = { 1, 3, 5, 7, 9, 12, 14, 16, 18, 21, 23, 25, 28, 30, 33, 36, 38, 41, 44, 47, 50, 55, 66, 88 };
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i].Number = numbers[i % numbers.Length];
            }
            return cards;
        }

        public static B6Fortune Draw()
        {
            return All[Dice.Next(All.Length)];
        }
    }

    [System.ComponentModel.DesignerCategory("Code")]
    [System.ComponentModel.ToolboxItem(false)]
    internal sealed class B6FortuneStage : Control
    {
        private enum Phase { Idle, Shake, Eject, Flip, Done }

        private sealed class Spark
        {
            public float X, Y, Vx, Vy, Life, Size;
        }

        private const float ShakeSeconds = 2.4f;
        private const float EjectSeconds = 0.9f;
        private const float FlipSeconds = 0.85f;

        private readonly Timer timer = new Timer { Interval = 16 };
        private readonly Stopwatch clock = Stopwatch.StartNew();
        private readonly Random random = new Random();
        private readonly List<Spark> sparks = new List<Spark>();
        private readonly float[] stickOffsets = new float[14];
        private readonly float[] stickTilts = new float[14];

        private Phase phase = Phase.Idle;
        private float phaseStart;
        private float now;
        private B6Fortune card;
        private string dishLine = string.Empty;
        private bool soundPlayed;

        public event EventHandler Completed;

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool IsBusy
        {
            get { return phase == Phase.Shake || phase == Phase.Eject || phase == Phase.Flip; }
        }

        public B6FortuneStage()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            for (int i = 0; i < stickOffsets.Length; i++)
            {
                stickOffsets[i] = -50f + i * (100f / (stickOffsets.Length - 1));
                stickTilts[i] = ((float)random.NextDouble() - 0.5f) * 16f;
            }
            timer.Tick += OnTick;
            timer.Start();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) timer.Dispose();
            base.Dispose(disposing);
        }

        public void Begin(B6Fortune chosen, string dishText)
        {
            if (IsBusy) return;
            card = chosen;
            dishLine = dishText;
            soundPlayed = false;
            sparks.Clear();
            SetPhase(Phase.Shake);
        }

        public void Reset()
        {
            if (IsBusy) return;
            card = null;
            sparks.Clear();
            SetPhase(Phase.Idle);
        }

        private void SetPhase(Phase next)
        {
            phase = next;
            phaseStart = (float)clock.Elapsed.TotalSeconds;
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (DesignMode || !Visible) return;

            now = (float)clock.Elapsed.TotalSeconds;
            float elapsed = now - phaseStart;

            if (phase == Phase.Shake && elapsed >= ShakeSeconds) SetPhase(Phase.Eject);
            else if (phase == Phase.Eject && elapsed >= EjectSeconds) SetPhase(Phase.Flip);
            else if (phase == Phase.Flip)
            {
                if (!soundPlayed && elapsed > FlipSeconds * 0.5f)
                {
                    soundPlayed = true;
                    BurstSparks();
                    try { System.Media.SystemSounds.Asterisk.Play(); } catch (Exception) { }
                }
                if (elapsed >= FlipSeconds)
                {
                    SetPhase(Phase.Done);
                    EventHandler handler = Completed;
                    if (handler != null) handler(this, EventArgs.Empty);
                }
            }

            for (int i = sparks.Count - 1; i >= 0; i--)
            {
                Spark s = sparks[i];
                s.X += s.Vx;
                s.Y += s.Vy;
                s.Vy += 0.04f;
                s.Life -= 0.014f;
                if (s.Life <= 0) sparks.RemoveAt(i);
            }

            Invalidate();
        }

        private void BurstSparks()
        {
            float cx = Width * 0.6f;
            float cy = Height * 0.45f;
            for (int i = 0; i < 40; i++)
            {
                double angle = random.NextDouble() * Math.PI * 2;
                float speed = 1f + (float)random.NextDouble() * 3.4f;
                sparks.Add(new Spark
                {
                    X = cx,
                    Y = cy,
                    Vx = (float)Math.Cos(angle) * speed,
                    Vy = (float)Math.Sin(angle) * speed - 1f,
                    Life = 0.7f + (float)random.NextDouble() * 0.5f,
                    Size = 3 + random.Next(5)
                });
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width < 4 || Height < 4)
            {
                return;
            }

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.Clear(B6Ui.SurfaceOf(this));

            float u = Math.Max(0.5f, Math.Min(Width / 540f, Height / 470f));
            float elapsed = now - phaseStart;

            float moveT = 0f;
            if (phase == Phase.Flip) moveT = B6Ui.EaseOutCubic(elapsed / FlipSeconds);
            else if (phase == Phase.Done) moveT = 1f;

            DrawBackdrop(g, u, moveT);

            float tubeScale = 1f - 0.34f * moveT;
            float tubeX = Width * (0.5f - 0.33f * moveT);
            float tubeY = Height * 0.88f;
            DrawTube(g, tubeX, tubeY, u * tubeScale, elapsed);

            if (phase == Phase.Idle)
            {
                DrawCenterText(g, u, "Thành tâm khấn nguyện, rồi bấm LẮC QUẺ", Height * 0.1f);
            }
            else if (phase == Phase.Shake)
            {
                DrawCenterText(g, u, "Đang lắc ống xăm…", Height * 0.1f);
            }

            if (card != null && (phase == Phase.Flip || phase == Phase.Done))
            {
                float flip = phase == Phase.Flip ? B6Ui.EaseOutBack(elapsed / FlipSeconds) : 1f;
                DrawCard(g, Width * 0.62f, Height * 0.48f, u, Math.Max(0.02f, Math.Min(1.04f, flip)));
            }

            foreach (Spark s in sparks)
            {
                DrawSpark(g, s);
            }
        }

        private void DrawBackdrop(Graphics g, float u, float moveT)
        {
            float cx = Width * (0.5f + 0.1f * moveT);
            float cy = Height * 0.5f;
            using (GraphicsPath path = new GraphicsPath())
            {
                float r = Math.Max(Width, Height) * 0.55f;
                path.AddEllipse(cx - r, cy - r, r * 2, r * 2);
                using (PathGradientBrush glow = new PathGradientBrush(path))
                {
                    glow.CenterColor = Color.FromArgb(64, B6Ui.Gold);
                    glow.SurroundColors = new[] { Color.FromArgb(0, B6Ui.Gold) };
                    g.FillPath(glow, path);
                }
            }
        }

        private void DrawCenterText(Graphics g, float u, string text, float y)
        {
            using (Font font = new Font("Palatino Linotype", 16 * u, FontStyle.Italic, GraphicsUnit.Pixel))
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(220, B6Ui.Cream)))
            using (StringFormat format = new StringFormat { Alignment = StringAlignment.Center })
            {
                g.DrawString(text, font, brush, new RectangleF(0, y, Width, 30 * u), format);
            }
        }

        private void DrawTube(Graphics g, float x, float y, float u, float elapsed)
        {
            float angle = 0f;
            float rattle = 0f;
            float ejectT = 0f;

            if (phase == Phase.Idle)
            {
                angle = (float)Math.Sin(now * 1.6f) * 1.5f;
            }
            else if (phase == Phase.Shake)
            {
                float envelope = (float)Math.Sin(Math.Min(1f, elapsed / ShakeSeconds) * Math.PI);
                envelope = Math.Min(1f, envelope * 1.6f);
                angle = (float)Math.Sin(elapsed * 30f) * 13f * envelope;
                rattle = envelope;
            }
            else if (phase == Phase.Eject)
            {
                ejectT = B6Ui.EaseOutCubic(elapsed / EjectSeconds);
                angle = (float)Math.Sin(elapsed * 18f) * 3f * (1f - ejectT);
            }
            else
            {
                ejectT = 1f;
            }

            GraphicsState state = g.Save();
            g.TranslateTransform(x, y);
            g.RotateTransform(angle);

            float tubeW = 132 * u;
            float tubeH = 200 * u;

            using (SolidBrush mouth = new SolidBrush(Color.FromArgb(40, 16, 12)))
            {
                g.FillEllipse(mouth, -tubeW / 2f, -tubeH - 9 * u, tubeW, 18 * u);
            }

            for (int i = 0; i < stickOffsets.Length; i++)
            {
                bool chosen = i == stickOffsets.Length / 2;
                if (chosen && (phase == Phase.Eject || phase == Phase.Flip || phase == Phase.Done))
                {
                    continue;
                }

                float bounce = (float)Math.Sin(elapsed * 34f + i * 1.9f) * 9f * u * rattle;
                float lift = (phase == Phase.Idle ? (float)Math.Sin(now * 2f + i) * 1.2f * u : 0f);
                DrawStick(g, stickOffsets[i] * u, -126 * u + bounce + lift, stickTilts[i] + (float)Math.Sin(elapsed * 25f + i) * 2f * rattle, u, 150 * u, null);
            }

            if (card != null && (phase == Phase.Eject || phase == Phase.Flip || phase == Phase.Done))
            {
                float rise = ejectT * 150 * u;
                float fade = phase == Phase.Done ? 0f : (phase == Phase.Flip ? 1f - Math.Min(1f, (now - phaseStart) / FlipSeconds) : 1f);
                if (fade > 0.02f)
                {
                    int stickIndex = stickOffsets.Length / 2;
                    DrawStick(g, stickOffsets[stickIndex] * u, -126 * u - rise, stickTilts[stickIndex] * (1f - ejectT), u, 150 * u, "SỐ " + card.Number.ToString("00"), fade);
                }
            }

            RectangleF body = new RectangleF(-tubeW / 2f, -tubeH, tubeW, tubeH);
            using (GraphicsPath path = B6Ui.RoundRect(body, 14 * u))
            using (LinearGradientBrush wood = B6Ui.Gradient(body, Color.FromArgb(168, 52, 38), Color.FromArgb(104, 26, 22), 0f))
            {
                ColorBlend blend = new ColorBlend(4);
                blend.Colors = new[] { Color.FromArgb(96, 24, 20), Color.FromArgb(188, 64, 46), Color.FromArgb(150, 44, 32), Color.FromArgb(88, 20, 18) };
                blend.Positions = new[] { 0f, 0.3f, 0.7f, 1f };
                wood.InterpolationColors = blend;
                g.FillPath(wood, path);
                using (Pen outline = new Pen(Color.FromArgb(70, 12, 8), 2f))
                {
                    g.DrawPath(outline, path);
                }
            }

            using (SolidBrush gold = new SolidBrush(B6Ui.Gold))
            {
                g.FillRectangle(gold, -tubeW / 2f, -tubeH + 16 * u, tubeW, 7 * u);
                g.FillRectangle(gold, -tubeW / 2f, -30 * u, tubeW, 7 * u);
            }
            RectangleF medal = new RectangleF(-30 * u, -tubeH / 2f - 38 * u, 60 * u, 60 * u);
            using (SolidBrush medalBack = new SolidBrush(Color.FromArgb(240, 214, 150)))
            using (Pen medalRing = new Pen(B6Ui.GoldDark, 2.5f * u))
            using (Font font = new Font("Palatino Linotype", 17 * u, FontStyle.Bold, GraphicsUnit.Pixel))
            using (SolidBrush ink = new SolidBrush(Color.FromArgb(140, 30, 24)))
            using (StringFormat center = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.FillEllipse(medalBack, medal);
                g.DrawEllipse(medalRing, medal);
                g.DrawString("LỘC", font, ink, medal, center);
            }

            g.Restore(state);
        }

        private void DrawStick(Graphics g, float x, float y, float tilt, float u, float length, string label, float alpha = 1f)
        {
            GraphicsState state = g.Save();
            g.TranslateTransform(x, y);
            g.RotateTransform(tilt);

            float width = (label == null ? 9f : 15f) * u;
            RectangleF stick = new RectangleF(-width / 2f, -length, width, length);
            int a = (int)(255 * Math.Max(0f, Math.Min(1f, alpha)));
            using (GraphicsPath path = B6Ui.RoundRect(stick, 3 * u))
            using (LinearGradientBrush wood = B6Ui.Gradient(stick, Color.FromArgb(a, 238, 204, 150), Color.FromArgb(a, 206, 164, 104), 0f))
            using (SolidBrush tip = new SolidBrush(Color.FromArgb(a, 190, 40, 40)))
            {
                g.FillPath(wood, path);
                g.FillRectangle(tip, stick.X, stick.Y, stick.Width, 20 * u);
            }

            if (label != null)
            {
                GraphicsState inner = g.Save();
                g.TranslateTransform(0, -length / 2f);
                g.RotateTransform(-90f);
                using (Font font = new Font("Segoe UI", 11 * u, FontStyle.Bold, GraphicsUnit.Pixel))
                using (SolidBrush ink = new SolidBrush(Color.FromArgb(a, 120, 28, 24)))
                using (StringFormat center = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(label, font, ink, new RectangleF(-40 * u, -8 * u, 80 * u, 16 * u), center);
                }
                g.Restore(inner);
            }

            g.Restore(state);
        }

        private void DrawCard(Graphics g, float cx, float cy, float u, float scaleX)
        {
            float w = 316 * u;
            float h = 382 * u;

            GraphicsState state = g.Save();
            g.TranslateTransform(cx, cy);
            g.ScaleTransform(scaleX, 1f);
            RectangleF rect = new RectangleF(-w / 2f, -h / 2f, w, h);

            using (GraphicsPath shadow = B6Ui.RoundRect(new RectangleF(rect.X + 4 * u, rect.Y + 8 * u, rect.Width, rect.Height), 16 * u))
            using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(90, 0, 0, 0)))
            {
                g.FillPath(shadowBrush, shadow);
            }

            using (GraphicsPath path = B6Ui.RoundRect(rect, 16 * u))
            using (LinearGradientBrush paper = B6Ui.Gradient(rect, Color.FromArgb(255, 247, 228), Color.FromArgb(244, 224, 188), 90f))
            using (Pen gold = new Pen(B6Ui.GoldDark, 3f * u))
            using (Pen thin = new Pen(Color.FromArgb(150, 205, 156, 52), 1.2f * u))
            {
                g.FillPath(paper, path);
                g.DrawPath(gold, path);
                RectangleF inner = rect;
                inner.Inflate(-10 * u, -10 * u);
                using (GraphicsPath innerPath = B6Ui.RoundRect(inner, 10 * u))
                {
                    g.DrawPath(thin, innerPath);
                }
            }

            Color red = Color.FromArgb(150, 34, 28);
            Color ink = Color.FromArgb(78, 48, 34);
            using (Font small = new Font("Segoe UI", 11.5f * u, FontStyle.Bold, GraphicsUnit.Pixel))
            using (Font titleFont = new Font("Palatino Linotype", 26 * u, FontStyle.Bold, GraphicsUnit.Pixel))
            using (Font poemFont = new Font("Palatino Linotype", 15.5f * u, FontStyle.Italic, GraphicsUnit.Pixel))
            using (Font blessFont = new Font("Segoe UI", 12.5f * u, FontStyle.Regular, GraphicsUnit.Pixel))
            using (Font dishFont = new Font("Segoe UI", 14 * u, FontStyle.Bold, GraphicsUnit.Pixel))
            using (SolidBrush redBrush = new SolidBrush(red))
            using (SolidBrush inkBrush = new SolidBrush(ink))
            using (StringFormat center = new StringFormat { Alignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisWord })
            {
                float y = rect.Y + 24 * u;
                g.DrawString("QUẺ SỐ " + card.Number.ToString("00"), small, redBrush, new RectangleF(rect.X, y, rect.Width, 20 * u), center);
                y += 24 * u;
                g.DrawString(card.Title, titleFont, redBrush, new RectangleF(rect.X + 8 * u, y, rect.Width - 16 * u, 38 * u), center);
                y += 46 * u;

                string badge = card.Level.ToUpperInvariant() + "  ·  " + card.ElementText.ToUpperInvariant();
                SizeF badgeSize = g.MeasureString(badge, small);
                RectangleF pill = new RectangleF(-badgeSize.Width / 2f - 12 * u, y, badgeSize.Width + 24 * u, 24 * u);
                pill.X += rect.X + rect.Width / 2f;
                using (GraphicsPath pillPath = B6Ui.RoundRect(pill, 12 * u))
                using (SolidBrush pillBack = new SolidBrush(red))
                using (SolidBrush pillText = new SolidBrush(Color.FromArgb(255, 238, 200)))
                {
                    g.FillPath(pillBack, pillPath);
                    g.DrawString(badge, small, pillText, pill.X + 12 * u, pill.Y + 4 * u);
                }
                y += 40 * u;

                using (Pen divider = new Pen(Color.FromArgb(150, 205, 156, 52), 1.2f * u))
                {
                    g.DrawLine(divider, rect.X + 60 * u, y, rect.Right - 60 * u, y);
                }
                y += 12 * u;

                g.DrawString(card.PoemLine1, poemFont, inkBrush, new RectangleF(rect.X + 18 * u, y, rect.Width - 36 * u, 24 * u), center);
                y += 24 * u;
                g.DrawString(card.PoemLine2, poemFont, inkBrush, new RectangleF(rect.X + 18 * u, y, rect.Width - 36 * u, 40 * u), center);
                y += 50 * u;

                g.DrawString(card.Blessing, blessFont, inkBrush, new RectangleF(rect.X + 22 * u, y, rect.Width - 44 * u, 70 * u), center);

                if (!string.IsNullOrEmpty(dishLine))
                {
                    g.DrawString("Món lộc hôm nay", small, redBrush, new RectangleF(rect.X, rect.Bottom - 62 * u, rect.Width, 18 * u), center);
                    g.DrawString(dishLine, dishFont, redBrush, new RectangleF(rect.X + 14 * u, rect.Bottom - 42 * u, rect.Width - 28 * u, 28 * u), center);
                }
            }

            g.Restore(state);
        }

        private static void DrawSpark(Graphics g, Spark s)
        {
            int alpha = (int)(255 * Math.Max(0f, Math.Min(1f, s.Life)));
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, 255, 214, 110)))
            {
                float r = s.Size * (0.5f + s.Life * 0.5f);
                PointF[] star =
                {
                    new PointF(s.X, s.Y - r), new PointF(s.X + r * 0.28f, s.Y - r * 0.28f),
                    new PointF(s.X + r, s.Y), new PointF(s.X + r * 0.28f, s.Y + r * 0.28f),
                    new PointF(s.X, s.Y + r), new PointF(s.X - r * 0.28f, s.Y + r * 0.28f),
                    new PointF(s.X - r, s.Y), new PointF(s.X - r * 0.28f, s.Y - r * 0.28f)
                };
                g.FillPolygon(brush, star);
            }
        }
    }

    internal sealed class B6Geo
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Source { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public bool IsPrecise { get; set; }
    }

    internal sealed class B6Place
    {
        public string Name { get; set; } = string.Empty;
        public string Kind { get; set; } = string.Empty;
        public string Cuisine { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int DistanceMeters { get; set; }

        public string DistanceText
        {
            get { return DistanceMeters < 1000 ? DistanceMeters + " m" : (DistanceMeters / 1000.0).ToString("0.0", CultureInfo.InvariantCulture) + " km"; }
        }
    }

    internal static class B6Locator
    {
        private static readonly HttpClient Http = CreateClient();

        private static HttpClient CreateClient()
        {
            HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(25) };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Lab02-HomNayAnGi/1.0 (student project)");
            return client;
        }

        public static async Task<B6Geo> LocateAsync()
        {
            B6Geo point = await TryWindowsLocationAsync();
            if (point != null)
            {
                return point;
            }
            return await TryIpLocationAsync();
        }

        private static async Task<B6Geo> TryWindowsLocationAsync()
        {
            const string script =
                "Add-Type -AssemblyName System.Device;" +
                "$w = New-Object System.Device.Location.GeoCoordinateWatcher([System.Device.Location.GeoPositionAccuracy]::High);" +
                "$null = $w.TryStart($false, [TimeSpan]::FromSeconds(8));" +
                "for ($i = 0; $i -lt 16 -and $w.Status -ne 'Ready'; $i++) { Start-Sleep -Milliseconds 500 }" +
                "$l = $w.Position.Location;" +
                "if (-not $l.IsUnknown) { [string]::Format([Globalization.CultureInfo]::InvariantCulture, '{0};{1}', $l.Latitude, $l.Longitude) }";

            try
            {
                string encoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(script));
                ProcessStartInfo info = new ProcessStartInfo("powershell.exe", "-NoProfile -NonInteractive -EncodedCommand " + encoded)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (Process process = Process.Start(info))
                {
                    Task<string> output = process.StandardOutput.ReadToEndAsync();
                    Task finished = Task.Run(() => process.WaitForExit(15000));
                    await finished;
                    if (!process.HasExited)
                    {
                        try { process.Kill(); } catch (Exception) { }
                        return null;
                    }

                    string text = (await output).Trim();
                    string[] parts = text.Split(';');
                    double lat, lng;
                    if (parts.Length == 2
                        && double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out lat)
                        && double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out lng)
                        && !(lat == 0 && lng == 0))
                    {
                        return new B6Geo { Latitude = lat, Longitude = lng, Source = "Định vị Windows (GPS / Wi-Fi)", IsPrecise = true };
                    }
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        private static async Task<B6Geo> TryIpLocationAsync()
        {
            try
            {
                string json = await Http.GetStringAsync("https://ipwho.is/");
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    JsonElement root = doc.RootElement;
                    JsonElement success;
                    if (root.TryGetProperty("success", out success) && success.ValueKind == JsonValueKind.False)
                    {
                        return null;
                    }

                    double lat = root.GetProperty("latitude").GetDouble();
                    double lng = root.GetProperty("longitude").GetDouble();
                    string city = root.TryGetProperty("city", out JsonElement c) ? c.GetString() : string.Empty;
                    return new B6Geo
                    {
                        Latitude = lat,
                        Longitude = lng,
                        Source = "Ước tính theo địa chỉ IP (sai lệch vài km)",
                        Label = city,
                        IsPrecise = false
                    };
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<string> ReverseGeocodeAsync(B6Geo point)
        {
            try
            {
                string url = "https://nominatim.openstreetmap.org/reverse?format=jsonv2&accept-language=vi&zoom=18&lat="
                    + point.Latitude.ToString(CultureInfo.InvariantCulture) + "&lon=" + point.Longitude.ToString(CultureInfo.InvariantCulture);
                string json = await Http.GetStringAsync(url);
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    JsonElement name;
                    return doc.RootElement.TryGetProperty("display_name", out name) ? name.GetString() : string.Empty;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static async Task<List<B6Place>> FindDishPlacesAsync(B6Geo point, int radiusMeters, string dishName)
        {
            List<string> phrases = DishPhrases(dishName);
            if (phrases.Count == 0)
            {
                return new List<B6Place>();
            }

            string pattern = ("(^| )(" + string.Join("|", phrases.ConvertAll(EscapeRegex)) + ")( |$)").Replace("\\", "\\\\");
            string around = "(around:" + radiusMeters + ","
                + point.Latitude.ToString(CultureInfo.InvariantCulture) + ","
                + point.Longitude.ToString(CultureInfo.InvariantCulture) + ")";
            string nameFilter = "[\"name\"~\"" + pattern + "\",i]";
            string query = "[out:json][timeout:25];("
                + "nwr" + nameFilter + "[\"amenity\"~\"restaurant|fast_food|cafe|food_court|ice_cream|bar|pub|biergarten\"]" + around + ";"
                + "nwr" + nameFilter + "[\"shop\"~\"bakery|pastry|confectionery|deli|beverages|tea|coffee\"]" + around + ";"
                + ");out center 200;";

            string[] endpoints =
            {
                "https://overpass-api.de/api/interpreter",
                "https://overpass.kumi.systems/api/interpreter",
                "https://overpass.private.coffee/api/interpreter"
            };

            List<B6Place> places = new List<B6Place>();
            Exception lastError = null;
            bool overpassOk = false;
            foreach (string endpoint in endpoints)
            {
                try
                {
                    using (System.Threading.CancellationTokenSource cancel = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(20)))
                    using (FormUrlEncodedContent content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("data", query) }))
                    using (HttpResponseMessage response = await Http.PostAsync(endpoint, content, cancel.Token))
                    {
                        response.EnsureSuccessStatusCode();
                        string json = await response.Content.ReadAsStringAsync();
                        places.AddRange(ParseOverpass(json, point));
                        overpassOk = true;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    lastError = ex;
                }
            }

            if (places.Count == 0)
            {
                bool nominatimOk = false;
                for (int i = 0; i < phrases.Count && i < 2; i++)
                {
                    try
                    {
                        places.AddRange(await SearchNominatimAsync(point, radiusMeters, phrases[i]));
                        nominatimOk = true;
                    }
                    catch (Exception ex)
                    {
                        lastError = ex;
                    }
                }

                if (!overpassOk && !nominatimOk)
                {
                    throw lastError ?? new InvalidOperationException("Không kết nối được máy chủ bản đồ.");
                }
            }

            List<B6Place> unique = new List<B6Place>();
            HashSet<string> seen = new HashSet<string>();
            foreach (B6Place place in places)
            {
                string key = place.Name.ToLowerInvariant() + "|" + Math.Round(place.Latitude, 3).ToString(CultureInfo.InvariantCulture) + "|" + Math.Round(place.Longitude, 3).ToString(CultureInfo.InvariantCulture);
                if (place.DistanceMeters <= radiusMeters && seen.Add(key))
                {
                    unique.Add(place);
                }
            }

            unique.Sort((a, b) => a.DistanceMeters.CompareTo(b.DistanceMeters));
            if (unique.Count > 150)
            {
                unique.RemoveRange(150, unique.Count - 150);
            }
            return unique;
        }

        private static List<string> DishPhrases(string dishName)
        {
            string cleaned = (dishName ?? string.Empty).Trim();
            int open = cleaned.IndexOf('(');
            if (open > 0) cleaned = cleaned.Substring(0, open).Trim();
            cleaned = cleaned.Replace("\"", string.Empty).Replace("\\", string.Empty);

            string[] words = cleaned.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> bases = new List<string>();
            if (words.Length > 0) bases.Add(string.Join(" ", words));
            if (words.Length > 3) bases.Add(string.Join(" ", words, 0, 3));
            if (words.Length > 2) bases.Add(string.Join(" ", words, 0, 2));

            List<string> result = new List<string>();
            foreach (string phrase in bases)
            {
                string lower = phrase.ToLowerInvariant();
                string[] variants =
                {
                    lower,
                    CultureInfo.GetCultureInfo("vi-VN").TextInfo.ToTitleCase(lower),
                    B6Ui.Fold(phrase)
                };
                foreach (string variant in variants)
                {
                    if (variant.Length > 0 && !result.Contains(variant)) result.Add(variant);
                }
            }
            return result;
        }

        private static string EscapeRegex(string text)
        {
            StringBuilder builder = new StringBuilder(text.Length + 8);
            foreach (char letter in text)
            {
                if ("\\.^$*+?()[]{}|".IndexOf(letter) >= 0) builder.Append('\\');
                builder.Append(letter);
            }
            return builder.ToString();
        }

        private static List<B6Place> ParseOverpass(string json, B6Geo point)
        {
            List<B6Place> places = new List<B6Place>();
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                foreach (JsonElement element in doc.RootElement.GetProperty("elements").EnumerateArray())
                {
                    JsonElement tags;
                    if (!element.TryGetProperty("tags", out tags)) continue;

                    double lat, lng;
                    if (element.TryGetProperty("lat", out JsonElement latElement))
                    {
                        lat = latElement.GetDouble();
                        lng = element.GetProperty("lon").GetDouble();
                    }
                    else if (element.TryGetProperty("center", out JsonElement center))
                    {
                        lat = center.GetProperty("lat").GetDouble();
                        lng = center.GetProperty("lon").GetDouble();
                    }
                    else
                    {
                        continue;
                    }

                    string address = string.Join(" ", NonEmpty(Tag(tags, "addr:housenumber"), Tag(tags, "addr:street"), Tag(tags, "addr:suburb"), Tag(tags, "addr:city")));
                    if (address.Length == 0) address = Tag(tags, "addr:full");
                    string kind = Tag(tags, "amenity");
                    places.Add(new B6Place
                    {
                        Name = Tag(tags, "name"),
                        Kind = kind.Length > 0 ? KindName(kind) : "Cửa hàng ăn uống",
                        Cuisine = CuisineName(Tag(tags, "cuisine")),
                        Address = address,
                        Latitude = lat,
                        Longitude = lng,
                        DistanceMeters = (int)Distance(point.Latitude, point.Longitude, lat, lng)
                    });
                }
            }
            return places;
        }

        private static async Task<List<B6Place>> SearchNominatimAsync(B6Geo point, int radiusMeters, string phrase)
        {
            double dLat = radiusMeters / 111000.0;
            double dLng = radiusMeters / (111000.0 * Math.Max(0.2, Math.Cos(point.Latitude * Math.PI / 180.0)));
            string box = (point.Longitude - dLng).ToString(CultureInfo.InvariantCulture) + ","
                + (point.Latitude + dLat).ToString(CultureInfo.InvariantCulture) + ","
                + (point.Longitude + dLng).ToString(CultureInfo.InvariantCulture) + ","
                + (point.Latitude - dLat).ToString(CultureInfo.InvariantCulture);
            string url = "https://nominatim.openstreetmap.org/search?format=jsonv2&accept-language=vi&limit=40&bounded=1&viewbox=" + box + "&q=" + Uri.EscapeDataString(phrase);

            List<B6Place> places = new List<B6Place>();
            using (System.Threading.CancellationTokenSource cancel = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(20)))
            {
                string json = await Http.GetStringAsync(url, cancel.Token);
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    foreach (JsonElement element in doc.RootElement.EnumerateArray())
                    {
                        string category = element.TryGetProperty("category", out JsonElement categoryElement) ? (categoryElement.GetString() ?? string.Empty) : string.Empty;
                        if (category != "amenity" && category != "shop" && category != "tourism") continue;

                        double lat, lng;
                        if (!double.TryParse(element.GetProperty("lat").GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out lat)) continue;
                        if (!double.TryParse(element.GetProperty("lon").GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out lng)) continue;

                        string display = element.TryGetProperty("display_name", out JsonElement displayElement) ? (displayElement.GetString() ?? string.Empty) : string.Empty;
                        string name = element.TryGetProperty("name", out JsonElement nameElement) ? (nameElement.GetString() ?? string.Empty) : string.Empty;
                        if (name.Length == 0)
                        {
                            int comma = display.IndexOf(',');
                            name = comma > 0 ? display.Substring(0, comma) : display;
                        }

                        string address = display;
                        if (name.Length > 0 && address.StartsWith(name, StringComparison.Ordinal))
                        {
                            address = address.Substring(name.Length).TrimStart(',', ' ');
                        }

                        places.Add(new B6Place
                        {
                            Name = name,
                            Kind = category == "shop" ? "Cửa hàng ăn uống" : "Quán ăn / nhà hàng",
                            Address = address,
                            Latitude = lat,
                            Longitude = lng,
                            DistanceMeters = (int)Distance(point.Latitude, point.Longitude, lat, lng)
                        });
                    }
                }
            }
            return places;
        }

        private static string Tag(JsonElement tags, string key)
        {
            JsonElement value;
            return tags.TryGetProperty(key, out value) ? (value.GetString() ?? string.Empty) : string.Empty;
        }

        private static IEnumerable<string> NonEmpty(params string[] values)
        {
            foreach (string value in values)
            {
                if (!string.IsNullOrWhiteSpace(value)) yield return value;
            }
        }

        private static string KindName(string amenity)
        {
            switch (amenity)
            {
                case "restaurant": return "Nhà hàng / quán ăn";
                case "fast_food": return "Ăn nhanh";
                case "cafe": return "Quán cà phê";
                case "food_court": return "Khu ẩm thực";
                case "ice_cream": return "Kem / tráng miệng";
                default: return amenity;
            }
        }

        private static string CuisineName(string cuisine)
        {
            if (string.IsNullOrEmpty(cuisine)) return string.Empty;
            return cuisine.Replace(";", ", ").Replace("_", " ");
        }

        public static double Distance(double lat1, double lng1, double lat2, double lng2)
        {
            const double R = 6371000;
            double dLat = (lat2 - lat1) * Math.PI / 180;
            double dLng = (lng2 - lng1) * Math.PI / 180;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                + Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        public static string MapsSearchUrl(string query, B6Geo around)
        {
            string encoded = Uri.EscapeDataString(query);
            if (around == null)
            {
                return "https://www.google.com/maps/search/?api=1&query=" + encoded;
            }
            return "https://www.google.com/maps/search/" + encoded + "/@"
                + around.Latitude.ToString(CultureInfo.InvariantCulture) + ","
                + around.Longitude.ToString(CultureInfo.InvariantCulture) + ",16z";
        }

        public static string MapsPlaceUrl(B6Place place)
        {
            return "https://www.google.com/maps/search/?api=1&query="
                + place.Latitude.ToString(CultureInfo.InvariantCulture) + "%2C" + place.Longitude.ToString(CultureInfo.InvariantCulture);
        }

        public static string MapsDirectionsUrl(B6Place place, B6Geo origin)
        {
            string url = "https://www.google.com/maps/dir/?api=1&travelmode=driving&destination="
                + place.Latitude.ToString(CultureInfo.InvariantCulture) + "%2C" + place.Longitude.ToString(CultureInfo.InvariantCulture);
            if (origin != null)
            {
                url += "&origin=" + origin.Latitude.ToString(CultureInfo.InvariantCulture) + "%2C" + origin.Longitude.ToString(CultureInfo.InvariantCulture);
            }
            return url;
        }

        public static string MapsDirectionsToAddress(string address)
        {
            return "https://www.google.com/maps/dir/?api=1&destination=" + Uri.EscapeDataString(address);
        }
    }

    [System.ComponentModel.DesignerCategory("Code")]
    [System.ComponentModel.ToolboxItem(false)]
    internal sealed class B6StatStrip : Control
    {
        private static readonly string[] Labels = { "MÓN ĂN TRONG THỰC ĐƠN", "NGƯỜI ĐÓNG GÓP", "NHÓM MÓN", "BỮA ĂN HIỆN TẠI" };
        private static readonly string[] Icons = { "✦", "☺", "▦", "☀" };
        private static readonly Color[][] Tones =
        {
            new[] { Color.FromArgb(200, 52, 34), Color.FromArgb(246, 120, 60) },
            new[] { Color.FromArgb(52, 124, 68), Color.FromArgb(120, 190, 96) },
            new[] { Color.FromArgb(170, 96, 28), Color.FromArgb(240, 176, 56) },
            new[] { Color.FromArgb(70, 40, 30), Color.FromArgb(150, 90, 56) }
        };

        private readonly Timer timer = new Timer { Interval = 50 };
        private readonly float[] shown = new float[3];
        private readonly int[] target = new int[3];
        private string mealText = string.Empty;
        private float time;

        public B6StatStrip()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Height = 70;
            mealText = B6Ui.MealForNow();
            timer.Tick += (s, e) =>
            {
                if (DesignMode || !Visible) return;
                time += 0.05f;
                for (int i = 0; i < 3; i++)
                {
                    float diff = target[i] - shown[i];
                    shown[i] = Math.Abs(diff) < 0.5f ? target[i] : shown[i] + diff * 0.12f;
                }
                Invalidate();
            };
            timer.Start();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) timer.Dispose();
            base.Dispose(disposing);
        }

        public void SetValues(int dishes, int people, int groups)
        {
            target[0] = dishes;
            target[1] = people;
            target[2] = groups;
            mealText = B6Ui.MealForNow() + "  ·  " + DateTime.Now.ToString("HH:mm");
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width < 4 || Height < 4)
            {
                return;
            }

            Graphics g = e.Graphics;
            g.Clear(B6Ui.SurfaceOf(this));
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            float k = B6Ui.Scale(this);
            float gap = 12 * k;
            float tileWidth = (Width - gap * 3 - 6 * k) / 4f;
            float tileHeight = Height - 10 * k;

            using (Font numberFont = new Font("Palatino Linotype", 26 * k, FontStyle.Bold, GraphicsUnit.Pixel))
            using (Font mealFont = new Font("Palatino Linotype", 20 * k, FontStyle.Bold, GraphicsUnit.Pixel))
            using (Font labelFont = new Font("Segoe UI", 10 * k, FontStyle.Bold, GraphicsUnit.Pixel))
            using (Font iconFont = new Font("Segoe UI Symbol", 30 * k, FontStyle.Regular, GraphicsUnit.Pixel))
            using (SolidBrush white = new SolidBrush(Color.White))
            using (SolidBrush soft = new SolidBrush(Color.FromArgb(225, 255, 244, 224)))
            using (SolidBrush iconBrush = new SolidBrush(Color.FromArgb(120, 255, 255, 255)))
            using (SolidBrush circle = new SolidBrush(Color.FromArgb(30, 255, 255, 255)))
            using (SolidBrush shadow = new SolidBrush(Color.FromArgb(38, 60, 30, 10)))
            {
                for (int i = 0; i < 4; i++)
                {
                    RectangleF tile = new RectangleF(3 * k + i * (tileWidth + gap), 2 * k, tileWidth, tileHeight);
                    RectangleF under = tile;
                    under.Offset(0, 4 * k);

                    using (GraphicsPath shadowPath = B6Ui.RoundRect(under, 16 * k))
                    {
                        g.FillPath(shadow, shadowPath);
                    }

                    using (GraphicsPath path = B6Ui.RoundRect(tile, 16 * k))
                    using (LinearGradientBrush fill = B6Ui.Gradient(tile, Tones[i][0], Tones[i][1], 25f))
                    {
                        g.FillPath(fill, path);
                        GraphicsState state = g.Save();
                        g.SetClip(path);
                        g.FillEllipse(circle, tile.Right - tileHeight * 0.95f, tile.Y - tileHeight * 0.35f, tileHeight * 1.2f, tileHeight * 1.2f);
                        g.Restore(state);
                    }

                    string value = i < 3 ? ((int)Math.Round(shown[i])).ToString() : mealText;
                    g.DrawString(value, i < 3 ? numberFont : mealFont, white, tile.X + 16 * k, tile.Y + (i < 3 ? 6 : 10) * k);
                    g.DrawString(Labels[i], labelFont, soft, tile.X + 17 * k, tile.Bottom - 22 * k);

                    float bob = (float)Math.Sin(time * 2f + i) * 3 * k;
                    g.DrawString(Icons[i], iconFont, iconBrush, tile.Right - 48 * k, tile.Y + 8 * k + bob);
                }
            }
        }
    }

    [System.ComponentModel.DesignerCategory("Code")]
    [System.ComponentModel.ToolboxItem(false)]
    internal sealed class B6Fade : Control
    {
        private readonly Timer timer = new Timer { Interval = 16 };
        private Bitmap snapshot;
        private Bitmap fresh;
        private float alpha;

        public B6Fade()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque, true);
            Visible = false;
            timer.Tick += (s, e) =>
            {
                alpha -= 0.09f;
                if (alpha <= 0f)
                {
                    timer.Stop();
                    Visible = false;
                    DisposeSnapshot();
                }
                else
                {
                    Invalidate();
                }
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                timer.Dispose();
                DisposeSnapshot();
            }
            base.Dispose(disposing);
        }

        private void DisposeSnapshot()
        {
            if (snapshot != null)
            {
                snapshot.Dispose();
                snapshot = null;
            }
            if (fresh != null)
            {
                fresh.Dispose();
                fresh = null;
            }
        }

        public void Run(Control host, Action change)
        {
            if (host.Width < 10 || host.Height < 10 || !host.Visible)
            {
                change();
                return;
            }

            DisposeSnapshot();
            snapshot = new Bitmap(host.Width, host.Height);
            host.DrawToBitmap(snapshot, new Rectangle(Point.Empty, host.Size));

            change();
            host.PerformLayout();
            host.Update();

            fresh = new Bitmap(host.Width, host.Height);
            host.DrawToBitmap(fresh, new Rectangle(Point.Empty, host.Size));

            Parent = host;
            Bounds = host.ClientRectangle;
            alpha = 1f;
            Visible = true;
            BringToFront();
            timer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width < 4 || Height < 4)
            {
                return;
            }

            if (snapshot == null)
            {
                return;
            }

            Rectangle area = new Rectangle(0, 0, Width, Height);
            if (fresh != null)
            {
                e.Graphics.DrawImage(fresh, area, 0, 0, fresh.Width, fresh.Height, GraphicsUnit.Pixel);
            }

            ColorMatrix matrix = new ColorMatrix { Matrix33 = Math.Max(0f, Math.Min(1f, B6Ui.EaseOutCubic(alpha))) };
            using (ImageAttributes attributes = new ImageAttributes())
            {
                attributes.SetColorMatrix(matrix);
                e.Graphics.DrawImage(snapshot, area, 0, 0, snapshot.Width, snapshot.Height, GraphicsUnit.Pixel, attributes);
            }
        }
    }

    [System.ComponentModel.DesignerCategory("Code")]
    [System.ComponentModel.ToolboxItem(false)]
    internal sealed class B6Detail : B6Card
    {
        private readonly B6Stage stage = new B6Stage { Radius = 16 };
        private readonly Label nameLabel = new Label();
        private readonly Label metaLabel = new Label();
        private readonly Label priceLabel = new Label();
        private readonly Label mealsLabel = new Label();
        private readonly Label descLabel = new Label();
        private readonly Label ownerLabel = new Label();
        private readonly Label emptyLabel = new Label();
        private readonly B6Button favoriteButton = new B6Button();
        private readonly B6Button nearbyButton = new B6Button();
        private readonly B6Button mapsButton = new B6Button();

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Dish Current { get; private set; }
        public event EventHandler FavoriteClicked;
        public event EventHandler NearbyClicked;
        public event EventHandler MapsClicked;

        public B6Detail()
        {
            SurfaceColor = Color.Empty;
            BackColor = B6Ui.NightCard;
            BorderColor = B6Ui.NightLine;
            Shadow = false;
            Radius = 20;
            Padding = new Padding(16);

            nameLabel.Font = new Font("Palatino Linotype", 17F, FontStyle.Bold);
            nameLabel.ForeColor = B6Ui.Cream;
            metaLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            metaLabel.ForeColor = B6Ui.Gold;
            priceLabel.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            priceLabel.ForeColor = B6Ui.Gold;
            priceLabel.TextAlign = ContentAlignment.MiddleRight;
            mealsLabel.Font = new Font("Segoe UI", 9.5F);
            mealsLabel.ForeColor = B6Ui.CreamMuted;
            descLabel.Font = new Font("Segoe UI", 10.5F);
            descLabel.ForeColor = Color.FromArgb(232, 218, 198);
            ownerLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Italic);
            ownerLabel.ForeColor = B6Ui.CreamMuted;
            emptyLabel.Font = new Font("Segoe UI", 11F, FontStyle.Italic);
            emptyLabel.ForeColor = B6Ui.CreamMuted;
            emptyLabel.TextAlign = ContentAlignment.MiddleCenter;
            emptyLabel.Text = "Chọn một món để xem\nmô tả, giá và buổi ăn phù hợp.";

            foreach (Label label in new[] { nameLabel, metaLabel, priceLabel, mealsLabel, descLabel, ownerLabel, emptyLabel })
            {
                label.BackColor = Color.Transparent;
                label.AutoEllipsis = true;
            }

            StyleButton(favoriteButton, "♡  YÊU THÍCH", B6Ui.NightCard2, B6Ui.Cream, B6Ui.NightLine);
            StyleButton(nearbyButton, "TÌM QUÁN GẦN ĐÂY", B6Ui.Gold, Color.FromArgb(48, 30, 12), Color.Empty);
            nearbyButton.HoverColor = B6Ui.GoldDark;
            StyleButton(mapsButton, "GOOGLE MAPS", B6Ui.NightCard2, B6Ui.Gold, B6Ui.NightLine);

            favoriteButton.Click += (s, e) => { EventHandler h = FavoriteClicked; if (h != null) h(this, e); };
            nearbyButton.Click += (s, e) => { EventHandler h = NearbyClicked; if (h != null) h(this, e); };
            mapsButton.Click += (s, e) => { EventHandler h = MapsClicked; if (h != null) h(this, e); };

            Controls.AddRange(new Control[] { stage, nameLabel, metaLabel, priceLabel, mealsLabel, descLabel, ownerLabel, favoriteButton, nearbyButton, mapsButton, emptyLabel });
            ShowEmpty();
        }

        private static void StyleButton(B6Button button, string text, Color back, Color fore, Color border)
        {
            button.Text = text;
            button.BackColor = back;
            button.ForeColor = fore;
            button.HoverColor = B6Ui.NightLine;
            button.BorderColor = border;
            button.SurfaceColor = B6Ui.NightCard;
            button.Radius = 14;
        }

        public void ShowEmpty()
        {
            Current = null;
            foreach (Control c in Controls) c.Visible = false;
            emptyLabel.Visible = true;
            Invalidate();
        }

        public void ShowDish(Dish dish, Image picture, bool favorite)
        {
            Current = dish;
            foreach (Control c in Controls) c.Visible = true;
            emptyLabel.Visible = false;

            stage.Reveal(picture, string.Empty, false);
            nameLabel.Text = dish.Name;
            metaLabel.Text = (dish.Category + (string.IsNullOrEmpty(dish.Region) ? string.Empty : "  ·  " + dish.Region)).ToUpperInvariant();
            priceLabel.Text = B6Ui.FormatPrice(dish.Price);
            mealsLabel.Text = "Hợp với buổi: " + dish.MealsText;
            descLabel.Text = string.IsNullOrWhiteSpace(dish.Description) ? "Chưa có mô tả cho món này." : dish.Description;
            ownerLabel.Text = "Đóng góp bởi " + dish.ContributorName;
            SetFavorite(favorite);
            PerformLayout();
        }

        public void SetFavorite(bool favorite)
        {
            favoriteButton.Text = favorite ? "♥  ĐÃ YÊU THÍCH" : "♡  YÊU THÍCH";
            favoriteButton.ForeColor = favorite ? Color.FromArgb(255, 120, 130) : B6Ui.Cream;
            favoriteButton.Invalidate();
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            if (stage == null) return;

            float k = B6Ui.Scale(this);
            int pad = (int)(16 * k);
            int w = Width - pad * 2 - (int)(8 * k);
            int y = pad;

            emptyLabel.SetBounds(pad, pad, w, Height - pad * 2);

            int imageHeight = Math.Min((int)(w * 0.68f), (int)(Height * 0.3f));
            stage.SetBounds(pad, y, w, imageHeight);
            y += imageHeight + (int)(12 * k);

            nameLabel.SetBounds(pad, y, w, (int)(30 * k));
            y += (int)(32 * k);
            metaLabel.SetBounds(pad, y, (int)(w * 0.62f), (int)(22 * k));
            priceLabel.SetBounds(pad + (int)(w * 0.55f), y - (int)(4 * k), (int)(w * 0.45f), (int)(30 * k));
            y += (int)(26 * k);
            mealsLabel.SetBounds(pad, y, w, (int)(22 * k));
            y += (int)(26 * k);

            int buttonHeight = (int)(40 * k);
            int bottom = Height - pad - (int)(8 * k);
            mapsButton.SetBounds(pad, bottom - buttonHeight, w, buttonHeight);
            nearbyButton.SetBounds(pad, bottom - buttonHeight * 2 - (int)(8 * k), w, buttonHeight);
            favoriteButton.SetBounds(pad, bottom - buttonHeight * 3 - (int)(16 * k), w, buttonHeight);
            ownerLabel.SetBounds(pad, bottom - buttonHeight * 3 - (int)(16 * k) - (int)(28 * k), w, (int)(22 * k));

            int descHeight = ownerLabel.Top - y - (int)(4 * k);
            descLabel.SetBounds(pad, y, w, Math.Max((int)(30 * k), descHeight));
        }
    }
}
