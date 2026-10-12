#nullable disable

namespace Lab02
{
    partial class Lab02_Bai02
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
            pnlGold = new Panel();
            lblSubtitle = new Label();
            lblTitle = new Label();
            pnlFile = new Panel();
            lblFileCaption = new Label();
            lblFileName = new Label();
            lblPathCaption = new Label();
            lblPathValue = new Label();
            btnRead = new Button();
            btnClear = new Button();
            btnExit = new Button();
            pnlSizeTile = new Panel();
            pnlSizeBar = new Panel();
            lblSizeCaption = new Label();
            lblSizeValue = new Label();
            pnlLinesTile = new Panel();
            pnlLinesBar = new Panel();
            lblLinesCaption = new Label();
            lblLinesValue = new Label();
            pnlWordsTile = new Panel();
            pnlWordsBar = new Panel();
            lblWordsCaption = new Label();
            lblWordsValue = new Label();
            pnlCharsTile = new Panel();
            pnlCharsBar = new Panel();
            lblCharsCaption = new Label();
            lblCharsValue = new Label();
            pnlContent = new Panel();
            lblContentTitle = new Label();
            rtbContent = new RichTextBox();
            pnlStatus = new Panel();
            lblStatus = new Label();
            toolTip = new ToolTip(components);
            pnlHeader.SuspendLayout();
            pnlFile.SuspendLayout();
            pnlSizeTile.SuspendLayout();
            pnlLinesTile.SuspendLayout();
            pnlWordsTile.SuspendLayout();
            pnlCharsTile.SuspendLayout();
            pnlContent.SuspendLayout();
            pnlStatus.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(13, 24, 46);
            pnlHeader.Controls.Add(pnlGold);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(980, 100);
            pnlHeader.TabIndex = 0;
            // 
            // pnlGold
            // 
            pnlGold.BackColor = Color.FromArgb(201, 162, 39);
            pnlGold.Dock = DockStyle.Bottom;
            pnlGold.Location = new Point(0, 97);
            pnlGold.Name = "pnlGold";
            pnlGold.Size = new Size(980, 3);
            pnlGold.TabIndex = 2;
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.BackColor = Color.Transparent;
            lblSubtitle.Font = new Font("Segoe UI", 10F);
            lblSubtitle.ForeColor = Color.FromArgb(176, 190, 214);
            lblSubtitle.Location = new Point(26, 60);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(380, 19);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Tên file, kích thước, đường dẫn, số dòng, số từ, số ký tự và nội dung";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Palatino Linotype", 22F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(22, 16);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(380, 32);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Bài 02 - Thông Tin File Văn Bản";
            // 
            // pnlFile
            // 
            pnlFile.BackColor = Color.White;
            pnlFile.Controls.Add(btnExit);
            pnlFile.Controls.Add(btnClear);
            pnlFile.Controls.Add(btnRead);
            pnlFile.Controls.Add(lblPathValue);
            pnlFile.Controls.Add(lblPathCaption);
            pnlFile.Controls.Add(lblFileName);
            pnlFile.Controls.Add(lblFileCaption);
            pnlFile.Location = new Point(24, 120);
            pnlFile.Name = "pnlFile";
            pnlFile.Size = new Size(932, 100);
            pnlFile.TabIndex = 1;
            // 
            // lblFileCaption
            // 
            lblFileCaption.AutoSize = true;
            lblFileCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblFileCaption.ForeColor = Color.FromArgb(201, 162, 39);
            lblFileCaption.Location = new Point(20, 12);
            lblFileCaption.Name = "lblFileCaption";
            lblFileCaption.Size = new Size(60, 15);
            lblFileCaption.TabIndex = 0;
            lblFileCaption.Text = "TÊN FILE";
            // 
            // lblFileName
            // 
            lblFileName.AutoEllipsis = true;
            lblFileName.Font = new Font("Palatino Linotype", 18F, FontStyle.Bold);
            lblFileName.ForeColor = Color.FromArgb(22, 36, 66);
            lblFileName.Location = new Point(20, 30);
            lblFileName.Name = "lblFileName";
            lblFileName.Size = new Size(510, 32);
            lblFileName.TabIndex = 1;
            lblFileName.Text = "Chưa chọn file";
            // 
            // lblPathCaption
            // 
            lblPathCaption.AutoSize = true;
            lblPathCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPathCaption.ForeColor = Color.FromArgb(201, 162, 39);
            lblPathCaption.Location = new Point(20, 70);
            lblPathCaption.Name = "lblPathCaption";
            lblPathCaption.Size = new Size(80, 15);
            lblPathCaption.TabIndex = 2;
            lblPathCaption.Text = "ĐƯỜNG DẪN";
            // 
            // lblPathValue
            // 
            lblPathValue.AutoEllipsis = true;
            lblPathValue.Font = new Font("Segoe UI", 10F);
            lblPathValue.ForeColor = Color.FromArgb(75, 85, 99);
            lblPathValue.Location = new Point(110, 69);
            lblPathValue.Name = "lblPathValue";
            lblPathValue.Size = new Size(420, 20);
            lblPathValue.TabIndex = 3;
            lblPathValue.Text = "—";
            // 
            // btnRead
            // 
            btnRead.Location = new Point(548, 24);
            btnRead.Name = "btnRead";
            btnRead.Size = new Size(176, 52);
            btnRead.TabIndex = 4;
            btnRead.Text = "ĐỌC FILE";
            btnRead.Click += btnRead_Click;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(736, 24);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(96, 52);
            btnClear.TabIndex = 5;
            btnClear.Text = "LÀM MỚI";
            btnClear.Click += btnClear_Click;
            // 
            // btnExit
            // 
            btnExit.Location = new Point(844, 24);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(72, 52);
            btnExit.TabIndex = 6;
            btnExit.Text = "THOÁT";
            btnExit.Click += btnExit_Click;
            // 
            // pnlSizeTile
            // 
            pnlSizeTile.BackColor = Color.White;
            pnlSizeTile.Controls.Add(lblSizeValue);
            pnlSizeTile.Controls.Add(lblSizeCaption);
            pnlSizeTile.Controls.Add(pnlSizeBar);
            pnlSizeTile.Location = new Point(24, 236);
            pnlSizeTile.Name = "pnlSizeTile";
            pnlSizeTile.Size = new Size(221, 90);
            pnlSizeTile.TabIndex = 2;
            // 
            // pnlSizeBar
            // 
            pnlSizeBar.BackColor = Color.FromArgb(201, 162, 39);
            pnlSizeBar.Dock = DockStyle.Top;
            pnlSizeBar.Location = new Point(0, 0);
            pnlSizeBar.Name = "pnlSizeBar";
            pnlSizeBar.Size = new Size(221, 4);
            pnlSizeBar.TabIndex = 0;
            // 
            // lblSizeCaption
            // 
            lblSizeCaption.AutoSize = true;
            lblSizeCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSizeCaption.ForeColor = Color.FromArgb(75, 85, 99);
            lblSizeCaption.Location = new Point(16, 18);
            lblSizeCaption.Name = "lblSizeCaption";
            lblSizeCaption.Size = new Size(80, 15);
            lblSizeCaption.TabIndex = 1;
            lblSizeCaption.Text = "KÍCH THƯỚC";
            // 
            // lblSizeValue
            // 
            lblSizeValue.AutoEllipsis = true;
            lblSizeValue.Font = new Font("Palatino Linotype", 22F, FontStyle.Bold);
            lblSizeValue.ForeColor = Color.FromArgb(22, 36, 66);
            lblSizeValue.Location = new Point(14, 40);
            lblSizeValue.Name = "lblSizeValue";
            lblSizeValue.Size = new Size(195, 40);
            lblSizeValue.TabIndex = 2;
            lblSizeValue.Text = "—";
            // 
            // pnlLinesTile
            // 
            pnlLinesTile.BackColor = Color.White;
            pnlLinesTile.Controls.Add(lblLinesValue);
            pnlLinesTile.Controls.Add(lblLinesCaption);
            pnlLinesTile.Controls.Add(pnlLinesBar);
            pnlLinesTile.Location = new Point(261, 236);
            pnlLinesTile.Name = "pnlLinesTile";
            pnlLinesTile.Size = new Size(221, 90);
            pnlLinesTile.TabIndex = 3;
            // 
            // pnlLinesBar
            // 
            pnlLinesBar.BackColor = Color.FromArgb(201, 162, 39);
            pnlLinesBar.Dock = DockStyle.Top;
            pnlLinesBar.Location = new Point(0, 0);
            pnlLinesBar.Name = "pnlLinesBar";
            pnlLinesBar.Size = new Size(221, 4);
            pnlLinesBar.TabIndex = 0;
            // 
            // lblLinesCaption
            // 
            lblLinesCaption.AutoSize = true;
            lblLinesCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblLinesCaption.ForeColor = Color.FromArgb(75, 85, 99);
            lblLinesCaption.Location = new Point(16, 18);
            lblLinesCaption.Name = "lblLinesCaption";
            lblLinesCaption.Size = new Size(60, 15);
            lblLinesCaption.TabIndex = 1;
            lblLinesCaption.Text = "SỐ DÒNG";
            // 
            // lblLinesValue
            // 
            lblLinesValue.AutoEllipsis = true;
            lblLinesValue.Font = new Font("Palatino Linotype", 22F, FontStyle.Bold);
            lblLinesValue.ForeColor = Color.FromArgb(22, 36, 66);
            lblLinesValue.Location = new Point(14, 40);
            lblLinesValue.Name = "lblLinesValue";
            lblLinesValue.Size = new Size(195, 40);
            lblLinesValue.TabIndex = 2;
            lblLinesValue.Text = "—";
            // 
            // pnlWordsTile
            // 
            pnlWordsTile.BackColor = Color.White;
            pnlWordsTile.Controls.Add(lblWordsValue);
            pnlWordsTile.Controls.Add(lblWordsCaption);
            pnlWordsTile.Controls.Add(pnlWordsBar);
            pnlWordsTile.Location = new Point(498, 236);
            pnlWordsTile.Name = "pnlWordsTile";
            pnlWordsTile.Size = new Size(221, 90);
            pnlWordsTile.TabIndex = 4;
            // 
            // pnlWordsBar
            // 
            pnlWordsBar.BackColor = Color.FromArgb(201, 162, 39);
            pnlWordsBar.Dock = DockStyle.Top;
            pnlWordsBar.Location = new Point(0, 0);
            pnlWordsBar.Name = "pnlWordsBar";
            pnlWordsBar.Size = new Size(221, 4);
            pnlWordsBar.TabIndex = 0;
            // 
            // lblWordsCaption
            // 
            lblWordsCaption.AutoSize = true;
            lblWordsCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblWordsCaption.ForeColor = Color.FromArgb(75, 85, 99);
            lblWordsCaption.Location = new Point(16, 18);
            lblWordsCaption.Name = "lblWordsCaption";
            lblWordsCaption.Size = new Size(50, 15);
            lblWordsCaption.TabIndex = 1;
            lblWordsCaption.Text = "SỐ TỪ";
            // 
            // lblWordsValue
            // 
            lblWordsValue.AutoEllipsis = true;
            lblWordsValue.Font = new Font("Palatino Linotype", 22F, FontStyle.Bold);
            lblWordsValue.ForeColor = Color.FromArgb(22, 36, 66);
            lblWordsValue.Location = new Point(14, 40);
            lblWordsValue.Name = "lblWordsValue";
            lblWordsValue.Size = new Size(195, 40);
            lblWordsValue.TabIndex = 2;
            lblWordsValue.Text = "—";
            // 
            // pnlCharsTile
            // 
            pnlCharsTile.BackColor = Color.White;
            pnlCharsTile.Controls.Add(lblCharsValue);
            pnlCharsTile.Controls.Add(lblCharsCaption);
            pnlCharsTile.Controls.Add(pnlCharsBar);
            pnlCharsTile.Location = new Point(735, 236);
            pnlCharsTile.Name = "pnlCharsTile";
            pnlCharsTile.Size = new Size(221, 90);
            pnlCharsTile.TabIndex = 5;
            // 
            // pnlCharsBar
            // 
            pnlCharsBar.BackColor = Color.FromArgb(201, 162, 39);
            pnlCharsBar.Dock = DockStyle.Top;
            pnlCharsBar.Location = new Point(0, 0);
            pnlCharsBar.Name = "pnlCharsBar";
            pnlCharsBar.Size = new Size(221, 4);
            pnlCharsBar.TabIndex = 0;
            // 
            // lblCharsCaption
            // 
            lblCharsCaption.AutoSize = true;
            lblCharsCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblCharsCaption.ForeColor = Color.FromArgb(75, 85, 99);
            lblCharsCaption.Location = new Point(16, 18);
            lblCharsCaption.Name = "lblCharsCaption";
            lblCharsCaption.Size = new Size(70, 15);
            lblCharsCaption.TabIndex = 1;
            lblCharsCaption.Text = "SỐ KÝ TỰ";
            // 
            // lblCharsValue
            // 
            lblCharsValue.AutoEllipsis = true;
            lblCharsValue.Font = new Font("Palatino Linotype", 22F, FontStyle.Bold);
            lblCharsValue.ForeColor = Color.FromArgb(22, 36, 66);
            lblCharsValue.Location = new Point(14, 40);
            lblCharsValue.Name = "lblCharsValue";
            lblCharsValue.Size = new Size(195, 40);
            lblCharsValue.TabIndex = 2;
            lblCharsValue.Text = "—";
            // 
            // pnlContent
            // 
            pnlContent.BackColor = Color.White;
            pnlContent.Controls.Add(rtbContent);
            pnlContent.Controls.Add(lblContentTitle);
            pnlContent.Location = new Point(24, 342);
            pnlContent.Name = "pnlContent";
            pnlContent.Size = new Size(932, 266);
            pnlContent.TabIndex = 6;
            // 
            // lblContentTitle
            // 
            lblContentTitle.AutoSize = true;
            lblContentTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblContentTitle.ForeColor = Color.FromArgb(201, 162, 39);
            lblContentTitle.Location = new Point(20, 14);
            lblContentTitle.Name = "lblContentTitle";
            lblContentTitle.Size = new Size(110, 15);
            lblContentTitle.TabIndex = 0;
            lblContentTitle.Text = "NỘI DUNG FILE";
            // 
            // rtbContent
            // 
            rtbContent.BackColor = Color.White;
            rtbContent.BorderStyle = BorderStyle.None;
            rtbContent.Font = new Font("Segoe UI", 11F);
            rtbContent.ForeColor = Color.FromArgb(31, 41, 55);
            rtbContent.Location = new Point(20, 42);
            rtbContent.Name = "rtbContent";
            rtbContent.ReadOnly = true;
            rtbContent.Size = new Size(892, 208);
            rtbContent.TabIndex = 1;
            rtbContent.Text = "";
            // 
            // pnlStatus
            // 
            pnlStatus.BackColor = Color.FromArgb(13, 24, 46);
            pnlStatus.Controls.Add(lblStatus);
            pnlStatus.Dock = DockStyle.Bottom;
            pnlStatus.Location = new Point(0, 624);
            pnlStatus.Name = "pnlStatus";
            pnlStatus.Size = new Size(980, 36);
            pnlStatus.TabIndex = 7;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI", 9F);
            lblStatus.ForeColor = Color.FromArgb(200, 210, 230);
            lblStatus.Location = new Point(24, 9);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(100, 15);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "Sẵn sàng. Bấm ĐỌC FILE hoặc kéo thả một file .txt vào cửa sổ.";
            // 
            // Lab02_Bai02
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(247, 244, 236);
            ClientSize = new Size(980, 660);
            Controls.Add(pnlStatus);
            Controls.Add(pnlContent);
            Controls.Add(pnlCharsTile);
            Controls.Add(pnlWordsTile);
            Controls.Add(pnlLinesTile);
            Controls.Add(pnlSizeTile);
            Controls.Add(pnlFile);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Lab02_Bai02";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Bài 02 - Đọc thông tin file .txt";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlFile.ResumeLayout(false);
            pnlFile.PerformLayout();
            pnlSizeTile.ResumeLayout(false);
            pnlSizeTile.PerformLayout();
            pnlLinesTile.ResumeLayout(false);
            pnlLinesTile.PerformLayout();
            pnlWordsTile.ResumeLayout(false);
            pnlWordsTile.PerformLayout();
            pnlCharsTile.ResumeLayout(false);
            pnlCharsTile.PerformLayout();
            pnlContent.ResumeLayout(false);
            pnlContent.PerformLayout();
            pnlStatus.ResumeLayout(false);
            pnlStatus.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlHeader;
        private Panel pnlGold;
        private Label lblSubtitle;
        private Label lblTitle;
        private Panel pnlFile;
        private Label lblFileCaption;
        private Label lblFileName;
        private Label lblPathCaption;
        private Label lblPathValue;
        private Button btnRead;
        private Button btnClear;
        private Button btnExit;
        private Panel pnlSizeTile;
        private Panel pnlSizeBar;
        private Label lblSizeCaption;
        private Label lblSizeValue;
        private Panel pnlLinesTile;
        private Panel pnlLinesBar;
        private Label lblLinesCaption;
        private Label lblLinesValue;
        private Panel pnlWordsTile;
        private Panel pnlWordsBar;
        private Label lblWordsCaption;
        private Label lblWordsValue;
        private Panel pnlCharsTile;
        private Panel pnlCharsBar;
        private Label lblCharsCaption;
        private Label lblCharsValue;
        private Panel pnlContent;
        private Label lblContentTitle;
        private RichTextBox rtbContent;
        private Panel pnlStatus;
        private Label lblStatus;
        private ToolTip toolTip;
    }
}
