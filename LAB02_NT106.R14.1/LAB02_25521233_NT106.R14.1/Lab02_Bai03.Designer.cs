#nullable disable

namespace Lab02
{
    partial class Lab02_Bai03
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
            pnlTop = new Panel();
            pnlLine = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            btnRead = new Button();
            btnCalc = new Button();
            btnClear = new Button();
            btnExit = new Button();
            pnlBody = new Panel();
            dgvResult = new DataGridView();
            colIndex = new DataGridViewTextBoxColumn();
            colExpression = new DataGridViewTextBoxColumn();
            colResult = new DataGridViewTextBoxColumn();
            lblEmpty = new Label();
            pnlStatus = new Panel();
            lblStatus = new Label();
            lblSummary = new Label();
            pnlTop.SuspendLayout();
            pnlBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvResult).BeginInit();
            pnlStatus.SuspendLayout();
            SuspendLayout();
            // 
            // pnlTop
            // 
            pnlTop.BackColor = Color.FromArgb(11, 15, 30);
            pnlTop.Controls.Add(pnlLine);
            pnlTop.Controls.Add(btnExit);
            pnlTop.Controls.Add(btnClear);
            pnlTop.Controls.Add(btnCalc);
            pnlTop.Controls.Add(btnRead);
            pnlTop.Controls.Add(lblSubtitle);
            pnlTop.Controls.Add(lblTitle);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Name = "pnlTop";
            pnlTop.Size = new Size(1040, 96);
            pnlTop.TabIndex = 0;
            // 
            // pnlLine
            // 
            pnlLine.BackColor = Color.FromArgb(143, 176, 255);
            pnlLine.Dock = DockStyle.Bottom;
            pnlLine.Location = new Point(0, 94);
            pnlLine.Name = "pnlLine";
            pnlLine.Size = new Size(1040, 2);
            pnlLine.TabIndex = 6;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Palatino Linotype", 21F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(240, 244, 255);
            lblTitle.Location = new Point(84, 12);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(380, 40);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Bài 03 - Tính Biểu Thức";
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.BackColor = Color.Transparent;
            lblSubtitle.Font = new Font("Segoe UI", 10F);
            lblSubtitle.ForeColor = Color.FromArgb(160, 174, 220);
            lblSubtitle.Location = new Point(88, 58);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(380, 19);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Đọc input3.txt, tính từng dòng, ghi output3.txt";
            // 
            // btnRead
            // 
            btnRead.Location = new Point(510, 26);
            btnRead.Name = "btnRead";
            btnRead.Size = new Size(128, 44);
            btnRead.TabIndex = 2;
            btnRead.Text = "ĐỌC FILE";
            btnRead.Click += btnRead_Click;
            // 
            // btnCalc
            // 
            btnCalc.Location = new Point(648, 26);
            btnCalc.Name = "btnCalc";
            btnCalc.Size = new Size(168, 44);
            btnCalc.TabIndex = 3;
            btnCalc.Text = "TÍNH VÀ GHI FILE";
            btnCalc.Click += btnCalc_Click;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(826, 26);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(96, 44);
            btnClear.TabIndex = 4;
            btnClear.Text = "LÀM MỚI";
            btnClear.Click += btnClear_Click;
            // 
            // btnExit
            // 
            btnExit.Location = new Point(932, 26);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(84, 44);
            btnExit.TabIndex = 5;
            btnExit.Text = "THOÁT";
            btnExit.Click += btnExit_Click;
            // 
            // pnlBody
            // 
            pnlBody.BackColor = Color.FromArgb(14, 19, 40);
            pnlBody.Controls.Add(dgvResult);
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Location = new Point(0, 96);
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(24);
            pnlBody.Size = new Size(1040, 544);
            pnlBody.TabIndex = 1;
            // 
            // dgvResult
            // 
            dgvResult.Columns.AddRange(new DataGridViewColumn[] { colIndex, colExpression, colResult });
            dgvResult.Controls.Add(lblEmpty);
            dgvResult.Dock = DockStyle.Fill;
            dgvResult.Location = new Point(24, 24);
            dgvResult.Name = "dgvResult";
            dgvResult.Size = new Size(992, 496);
            dgvResult.TabIndex = 0;
            // 
            // colIndex
            // 
            colIndex.HeaderText = "STT";
            colIndex.Name = "colIndex";
            colIndex.ReadOnly = true;
            colIndex.Resizable = DataGridViewTriState.False;
            colIndex.SortMode = DataGridViewColumnSortMode.NotSortable;
            colIndex.Width = 80;
            // 
            // colExpression
            // 
            colExpression.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colExpression.HeaderText = "BIỂU THỨC";
            colExpression.Name = "colExpression";
            colExpression.ReadOnly = true;
            colExpression.Resizable = DataGridViewTriState.False;
            colExpression.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // colResult
            // 
            colResult.HeaderText = "KẾT QUẢ";
            colResult.Name = "colResult";
            colResult.ReadOnly = true;
            colResult.Resizable = DataGridViewTriState.False;
            colResult.SortMode = DataGridViewColumnSortMode.NotSortable;
            colResult.Width = 300;
            // 
            // lblEmpty
            // 
            lblEmpty.BackColor = Color.FromArgb(20, 27, 52);
            lblEmpty.Dock = DockStyle.Fill;
            lblEmpty.Font = new Font("Palatino Linotype", 15F);
            lblEmpty.ForeColor = Color.FromArgb(140, 155, 205);
            lblEmpty.Location = new Point(0, 0);
            lblEmpty.Name = "lblEmpty";
            lblEmpty.Size = new Size(992, 496);
            lblEmpty.TabIndex = 0;
            lblEmpty.Text = "Chưa có dữ liệu\r\nBấm ĐỌC FILE để nạp input3.txt";
            lblEmpty.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlStatus
            // 
            pnlStatus.BackColor = Color.FromArgb(8, 11, 26);
            pnlStatus.Controls.Add(lblStatus);
            pnlStatus.Controls.Add(lblSummary);
            pnlStatus.Dock = DockStyle.Bottom;
            pnlStatus.Location = new Point(0, 640);
            pnlStatus.Name = "pnlStatus";
            pnlStatus.Size = new Size(1040, 40);
            pnlStatus.TabIndex = 2;
            // 
            // lblStatus
            // 
            lblStatus.AutoEllipsis = true;
            lblStatus.Dock = DockStyle.Fill;
            lblStatus.Font = new Font("Segoe UI", 9F);
            lblStatus.ForeColor = Color.FromArgb(140, 155, 205);
            lblStatus.Location = new Point(380, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Padding = new Padding(0, 0, 24, 0);
            lblStatus.Size = new Size(660, 40);
            lblStatus.TabIndex = 1;
            lblStatus.Text = "Bấm ĐỌC FILE để nạp input3.txt";
            lblStatus.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblSummary
            // 
            lblSummary.Dock = DockStyle.Left;
            lblSummary.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblSummary.ForeColor = Color.FromArgb(255, 214, 126);
            lblSummary.Location = new Point(0, 0);
            lblSummary.Name = "lblSummary";
            lblSummary.Padding = new Padding(24, 0, 0, 0);
            lblSummary.Size = new Size(380, 40);
            lblSummary.TabIndex = 0;
            lblSummary.Text = "Chưa có dữ liệu";
            lblSummary.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lab02_Bai03
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(14, 19, 40);
            ClientSize = new Size(1040, 680);
            Controls.Add(pnlBody);
            Controls.Add(pnlStatus);
            Controls.Add(pnlTop);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Lab02_Bai03";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Bài 03 - Đọc, tính và ghi file";
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            pnlBody.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvResult).EndInit();
            pnlStatus.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlTop;
        private Panel pnlLine;
        private Label lblTitle;
        private Label lblSubtitle;
        private Button btnRead;
        private Button btnCalc;
        private Button btnClear;
        private Button btnExit;
        private Panel pnlBody;
        private DataGridView dgvResult;
        private DataGridViewTextBoxColumn colIndex;
        private DataGridViewTextBoxColumn colExpression;
        private DataGridViewTextBoxColumn colResult;
        private Label lblEmpty;
        private Panel pnlStatus;
        private Label lblStatus;
        private Label lblSummary;
    }
}
