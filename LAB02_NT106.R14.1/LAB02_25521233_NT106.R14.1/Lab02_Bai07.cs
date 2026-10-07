#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using Lab02.Bai07Browser;

namespace Lab02
{
    public partial class Lab02_Bai07 : Form
    {
        private enum ViewMode
        {
            Content,
            Properties,
            Search,
            Favorites
        }

        private enum BackgroundTask
        {
            None,
            Search,
            FileHash,
            FolderSize
        }

        private const int MaxEntriesPerFolder = 3000;
        private const int MaxSearchResults = 500;
        private const string HashRowKey = "sha256";
        private const string FolderSizeRowKey = "foldersize";

        private static readonly object PlaceholderTag = new object();
        private static readonly char[] PathSeparators = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

        private FavoriteStore favoriteStore;
        private ImageList nodeImages;
        private Font contentFont;
        private CancellationTokenSource contentCancellation;
        private CancellationTokenSource backgroundCancellation;
        private BackgroundTask activeTask = BackgroundTask.None;
        private bool isRebuildingTree;

        public Lab02_Bai07()
        {
            InitializeComponent();
            ApplyTheme();
            ConfigureListViews();
        }

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr windowHandle, string subAppName, string subIdList);

        private void Lab02_Bai07_Load(object sender, EventArgs e)
        {
            favoriteStore = new FavoriteStore();
            contentFont = new Font("Consolas", 10F);
            txtContent.Font = contentFont;

            nodeImages = FileCatalog.BuildImageList(LogicalToDeviceUnits(18));
            tvExplorer.ImageList = nodeImages;
            tvExplorer.ItemHeight = LogicalToDeviceUnits(26);
            EnableSmoothDrawing(tvExplorer);
            EnableSmoothDrawing(lvProperties);
            EnableSmoothDrawing(lvSearchResults);
            EnableSmoothDrawing(lvFavorites);
            SetWindowTheme(tvExplorer.Handle, "explorer", null);

            BuildDriveNodes();
            RefreshFavoriteList();
            SetViewMode(ViewMode.Content);
            ShowMessageSurface("Chọn một tệp ở khung bên trái để xem nội dung.");

            if (tvExplorer.Nodes.Count > 0)
            {
                tvExplorer.SelectedNode = tvExplorer.Nodes[0];
            }
        }

        private void Lab02_Bai07_FormClosed(object sender, FormClosedEventArgs e)
        {
            contentCancellation?.Cancel();
            backgroundCancellation?.Cancel();

            ReplacePreviewImage(null);
            nodeImages?.Dispose();
            contentFont?.Dispose();
        }

        private void Lab02_Bai07_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                btnRefresh.PerformClick();
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.F)
            {
                SetViewMode(ViewMode.Search);
                txtSearch.Focus();
                e.Handled = true;
            }
        }

        // ------------------------------------------------------------------
        // Giao diện
        // ------------------------------------------------------------------

        private void ApplyTheme()
        {
            pnlHeader.Paint += Theme.PaintHeaderBackground;

            Theme.StyleSecondary(btnGoUp);
            Theme.StyleSecondary(btnRefresh);
            Theme.StyleAccent(btnGo);
            Theme.StyleSecondary(btnFavorite);
            Theme.StyleSecondary(btnOpenExplorer);
            Theme.StylePrimary(btnSearch);
            Theme.StylePrimary(btnComputeHash);
            Theme.StylePrimary(btnComputeFolderSize);
            Theme.StylePrimary(btnOpenFavorite);
            Theme.StyleDanger(btnRemoveFavorite);
            Theme.StyleDanger(btnCancelWork);
            btnCancelWork.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        }

        private void ConfigureListViews()
        {
            ConfigureListView(lvProperties, ("Thuộc tính", 180), ("Giá trị", 520));
            ConfigureListView(lvSearchResults, ("Tên", 250), ("Loại", 110), ("Kích thước", 110), ("Thư mục chứa", 400));
            ConfigureListView(lvFavorites, ("Tên", 220), ("Loại", 100), ("Đường dẫn", 450));
        }

        private static void ConfigureListView(ListView list, params (string Title, int Width)[] columns)
        {
            list.View = View.Details;
            list.FullRowSelect = true;
            list.HideSelection = false;
            list.MultiSelect = false;
            list.BorderStyle = BorderStyle.None;
            list.Font = new Font("Segoe UI", 10F);
            list.BackColor = Color.White;
            list.ForeColor = Theme.TextMain;

            foreach ((string title, int width) in columns)
            {
                list.Columns.Add(title, width);
            }

            list.SizeChanged += (sender, e) => StretchLastColumn(list);
        }

        private static void StretchLastColumn(ListView list)
        {
            if (list.Columns.Count == 0 || list.ClientSize.Width <= 0)
            {
                return;
            }

            int usedWidth = 0;
            for (int index = 0; index < list.Columns.Count - 1; index++)
            {
                usedWidth += list.Columns[index].Width;
            }

            list.Columns[list.Columns.Count - 1].Width = Math.Max(120, list.ClientSize.Width - usedWidth);
        }

        private static void EnableSmoothDrawing(Control control)
        {
            PropertyInfo property = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            property?.SetValue(control, true);
        }

        private void SetViewMode(ViewMode mode)
        {
            pnlContent.Visible = mode == ViewMode.Content;
            pnlProperties.Visible = mode == ViewMode.Properties;
            pnlSearch.Visible = mode == ViewMode.Search;
            pnlFavorites.Visible = mode == ViewMode.Favorites;

            StyleModeButton(btnModeContent, mode == ViewMode.Content);
            StyleModeButton(btnModeProperties, mode == ViewMode.Properties);
            StyleModeButton(btnModeSearch, mode == ViewMode.Search);
            StyleModeButton(btnModeFavorites, mode == ViewMode.Favorites);
        }

        private static void StyleModeButton(Button button, bool isActive)
        {
            if (isActive)
            {
                Theme.StylePrimary(button);
            }
            else
            {
                Theme.StyleSecondary(button);
            }
        }

        private void btnModeContent_Click(object sender, EventArgs e) => SetViewMode(ViewMode.Content);

        private void btnModeProperties_Click(object sender, EventArgs e) => SetViewMode(ViewMode.Properties);

        private void btnModeSearch_Click(object sender, EventArgs e)
        {
            SetViewMode(ViewMode.Search);
            txtSearch.Focus();
        }

        private void btnModeFavorites_Click(object sender, EventArgs e) => SetViewMode(ViewMode.Favorites);

        // ------------------------------------------------------------------
        // Cây thư mục
        // ------------------------------------------------------------------

        private void BuildDriveNodes()
        {
            isRebuildingTree = true;
            tvExplorer.BeginUpdate();
            tvExplorer.Nodes.Clear();

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                bool isReady = drive.IsReady;
                string label = isReady ? ReadVolumeLabel(drive) : "Chưa sẵn sàng";
                string caption = label.Length > 0 ? label : DescribeDriveType(drive.DriveType);

                TreeNode node = new TreeNode(caption + " (" + drive.Name + ")") { Tag = new ExplorerItem(drive.Name, true) };
                ApplyIcon(node, isReady ? FileCatalog.DriveIcon : FileCatalog.LockedIcon);
                if (isReady)
                {
                    node.Nodes.Add(CreatePlaceholderNode());
                }

                tvExplorer.Nodes.Add(node);
            }

            tvExplorer.EndUpdate();
            isRebuildingTree = false;
        }

        private static string ReadVolumeLabel(DriveInfo drive)
        {
            try
            {
                return drive.VolumeLabel ?? string.Empty;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                return string.Empty;
            }
        }

        private static string DescribeDriveType(DriveType type)
        {
            switch (type)
            {
                case DriveType.Removable: return "Ổ đĩa di động";
                case DriveType.Network: return "Ổ đĩa mạng";
                case DriveType.CDRom: return "Ổ đĩa quang";
                default: return "Ổ đĩa cục bộ";
            }
        }

        private static TreeNode CreatePlaceholderNode()
        {
            return new TreeNode("…") { Tag = PlaceholderTag };
        }

        private static bool IsPlaceholder(TreeNode node)
        {
            return ReferenceEquals(node.Tag, PlaceholderTag);
        }

        private static void ApplyIcon(TreeNode node, string iconKey)
        {
            node.ImageKey = iconKey;
            node.SelectedImageKey = iconKey;
        }

        private void tvExplorer_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            EnsureChildrenLoaded(e.Node);
        }

        private void EnsureChildrenLoaded(TreeNode node)
        {
            ExplorerItem item = node.Tag as ExplorerItem;
            if (item != null && item.IsDirectory && !item.IsLoaded)
            {
                LoadChildren(node, item);
            }
        }

        private bool IsVisibleEntry(FileSystemInfo entry)
        {
            return chkShowHidden.Checked || (entry.Attributes & (FileAttributes.Hidden | FileAttributes.System)) == 0;
        }

        private void LoadChildren(TreeNode parent, ExplorerItem parentItem)
        {
            List<DirectoryInfo> folders;
            List<FileInfo> files;

            try
            {
                DirectoryInfo directory = new DirectoryInfo(parentItem.FullPath);
                folders = directory.EnumerateDirectories().Where(IsVisibleEntry)
                    .OrderBy(folder => folder.Name, StringComparer.CurrentCultureIgnoreCase).ToList();
                files = directory.EnumerateFiles().Where(IsVisibleEntry)
                    .OrderBy(file => file.Name, StringComparer.CurrentCultureIgnoreCase).ToList();
            }
            catch (UnauthorizedAccessException)
            {
                MarkAsInaccessible(parent, "Không có quyền truy cập thư mục này.");
                return;
            }
            catch (IOException ex)
            {
                MarkAsInaccessible(parent, "Không đọc được thư mục: " + ex.Message);
                return;
            }

            tvExplorer.BeginUpdate();
            parent.Nodes.Clear();

            int remaining = MaxEntriesPerFolder;
            foreach (DirectoryInfo folder in folders)
            {
                if (remaining == 0) break;
                TreeNode folderNode = new TreeNode(folder.Name) { Tag = new ExplorerItem(folder.FullName, true) };
                ApplyIcon(folderNode, FileCatalog.FolderIcon);
                folderNode.Nodes.Add(CreatePlaceholderNode());
                parent.Nodes.Add(folderNode);
                remaining--;
            }

            foreach (FileInfo file in files)
            {
                if (remaining == 0) break;
                TreeNode fileNode = new TreeNode(file.Name) { Tag = new ExplorerItem(file.FullName, false) };
                ApplyIcon(fileNode, FileCatalog.IconKeyFor(file.Extension));
                parent.Nodes.Add(fileNode);
                remaining--;
            }

            int hiddenCount = folders.Count + files.Count - parent.Nodes.Count;
            if (hiddenCount > 0)
            {
                TreeNode overflowNode = new TreeNode("… còn " + hiddenCount.ToString("N0") + " mục chưa hiển thị (dùng tab Tìm kiếm)");
                overflowNode.ForeColor = Theme.TextMuted;
                parent.Nodes.Add(overflowNode);
            }

            tvExplorer.EndUpdate();

            parentItem.IsLoaded = true;
            parentItem.ChildFolderCount = folders.Count;
            parentItem.ChildFileCount = files.Count;
        }

        private void MarkAsInaccessible(TreeNode node, string message)
        {
            node.Nodes.Clear();
            ApplyIcon(node, FileCatalog.LockedIcon);
            lblStatus.Text = message;
            ExplorerItem item = node.Tag as ExplorerItem;
            if (item != null)
            {
                item.IsLoaded = true;
            }
        }

        private ExplorerItem GetSelectedItem()
        {
            return tvExplorer.SelectedNode?.Tag as ExplorerItem;
        }

        private void tvExplorer_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            ExplorerItem item = e.Node.Tag as ExplorerItem;
            if (item == null || !item.IsDirectory)
            {
                return;
            }

            if (e.Node.IsExpanded)
            {
                tvExplorer.TopNode = e.Node;
            }

            ShowFolderSummary(item);
        }

        private void tvExplorer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                GoToParentFolder();
                e.Handled = true;
            }
        }

        private void tvExplorer_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ExplorerItem item = e.Node.Tag as ExplorerItem;
            if (item == null || isRebuildingTree)
            {
                return;
            }

            if (activeTask == BackgroundTask.FileHash || activeTask == BackgroundTask.FolderSize)
            {
                backgroundCancellation?.Cancel();
            }

            txtAddress.Text = item.FullPath;
            lblSearchScope.Text = "Phạm vi tìm: " + ResolveSearchScope(item);
            btnGoUp.Enabled = !item.IsDriveRoot;
            btnComputeHash.Enabled = activeTask == BackgroundTask.None && !item.IsDirectory;
            btnComputeFolderSize.Enabled = activeTask == BackgroundTask.None && item.IsDirectory;
            lblStatus.Text = item.FullPath;

            UpdateFavoriteButton();
            FillProperties(item);

            if (item.IsDirectory)
            {
                contentCancellation?.Cancel();
                ShowFolderSummary(item);
            }
            else
            {
                ShowFileContent(item);
            }
        }

        // ------------------------------------------------------------------
        // Điều hướng
        // ------------------------------------------------------------------

        private static bool IsSamePath(string first, string second)
        {
            return string.Equals(first.TrimEnd(PathSeparators), second.TrimEnd(PathSeparators), StringComparison.OrdinalIgnoreCase);
        }

        private static TreeNode FindChildByPath(TreeNodeCollection nodes, string path)
        {
            foreach (TreeNode node in nodes)
            {
                ExplorerItem item = node.Tag as ExplorerItem;
                if (item != null && IsSamePath(item.FullPath, path))
                {
                    return node;
                }
            }

            return null;
        }

        private bool SelectPath(string fullPath)
        {
            string root = Path.GetPathRoot(fullPath);
            TreeNode current = FindChildByPath(tvExplorer.Nodes, root);
            if (current == null)
            {
                return false;
            }

            string[] segments = fullPath.Substring(root.Length).Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries);
            string walkedPath = root;

            foreach (string segment in segments)
            {
                EnsureChildrenLoaded(current);
                walkedPath = Path.Combine(walkedPath, segment);
                TreeNode next = FindChildByPath(current.Nodes, walkedPath);
                if (next == null)
                {
                    return false;
                }

                current = next;
            }

            tvExplorer.SelectedNode = current;
            current.EnsureVisible();
            return true;
        }

        private bool NavigateTo(string rawPath)
        {
            string path = rawPath.Trim().Trim('"');
            if (path.Length == 0)
            {
                return false;
            }

            string fullPath;
            try
            {
                fullPath = Path.GetFullPath(path);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is NotSupportedException || ex is PathTooLongException)
            {
                ShowWarning("Đường dẫn không hợp lệ.");
                return false;
            }

            if (!Directory.Exists(fullPath) && !File.Exists(fullPath))
            {
                ShowWarning("Không tìm thấy đường dẫn:\n" + fullPath);
                return false;
            }

            if (!SelectPath(fullPath))
            {
                ShowWarning("Không hiển thị được mục này trong cây (có thể đang bị ẩn hoặc nằm ngoài các ổ đĩa).\nHãy thử bật \"Hiện tệp ẩn\".");
                return false;
            }

            return true;
        }

        private void GoToParentFolder()
        {
            ExplorerItem item = GetSelectedItem();
            if (item == null || item.IsDriveRoot)
            {
                return;
            }

            DirectoryInfo parent = Directory.GetParent(item.FullPath);
            if (parent != null)
            {
                SelectPath(parent.FullName);
            }
        }

        private void btnGoUp_Click(object sender, EventArgs e) => GoToParentFolder();

        private void btnGo_Click(object sender, EventArgs e) => NavigateTo(txtAddress.Text);

        private void txtAddress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                NavigateTo(txtAddress.Text);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            TreeNode selected = tvExplorer.SelectedNode;
            ExplorerItem item = selected?.Tag as ExplorerItem;
            if (item == null)
            {
                return;
            }

            TreeNode folderNode = item.IsDirectory ? selected : selected.Parent;
            ExplorerItem folderItem = folderNode?.Tag as ExplorerItem;
            if (folderItem == null)
            {
                return;
            }

            string selectedPath = item.FullPath;
            folderItem.IsLoaded = false;
            LoadChildren(folderNode, folderItem);
            folderNode.Expand();
            SelectPath(selectedPath);
            lblStatus.Text = "Đã làm mới: " + folderItem.FullPath;
        }

        private void chkShowHidden_CheckedChanged(object sender, EventArgs e)
        {
            string keepPath = GetSelectedItem()?.FullPath;
            BuildDriveNodes();
            if (!string.IsNullOrEmpty(keepPath))
            {
                SelectPath(keepPath);
            }
        }

        private void btnOpenExplorer_Click(object sender, EventArgs e)
        {
            ExplorerItem item = GetSelectedItem();
            if (item == null)
            {
                return;
            }

            string argument = item.IsDirectory ? "\"" + item.FullPath + "\"" : "/select,\"" + item.FullPath + "\"";
            try
            {
                Process.Start(new ProcessStartInfo("explorer.exe", argument) { UseShellExecute = true });
            }
            catch (Win32Exception ex)
            {
                ShowWarning("Không mở được Windows Explorer: " + ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // Hiển thị nội dung
        // ------------------------------------------------------------------

        private void ShowMessageSurface(string message)
        {
            lblPlaceholder.Text = message;
            lblPlaceholder.Visible = true;
            picPreview.Visible = false;
            txtContent.Visible = false;
        }

        private void ReplacePreviewImage(Image newImage)
        {
            Image previous = picPreview.Image;
            picPreview.Image = newImage;
            previous?.Dispose();
        }

        private void ShowFolderSummary(ExplorerItem item)
        {
            lblContentTitle.Text = item.Name;
            ReplacePreviewImage(null);

            string summary = item.IsLoaded
                ? "Thư mục chứa " + item.ChildFolderCount.ToString("N0") + " thư mục con và " + item.ChildFileCount.ToString("N0") + " tệp."
                : "Nhấp đúp vào thư mục để mở và xem các mục bên trong.";
            ShowMessageSurface(summary);
        }

        private async void ShowFileContent(ExplorerItem item)
        {
            contentCancellation?.Cancel();
            contentCancellation = new CancellationTokenSource();
            CancellationToken token = contentCancellation.Token;

            lblContentTitle.Text = item.Name;
            ShowMessageSurface("Đang đọc tệp…");

            FileContent content;
            try
            {
                content = await Task.Run(() => FileViewer.Load(item.FullPath, token), token);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            if (token.IsCancellationRequested || IsDisposed)
            {
                content.Dispose();
                return;
            }

            DisplayContent(item, content);
        }

        private void DisplayContent(ExplorerItem item, FileContent content)
        {
            string sizeText = FileCatalog.FormatBytes(content.SizeBytes);
            lblContentTitle.Text = item.Name + "   ·   " + sizeText;

            switch (content.Kind)
            {
                case FileContentKind.Image:
                    ReplacePreviewImage(content.Picture);
                    content.Picture = null;
                    lblContentTitle.Text += "   ·   " + content.Summary;
                    lblPlaceholder.Visible = false;
                    txtContent.Visible = false;
                    picPreview.Visible = true;
                    break;

                case FileContentKind.Text:
                case FileContentKind.Binary:
                    ReplacePreviewImage(null);
                    txtContent.WordWrap = content.Kind == FileContentKind.Text;
                    txtContent.Text = content.Text;
                    lblContentTitle.Text += "   ·   " + content.Summary;
                    lblPlaceholder.Visible = false;
                    picPreview.Visible = false;
                    txtContent.Visible = true;
                    break;

                default:
                    ReplacePreviewImage(null);
                    ShowMessageSurface(content.Summary);
                    break;
            }

            content.Dispose();
        }

        // ------------------------------------------------------------------
        // Thuộc tính, SHA-256 và dung lượng thư mục
        // ------------------------------------------------------------------

        private void SetPropertyRow(string key, string title, string value)
        {
            ListViewItem row = lvProperties.Items.ContainsKey(key) ? lvProperties.Items[key] : null;
            if (row == null)
            {
                row = new ListViewItem(title) { Name = key };
                row.SubItems.Add(value);
                lvProperties.Items.Add(row);
            }
            else
            {
                row.SubItems[1].Text = value;
            }
        }

        private void FillProperties(ExplorerItem item)
        {
            lvProperties.BeginUpdate();
            lvProperties.Items.Clear();

            SetPropertyRow("name", "Tên", item.Name);
            SetPropertyRow("type", "Loại", FileCatalog.Describe(Path.GetExtension(item.FullPath), item.IsDirectory, item.IsDriveRoot));
            SetPropertyRow("path", "Đường dẫn đầy đủ", item.FullPath);

            try
            {
                if (item.IsDriveRoot)
                {
                    DriveInfo drive = new DriveInfo(item.FullPath);
                    SetPropertyRow("format", "Định dạng", drive.DriveFormat);
                    SetPropertyRow("total", "Tổng dung lượng", FileCatalog.FormatBytes(drive.TotalSize));
                    SetPropertyRow("free", "Còn trống", FileCatalog.FormatBytes(drive.AvailableFreeSpace));
                }
                else
                {
                    FileSystemInfo info = item.IsDirectory ? (FileSystemInfo)new DirectoryInfo(item.FullPath) : new FileInfo(item.FullPath);
                    if (!item.IsDirectory)
                    {
                        long length = ((FileInfo)info).Length;
                        SetPropertyRow("size", "Kích thước", FileCatalog.FormatBytes(length) + "  (" + length.ToString("N0") + " byte)");
                    }

                    SetPropertyRow("created", "Ngày tạo", FormatDate(info.CreationTime));
                    SetPropertyRow("modified", "Sửa đổi lần cuối", FormatDate(info.LastWriteTime));
                    SetPropertyRow("accessed", "Truy cập lần cuối", FormatDate(info.LastAccessTime));
                    SetPropertyRow("attributes", "Thuộc tính", DescribeAttributes(info.Attributes));
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                SetPropertyRow("error", "Lưu ý", "Không đọc được đầy đủ thuộc tính: " + ex.Message);
            }

            lvProperties.EndUpdate();
        }

        private static string FormatDate(DateTime value)
        {
            return value.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }

        private static string DescribeAttributes(FileAttributes attributes)
        {
            List<string> names = new List<string>();
            if ((attributes & FileAttributes.ReadOnly) != 0) names.Add("Chỉ đọc");
            if ((attributes & FileAttributes.Hidden) != 0) names.Add("Ẩn");
            if ((attributes & FileAttributes.System) != 0) names.Add("Hệ thống");
            if ((attributes & FileAttributes.Archive) != 0) names.Add("Lưu trữ");
            if ((attributes & FileAttributes.Compressed) != 0) names.Add("Nén");
            return names.Count == 0 ? "Bình thường" : string.Join(", ", names);
        }

        private async void btnComputeHash_Click(object sender, EventArgs e)
        {
            ExplorerItem item = GetSelectedItem();
            if (item == null || item.IsDirectory || activeTask != BackgroundTask.None)
            {
                return;
            }

            BeginBackgroundTask(BackgroundTask.FileHash, "Đang tính SHA-256…");
            CancellationToken token = backgroundCancellation.Token;
            IProgress<int> progress = new Progress<int>(percent => ReportStatus("Đang tính SHA-256… " + percent + "%"));

            try
            {
                string hash = await Task.Run(() => FolderScanner.ComputeSha256(item.FullPath, token, progress), token);
                if (IsDisposed) return;
                SetPropertyRow(HashRowKey, "SHA-256", hash);
                lblStatus.Text = "Đã tính xong SHA-256 của " + item.Name;
            }
            catch (OperationCanceledException)
            {
                if (!IsDisposed) lblStatus.Text = "Đã hủy tính SHA-256.";
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                if (!IsDisposed) ShowWarning("Không đọc được tệp để tính SHA-256: " + ex.Message);
            }
            finally
            {
                EndBackgroundTask();
            }
        }

        private async void btnComputeFolderSize_Click(object sender, EventArgs e)
        {
            ExplorerItem item = GetSelectedItem();
            if (item == null || !item.IsDirectory || activeTask != BackgroundTask.None)
            {
                return;
            }

            BeginBackgroundTask(BackgroundTask.FolderSize, "Đang đo dung lượng thư mục…");
            CancellationToken token = backgroundCancellation.Token;
            IProgress<string> progress = new Progress<string>(ReportStatus);

            try
            {
                FolderStatistics statistics = await Task.Run(() => FolderScanner.MeasureFolder(item.FullPath, token, progress), token);
                if (IsDisposed) return;

                string summary = FileCatalog.FormatBytes(statistics.TotalBytes) + "  ·  "
                    + statistics.FileCount.ToString("N0") + " tệp  ·  " + statistics.FolderCount.ToString("N0") + " thư mục";
                if (statistics.SkippedFolderCount > 0)
                {
                    summary += "  ·  bỏ qua " + statistics.SkippedFolderCount + " thư mục không có quyền";
                }

                SetPropertyRow(FolderSizeRowKey, "Dung lượng thư mục", summary);
                lblStatus.Text = "Đã đo xong: " + item.Name;
            }
            catch (OperationCanceledException)
            {
                if (!IsDisposed) lblStatus.Text = "Đã hủy đo dung lượng.";
            }
            finally
            {
                EndBackgroundTask();
            }
        }

        // ------------------------------------------------------------------
        // Tìm kiếm
        // ------------------------------------------------------------------

        private static string ResolveSearchScope(ExplorerItem item)
        {
            if (item == null)
            {
                return string.Empty;
            }

            return item.IsDirectory ? item.FullPath : Path.GetDirectoryName(item.FullPath) ?? string.Empty;
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnSearch.PerformClick();
            }
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            if (activeTask != BackgroundTask.None)
            {
                return;
            }

            string keyword = txtSearch.Text.Trim();
            string scope = ResolveSearchScope(GetSelectedItem());
            if (keyword.Length == 0)
            {
                ShowWarning("Hãy nhập tên tệp hoặc thư mục cần tìm.");
                return;
            }

            if (scope.Length == 0 || !Directory.Exists(scope))
            {
                ShowWarning("Hãy chọn một thư mục hoặc ổ đĩa trong cây để làm phạm vi tìm kiếm.");
                return;
            }

            BeginBackgroundTask(BackgroundTask.Search, "Đang tìm \"" + keyword + "\"…");
            CancellationToken token = backgroundCancellation.Token;
            IProgress<string> progress = new Progress<string>(ReportStatus);
            lvSearchResults.Items.Clear();

            try
            {
                List<SearchHit> hits = await Task.Run(() => FolderScanner.SearchByName(scope, keyword, MaxSearchResults, token, progress), token);
                if (IsDisposed) return;

                FillSearchResults(hits);
                string limitNote = hits.Count >= MaxSearchResults ? " (đã đạt giới hạn " + MaxSearchResults + " kết quả)" : string.Empty;
                lblStatus.Text = "Tìm thấy " + hits.Count + " mục khớp \"" + keyword + "\"" + limitNote;
            }
            catch (OperationCanceledException)
            {
                if (!IsDisposed) lblStatus.Text = "Đã hủy tìm kiếm.";
            }
            finally
            {
                EndBackgroundTask();
            }
        }

        private void FillSearchResults(List<SearchHit> hits)
        {
            lvSearchResults.BeginUpdate();
            lvSearchResults.Items.Clear();
            foreach (SearchHit hit in hits)
            {
                ListViewItem row = new ListViewItem(hit.Name) { Tag = hit.FullPath };
                row.SubItems.Add(hit.IsDirectory ? "Thư mục" : FileCatalog.Describe(Path.GetExtension(hit.Name), false, false));
                row.SubItems.Add(hit.IsDirectory ? string.Empty : FileCatalog.FormatBytes(hit.SizeBytes));
                row.SubItems.Add(hit.ParentPath);
                lvSearchResults.Items.Add(row);
            }
            lvSearchResults.EndUpdate();
            StretchLastColumn(lvSearchResults);
        }

        private void lvSearchResults_DoubleClick(object sender, EventArgs e)
        {
            if (lvSearchResults.SelectedItems.Count == 0)
            {
                return;
            }

            if (NavigateTo((string)lvSearchResults.SelectedItems[0].Tag))
            {
                SetViewMode(ViewMode.Content);
            }
        }

        // ------------------------------------------------------------------
        // Yêu thích
        // ------------------------------------------------------------------

        private void btnFavorite_Click(object sender, EventArgs e)
        {
            ExplorerItem item = GetSelectedItem();
            if (item == null)
            {
                return;
            }

            bool nowFavorite = favoriteStore.Toggle(item.FullPath);
            RefreshFavoriteList();
            UpdateFavoriteButton();
            lblStatus.Text = nowFavorite ? "Đã thêm vào yêu thích: " + item.Name : "Đã bỏ khỏi yêu thích: " + item.Name;
        }

        private void UpdateFavoriteButton()
        {
            ExplorerItem item = GetSelectedItem();
            bool isFavorite = item != null && favoriteStore != null && favoriteStore.Contains(item.FullPath);
            btnFavorite.Text = isFavorite ? "★  Đã yêu thích" : "☆  Yêu thích";
            btnFavorite.Enabled = item != null;
        }

        private void RefreshFavoriteList()
        {
            lvFavorites.BeginUpdate();
            lvFavorites.Items.Clear();
            foreach (FavoriteEntry entry in favoriteStore.Entries)
            {
                bool isFolder = Directory.Exists(entry.Path);
                string name = Path.GetFileName(entry.Path.TrimEnd(PathSeparators));
                ListViewItem row = new ListViewItem(name.Length == 0 ? entry.Path : name) { Tag = entry.Path };
                row.SubItems.Add(isFolder ? "Thư mục" : (File.Exists(entry.Path) ? "Tệp" : "Không còn tồn tại"));
                row.SubItems.Add(entry.Path);
                lvFavorites.Items.Add(row);
            }
            lvFavorites.EndUpdate();
            StretchLastColumn(lvFavorites);
        }

        private void OpenSelectedFavorite()
        {
            if (lvFavorites.SelectedItems.Count == 0)
            {
                ShowWarning("Hãy chọn một mục trong danh sách yêu thích.");
                return;
            }

            if (NavigateTo((string)lvFavorites.SelectedItems[0].Tag))
            {
                SetViewMode(ViewMode.Content);
            }
        }

        private void lvFavorites_DoubleClick(object sender, EventArgs e) => OpenSelectedFavorite();

        private void btnOpenFavorite_Click(object sender, EventArgs e) => OpenSelectedFavorite();

        private void btnRemoveFavorite_Click(object sender, EventArgs e)
        {
            if (lvFavorites.SelectedItems.Count == 0)
            {
                ShowWarning("Hãy chọn mục cần bỏ khỏi danh sách yêu thích.");
                return;
            }

            favoriteStore.Remove((string)lvFavorites.SelectedItems[0].Tag);
            RefreshFavoriteList();
            UpdateFavoriteButton();
        }

        // ------------------------------------------------------------------
        // Tác vụ nền
        // ------------------------------------------------------------------

        private void BeginBackgroundTask(BackgroundTask task, string message)
        {
            activeTask = task;
            backgroundCancellation = new CancellationTokenSource();
            progressBusy.Visible = true;
            btnCancelWork.Visible = true;
            lblStatus.Text = message;
            btnSearch.Enabled = false;
            btnComputeHash.Enabled = false;
            btnComputeFolderSize.Enabled = false;
        }

        private void EndBackgroundTask()
        {
            activeTask = BackgroundTask.None;
            backgroundCancellation?.Dispose();
            backgroundCancellation = null;

            if (IsDisposed)
            {
                return;
            }

            ExplorerItem item = GetSelectedItem();
            progressBusy.Visible = false;
            btnCancelWork.Visible = false;
            btnSearch.Enabled = true;
            btnComputeHash.Enabled = item != null && !item.IsDirectory;
            btnComputeFolderSize.Enabled = item != null && item.IsDirectory;
        }

        private void btnCancelWork_Click(object sender, EventArgs e)
        {
            backgroundCancellation?.Cancel();
        }

        private void ReportStatus(string text)
        {
            if (!IsDisposed)
            {
                lblStatus.Text = text;
            }
        }

        private void ShowWarning(string message)
        {
            MessageBox.Show(this, message, "Bài 07 - Duyệt thư mục", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
