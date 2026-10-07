namespace Lab02
{
    partial class Lab02_Bai07
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            pnlGoldLine = new Panel();
            lblShortcutHint = new Label();
            lblSubtitle = new Label();
            lblTitle = new Label();
            lblEyebrow = new Label();
            tlpToolbar = new TableLayoutPanel();
            btnGoUp = new Button();
            btnRefresh = new Button();
            txtAddress = new TextBox();
            btnGo = new Button();
            btnFavorite = new Button();
            btnOpenExplorer = new Button();
            chkShowHidden = new CheckBox();
            pnlStatusBar = new Panel();
            lblStatus = new Label();
            progressBusy = new ProgressBar();
            btnCancelWork = new Button();
            splitMain = new SplitContainer();
            pnlTreeFrame = new Panel();
            tvExplorer = new TreeView();
            pnlRight = new Panel();
            pnlViewHost = new Panel();
            pnlContent = new Panel();
            lblPlaceholder = new Label();
            txtContent = new TextBox();
            picPreview = new PictureBox();
            lblContentTitle = new Label();
            pnlProperties = new Panel();
            lvProperties = new ListView();
            pnlPropertyActions = new Panel();
            btnComputeHash = new Button();
            btnComputeFolderSize = new Button();
            pnlSearch = new Panel();
            lvSearchResults = new ListView();
            lblSearchScope = new Label();
            tlpSearchBar = new TableLayoutPanel();
            txtSearch = new TextBox();
            btnSearch = new Button();
            pnlFavorites = new Panel();
            lvFavorites = new ListView();
            pnlFavoriteActions = new Panel();
            btnOpenFavorite = new Button();
            btnRemoveFavorite = new Button();
            flowModes = new FlowLayoutPanel();
            btnModeContent = new Button();
            btnModeProperties = new Button();
            btnModeSearch = new Button();
            btnModeFavorites = new Button();
            pnlHeader.SuspendLayout();
            tlpToolbar.SuspendLayout();
            pnlStatusBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            pnlTreeFrame.SuspendLayout();
            pnlRight.SuspendLayout();
            pnlViewHost.SuspendLayout();
            pnlContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
            pnlProperties.SuspendLayout();
            pnlPropertyActions.SuspendLayout();
            pnlSearch.SuspendLayout();
            tlpSearchBar.SuspendLayout();
            pnlFavorites.SuspendLayout();
            pnlFavoriteActions.SuspendLayout();
            flowModes.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(13, 24, 46);
            pnlHeader.Controls.Add(pnlGoldLine);
            pnlHeader.Controls.Add(lblShortcutHint);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblEyebrow);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1180, 100);
            pnlHeader.TabIndex = 0;
            // 
            // pnlGoldLine
            // 
            pnlGoldLine.BackColor = Color.FromArgb(201, 162, 39);
            pnlGoldLine.Dock = DockStyle.Bottom;
            pnlGoldLine.Location = new Point(0, 97);
            pnlGoldLine.Name = "pnlGoldLine";
            pnlGoldLine.Size = new Size(1180, 3);
            pnlGoldLine.TabIndex = 4;
            // 
            // lblShortcutHint
            // 
            lblShortcutHint.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblShortcutHint.AutoSize = true;
            lblShortcutHint.BackColor = Color.Transparent;
            lblShortcutHint.Font = new Font("Segoe UI", 9F);
            lblShortcutHint.ForeColor = Color.FromArgb(176, 190, 214);
            lblShortcutHint.Location = new Point(790, 38);
            lblShortcutHint.Name = "lblShortcutHint";
            lblShortcutHint.Size = new Size(360, 45);
            lblShortcutHint.TabIndex = 3;
            lblShortcutHint.Text = "F5  làm mới thư mục\r\nCtrl + F  tìm kiếm   ·   Backspace  lên thư mục cha\r\nNhấp đúp thư mục để mở, chọn tệp để xem nội dung";
            lblShortcutHint.TextAlign = ContentAlignment.TopRight;
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.BackColor = Color.Transparent;
            lblSubtitle.Font = new Font("Segoe UI", 10.5F);
            lblSubtitle.ForeColor = Color.FromArgb(176, 190, 214);
            lblSubtitle.Location = new Point(42, 70);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(320, 19);
            lblSubtitle.TabIndex = 2;
            lblSubtitle.Text = "Duyệt ổ đĩa, thư mục và xem nội dung tệp tin";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Palatino Linotype", 22F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(38, 28);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(360, 35);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "Thư Viện Tệp Tin";
            // 
            // lblEyebrow
            // 
            lblEyebrow.AutoSize = true;
            lblEyebrow.BackColor = Color.Transparent;
            lblEyebrow.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblEyebrow.ForeColor = Color.FromArgb(201, 162, 39);
            lblEyebrow.Location = new Point(42, 10);
            lblEyebrow.Name = "lblEyebrow";
            lblEyebrow.Size = new Size(300, 15);
            lblEyebrow.TabIndex = 0;
            lblEyebrow.Text = "LẬP TRÌNH MẠNG CĂN BẢN   ·   BÀI 07";
            // 
            // tlpToolbar
            // 
            tlpToolbar.BackColor = Color.FromArgb(252, 249, 240);
            tlpToolbar.ColumnCount = 7;
            tlpToolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 88F));
            tlpToolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 116F));
            tlpToolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpToolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 96F));
            tlpToolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 156F));
            tlpToolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170F));
            tlpToolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            tlpToolbar.Controls.Add(btnGoUp, 0, 0);
            tlpToolbar.Controls.Add(btnRefresh, 1, 0);
            tlpToolbar.Controls.Add(txtAddress, 2, 0);
            tlpToolbar.Controls.Add(btnGo, 3, 0);
            tlpToolbar.Controls.Add(btnFavorite, 4, 0);
            tlpToolbar.Controls.Add(btnOpenExplorer, 5, 0);
            tlpToolbar.Controls.Add(chkShowHidden, 6, 0);
            tlpToolbar.Dock = DockStyle.Top;
            tlpToolbar.Location = new Point(0, 100);
            tlpToolbar.Name = "tlpToolbar";
            tlpToolbar.Padding = new Padding(10, 6, 10, 6);
            tlpToolbar.RowCount = 1;
            tlpToolbar.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpToolbar.Size = new Size(1180, 56);
            tlpToolbar.TabIndex = 1;
            // 
            // btnGoUp
            // 
            btnGoUp.Dock = DockStyle.Fill;
            btnGoUp.Margin = new Padding(3);
            btnGoUp.Name = "btnGoUp";
            btnGoUp.TabIndex = 0;
            btnGoUp.Text = "↑  Lên";
            btnGoUp.Click += btnGoUp_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.Dock = DockStyle.Fill;
            btnRefresh.Margin = new Padding(3);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.TabIndex = 1;
            btnRefresh.Text = "⟳  Làm mới";
            btnRefresh.Click += btnRefresh_Click;
            // 
            // txtAddress
            // 
            txtAddress.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtAddress.BorderStyle = BorderStyle.FixedSingle;
            txtAddress.Font = new Font("Segoe UI", 11F);
            txtAddress.Margin = new Padding(6, 3, 6, 3);
            txtAddress.Name = "txtAddress";
            txtAddress.TabIndex = 2;
            txtAddress.KeyDown += txtAddress_KeyDown;
            // 
            // btnGo
            // 
            btnGo.Dock = DockStyle.Fill;
            btnGo.Margin = new Padding(3);
            btnGo.Name = "btnGo";
            btnGo.TabIndex = 3;
            btnGo.Text = "ĐI ĐẾN";
            btnGo.Click += btnGo_Click;
            // 
            // btnFavorite
            // 
            btnFavorite.Dock = DockStyle.Fill;
            btnFavorite.Margin = new Padding(3);
            btnFavorite.Name = "btnFavorite";
            btnFavorite.TabIndex = 4;
            btnFavorite.Text = "☆  Yêu thích";
            btnFavorite.Click += btnFavorite_Click;
            // 
            // btnOpenExplorer
            // 
            btnOpenExplorer.Dock = DockStyle.Fill;
            btnOpenExplorer.Margin = new Padding(3);
            btnOpenExplorer.Name = "btnOpenExplorer";
            btnOpenExplorer.TabIndex = 5;
            btnOpenExplorer.Text = "Mở trong Explorer";
            btnOpenExplorer.Click += btnOpenExplorer_Click;
            // 
            // chkShowHidden
            // 
            chkShowHidden.Anchor = AnchorStyles.Left;
            chkShowHidden.AutoSize = true;
            chkShowHidden.Font = new Font("Segoe UI", 9.5F);
            chkShowHidden.ForeColor = Color.FromArgb(31, 41, 55);
            chkShowHidden.Margin = new Padding(8, 3, 3, 3);
            chkShowHidden.Name = "chkShowHidden";
            chkShowHidden.TabIndex = 6;
            chkShowHidden.Text = "Hiện tệp ẩn";
            chkShowHidden.CheckedChanged += chkShowHidden_CheckedChanged;
            // 
            // pnlStatusBar
            // 
            pnlStatusBar.BackColor = Color.FromArgb(22, 36, 66);
            pnlStatusBar.Controls.Add(lblStatus);
            pnlStatusBar.Controls.Add(progressBusy);
            pnlStatusBar.Controls.Add(btnCancelWork);
            pnlStatusBar.Dock = DockStyle.Bottom;
            pnlStatusBar.Location = new Point(0, 686);
            pnlStatusBar.Name = "pnlStatusBar";
            pnlStatusBar.Padding = new Padding(4, 4, 8, 4);
            pnlStatusBar.Size = new Size(1180, 34);
            pnlStatusBar.TabIndex = 3;
            // 
            // lblStatus
            // 
            lblStatus.AutoEllipsis = true;
            lblStatus.Dock = DockStyle.Fill;
            lblStatus.Font = new Font("Segoe UI", 9.5F);
            lblStatus.ForeColor = Color.FromArgb(222, 230, 244);
            lblStatus.Name = "lblStatus";
            lblStatus.Padding = new Padding(10, 0, 0, 0);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "Sẵn sàng.";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // progressBusy
            // 
            progressBusy.Dock = DockStyle.Right;
            progressBusy.MarqueeAnimationSpeed = 40;
            progressBusy.Name = "progressBusy";
            progressBusy.Size = new Size(160, 26);
            progressBusy.Style = ProgressBarStyle.Marquee;
            progressBusy.TabIndex = 1;
            progressBusy.Visible = false;
            // 
            // btnCancelWork
            // 
            btnCancelWork.Dock = DockStyle.Right;
            btnCancelWork.Name = "btnCancelWork";
            btnCancelWork.Size = new Size(110, 26);
            btnCancelWork.TabIndex = 2;
            btnCancelWork.Text = "HỦY";
            btnCancelWork.Visible = false;
            btnCancelWork.Click += btnCancelWork_Click;
            // 
            // splitMain
            // 
            splitMain.BackColor = Color.FromArgb(247, 244, 236);
            splitMain.Dock = DockStyle.Fill;
            splitMain.Location = new Point(0, 156);
            splitMain.Name = "splitMain";
            splitMain.Panel1.Controls.Add(pnlTreeFrame);
            splitMain.Panel1.Padding = new Padding(10, 4, 4, 10);
            splitMain.Panel1MinSize = 240;
            splitMain.Panel2.Controls.Add(pnlRight);
            splitMain.Panel2MinSize = 420;
            splitMain.Size = new Size(1180, 530);
            splitMain.SplitterDistance = 360;
            splitMain.SplitterWidth = 6;
            splitMain.TabIndex = 2;
            // 
            // pnlTreeFrame
            // 
            pnlTreeFrame.BackColor = Color.FromArgb(229, 223, 207);
            pnlTreeFrame.Controls.Add(tvExplorer);
            pnlTreeFrame.Dock = DockStyle.Fill;
            pnlTreeFrame.Name = "pnlTreeFrame";
            pnlTreeFrame.Padding = new Padding(1);
            pnlTreeFrame.TabIndex = 0;
            // 
            // tvExplorer
            // 
            tvExplorer.BackColor = Color.White;
            tvExplorer.BorderStyle = BorderStyle.None;
            tvExplorer.Dock = DockStyle.Fill;
            tvExplorer.Font = new Font("Segoe UI", 10F);
            tvExplorer.FullRowSelect = true;
            tvExplorer.HideSelection = false;
            tvExplorer.HotTracking = true;
            tvExplorer.Name = "tvExplorer";
            tvExplorer.ShowLines = false;
            tvExplorer.TabIndex = 0;
            tvExplorer.BeforeExpand += tvExplorer_BeforeExpand;
            tvExplorer.AfterSelect += tvExplorer_AfterSelect;
            tvExplorer.NodeMouseDoubleClick += tvExplorer_NodeMouseDoubleClick;
            tvExplorer.KeyDown += tvExplorer_KeyDown;
            // 
            // pnlRight
            // 
            pnlRight.BackColor = Color.FromArgb(247, 244, 236);
            pnlRight.Controls.Add(pnlViewHost);
            pnlRight.Controls.Add(flowModes);
            pnlRight.Dock = DockStyle.Fill;
            pnlRight.Name = "pnlRight";
            pnlRight.Padding = new Padding(4, 4, 10, 10);
            pnlRight.TabIndex = 0;
            // 
            // flowModes
            // 
            flowModes.Controls.Add(btnModeContent);
            flowModes.Controls.Add(btnModeProperties);
            flowModes.Controls.Add(btnModeSearch);
            flowModes.Controls.Add(btnModeFavorites);
            flowModes.Dock = DockStyle.Top;
            flowModes.Name = "flowModes";
            flowModes.Padding = new Padding(0, 0, 0, 6);
            flowModes.Size = new Size(766, 48);
            flowModes.TabIndex = 0;
            // 
            // btnModeContent
            // 
            btnModeContent.Margin = new Padding(0, 0, 8, 0);
            btnModeContent.Name = "btnModeContent";
            btnModeContent.Size = new Size(140, 38);
            btnModeContent.TabIndex = 0;
            btnModeContent.Text = "Nội dung";
            btnModeContent.Click += btnModeContent_Click;
            // 
            // btnModeProperties
            // 
            btnModeProperties.Margin = new Padding(0, 0, 8, 0);
            btnModeProperties.Name = "btnModeProperties";
            btnModeProperties.Size = new Size(140, 38);
            btnModeProperties.TabIndex = 1;
            btnModeProperties.Text = "Thuộc tính";
            btnModeProperties.Click += btnModeProperties_Click;
            // 
            // btnModeSearch
            // 
            btnModeSearch.Margin = new Padding(0, 0, 8, 0);
            btnModeSearch.Name = "btnModeSearch";
            btnModeSearch.Size = new Size(140, 38);
            btnModeSearch.TabIndex = 2;
            btnModeSearch.Text = "Tìm kiếm";
            btnModeSearch.Click += btnModeSearch_Click;
            // 
            // btnModeFavorites
            // 
            btnModeFavorites.Margin = new Padding(0, 0, 8, 0);
            btnModeFavorites.Name = "btnModeFavorites";
            btnModeFavorites.Size = new Size(140, 38);
            btnModeFavorites.TabIndex = 3;
            btnModeFavorites.Text = "Yêu thích";
            btnModeFavorites.Click += btnModeFavorites_Click;
            // 
            // pnlViewHost
            // 
            pnlViewHost.BackColor = Color.FromArgb(229, 223, 207);
            pnlViewHost.Controls.Add(pnlContent);
            pnlViewHost.Controls.Add(pnlProperties);
            pnlViewHost.Controls.Add(pnlSearch);
            pnlViewHost.Controls.Add(pnlFavorites);
            pnlViewHost.Dock = DockStyle.Fill;
            pnlViewHost.Name = "pnlViewHost";
            pnlViewHost.Padding = new Padding(1);
            pnlViewHost.TabIndex = 1;
            // 
            // pnlContent
            // 
            pnlContent.BackColor = Color.White;
            pnlContent.Controls.Add(lblPlaceholder);
            pnlContent.Controls.Add(txtContent);
            pnlContent.Controls.Add(picPreview);
            pnlContent.Controls.Add(lblContentTitle);
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Name = "pnlContent";
            pnlContent.TabIndex = 0;
            // 
            // lblPlaceholder
            // 
            lblPlaceholder.Dock = DockStyle.Fill;
            lblPlaceholder.Font = new Font("Segoe UI", 11.5F, FontStyle.Italic);
            lblPlaceholder.ForeColor = Color.FromArgb(107, 114, 128);
            lblPlaceholder.Name = "lblPlaceholder";
            lblPlaceholder.Padding = new Padding(30);
            lblPlaceholder.TabIndex = 0;
            lblPlaceholder.Text = "Chọn một tệp ở khung bên trái để xem nội dung.";
            lblPlaceholder.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtContent
            // 
            txtContent.BackColor = Color.White;
            txtContent.BorderStyle = BorderStyle.None;
            txtContent.Dock = DockStyle.Fill;
            txtContent.Font = new Font("Consolas", 10F);
            txtContent.ForeColor = Color.FromArgb(31, 41, 55);
            txtContent.MaxLength = int.MaxValue;
            txtContent.Multiline = true;
            txtContent.Name = "txtContent";
            txtContent.ReadOnly = true;
            txtContent.ScrollBars = ScrollBars.Both;
            txtContent.TabIndex = 1;
            txtContent.Visible = false;
            txtContent.WordWrap = false;
            // 
            // picPreview
            // 
            picPreview.BackColor = Color.White;
            picPreview.Dock = DockStyle.Fill;
            picPreview.Name = "picPreview";
            picPreview.SizeMode = PictureBoxSizeMode.Zoom;
            picPreview.TabIndex = 2;
            picPreview.TabStop = false;
            picPreview.Visible = false;
            // 
            // lblContentTitle
            // 
            lblContentTitle.AutoEllipsis = true;
            lblContentTitle.BackColor = Color.FromArgb(252, 249, 240);
            lblContentTitle.Dock = DockStyle.Top;
            lblContentTitle.Font = new Font("Palatino Linotype", 12.5F, FontStyle.Bold);
            lblContentTitle.ForeColor = Color.FromArgb(22, 36, 66);
            lblContentTitle.Name = "lblContentTitle";
            lblContentTitle.Padding = new Padding(14, 0, 0, 0);
            lblContentTitle.Size = new Size(764, 40);
            lblContentTitle.TabIndex = 3;
            lblContentTitle.Text = "Nội dung tệp";
            lblContentTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pnlProperties
            // 
            pnlProperties.BackColor = Color.White;
            pnlProperties.Controls.Add(lvProperties);
            pnlProperties.Controls.Add(pnlPropertyActions);
            pnlProperties.Dock = DockStyle.Fill;
            pnlProperties.Name = "pnlProperties";
            pnlProperties.TabIndex = 1;
            pnlProperties.Visible = false;
            // 
            // lvProperties
            // 
            lvProperties.Dock = DockStyle.Fill;
            lvProperties.Name = "lvProperties";
            lvProperties.TabIndex = 0;
            // 
            // pnlPropertyActions
            // 
            pnlPropertyActions.BackColor = Color.FromArgb(252, 249, 240);
            pnlPropertyActions.Controls.Add(btnComputeHash);
            pnlPropertyActions.Controls.Add(btnComputeFolderSize);
            pnlPropertyActions.Dock = DockStyle.Bottom;
            pnlPropertyActions.Name = "pnlPropertyActions";
            pnlPropertyActions.Size = new Size(764, 56);
            pnlPropertyActions.TabIndex = 1;
            // 
            // btnComputeHash
            // 
            btnComputeHash.Location = new Point(12, 10);
            btnComputeHash.Name = "btnComputeHash";
            btnComputeHash.Size = new Size(200, 36);
            btnComputeHash.TabIndex = 0;
            btnComputeHash.Text = "TÍNH SHA-256 CỦA TỆP";
            btnComputeHash.Click += btnComputeHash_Click;
            // 
            // btnComputeFolderSize
            // 
            btnComputeFolderSize.Location = new Point(222, 10);
            btnComputeFolderSize.Name = "btnComputeFolderSize";
            btnComputeFolderSize.Size = new Size(250, 36);
            btnComputeFolderSize.TabIndex = 1;
            btnComputeFolderSize.Text = "ĐO DUNG LƯỢNG THƯ MỤC";
            btnComputeFolderSize.Click += btnComputeFolderSize_Click;
            // 
            // pnlSearch
            // 
            pnlSearch.BackColor = Color.White;
            pnlSearch.Controls.Add(lvSearchResults);
            pnlSearch.Controls.Add(lblSearchScope);
            pnlSearch.Controls.Add(tlpSearchBar);
            pnlSearch.Dock = DockStyle.Fill;
            pnlSearch.Name = "pnlSearch";
            pnlSearch.TabIndex = 2;
            pnlSearch.Visible = false;
            // 
            // lvSearchResults
            // 
            lvSearchResults.Dock = DockStyle.Fill;
            lvSearchResults.Name = "lvSearchResults";
            lvSearchResults.TabIndex = 0;
            lvSearchResults.DoubleClick += lvSearchResults_DoubleClick;
            // 
            // lblSearchScope
            // 
            lblSearchScope.AutoEllipsis = true;
            lblSearchScope.BackColor = Color.FromArgb(252, 249, 240);
            lblSearchScope.Dock = DockStyle.Top;
            lblSearchScope.Font = new Font("Segoe UI", 9.5F, FontStyle.Italic);
            lblSearchScope.ForeColor = Color.FromArgb(107, 114, 128);
            lblSearchScope.Name = "lblSearchScope";
            lblSearchScope.Padding = new Padding(12, 0, 0, 0);
            lblSearchScope.Size = new Size(764, 28);
            lblSearchScope.TabIndex = 1;
            lblSearchScope.Text = "Phạm vi tìm: chọn một thư mục trong cây.";
            lblSearchScope.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tlpSearchBar
            // 
            tlpSearchBar.BackColor = Color.FromArgb(252, 249, 240);
            tlpSearchBar.ColumnCount = 2;
            tlpSearchBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpSearchBar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            tlpSearchBar.Controls.Add(txtSearch, 0, 0);
            tlpSearchBar.Controls.Add(btnSearch, 1, 0);
            tlpSearchBar.Dock = DockStyle.Top;
            tlpSearchBar.Name = "tlpSearchBar";
            tlpSearchBar.Padding = new Padding(8, 8, 8, 4);
            tlpSearchBar.RowCount = 1;
            tlpSearchBar.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpSearchBar.Size = new Size(764, 52);
            tlpSearchBar.TabIndex = 2;
            // 
            // txtSearch
            // 
            txtSearch.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.Font = new Font("Segoe UI", 11F);
            txtSearch.Margin = new Padding(3, 3, 8, 3);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "Nhập một phần tên tệp hoặc thư mục (không cần gõ dấu)";
            txtSearch.TabIndex = 0;
            txtSearch.KeyDown += txtSearch_KeyDown;
            // 
            // btnSearch
            // 
            btnSearch.Dock = DockStyle.Fill;
            btnSearch.Margin = new Padding(3);
            btnSearch.Name = "btnSearch";
            btnSearch.TabIndex = 1;
            btnSearch.Text = "TÌM";
            btnSearch.Click += btnSearch_Click;
            // 
            // pnlFavorites
            // 
            pnlFavorites.BackColor = Color.White;
            pnlFavorites.Controls.Add(lvFavorites);
            pnlFavorites.Controls.Add(pnlFavoriteActions);
            pnlFavorites.Dock = DockStyle.Fill;
            pnlFavorites.Name = "pnlFavorites";
            pnlFavorites.TabIndex = 3;
            pnlFavorites.Visible = false;
            // 
            // lvFavorites
            // 
            lvFavorites.Dock = DockStyle.Fill;
            lvFavorites.Name = "lvFavorites";
            lvFavorites.TabIndex = 0;
            lvFavorites.DoubleClick += lvFavorites_DoubleClick;
            // 
            // pnlFavoriteActions
            // 
            pnlFavoriteActions.BackColor = Color.FromArgb(252, 249, 240);
            pnlFavoriteActions.Controls.Add(btnOpenFavorite);
            pnlFavoriteActions.Controls.Add(btnRemoveFavorite);
            pnlFavoriteActions.Dock = DockStyle.Bottom;
            pnlFavoriteActions.Name = "pnlFavoriteActions";
            pnlFavoriteActions.Size = new Size(764, 56);
            pnlFavoriteActions.TabIndex = 1;
            // 
            // btnOpenFavorite
            // 
            btnOpenFavorite.Location = new Point(12, 10);
            btnOpenFavorite.Name = "btnOpenFavorite";
            btnOpenFavorite.Size = new Size(150, 36);
            btnOpenFavorite.TabIndex = 0;
            btnOpenFavorite.Text = "MỞ MỤC NÀY";
            btnOpenFavorite.Click += btnOpenFavorite_Click;
            // 
            // btnRemoveFavorite
            // 
            btnRemoveFavorite.Location = new Point(172, 10);
            btnRemoveFavorite.Name = "btnRemoveFavorite";
            btnRemoveFavorite.Size = new Size(230, 36);
            btnRemoveFavorite.TabIndex = 1;
            btnRemoveFavorite.Text = "BỎ KHỎI YÊU THÍCH";
            btnRemoveFavorite.Click += btnRemoveFavorite_Click;
            // 
            // Lab02_Bai07
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(247, 244, 236);
            ClientSize = new Size(1180, 720);
            Controls.Add(splitMain);
            Controls.Add(pnlStatusBar);
            Controls.Add(tlpToolbar);
            Controls.Add(pnlHeader);
            KeyPreview = true;
            MinimumSize = new Size(1000, 640);
            Name = "Lab02_Bai07";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Bài 07 - Duyệt thư mục";
            Load += Lab02_Bai07_Load;
            FormClosed += Lab02_Bai07_FormClosed;
            KeyDown += Lab02_Bai07_KeyDown;
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            tlpToolbar.ResumeLayout(false);
            tlpToolbar.PerformLayout();
            pnlStatusBar.ResumeLayout(false);
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            pnlTreeFrame.ResumeLayout(false);
            pnlRight.ResumeLayout(false);
            pnlViewHost.ResumeLayout(false);
            pnlContent.ResumeLayout(false);
            pnlContent.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picPreview).EndInit();
            pnlProperties.ResumeLayout(false);
            pnlPropertyActions.ResumeLayout(false);
            pnlSearch.ResumeLayout(false);
            tlpSearchBar.ResumeLayout(false);
            tlpSearchBar.PerformLayout();
            pnlFavorites.ResumeLayout(false);
            pnlFavoriteActions.ResumeLayout(false);
            flowModes.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlHeader;
        private Panel pnlGoldLine;
        private Label lblShortcutHint;
        private Label lblSubtitle;
        private Label lblTitle;
        private Label lblEyebrow;
        private TableLayoutPanel tlpToolbar;
        private Button btnGoUp;
        private Button btnRefresh;
        private TextBox txtAddress;
        private Button btnGo;
        private Button btnFavorite;
        private Button btnOpenExplorer;
        private CheckBox chkShowHidden;
        private Panel pnlStatusBar;
        private Label lblStatus;
        private ProgressBar progressBusy;
        private Button btnCancelWork;
        private SplitContainer splitMain;
        private Panel pnlTreeFrame;
        private TreeView tvExplorer;
        private Panel pnlRight;
        private FlowLayoutPanel flowModes;
        private Button btnModeContent;
        private Button btnModeProperties;
        private Button btnModeSearch;
        private Button btnModeFavorites;
        private Panel pnlViewHost;
        private Panel pnlContent;
        private Label lblPlaceholder;
        private TextBox txtContent;
        private PictureBox picPreview;
        private Label lblContentTitle;
        private Panel pnlProperties;
        private ListView lvProperties;
        private Panel pnlPropertyActions;
        private Button btnComputeHash;
        private Button btnComputeFolderSize;
        private Panel pnlSearch;
        private ListView lvSearchResults;
        private Label lblSearchScope;
        private TableLayoutPanel tlpSearchBar;
        private TextBox txtSearch;
        private Button btnSearch;
        private Panel pnlFavorites;
        private ListView lvFavorites;
        private Panel pnlFavoriteActions;
        private Button btnOpenFavorite;
        private Button btnRemoveFavorite;
    }
}
