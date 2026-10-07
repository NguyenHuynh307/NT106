namespace Lab02
{
    partial class Lab02_Bai05
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
            pnlHeader = new AnimatedHeader();
            pnlStatus = new Panel();
            pnlBody = new Panel();
            pnlLeft = new Panel();
            pnlMid = new Panel();
            pnlRight = new Panel();
            pnlSeatMatrix = new Panel();
            lblStatus = new Label();
            lblAddTitle = new Label();
            lblNameCap = new Label();
            lblPriceCap = new Label();
            lblRoomsCap = new Label();
            lblAddHint = new Label();
            lblListTitle = new Label();
            lblMovieCount = new Label();
            lblBookTitle = new Label();
            lblCustomerCap = new Label();
            lblMovieCap = new Label();
            lblRoomCap = new Label();
            lblShowtimeCap = new Label();
            lblPriceTag = new Label();
            lblNotice = new Label();
            lblScreen = new Label();
            lblStatsTitle = new Label();
            lblTotal = new Label();
            lblInvoiceTitle = new Label();
            lblProgressCap = new Label();
            flowLegend = new FlowLayoutPanel();
            tlpMain = new TableLayoutPanel();
            tlpLeft = new TableLayoutPanel();
            tlpPriceRoom = new TableLayoutPanel();
            tlpAddButtons = new TableLayoutPanel();
            tlpListHead = new TableLayoutPanel();
            tlpMid = new TableLayoutPanel();
            tlpRoomShow = new TableLayoutPanel();
            tlpRight = new TableLayoutPanel();
            btnReadFile = new Button();
            btnExport = new Button();
            btnReset = new Button();
            btnExit = new Button();
            btnAddMovie = new Button();
            btnSaveFile = new Button();
            btnRemoveMovie = new Button();
            btnBook = new Button();
            txtMovieName = new TextBox();
            txtPrice = new TextBox();
            txtRooms = new TextBox();
            txtCustomer = new TextBox();
            cboMovie = new ComboBox();
            cboRoom = new ComboBox();
            cboShowtime = new ComboBox();
            dgvMovies = new DataGridView();
            dgvStats = new DataGridView();
            rtbInvoice = new RichTextBox();
            prgExport = new StarProgressBar();
            colMovieName = new DataGridViewTextBoxColumn();
            colMoviePrice = new DataGridViewTextBoxColumn();
            colMovieRooms = new DataGridViewTextBoxColumn();
            colRank = new DataGridViewTextBoxColumn();
            colStatMovie = new DataGridViewTextBoxColumn();
            colSold = new DataGridViewTextBoxColumn();
            colLeft = new DataGridViewTextBoxColumn();
            colRate = new DataGridViewTextBoxColumn();
            colRevenue = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dgvMovies).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvStats).BeginInit();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(6, 7, 26);
            pnlHeader.Controls.Add(btnReadFile);
            pnlHeader.Controls.Add(btnExport);
            pnlHeader.Controls.Add(btnReset);
            pnlHeader.Controls.Add(btnExit);
            pnlHeader.Subtitle = "Đọc input5.txt, đặt vé, thống kê doanh thu, xuất output5.txt";
            pnlHeader.Title = "Bài 05 - Quản Lý Phòng Vé";
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1240, 96);
            pnlHeader.TabIndex = 0;
            // 
            // btnReadFile
            // 
            btnReadFile.Margin = new Padding(4, 0, 4, 0);
            btnReadFile.Name = "btnReadFile";
            btnReadFile.Size = new Size(120, 40);
            btnReadFile.TabIndex = 1;
            btnReadFile.Text = "ĐỌC FILE";
            btnReadFile.Click += btnReadFile_Click;
            // 
            // btnExport
            // 
            btnExport.Margin = new Padding(4, 0, 4, 0);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(160, 40);
            btnExport.TabIndex = 2;
            btnExport.Text = "XUẤT THỐNG KÊ";
            btnExport.Click += btnExport_Click;
            // 
            // btnReset
            // 
            btnReset.Margin = new Padding(4, 0, 4, 0);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(112, 40);
            btnReset.TabIndex = 3;
            btnReset.Text = "LÀM MỚI";
            btnReset.Click += btnReset_Click;
            // 
            // btnExit
            // 
            btnExit.Margin = new Padding(4, 0, 4, 0);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(100, 40);
            btnExit.TabIndex = 4;
            btnExit.Text = "THOÁT";
            btnExit.Click += btnExit_Click;
            // 
            // pnlStatus
            // 
            pnlStatus.BackColor = Color.FromArgb(3, 4, 14);
            pnlStatus.Controls.Add(lblStatus);
            pnlStatus.Dock = DockStyle.Bottom;
            pnlStatus.Name = "pnlStatus";
            pnlStatus.Size = new Size(1240, 32);
            pnlStatus.TabIndex = 2;
            // 
            // lblStatus
            // 
            lblStatus.Dock = DockStyle.Fill;
            lblStatus.Font = new Font("Segoe UI", 9F);
            lblStatus.ForeColor = Color.FromArgb(120, 225, 255);
            lblStatus.Name = "lblStatus";
            lblStatus.Padding = new Padding(20, 0, 20, 0);
            lblStatus.Text = "Sẵn sàng.";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pnlBody
            // 
            pnlBody.BackColor = Color.FromArgb(5, 6, 20);
            pnlBody.Controls.Add(tlpMain);
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(14, 12, 14, 14);
            pnlBody.TabIndex = 1;
            // 
            // tlpMain
            // 
            tlpMain.BackColor = Color.Transparent;
            tlpMain.ColumnCount = 3;
            tlpMain.Dock = DockStyle.Fill;
            tlpMain.Name = "tlpMain";
            tlpMain.RowCount = 1;
            tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 26F));
            tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));
            tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpMain.Controls.Add(pnlLeft, 0, 0);
            tlpMain.Controls.Add(pnlMid, 1, 0);
            tlpMain.Controls.Add(pnlRight, 2, 0);
            // 
            // pnlLeft
            // 
            pnlLeft.BackColor = Color.FromArgb(14, 16, 40);
            pnlLeft.Controls.Add(tlpLeft);
            pnlLeft.Dock = DockStyle.Fill;
            pnlLeft.Margin = new Padding(6, 0, 6, 0);
            pnlLeft.Name = "pnlLeft";
            pnlLeft.Padding = new Padding(16, 14, 16, 14);
            // 
            // pnlMid
            // 
            pnlMid.BackColor = Color.FromArgb(14, 16, 40);
            pnlMid.Controls.Add(tlpMid);
            pnlMid.Dock = DockStyle.Fill;
            pnlMid.Margin = new Padding(6, 0, 6, 0);
            pnlMid.Name = "pnlMid";
            pnlMid.Padding = new Padding(16, 14, 16, 14);
            // 
            // pnlRight
            // 
            pnlRight.BackColor = Color.FromArgb(14, 16, 40);
            pnlRight.Controls.Add(tlpRight);
            pnlRight.Dock = DockStyle.Fill;
            pnlRight.Margin = new Padding(6, 0, 6, 0);
            pnlRight.Name = "pnlRight";
            pnlRight.Padding = new Padding(16, 14, 16, 14);
            // 
            // tlpLeft
            // 
            tlpLeft.BackColor = Color.Transparent;
            tlpLeft.ColumnCount = 1;
            tlpLeft.Dock = DockStyle.Fill;
            tlpLeft.Name = "tlpLeft";
            tlpLeft.RowCount = 9;
            tlpLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpLeft.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpLeft.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpLeft.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpLeft.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpLeft.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpLeft.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
            tlpLeft.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpLeft.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            tlpLeft.Controls.Add(lblAddTitle, 0, 0);
            tlpLeft.Controls.Add(lblNameCap, 0, 1);
            tlpLeft.Controls.Add(txtMovieName, 0, 2);
            tlpLeft.Controls.Add(tlpPriceRoom, 0, 3);
            tlpLeft.Controls.Add(lblAddHint, 0, 4);
            tlpLeft.Controls.Add(tlpAddButtons, 0, 5);
            tlpLeft.Controls.Add(tlpListHead, 0, 6);
            tlpLeft.Controls.Add(dgvMovies, 0, 7);
            tlpLeft.Controls.Add(btnRemoveMovie, 0, 8);
            // 
            // lblAddTitle
            // 
            lblAddTitle.AutoSize = true;
            lblAddTitle.Anchor = AnchorStyles.Left;
            lblAddTitle.BackColor = Color.Transparent;
            lblAddTitle.Font = new Font("Palatino Linotype", 13F, FontStyle.Bold);
            lblAddTitle.ForeColor = Color.FromArgb(226, 232, 255);
            lblAddTitle.Margin = new Padding(0, 0, 0, 8);
            lblAddTitle.Name = "lblAddTitle";
            lblAddTitle.Text = "1. Nhập phim";
            // 
            // lblNameCap
            // 
            lblNameCap.AutoSize = true;
            lblNameCap.Anchor = AnchorStyles.Left;
            lblNameCap.BackColor = Color.Transparent;
            lblNameCap.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblNameCap.ForeColor = Color.FromArgb(140, 150, 215);
            lblNameCap.Margin = new Padding(0, 0, 0, 4);
            lblNameCap.Name = "lblNameCap";
            lblNameCap.Text = "TÊN PHIM";
            // 
            // txtMovieName
            // 
            txtMovieName.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtMovieName.BorderStyle = BorderStyle.FixedSingle;
            txtMovieName.Font = new Font("Segoe UI", 10F);
            txtMovieName.Margin = new Padding(0, 0, 0, 12);
            txtMovieName.Name = "txtMovieName";
            txtMovieName.TabIndex = 1;
            // 
            // tlpPriceRoom
            // 
            tlpPriceRoom.BackColor = Color.Transparent;
            tlpPriceRoom.ColumnCount = 2;
            tlpPriceRoom.Dock = DockStyle.Fill;
            tlpPriceRoom.Name = "tlpPriceRoom";
            tlpPriceRoom.RowCount = 2;
            tlpPriceRoom.AutoSize = true;
            tlpPriceRoom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44F));
            tlpPriceRoom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 56F));
            tlpPriceRoom.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpPriceRoom.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpPriceRoom.Controls.Add(lblPriceCap, 0, 0);
            tlpPriceRoom.Controls.Add(lblRoomsCap, 1, 0);
            tlpPriceRoom.Controls.Add(txtPrice, 0, 1);
            tlpPriceRoom.Controls.Add(txtRooms, 1, 1);
            // 
            // lblPriceCap
            // 
            lblPriceCap.AutoSize = true;
            lblPriceCap.Anchor = AnchorStyles.Left;
            lblPriceCap.BackColor = Color.Transparent;
            lblPriceCap.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblPriceCap.ForeColor = Color.FromArgb(140, 150, 215);
            lblPriceCap.Margin = new Padding(0, 0, 0, 4);
            lblPriceCap.Name = "lblPriceCap";
            lblPriceCap.Text = "GIÁ VÉ CHUẨN (Đ)";
            // 
            // lblRoomsCap
            // 
            lblRoomsCap.AutoSize = true;
            lblRoomsCap.Anchor = AnchorStyles.Left;
            lblRoomsCap.BackColor = Color.Transparent;
            lblRoomsCap.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblRoomsCap.ForeColor = Color.FromArgb(140, 150, 215);
            lblRoomsCap.Margin = new Padding(0, 0, 0, 4);
            lblRoomsCap.Name = "lblRoomsCap";
            lblRoomsCap.Text = "PHÒNG CHIẾU";
            // 
            // txtPrice
            // 
            txtPrice.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtPrice.BorderStyle = BorderStyle.FixedSingle;
            txtPrice.Font = new Font("Segoe UI", 10F);
            txtPrice.Margin = new Padding(0, 0, 8, 10);
            txtPrice.Name = "txtPrice";
            txtPrice.TabIndex = 2;
            // 
            // txtRooms
            // 
            txtRooms.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtRooms.BorderStyle = BorderStyle.FixedSingle;
            txtRooms.Font = new Font("Segoe UI", 10F);
            txtRooms.Margin = new Padding(0, 0, 0, 10);
            txtRooms.Name = "txtRooms";
            txtRooms.TabIndex = 3;
            // 
            // lblAddHint
            // 
            lblAddHint.AutoSize = true;
            lblAddHint.Anchor = AnchorStyles.Left;
            lblAddHint.BackColor = Color.Transparent;
            lblAddHint.Font = new Font("Segoe UI", 8F);
            lblAddHint.ForeColor = Color.FromArgb(120, 130, 185);
            lblAddHint.Margin = new Padding(0, 0, 0, 6);
            lblAddHint.Name = "lblAddHint";
            lblAddHint.Text = "Nhiều phòng cách nhau dấu phẩy: 1, 2, 3";
            // 
            // tlpAddButtons
            // 
            tlpAddButtons.BackColor = Color.Transparent;
            tlpAddButtons.ColumnCount = 2;
            tlpAddButtons.Dock = DockStyle.Fill;
            tlpAddButtons.Name = "tlpAddButtons";
            tlpAddButtons.RowCount = 1;
            tlpAddButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpAddButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpAddButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpAddButtons.Controls.Add(btnAddMovie, 0, 0);
            tlpAddButtons.Controls.Add(btnSaveFile, 1, 0);
            // 
            // btnAddMovie
            // 
            btnAddMovie.Dock = DockStyle.Fill;
            btnAddMovie.Margin = new Padding(0, 4, 6, 4);
            btnAddMovie.Name = "btnAddMovie";
            btnAddMovie.TabIndex = 4;
            btnAddMovie.Text = "THÊM PHIM";
            btnAddMovie.Click += btnAddMovie_Click;
            // 
            // btnSaveFile
            // 
            btnSaveFile.Dock = DockStyle.Fill;
            btnSaveFile.Margin = new Padding(6, 4, 0, 4);
            btnSaveFile.Name = "btnSaveFile";
            btnSaveFile.TabIndex = 5;
            btnSaveFile.Text = "GHI input5.txt";
            btnSaveFile.Click += btnSaveFile_Click;
            // 
            // tlpListHead
            // 
            tlpListHead.BackColor = Color.Transparent;
            tlpListHead.ColumnCount = 2;
            tlpListHead.Dock = DockStyle.Fill;
            tlpListHead.Name = "tlpListHead";
            tlpListHead.RowCount = 1;
            tlpListHead.AutoSize = true;
            tlpListHead.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpListHead.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tlpListHead.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpListHead.Controls.Add(lblListTitle, 0, 0);
            tlpListHead.Controls.Add(lblMovieCount, 1, 0);
            // 
            // lblListTitle
            // 
            lblListTitle.AutoSize = true;
            lblListTitle.Anchor = AnchorStyles.Left;
            lblListTitle.BackColor = Color.Transparent;
            lblListTitle.Font = new Font("Palatino Linotype", 13F, FontStyle.Bold);
            lblListTitle.ForeColor = Color.FromArgb(226, 232, 255);
            lblListTitle.Margin = new Padding(0, 10, 0, 8);
            lblListTitle.Name = "lblListTitle";
            lblListTitle.Text = "2. Danh sách phim";
            // 
            // lblMovieCount
            // 
            lblMovieCount.AutoSize = true;
            lblMovieCount.Anchor = AnchorStyles.Right;
            lblMovieCount.BackColor = Color.Transparent;
            lblMovieCount.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblMovieCount.ForeColor = Color.FromArgb(255, 86, 170);
            lblMovieCount.Margin = new Padding(0, 10, 0, 8);
            lblMovieCount.Name = "lblMovieCount";
            lblMovieCount.Text = "0 phim";
            // 
            // dgvMovies
            // 
            dgvMovies.AllowUserToResizeColumns = false;
            dgvMovies.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMovies.Columns.AddRange(new DataGridViewColumn[] { colMovieName, colMoviePrice, colMovieRooms });
            dgvMovies.Dock = DockStyle.Fill;
            dgvMovies.Margin = new Padding(0, 0, 0, 10);
            dgvMovies.Name = "dgvMovies";
            dgvMovies.TabIndex = 6;
            // 
            // btnRemoveMovie
            // 
            btnRemoveMovie.Dock = DockStyle.Fill;
            btnRemoveMovie.Margin = new Padding(0);
            btnRemoveMovie.Name = "btnRemoveMovie";
            btnRemoveMovie.TabIndex = 7;
            btnRemoveMovie.Text = "XÓA PHIM ĐANG CHỌN";
            btnRemoveMovie.Click += btnRemoveMovie_Click;
            // 
            // tlpMid
            // 
            tlpMid.BackColor = Color.Transparent;
            tlpMid.ColumnCount = 1;
            tlpMid.Dock = DockStyle.Fill;
            tlpMid.Name = "tlpMid";
            tlpMid.RowCount = 12;
            tlpMid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpMid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpMid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpMid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpMid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpMid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpMid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpMid.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            tlpMid.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            tlpMid.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
            tlpMid.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpMid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpMid.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tlpMid.Controls.Add(lblBookTitle, 0, 0);
            tlpMid.Controls.Add(lblCustomerCap, 0, 1);
            tlpMid.Controls.Add(txtCustomer, 0, 2);
            tlpMid.Controls.Add(lblMovieCap, 0, 3);
            tlpMid.Controls.Add(cboMovie, 0, 4);
            tlpMid.Controls.Add(tlpRoomShow, 0, 5);
            tlpMid.Controls.Add(lblPriceTag, 0, 6);
            tlpMid.Controls.Add(lblNotice, 0, 7);
            tlpMid.Controls.Add(lblScreen, 0, 8);
            tlpMid.Controls.Add(pnlSeatMatrix, 0, 9);
            tlpMid.Controls.Add(flowLegend, 0, 10);
            tlpMid.Controls.Add(btnBook, 0, 11);
            // 
            // lblBookTitle
            // 
            lblBookTitle.AutoSize = true;
            lblBookTitle.Anchor = AnchorStyles.Left;
            lblBookTitle.BackColor = Color.Transparent;
            lblBookTitle.Font = new Font("Palatino Linotype", 13F, FontStyle.Bold);
            lblBookTitle.ForeColor = Color.FromArgb(226, 232, 255);
            lblBookTitle.Margin = new Padding(0, 0, 0, 8);
            lblBookTitle.Name = "lblBookTitle";
            lblBookTitle.Text = "3. Đặt vé";
            // 
            // lblCustomerCap
            // 
            lblCustomerCap.AutoSize = true;
            lblCustomerCap.Anchor = AnchorStyles.Left;
            lblCustomerCap.BackColor = Color.Transparent;
            lblCustomerCap.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblCustomerCap.ForeColor = Color.FromArgb(140, 150, 215);
            lblCustomerCap.Margin = new Padding(0, 0, 0, 4);
            lblCustomerCap.Name = "lblCustomerCap";
            lblCustomerCap.Text = "KHÁCH HÀNG";
            // 
            // txtCustomer
            // 
            txtCustomer.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtCustomer.BorderStyle = BorderStyle.FixedSingle;
            txtCustomer.Font = new Font("Segoe UI", 10F);
            txtCustomer.Margin = new Padding(0, 0, 0, 12);
            txtCustomer.Name = "txtCustomer";
            txtCustomer.TabIndex = 1;
            // 
            // lblMovieCap
            // 
            lblMovieCap.AutoSize = true;
            lblMovieCap.Anchor = AnchorStyles.Left;
            lblMovieCap.BackColor = Color.Transparent;
            lblMovieCap.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblMovieCap.ForeColor = Color.FromArgb(140, 150, 215);
            lblMovieCap.Margin = new Padding(0, 0, 0, 4);
            lblMovieCap.Name = "lblMovieCap";
            lblMovieCap.Text = "PHIM";
            // 
            // cboMovie
            // 
            cboMovie.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            cboMovie.DropDownStyle = ComboBoxStyle.DropDownList;
            cboMovie.FormattingEnabled = true;
            cboMovie.Font = new Font("Segoe UI", 10F);
            cboMovie.Margin = new Padding(0, 0, 0, 12);
            cboMovie.Name = "cboMovie";
            cboMovie.TabIndex = 2;
            cboMovie.SelectedIndexChanged += cboMovie_SelectedIndexChanged;
            // 
            // tlpRoomShow
            // 
            tlpRoomShow.BackColor = Color.Transparent;
            tlpRoomShow.ColumnCount = 2;
            tlpRoomShow.Dock = DockStyle.Fill;
            tlpRoomShow.Name = "tlpRoomShow";
            tlpRoomShow.RowCount = 2;
            tlpRoomShow.AutoSize = true;
            tlpRoomShow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            tlpRoomShow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
            tlpRoomShow.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpRoomShow.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpRoomShow.Controls.Add(lblRoomCap, 0, 0);
            tlpRoomShow.Controls.Add(lblShowtimeCap, 1, 0);
            tlpRoomShow.Controls.Add(cboRoom, 0, 1);
            tlpRoomShow.Controls.Add(cboShowtime, 1, 1);
            // 
            // lblRoomCap
            // 
            lblRoomCap.AutoSize = true;
            lblRoomCap.Anchor = AnchorStyles.Left;
            lblRoomCap.BackColor = Color.Transparent;
            lblRoomCap.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblRoomCap.ForeColor = Color.FromArgb(140, 150, 215);
            lblRoomCap.Margin = new Padding(0, 0, 0, 4);
            lblRoomCap.Name = "lblRoomCap";
            lblRoomCap.Text = "PHÒNG";
            // 
            // lblShowtimeCap
            // 
            lblShowtimeCap.AutoSize = true;
            lblShowtimeCap.Anchor = AnchorStyles.Left;
            lblShowtimeCap.BackColor = Color.Transparent;
            lblShowtimeCap.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblShowtimeCap.ForeColor = Color.FromArgb(140, 150, 215);
            lblShowtimeCap.Margin = new Padding(0, 0, 0, 4);
            lblShowtimeCap.Name = "lblShowtimeCap";
            lblShowtimeCap.Text = "SUẤT CHIẾU";
            // 
            // cboRoom
            // 
            cboRoom.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            cboRoom.DropDownStyle = ComboBoxStyle.DropDownList;
            cboRoom.FormattingEnabled = true;
            cboRoom.Font = new Font("Segoe UI", 10F);
            cboRoom.Margin = new Padding(0, 0, 8, 6);
            cboRoom.Name = "cboRoom";
            cboRoom.TabIndex = 3;
            cboRoom.SelectedIndexChanged += cboRoom_SelectedIndexChanged;
            // 
            // cboShowtime
            // 
            cboShowtime.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            cboShowtime.DropDownStyle = ComboBoxStyle.DropDownList;
            cboShowtime.FormattingEnabled = true;
            cboShowtime.Font = new Font("Segoe UI", 10F);
            cboShowtime.Margin = new Padding(0, 0, 0, 6);
            cboShowtime.Name = "cboShowtime";
            cboShowtime.TabIndex = 4;
            cboShowtime.SelectedIndexChanged += cboShowtime_SelectedIndexChanged;
            // 
            // lblPriceTag
            // 
            lblPriceTag.AutoEllipsis = true;
            lblPriceTag.BackColor = Color.Transparent;
            lblPriceTag.Dock = DockStyle.Fill;
            lblPriceTag.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            lblPriceTag.ForeColor = Color.FromArgb(255, 206, 100);
            lblPriceTag.Margin = new Padding(0);
            lblPriceTag.Name = "lblPriceTag";
            lblPriceTag.Text = "";
            lblPriceTag.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblNotice
            // 
            lblNotice.AutoEllipsis = true;
            lblNotice.BackColor = Color.Transparent;
            lblNotice.Dock = DockStyle.Fill;
            lblNotice.Font = new Font("Segoe UI", 8.5F);
            lblNotice.ForeColor = Color.FromArgb(150, 160, 215);
            lblNotice.Margin = new Padding(0);
            lblNotice.Name = "lblNotice";
            lblNotice.Text = "";
            lblNotice.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblScreen
            // 
            lblScreen.BackColor = Color.FromArgb(70, 58, 150);
            lblScreen.Dock = DockStyle.Fill;
            lblScreen.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblScreen.ForeColor = Color.FromArgb(215, 210, 255);
            lblScreen.Margin = new Padding(36, 2, 36, 2);
            lblScreen.Name = "lblScreen";
            lblScreen.Text = "MÀN HÌNH";
            lblScreen.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlSeatMatrix
            // 
            pnlSeatMatrix.BackColor = Color.Transparent;
            pnlSeatMatrix.Dock = DockStyle.Fill;
            pnlSeatMatrix.Margin = new Padding(0, 6, 0, 6);
            pnlSeatMatrix.MinimumSize = new Size(0, 120);
            pnlSeatMatrix.Name = "pnlSeatMatrix";
            // 
            // flowLegend
            // 
            flowLegend.AutoSize = true;
            flowLegend.BackColor = Color.Transparent;
            flowLegend.Dock = DockStyle.Fill;
            flowLegend.Margin = new Padding(0, 0, 0, 6);
            flowLegend.Name = "flowLegend";
            // 
            // btnBook
            // 
            btnBook.Dock = DockStyle.Fill;
            btnBook.Margin = new Padding(0);
            btnBook.Name = "btnBook";
            btnBook.TabIndex = 5;
            btnBook.Text = "ĐẶT VÉ";
            btnBook.Click += btnBook_Click;
            // 
            // tlpRight
            // 
            tlpRight.BackColor = Color.Transparent;
            tlpRight.ColumnCount = 1;
            tlpRight.Dock = DockStyle.Fill;
            tlpRight.Name = "tlpRight";
            tlpRight.RowCount = 7;
            tlpRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpRight.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpRight.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            tlpRight.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpRight.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpRight.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            tlpRight.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            tlpRight.Controls.Add(lblStatsTitle, 0, 0);
            tlpRight.Controls.Add(dgvStats, 0, 1);
            tlpRight.Controls.Add(lblTotal, 0, 2);
            tlpRight.Controls.Add(lblInvoiceTitle, 0, 3);
            tlpRight.Controls.Add(rtbInvoice, 0, 4);
            tlpRight.Controls.Add(lblProgressCap, 0, 5);
            tlpRight.Controls.Add(prgExport, 0, 6);
            // 
            // lblStatsTitle
            // 
            lblStatsTitle.AutoSize = true;
            lblStatsTitle.Anchor = AnchorStyles.Left;
            lblStatsTitle.BackColor = Color.Transparent;
            lblStatsTitle.Font = new Font("Palatino Linotype", 13F, FontStyle.Bold);
            lblStatsTitle.ForeColor = Color.FromArgb(226, 232, 255);
            lblStatsTitle.Margin = new Padding(0, 0, 0, 8);
            lblStatsTitle.Name = "lblStatsTitle";
            lblStatsTitle.Text = "4. Thống kê doanh thu";
            // 
            // dgvStats
            // 
            dgvStats.AllowUserToResizeColumns = false;
            dgvStats.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvStats.Columns.AddRange(new DataGridViewColumn[] { colRank, colStatMovie, colSold, colLeft, colRate, colRevenue });
            dgvStats.Dock = DockStyle.Fill;
            dgvStats.Margin = new Padding(0, 0, 0, 10);
            dgvStats.Name = "dgvStats";
            dgvStats.TabIndex = 1;
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.Anchor = AnchorStyles.Left;
            lblTotal.BackColor = Color.Transparent;
            lblTotal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTotal.ForeColor = Color.FromArgb(255, 206, 100);
            lblTotal.Margin = new Padding(0, 0, 0, 10);
            lblTotal.Name = "lblTotal";
            lblTotal.Text = "Tổng: 0 vé  ·  0 đ";
            // 
            // lblInvoiceTitle
            // 
            lblInvoiceTitle.AutoSize = true;
            lblInvoiceTitle.Anchor = AnchorStyles.Left;
            lblInvoiceTitle.BackColor = Color.Transparent;
            lblInvoiceTitle.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblInvoiceTitle.ForeColor = Color.FromArgb(140, 150, 215);
            lblInvoiceTitle.Margin = new Padding(0, 0, 0, 4);
            lblInvoiceTitle.Name = "lblInvoiceTitle";
            lblInvoiceTitle.Text = "HÓA ĐƠN GẦN NHẤT";
            // 
            // rtbInvoice
            // 
            rtbInvoice.BorderStyle = BorderStyle.None;
            rtbInvoice.Dock = DockStyle.Fill;
            rtbInvoice.Font = new Font("Consolas", 9.5F);
            rtbInvoice.Margin = new Padding(0, 0, 0, 10);
            rtbInvoice.Name = "rtbInvoice";
            rtbInvoice.ReadOnly = true;
            rtbInvoice.TabStop = false;
            rtbInvoice.Text = "";
            // 
            // lblProgressCap
            // 
            lblProgressCap.AutoSize = true;
            lblProgressCap.Anchor = AnchorStyles.Left;
            lblProgressCap.BackColor = Color.Transparent;
            lblProgressCap.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblProgressCap.ForeColor = Color.FromArgb(140, 150, 215);
            lblProgressCap.Margin = new Padding(0, 0, 0, 4);
            lblProgressCap.Name = "lblProgressCap";
            lblProgressCap.Text = "TIẾN ĐỘ XUẤT output5.txt";
            // 
            // prgExport
            // 
            prgExport.Dock = DockStyle.Fill;
            prgExport.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            prgExport.Margin = new Padding(0);
            prgExport.Name = "prgExport";
            prgExport.TabIndex = 8;
            // 
            // colMovieName
            // 
            colMovieName.FillWeight = 52F;
            colMovieName.HeaderText = "TÊN PHIM";
            colMovieName.MinimumWidth = 40;
            colMovieName.Name = "colMovieName";
            colMovieName.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // colMoviePrice
            // 
            colMoviePrice.FillWeight = 26F;
            colMoviePrice.HeaderText = "GIÁ";
            colMoviePrice.MinimumWidth = 40;
            colMoviePrice.Name = "colMoviePrice";
            colMoviePrice.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // colMovieRooms
            // 
            colMovieRooms.FillWeight = 22F;
            colMovieRooms.HeaderText = "PHÒNG";
            colMovieRooms.MinimumWidth = 40;
            colMovieRooms.Name = "colMovieRooms";
            colMovieRooms.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // colRank
            // 
            colRank.FillWeight = 12F;
            colRank.HeaderText = "HẠNG";
            colRank.MinimumWidth = 40;
            colRank.Name = "colRank";
            colRank.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // colStatMovie
            // 
            colStatMovie.FillWeight = 31F;
            colStatMovie.HeaderText = "PHIM";
            colStatMovie.MinimumWidth = 40;
            colStatMovie.Name = "colStatMovie";
            colStatMovie.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // colSold
            // 
            colSold.FillWeight = 9F;
            colSold.HeaderText = "BÁN";
            colSold.MinimumWidth = 40;
            colSold.Name = "colSold";
            colSold.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // colLeft
            // 
            colLeft.FillWeight = 9F;
            colLeft.HeaderText = "TỒN";
            colLeft.MinimumWidth = 40;
            colLeft.Name = "colLeft";
            colLeft.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // colRate
            // 
            colRate.FillWeight = 15F;
            colRate.HeaderText = "TỈ LỆ";
            colRate.MinimumWidth = 40;
            colRate.Name = "colRate";
            colRate.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // colRevenue
            // 
            colRevenue.FillWeight = 24F;
            colRevenue.HeaderText = "DOANH THU";
            colRevenue.MinimumWidth = 40;
            colRevenue.Name = "colRevenue";
            colRevenue.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // Lab02_Bai05
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(5, 6, 20);
            ClientSize = new Size(1240, 740);
            Controls.Add(pnlBody);
            Controls.Add(pnlStatus);
            Controls.Add(pnlHeader);
            MinimumSize = new Size(1120, 720);
            Name = "Lab02_Bai05";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Bài 05 - Quản lý phòng vé";
            ((System.ComponentModel.ISupportInitialize)dgvMovies).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvStats).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private AnimatedHeader pnlHeader;
        private Panel pnlStatus;
        private Panel pnlBody;
        private Panel pnlLeft;
        private Panel pnlMid;
        private Panel pnlRight;
        private Panel pnlSeatMatrix;
        private Label lblStatus;
        private Label lblAddTitle;
        private Label lblNameCap;
        private Label lblPriceCap;
        private Label lblRoomsCap;
        private Label lblAddHint;
        private Label lblListTitle;
        private Label lblMovieCount;
        private Label lblBookTitle;
        private Label lblCustomerCap;
        private Label lblMovieCap;
        private Label lblRoomCap;
        private Label lblShowtimeCap;
        private Label lblPriceTag;
        private Label lblNotice;
        private Label lblScreen;
        private Label lblStatsTitle;
        private Label lblTotal;
        private Label lblInvoiceTitle;
        private Label lblProgressCap;
        private FlowLayoutPanel flowLegend;
        private TableLayoutPanel tlpMain;
        private TableLayoutPanel tlpLeft;
        private TableLayoutPanel tlpPriceRoom;
        private TableLayoutPanel tlpAddButtons;
        private TableLayoutPanel tlpListHead;
        private TableLayoutPanel tlpMid;
        private TableLayoutPanel tlpRoomShow;
        private TableLayoutPanel tlpRight;
        private Button btnReadFile;
        private Button btnExport;
        private Button btnReset;
        private Button btnExit;
        private Button btnAddMovie;
        private Button btnSaveFile;
        private Button btnRemoveMovie;
        private Button btnBook;
        private TextBox txtMovieName;
        private TextBox txtPrice;
        private TextBox txtRooms;
        private TextBox txtCustomer;
        private ComboBox cboMovie;
        private ComboBox cboRoom;
        private ComboBox cboShowtime;
        private DataGridView dgvMovies;
        private DataGridView dgvStats;
        private RichTextBox rtbInvoice;
        private StarProgressBar prgExport;
        private DataGridViewTextBoxColumn colMovieName;
        private DataGridViewTextBoxColumn colMoviePrice;
        private DataGridViewTextBoxColumn colMovieRooms;
        private DataGridViewTextBoxColumn colRank;
        private DataGridViewTextBoxColumn colStatMovie;
        private DataGridViewTextBoxColumn colSold;
        private DataGridViewTextBoxColumn colLeft;
        private DataGridViewTextBoxColumn colRate;
        private DataGridViewTextBoxColumn colRevenue;
    }
}
