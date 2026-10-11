namespace Lab02
{
    partial class Lab02_Bai01
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
            pnlGold = new Panel();
            lblSubtitle = new Label();
            lblTitle = new Label();
            pnlActions = new Panel();
            lblActions = new Label();
            btnRead = new Button();
            btnWrite = new Button();
            btnClear = new Button();
            btnExit = new Button();
            pnlOriginal = new Panel();
            lblOriginal = new Label();
            rtbOriginal = new RichTextBox();
            pnlUpper = new Panel();
            lblUpper = new Label();
            rtbUpper = new RichTextBox();
            pnlStatus = new Panel();
            lblStatus = new Label();
            pnlHeader.SuspendLayout();
            pnlActions.SuspendLayout();
            pnlOriginal.SuspendLayout();
            pnlUpper.SuspendLayout();
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
            pnlHeader.Size = new Size(920, 100);
            pnlHeader.TabIndex = 0;
            // 
            // pnlGold
            // 
            pnlGold.BackColor = Color.FromArgb(201, 162, 39);
            pnlGold.Dock = DockStyle.Bottom;
            pnlGold.Location = new Point(0, 97);
            pnlGold.Name = "pnlGold";
            pnlGold.Size = new Size(920, 3);
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
            lblSubtitle.Text = "Đọc file input1.txt, ghi nội dung in hoa ra output1.txt";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Palatino Linotype", 20F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(22, 16);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(380, 32);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Bài 01 - Ghi và Đọc File";
            // 
            // pnlActions
            // 
            pnlActions.BackColor = Color.White;
            pnlActions.Controls.Add(btnExit);
            pnlActions.Controls.Add(btnClear);
            pnlActions.Controls.Add(btnWrite);
            pnlActions.Controls.Add(btnRead);
            pnlActions.Controls.Add(lblActions);
            pnlActions.Location = new Point(24, 124);
            pnlActions.Name = "pnlActions";
            pnlActions.Size = new Size(240, 436);
            pnlActions.TabIndex = 1;
            // 
            // lblActions
            // 
            lblActions.AutoSize = true;
            lblActions.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblActions.ForeColor = Color.FromArgb(201, 162, 39);
            lblActions.Location = new Point(20, 18);
            lblActions.Name = "lblActions";
            lblActions.Size = new Size(70, 15);
            lblActions.TabIndex = 0;
            lblActions.Text = "THAO TÁC";
            // 
            // btnRead
            // 
            btnRead.Location = new Point(20, 52);
            btnRead.Name = "btnRead";
            btnRead.Size = new Size(200, 52);
            btnRead.TabIndex = 1;
            btnRead.Text = "ĐỌC FILE";
            btnRead.Click += btnRead_Click;
            // 
            // btnWrite
            // 
            btnWrite.Location = new Point(20, 120);
            btnWrite.Name = "btnWrite";
            btnWrite.Size = new Size(200, 52);
            btnWrite.TabIndex = 2;
            btnWrite.Text = "GHI FILE (IN HOA)";
            btnWrite.Click += btnWrite_Click;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(20, 188);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(200, 52);
            btnClear.TabIndex = 3;
            btnClear.Text = "LÀM MỚI";
            btnClear.Click += btnClear_Click;
            // 
            // btnExit
            // 
            btnExit.Location = new Point(20, 364);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(200, 52);
            btnExit.TabIndex = 4;
            btnExit.Text = "THOÁT";
            btnExit.Click += btnExit_Click;
            // 
            // pnlOriginal
            // 
            pnlOriginal.BackColor = Color.White;
            pnlOriginal.Controls.Add(rtbOriginal);
            pnlOriginal.Controls.Add(lblOriginal);
            pnlOriginal.Location = new Point(284, 124);
            pnlOriginal.Name = "pnlOriginal";
            pnlOriginal.Size = new Size(612, 210);
            pnlOriginal.TabIndex = 2;
            // 
            // lblOriginal
            // 
            lblOriginal.AutoSize = true;
            lblOriginal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblOriginal.ForeColor = Color.FromArgb(201, 162, 39);
            lblOriginal.Location = new Point(20, 14);
            lblOriginal.Name = "lblOriginal";
            lblOriginal.Size = new Size(140, 15);
            lblOriginal.TabIndex = 0;
            lblOriginal.Text = "NỘI DUNG FILE GỐC";
            // 
            // rtbOriginal
            // 
            rtbOriginal.BackColor = Color.White;
            rtbOriginal.BorderStyle = BorderStyle.None;
            rtbOriginal.Font = new Font("Segoe UI", 10.5F);
            rtbOriginal.ForeColor = Color.FromArgb(31, 41, 55);
            rtbOriginal.Location = new Point(20, 42);
            rtbOriginal.Name = "rtbOriginal";
            rtbOriginal.ReadOnly = true;
            rtbOriginal.Size = new Size(572, 152);
            rtbOriginal.TabIndex = 1;
            rtbOriginal.Text = "";
            // 
            // pnlUpper
            // 
            pnlUpper.BackColor = Color.White;
            pnlUpper.Controls.Add(rtbUpper);
            pnlUpper.Controls.Add(lblUpper);
            pnlUpper.Location = new Point(284, 350);
            pnlUpper.Name = "pnlUpper";
            pnlUpper.Size = new Size(612, 210);
            pnlUpper.TabIndex = 3;
            // 
            // lblUpper
            // 
            lblUpper.AutoSize = true;
            lblUpper.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblUpper.ForeColor = Color.FromArgb(201, 162, 39);
            lblUpper.Location = new Point(20, 14);
            lblUpper.Name = "lblUpper";
            lblUpper.Size = new Size(240, 15);
            lblUpper.TabIndex = 0;
            lblUpper.Text = "NỘI DUNG SAU KHI CHUYỂN IN HOA";
            // 
            // rtbUpper
            // 
            rtbUpper.BackColor = Color.White;
            rtbUpper.BorderStyle = BorderStyle.None;
            rtbUpper.Font = new Font("Segoe UI", 10.5F);
            rtbUpper.ForeColor = Color.FromArgb(31, 41, 55);
            rtbUpper.Location = new Point(20, 42);
            rtbUpper.Name = "rtbUpper";
            rtbUpper.ReadOnly = true;
            rtbUpper.Size = new Size(572, 152);
            rtbUpper.TabIndex = 1;
            rtbUpper.Text = "";
            // 
            // pnlStatus
            // 
            pnlStatus.BackColor = Color.FromArgb(13, 24, 46);
            pnlStatus.Controls.Add(lblStatus);
            pnlStatus.Dock = DockStyle.Bottom;
            pnlStatus.Location = new Point(0, 584);
            pnlStatus.Name = "pnlStatus";
            pnlStatus.Size = new Size(920, 36);
            pnlStatus.TabIndex = 4;
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
            lblStatus.Text = "Chưa chọn file";
            // 
            // Lab02_Bai01
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(247, 244, 236);
            ClientSize = new Size(920, 620);
            Controls.Add(pnlStatus);
            Controls.Add(pnlUpper);
            Controls.Add(pnlOriginal);
            Controls.Add(pnlActions);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Lab02_Bai01";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Bài 01 - Ghi và đọc file";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlActions.ResumeLayout(false);
            pnlActions.PerformLayout();
            pnlOriginal.ResumeLayout(false);
            pnlOriginal.PerformLayout();
            pnlUpper.ResumeLayout(false);
            pnlUpper.PerformLayout();
            pnlStatus.ResumeLayout(false);
            pnlStatus.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlHeader;
        private Panel pnlGold;
        private Label lblSubtitle;
        private Label lblTitle;
        private Panel pnlActions;
        private Label lblActions;
        private Button btnRead;
        private Button btnWrite;
        private Button btnClear;
        private Button btnExit;
        private Panel pnlOriginal;
        private Label lblOriginal;
        private RichTextBox rtbOriginal;
        private Panel pnlUpper;
        private Label lblUpper;
        private RichTextBox rtbUpper;
        private Panel pnlStatus;
        private Label lblStatus;
    }
}
