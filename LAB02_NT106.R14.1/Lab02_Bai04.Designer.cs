#nullable disable

namespace Lab02
{
    partial class Lab02_Bai04
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
            components = new System.ComponentModel.Container();
            pnlHeader = new Panel();
            pnlMint = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            pnlStatus = new Panel();
            lblStatus = new Label();
            btnRefresh = new Button();
            btnExit = new Button();
            pnlInput = new Panel();
            lblInputTitle = new Label();
            lblNameCap = new Label();
            txtName = new TextBox();
            lblIdCap = new Label();
            txtId = new TextBox();
            lblPhoneCap = new Label();
            txtPhone = new TextBox();
            lblScore1Cap = new Label();
            txtScore1 = new TextBox();
            lblScore2Cap = new Label();
            txtScore2 = new TextBox();
            lblScore3Cap = new Label();
            txtScore3 = new TextBox();
            pnlPreview = new Panel();
            lblPreviewCap = new Label();
            lblPreviewValue = new Label();
            lblPreviewRank = new Label();
            btnAdd = new Button();
            btnResetForm = new Button();
            lblHint = new Label();
            pnlList = new Panel();
            lblListTitle = new Label();
            lblPendingCount = new Label();
            dgvPending = new DataGridView();
            colId = new DataGridViewTextBoxColumn();
            colName = new DataGridViewTextBoxColumn();
            colScores = new DataGridViewTextBoxColumn();
            btnDelete = new Button();
            btnWrite = new Button();
            pnlRead = new Panel();
            lblReadTitle = new Label();
            btnRead = new Button();
            txtSearch = new TextBox();
            btnSearch = new Button();
            pnlStudent = new Panel();
            lblStuName = new Label();
            lblStuMeta = new Label();
            lblStu1Cap = new Label();
            lblStu1Val = new Label();
            lblStu2Cap = new Label();
            lblStu2Val = new Label();
            lblStu3Cap = new Label();
            lblStu3Val = new Label();
            pnlStuLine = new Panel();
            lblAvgCap = new Label();
            lblAvgValue = new Label();
            lblRank = new Label();
            btnBack = new Button();
            lblPage = new Label();
            btnNext = new Button();
            pnlStats = new Panel();
            lblCountCap = new Label();
            lblCountValue = new Label();
            lblMeanCap = new Label();
            lblMeanValue = new Label();
            lblMaxCap = new Label();
            lblMaxValue = new Label();
            lblMinCap = new Label();
            lblMinValue = new Label();
            errorProvider = new ErrorProvider(components);
            toolTip = new ToolTip(components);
            pnlHeader.SuspendLayout();
            pnlStatus.SuspendLayout();
            pnlInput.SuspendLayout();
            pnlPreview.SuspendLayout();
            pnlList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPending).BeginInit();
            pnlRead.SuspendLayout();
            pnlStudent.SuspendLayout();
            pnlStats.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
            SuspendLayout();

            pnlHeader.BackColor = Color.FromArgb(10, 40, 34);
            pnlHeader.Controls.Add(btnExit);
            pnlHeader.Controls.Add(btnRefresh);
            pnlHeader.Controls.Add(pnlMint);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1080, 80);
            pnlHeader.TabIndex = 0;

            pnlMint.BackColor = Color.FromArgb(140, 214, 176);
            pnlMint.Dock = DockStyle.Bottom;
            pnlMint.Location = new Point(0, 77);
            pnlMint.Name = "pnlMint";
            pnlMint.Size = new Size(1080, 3);
            pnlMint.TabIndex = 2;

            lblTitle.AutoSize = true;
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Palatino Linotype", 21F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(244, 250, 246);
            lblTitle.Location = new Point(26, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(380, 40);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Bài 04 - Quản Lý Sinh Viên";

            lblSubtitle.AutoSize = true;
            lblSubtitle.BackColor = Color.Transparent;
            lblSubtitle.Font = new Font("Segoe UI", 10F);
            lblSubtitle.ForeColor = Color.FromArgb(170, 214, 190);
            lblSubtitle.Location = new Point(30, 54);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(380, 19);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Nhập sinh viên, ghi input4.txt, đọc lại, tính điểm trung bình, ghi output4.txt";

            pnlStatus.BackColor = Color.FromArgb(10, 40, 34);
                        pnlStatus.Controls.Add(lblStatus);
            pnlStatus.Dock = DockStyle.Bottom;
            pnlStatus.Location = new Point(0, 612);
            pnlStatus.Name = "pnlStatus";
            pnlStatus.Size = new Size(1080, 28);
            pnlStatus.TabIndex = 1;

            lblStatus.AutoEllipsis = true;
            lblStatus.Dock = DockStyle.Fill;
            lblStatus.Font = new Font("Segoe UI", 9F);
            lblStatus.ForeColor = Color.FromArgb(190, 226, 206);
            lblStatus.Location = new Point(0, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Padding = new Padding(24, 0, 24, 0);
            lblStatus.Size = new Size(1080, 28);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "Sẵn sàng. Nhập thông tin sinh viên ở bước 1.";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;

            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefresh.Location = new Point(800, 22);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(124, 36);
            btnRefresh.TabIndex = 1;
            btnRefresh.Text = "LÀM MỚI";
            btnRefresh.Click += btnRefresh_Click;

            btnExit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExit.Location = new Point(934, 22);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(100, 36);
            btnExit.TabIndex = 2;
            btnExit.Text = "THOÁT";
            btnExit.Click += btnExit_Click;

            pnlInput.BackColor = Color.White;
            pnlInput.Controls.Add(lblHint);
            pnlInput.Controls.Add(btnResetForm);
            pnlInput.Controls.Add(btnAdd);
            pnlInput.Controls.Add(pnlPreview);
            pnlInput.Controls.Add(txtScore3);
            pnlInput.Controls.Add(lblScore3Cap);
            pnlInput.Controls.Add(txtScore2);
            pnlInput.Controls.Add(lblScore2Cap);
            pnlInput.Controls.Add(txtScore1);
            pnlInput.Controls.Add(lblScore1Cap);
            pnlInput.Controls.Add(txtPhone);
            pnlInput.Controls.Add(lblPhoneCap);
            pnlInput.Controls.Add(txtId);
            pnlInput.Controls.Add(lblIdCap);
            pnlInput.Controls.Add(txtName);
            pnlInput.Controls.Add(lblNameCap);
            pnlInput.Controls.Add(lblInputTitle);
            pnlInput.Location = new Point(20, 86);
            pnlInput.Name = "pnlInput";
            pnlInput.Size = new Size(316, 520);
            pnlInput.TabIndex = 2;

            lblInputTitle.AutoSize = true;
            lblInputTitle.Font = new Font("Palatino Linotype", 14F, FontStyle.Bold);
            lblInputTitle.ForeColor = Color.FromArgb(24, 76, 60);
            lblInputTitle.Location = new Point(20, 14);
            lblInputTitle.Name = "lblInputTitle";
            lblInputTitle.Size = new Size(200, 27);
            lblInputTitle.TabIndex = 0;
            lblInputTitle.Text = "1. Nhập sinh viên";

            lblNameCap.AutoSize = true;
            lblNameCap.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblNameCap.ForeColor = Color.FromArgb(92, 110, 102);
            lblNameCap.Location = new Point(20, 54);
            lblNameCap.Name = "lblNameCap";
            lblNameCap.Size = new Size(70, 15);
            lblNameCap.TabIndex = 1;
            lblNameCap.Text = "Họ và tên";

            txtName.BackColor = Color.FromArgb(250, 252, 250);
            txtName.BorderStyle = BorderStyle.FixedSingle;
            txtName.Font = new Font("Segoe UI", 11F);
            txtName.ForeColor = Color.FromArgb(24, 38, 34);
            txtName.Location = new Point(20, 74);
            txtName.Name = "txtName";
            txtName.Size = new Size(276, 27);
            txtName.TabIndex = 0;
            txtName.TextChanged += OnInputChanged;

            lblIdCap.AutoSize = true;
            lblIdCap.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblIdCap.ForeColor = Color.FromArgb(92, 110, 102);
            lblIdCap.Location = new Point(20, 116);
            lblIdCap.Name = "lblIdCap";
            lblIdCap.Size = new Size(70, 15);
            lblIdCap.TabIndex = 2;
            lblIdCap.Text = "MSSV";

            txtId.BackColor = Color.FromArgb(250, 252, 250);
            txtId.BorderStyle = BorderStyle.FixedSingle;
            txtId.Font = new Font("Segoe UI", 11F);
            txtId.ForeColor = Color.FromArgb(24, 38, 34);
            txtId.Location = new Point(20, 136);
            txtId.MaxLength = 8;
            txtId.Name = "txtId";
            txtId.Size = new Size(130, 27);
            txtId.TabIndex = 1;
            txtId.TextChanged += OnInputChanged;

            lblPhoneCap.AutoSize = true;
            lblPhoneCap.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPhoneCap.ForeColor = Color.FromArgb(92, 110, 102);
            lblPhoneCap.Location = new Point(166, 116);
            lblPhoneCap.Name = "lblPhoneCap";
            lblPhoneCap.Size = new Size(70, 15);
            lblPhoneCap.TabIndex = 3;
            lblPhoneCap.Text = "Điện thoại";

            txtPhone.BackColor = Color.FromArgb(250, 252, 250);
            txtPhone.BorderStyle = BorderStyle.FixedSingle;
            txtPhone.Font = new Font("Segoe UI", 11F);
            txtPhone.ForeColor = Color.FromArgb(24, 38, 34);
            txtPhone.Location = new Point(166, 136);
            txtPhone.MaxLength = 10;
            txtPhone.Name = "txtPhone";
            txtPhone.Size = new Size(130, 27);
            txtPhone.TabIndex = 2;
            txtPhone.TextChanged += OnInputChanged;

            lblScore1Cap.AutoSize = true;
            lblScore1Cap.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblScore1Cap.ForeColor = Color.FromArgb(92, 110, 102);
            lblScore1Cap.Location = new Point(20, 178);
            lblScore1Cap.Name = "lblScore1Cap";
            lblScore1Cap.Size = new Size(70, 15);
            lblScore1Cap.TabIndex = 4;
            lblScore1Cap.Text = "Điểm môn 1";

            txtScore1.BackColor = Color.FromArgb(250, 252, 250);
            txtScore1.BorderStyle = BorderStyle.FixedSingle;
            txtScore1.Font = new Font("Segoe UI", 11F);
            txtScore1.ForeColor = Color.FromArgb(24, 38, 34);
            txtScore1.Location = new Point(20, 198);
            txtScore1.MaxLength = 5;
            txtScore1.Name = "txtScore1";
            txtScore1.Size = new Size(82, 27);
            txtScore1.TabIndex = 3;
            txtScore1.TextChanged += OnInputChanged;

            lblScore2Cap.AutoSize = true;
            lblScore2Cap.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblScore2Cap.ForeColor = Color.FromArgb(92, 110, 102);
            lblScore2Cap.Location = new Point(117, 178);
            lblScore2Cap.Name = "lblScore2Cap";
            lblScore2Cap.Size = new Size(70, 15);
            lblScore2Cap.TabIndex = 5;
            lblScore2Cap.Text = "Điểm môn 2";

            txtScore2.BackColor = Color.FromArgb(250, 252, 250);
            txtScore2.BorderStyle = BorderStyle.FixedSingle;
            txtScore2.Font = new Font("Segoe UI", 11F);
            txtScore2.ForeColor = Color.FromArgb(24, 38, 34);
            txtScore2.Location = new Point(117, 198);
            txtScore2.MaxLength = 5;
            txtScore2.Name = "txtScore2";
            txtScore2.Size = new Size(82, 27);
            txtScore2.TabIndex = 4;
            txtScore2.TextChanged += OnInputChanged;

            lblScore3Cap.AutoSize = true;
            lblScore3Cap.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblScore3Cap.ForeColor = Color.FromArgb(92, 110, 102);
            lblScore3Cap.Location = new Point(214, 178);
            lblScore3Cap.Name = "lblScore3Cap";
            lblScore3Cap.Size = new Size(70, 15);
            lblScore3Cap.TabIndex = 6;
            lblScore3Cap.Text = "Điểm môn 3";

            txtScore3.BackColor = Color.FromArgb(250, 252, 250);
            txtScore3.BorderStyle = BorderStyle.FixedSingle;
            txtScore3.Font = new Font("Segoe UI", 11F);
            txtScore3.ForeColor = Color.FromArgb(24, 38, 34);
            txtScore3.Location = new Point(214, 198);
            txtScore3.MaxLength = 5;
            txtScore3.Name = "txtScore3";
            txtScore3.Size = new Size(82, 27);
            txtScore3.TabIndex = 5;
            txtScore3.TextChanged += OnInputChanged;
            txtScore3.KeyDown += txtScore3_KeyDown;

            pnlPreview.BackColor = Color.FromArgb(240, 247, 243);
            pnlPreview.Controls.Add(lblPreviewRank);
            pnlPreview.Controls.Add(lblPreviewValue);
            pnlPreview.Controls.Add(lblPreviewCap);
            pnlPreview.Location = new Point(20, 246);
            pnlPreview.Name = "pnlPreview";
            pnlPreview.Size = new Size(276, 66);
            pnlPreview.TabIndex = 7;

            lblPreviewCap.AutoSize = true;
            lblPreviewCap.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblPreviewCap.ForeColor = Color.FromArgb(92, 110, 102);
            lblPreviewCap.Location = new Point(14, 8);
            lblPreviewCap.Name = "lblPreviewCap";
            lblPreviewCap.Size = new Size(120, 15);
            lblPreviewCap.TabIndex = 0;
            lblPreviewCap.Text = "ĐIỂM TRUNG BÌNH (XEM TRƯỚC)";

            lblPreviewValue.AutoSize = true;
            lblPreviewValue.Font = new Font("Palatino Linotype", 20F, FontStyle.Bold);
            lblPreviewValue.ForeColor = Color.FromArgb(24, 76, 60);
            lblPreviewValue.Location = new Point(12, 26);
            lblPreviewValue.Name = "lblPreviewValue";
            lblPreviewValue.Size = new Size(50, 38);
            lblPreviewValue.TabIndex = 1;
            lblPreviewValue.Text = "—";

            lblPreviewRank.AutoSize = false;
            lblPreviewRank.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            lblPreviewRank.ForeColor = Color.FromArgb(92, 110, 102);
            lblPreviewRank.Location = new Point(124, 34);
            lblPreviewRank.Name = "lblPreviewRank";
            lblPreviewRank.Size = new Size(142, 24);
            lblPreviewRank.TabIndex = 2;
            lblPreviewRank.Text = "Nhập đủ 3 điểm";
            lblPreviewRank.TextAlign = ContentAlignment.MiddleRight;

            btnAdd.Location = new Point(20, 328);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(276, 46);
            btnAdd.TabIndex = 6;
            btnAdd.Text = "THÊM VÀO DANH SÁCH";
            btnAdd.Click += btnAdd_Click;

            btnResetForm.Location = new Point(20, 384);
            btnResetForm.Name = "btnResetForm";
            btnResetForm.Size = new Size(276, 36);
            btnResetForm.TabIndex = 7;
            btnResetForm.Text = "XÓA Ô NHẬP";
            btnResetForm.Click += btnResetForm_Click;

            lblHint.Font = new Font("Segoe UI", 9F);
            lblHint.ForeColor = Color.FromArgb(92, 110, 102);
            lblHint.Location = new Point(20, 436);
            lblHint.Name = "lblHint";
            lblHint.Size = new Size(276, 58);
            lblHint.TabIndex = 8;
            lblHint.Text = "•  MSSV: đúng 8 chữ số\r\n•  Điện thoại: 10 chữ số, bắt đầu bằng 0\r\n•  Điểm mỗi môn: từ 0 đến 10";

            pnlList.BackColor = Color.White;
            pnlList.Controls.Add(btnWrite);
            pnlList.Controls.Add(btnDelete);
            pnlList.Controls.Add(dgvPending);
            pnlList.Controls.Add(lblPendingCount);
            pnlList.Controls.Add(lblListTitle);
            pnlList.Location = new Point(352, 86);
            pnlList.Name = "pnlList";
            pnlList.Size = new Size(340, 520);
            pnlList.TabIndex = 3;

            lblListTitle.AutoSize = true;
            lblListTitle.Font = new Font("Palatino Linotype", 14F, FontStyle.Bold);
            lblListTitle.ForeColor = Color.FromArgb(24, 76, 60);
            lblListTitle.Location = new Point(20, 14);
            lblListTitle.Name = "lblListTitle";
            lblListTitle.Size = new Size(200, 27);
            lblListTitle.TabIndex = 0;
            lblListTitle.Text = "2. Danh sách chờ ghi";

            lblPendingCount.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblPendingCount.ForeColor = Color.FromArgb(178, 58, 96);
            lblPendingCount.Location = new Point(230, 20);
            lblPendingCount.Name = "lblPendingCount";
            lblPendingCount.Size = new Size(90, 22);
            lblPendingCount.TabIndex = 1;
            lblPendingCount.Text = "0 sinh viên";
            lblPendingCount.TextAlign = ContentAlignment.MiddleRight;

            dgvPending.Columns.AddRange(new DataGridViewColumn[] { colId, colName, colScores });
            dgvPending.Location = new Point(20, 56);
            dgvPending.Name = "dgvPending";
            dgvPending.Size = new Size(300, 330);
            dgvPending.TabIndex = 2;

            colId.HeaderText = "MSSV";
            colId.Name = "colId";
            colId.Resizable = DataGridViewTriState.False;
            colId.SortMode = DataGridViewColumnSortMode.NotSortable;
            colId.Width = 82;

            colName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colName.HeaderText = "HỌ VÀ TÊN";
            colName.Name = "colName";
            colName.Resizable = DataGridViewTriState.False;
            colName.SortMode = DataGridViewColumnSortMode.NotSortable;

            colScores.HeaderText = "ĐIỂM";
            colScores.Name = "colScores";
            colScores.Resizable = DataGridViewTriState.False;
            colScores.SortMode = DataGridViewColumnSortMode.NotSortable;
            colScores.Width = 92;

            btnDelete.Location = new Point(20, 396);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(300, 36);
            btnDelete.TabIndex = 3;
            btnDelete.Text = "XÓA DÒNG ĐANG CHỌN";
            btnDelete.Click += btnDelete_Click;

            btnWrite.Location = new Point(20, 444);
            btnWrite.Name = "btnWrite";
            btnWrite.Size = new Size(300, 52);
            btnWrite.TabIndex = 4;
            btnWrite.Text = "GHI FILE input4.txt";
            btnWrite.Click += btnWrite_Click;

            pnlRead.BackColor = Color.White;
            pnlRead.Controls.Add(pnlStats);
            pnlRead.Controls.Add(btnNext);
            pnlRead.Controls.Add(lblPage);
            pnlRead.Controls.Add(btnBack);
            pnlRead.Controls.Add(pnlStudent);
            pnlRead.Controls.Add(btnSearch);
            pnlRead.Controls.Add(txtSearch);
            pnlRead.Controls.Add(btnRead);
            pnlRead.Controls.Add(lblReadTitle);
            pnlRead.Location = new Point(708, 86);
            pnlRead.Name = "pnlRead";
            pnlRead.Size = new Size(352, 520);
            pnlRead.TabIndex = 4;

            lblReadTitle.AutoSize = true;
            lblReadTitle.Font = new Font("Palatino Linotype", 14F, FontStyle.Bold);
            lblReadTitle.ForeColor = Color.FromArgb(24, 76, 60);
            lblReadTitle.Location = new Point(20, 14);
            lblReadTitle.Name = "lblReadTitle";
            lblReadTitle.Size = new Size(260, 27);
            lblReadTitle.TabIndex = 0;
            lblReadTitle.Text = "3. Đọc file và tính điểm";

            btnRead.Location = new Point(20, 56);
            btnRead.Name = "btnRead";
            btnRead.Size = new Size(312, 46);
            btnRead.TabIndex = 1;
            btnRead.Text = "ĐỌC input4.txt VÀ TÍNH ĐIỂM";
            btnRead.Click += btnRead_Click;

            txtSearch.BackColor = Color.FromArgb(250, 252, 250);
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.Font = new Font("Segoe UI", 10.5F);
            txtSearch.ForeColor = Color.FromArgb(24, 38, 34);
            txtSearch.Location = new Point(20, 114);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "Tìm theo tên hoặc MSSV";
            txtSearch.Size = new Size(224, 26);
            txtSearch.TabIndex = 2;
            txtSearch.KeyDown += txtSearch_KeyDown;

            btnSearch.Location = new Point(252, 112);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(80, 30);
            btnSearch.TabIndex = 3;
            btnSearch.Text = "TÌM";
            btnSearch.Click += btnSearch_Click;

            pnlStudent.BackColor = Color.FromArgb(246, 250, 247);
            pnlStudent.Controls.Add(lblRank);
            pnlStudent.Controls.Add(lblAvgValue);
            pnlStudent.Controls.Add(lblAvgCap);
            pnlStudent.Controls.Add(pnlStuLine);
            pnlStudent.Controls.Add(lblStu3Val);
            pnlStudent.Controls.Add(lblStu3Cap);
            pnlStudent.Controls.Add(lblStu2Val);
            pnlStudent.Controls.Add(lblStu2Cap);
            pnlStudent.Controls.Add(lblStu1Val);
            pnlStudent.Controls.Add(lblStu1Cap);
            pnlStudent.Controls.Add(lblStuMeta);
            pnlStudent.Controls.Add(lblStuName);
            pnlStudent.Location = new Point(20, 156);
            pnlStudent.Name = "pnlStudent";
            pnlStudent.Size = new Size(312, 226);
            pnlStudent.TabIndex = 4;

            lblStuName.AutoEllipsis = true;
            lblStuName.Font = new Font("Palatino Linotype", 16F, FontStyle.Bold);
            lblStuName.ForeColor = Color.FromArgb(24, 38, 34);
            lblStuName.Location = new Point(16, 14);
            lblStuName.Name = "lblStuName";
            lblStuName.Size = new Size(280, 32);
            lblStuName.TabIndex = 0;
            lblStuName.Text = "Chưa có dữ liệu";

            lblStuMeta.AutoEllipsis = true;
            lblStuMeta.Font = new Font("Segoe UI", 10F);
            lblStuMeta.ForeColor = Color.FromArgb(92, 110, 102);
            lblStuMeta.Location = new Point(16, 48);
            lblStuMeta.Name = "lblStuMeta";
            lblStuMeta.Size = new Size(280, 20);
            lblStuMeta.TabIndex = 1;
            lblStuMeta.Text = "Bấm ĐỌC FILE để nạp input4.txt";

            lblStu1Cap.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblStu1Cap.ForeColor = Color.FromArgb(92, 110, 102);
            lblStu1Cap.Location = new Point(16, 82);
            lblStu1Cap.Name = "lblStu1Cap";
            lblStu1Cap.Size = new Size(88, 16);
            lblStu1Cap.TabIndex = 2;
            lblStu1Cap.Text = "MÔN 1";

            lblStu1Val.Font = new Font("Palatino Linotype", 17F, FontStyle.Bold);
            lblStu1Val.ForeColor = Color.FromArgb(24, 76, 60);
            lblStu1Val.Location = new Point(16, 98);
            lblStu1Val.Name = "lblStu1Val";
            lblStu1Val.Size = new Size(88, 34);
            lblStu1Val.TabIndex = 3;
            lblStu1Val.Text = "—";

            lblStu2Cap.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblStu2Cap.ForeColor = Color.FromArgb(92, 110, 102);
            lblStu2Cap.Location = new Point(112, 82);
            lblStu2Cap.Name = "lblStu2Cap";
            lblStu2Cap.Size = new Size(88, 16);
            lblStu2Cap.TabIndex = 4;
            lblStu2Cap.Text = "MÔN 2";

            lblStu2Val.Font = new Font("Palatino Linotype", 17F, FontStyle.Bold);
            lblStu2Val.ForeColor = Color.FromArgb(24, 76, 60);
            lblStu2Val.Location = new Point(112, 98);
            lblStu2Val.Name = "lblStu2Val";
            lblStu2Val.Size = new Size(88, 34);
            lblStu2Val.TabIndex = 5;
            lblStu2Val.Text = "—";

            lblStu3Cap.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblStu3Cap.ForeColor = Color.FromArgb(92, 110, 102);
            lblStu3Cap.Location = new Point(208, 82);
            lblStu3Cap.Name = "lblStu3Cap";
            lblStu3Cap.Size = new Size(88, 16);
            lblStu3Cap.TabIndex = 6;
            lblStu3Cap.Text = "MÔN 3";

            lblStu3Val.Font = new Font("Palatino Linotype", 17F, FontStyle.Bold);
            lblStu3Val.ForeColor = Color.FromArgb(24, 76, 60);
            lblStu3Val.Location = new Point(208, 98);
            lblStu3Val.Name = "lblStu3Val";
            lblStu3Val.Size = new Size(88, 34);
            lblStu3Val.TabIndex = 7;
            lblStu3Val.Text = "—";

            pnlStuLine.BackColor = Color.FromArgb(206, 222, 212);
            pnlStuLine.Location = new Point(16, 142);
            pnlStuLine.Name = "pnlStuLine";
            pnlStuLine.Size = new Size(280, 1);
            pnlStuLine.TabIndex = 8;

            lblAvgCap.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblAvgCap.ForeColor = Color.FromArgb(92, 110, 102);
            lblAvgCap.Location = new Point(16, 152);
            lblAvgCap.Name = "lblAvgCap";
            lblAvgCap.Size = new Size(160, 16);
            lblAvgCap.TabIndex = 9;
            lblAvgCap.Text = "ĐIỂM TRUNG BÌNH";

            lblAvgValue.Font = new Font("Palatino Linotype", 28F, FontStyle.Bold);
            lblAvgValue.ForeColor = Color.FromArgb(24, 76, 60);
            lblAvgValue.Location = new Point(14, 170);
            lblAvgValue.Name = "lblAvgValue";
            lblAvgValue.Size = new Size(160, 50);
            lblAvgValue.TabIndex = 10;
            lblAvgValue.Text = "—";

            lblRank.BackColor = Color.FromArgb(28, 112, 80);
            lblRank.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblRank.ForeColor = Color.White;
            lblRank.Location = new Point(186, 178);
            lblRank.Name = "lblRank";
            lblRank.Size = new Size(110, 34);
            lblRank.TabIndex = 11;
            lblRank.Text = "Giỏi";
            lblRank.TextAlign = ContentAlignment.MiddleCenter;
            lblRank.Visible = false;

            btnBack.Location = new Point(20, 394);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(88, 40);
            btnBack.TabIndex = 5;
            btnBack.Text = "‹  TRƯỚC";
            btnBack.Click += btnBack_Click;

            lblPage.Font = new Font("Palatino Linotype", 12F, FontStyle.Bold);
            lblPage.ForeColor = Color.FromArgb(24, 38, 34);
            lblPage.Location = new Point(112, 394);
            lblPage.Name = "lblPage";
            lblPage.Size = new Size(128, 40);
            lblPage.TabIndex = 6;
            lblPage.Text = "Trang 0 / 0";
            lblPage.TextAlign = ContentAlignment.MiddleCenter;

            btnNext.Location = new Point(244, 394);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(88, 40);
            btnNext.TabIndex = 7;
            btnNext.Text = "SAU  ›";
            btnNext.Click += btnNext_Click;

            pnlStats.BackColor = Color.FromArgb(240, 247, 243);
            pnlStats.Controls.Add(lblMinValue);
            pnlStats.Controls.Add(lblMinCap);
            pnlStats.Controls.Add(lblMaxValue);
            pnlStats.Controls.Add(lblMaxCap);
            pnlStats.Controls.Add(lblMeanValue);
            pnlStats.Controls.Add(lblMeanCap);
            pnlStats.Controls.Add(lblCountValue);
            pnlStats.Controls.Add(lblCountCap);
            pnlStats.Location = new Point(20, 444);
            pnlStats.Name = "pnlStats";
            pnlStats.Size = new Size(312, 68);
            pnlStats.TabIndex = 8;

            lblCountCap.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblCountCap.ForeColor = Color.FromArgb(92, 110, 102);
            lblCountCap.Location = new Point(10, 10);
            lblCountCap.Name = "lblCountCap";
            lblCountCap.Size = new Size(68, 16);
            lblCountCap.TabIndex = 0;
            lblCountCap.Text = "SĨ SỐ";

            lblCountValue.AutoEllipsis = true;
            lblCountValue.Font = new Font("Palatino Linotype", 13F, FontStyle.Bold);
            lblCountValue.ForeColor = Color.FromArgb(24, 38, 34);
            lblCountValue.Location = new Point(10, 30);
            lblCountValue.Name = "lblCountValue";
            lblCountValue.Size = new Size(68, 28);
            lblCountValue.TabIndex = 1;
            lblCountValue.Text = "—";

            lblMeanCap.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblMeanCap.ForeColor = Color.FromArgb(92, 110, 102);
            lblMeanCap.Location = new Point(88, 10);
            lblMeanCap.Name = "lblMeanCap";
            lblMeanCap.Size = new Size(68, 16);
            lblMeanCap.TabIndex = 2;
            lblMeanCap.Text = "ĐTB LỚP";

            lblMeanValue.AutoEllipsis = true;
            lblMeanValue.Font = new Font("Palatino Linotype", 13F, FontStyle.Bold);
            lblMeanValue.ForeColor = Color.FromArgb(24, 38, 34);
            lblMeanValue.Location = new Point(88, 30);
            lblMeanValue.Name = "lblMeanValue";
            lblMeanValue.Size = new Size(68, 28);
            lblMeanValue.TabIndex = 3;
            lblMeanValue.Text = "—";

            lblMaxCap.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblMaxCap.ForeColor = Color.FromArgb(92, 110, 102);
            lblMaxCap.Location = new Point(166, 10);
            lblMaxCap.Name = "lblMaxCap";
            lblMaxCap.Size = new Size(68, 16);
            lblMaxCap.TabIndex = 4;
            lblMaxCap.Text = "CAO NHẤT";

            lblMaxValue.AutoEllipsis = true;
            lblMaxValue.Font = new Font("Palatino Linotype", 13F, FontStyle.Bold);
            lblMaxValue.ForeColor = Color.FromArgb(28, 112, 80);
            lblMaxValue.Location = new Point(166, 30);
            lblMaxValue.Name = "lblMaxValue";
            lblMaxValue.Size = new Size(68, 28);
            lblMaxValue.TabIndex = 5;
            lblMaxValue.Text = "—";

            lblMinCap.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblMinCap.ForeColor = Color.FromArgb(92, 110, 102);
            lblMinCap.Location = new Point(240, 10);
            lblMinCap.Name = "lblMinCap";
            lblMinCap.Size = new Size(66, 16);
            lblMinCap.TabIndex = 6;
            lblMinCap.Text = "THẤP NHẤT";

            lblMinValue.AutoEllipsis = true;
            lblMinValue.Font = new Font("Palatino Linotype", 13F, FontStyle.Bold);
            lblMinValue.ForeColor = Color.FromArgb(176, 52, 58);
            lblMinValue.Location = new Point(240, 30);
            lblMinValue.Name = "lblMinValue";
            lblMinValue.Size = new Size(66, 28);
            lblMinValue.TabIndex = 7;
            lblMinValue.Text = "—";

            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            errorProvider.ContainerControl = this;

            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(236, 243, 238);
            ClientSize = new Size(1080, 640);
            Controls.Add(pnlRead);
            Controls.Add(pnlList);
            Controls.Add(pnlInput);
            Controls.Add(pnlStatus);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Lab02_Bai04";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Bài 04 - Quản lý sinh viên";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlStatus.ResumeLayout(false);
            pnlInput.ResumeLayout(false);
            pnlInput.PerformLayout();
            pnlPreview.ResumeLayout(false);
            pnlPreview.PerformLayout();
            pnlList.ResumeLayout(false);
            pnlList.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPending).EndInit();
            pnlRead.ResumeLayout(false);
            pnlRead.PerformLayout();
            pnlStudent.ResumeLayout(false);
            pnlStats.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlHeader;
        private Panel pnlMint;
        private Label lblTitle;
        private Label lblSubtitle;
        private Panel pnlStatus;
        private Label lblStatus;
        private Button btnRefresh;
        private Button btnExit;
        private Panel pnlInput;
        private Label lblInputTitle;
        private Label lblNameCap;
        private TextBox txtName;
        private Label lblIdCap;
        private TextBox txtId;
        private Label lblPhoneCap;
        private TextBox txtPhone;
        private Label lblScore1Cap;
        private TextBox txtScore1;
        private Label lblScore2Cap;
        private TextBox txtScore2;
        private Label lblScore3Cap;
        private TextBox txtScore3;
        private Panel pnlPreview;
        private Label lblPreviewCap;
        private Label lblPreviewValue;
        private Label lblPreviewRank;
        private Button btnAdd;
        private Button btnResetForm;
        private Label lblHint;
        private Panel pnlList;
        private Label lblListTitle;
        private Label lblPendingCount;
        private DataGridView dgvPending;
        private DataGridViewTextBoxColumn colId;
        private DataGridViewTextBoxColumn colName;
        private DataGridViewTextBoxColumn colScores;
        private Button btnDelete;
        private Button btnWrite;
        private Panel pnlRead;
        private Label lblReadTitle;
        private Button btnRead;
        private TextBox txtSearch;
        private Button btnSearch;
        private Panel pnlStudent;
        private Label lblStuName;
        private Label lblStuMeta;
        private Label lblStu1Cap;
        private Label lblStu1Val;
        private Label lblStu2Cap;
        private Label lblStu2Val;
        private Label lblStu3Cap;
        private Label lblStu3Val;
        private Panel pnlStuLine;
        private Label lblAvgCap;
        private Label lblAvgValue;
        private Label lblRank;
        private Button btnBack;
        private Label lblPage;
        private Button btnNext;
        private Panel pnlStats;
        private Label lblCountCap;
        private Label lblCountValue;
        private Label lblMeanCap;
        private Label lblMeanValue;
        private Label lblMaxCap;
        private Label lblMaxValue;
        private Label lblMinCap;
        private Label lblMinValue;
        private ErrorProvider errorProvider;
        private ToolTip toolTip;
    }
}
