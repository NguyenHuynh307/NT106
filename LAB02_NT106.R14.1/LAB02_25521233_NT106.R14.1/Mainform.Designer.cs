namespace Lab02
{
    partial class Mainform
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
            lblEyebrow = new Label();
            flowMenu = new FlowLayoutPanel();
            btnExit = new Button();
            lblFooter = new Label();
            pnlHeader.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(13, 24, 46);
            pnlHeader.Controls.Add(pnlGold);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblEyebrow);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1000, 130);
            pnlHeader.TabIndex = 0;
            // 
            // pnlGold
            // 
            pnlGold.BackColor = Color.FromArgb(201, 162, 39);
            pnlGold.Dock = DockStyle.Bottom;
            pnlGold.Location = new Point(0, 127);
            pnlGold.Name = "pnlGold";
            pnlGold.Size = new Size(1000, 3);
            pnlGold.TabIndex = 3;
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.BackColor = Color.Transparent;
            lblSubtitle.Font = new Font("Segoe UI", 10.5F);
            lblSubtitle.ForeColor = Color.FromArgb(176, 190, 214);
            lblSubtitle.Location = new Point(42, 92);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(200, 19);
            lblSubtitle.TabIndex = 2;
            lblSubtitle.Text = "File và I/O Stream trong C#";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Palatino Linotype", 24F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(38, 42);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(420, 38);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "Bảng Điều Hướng Lab 02";
            // 
            // lblEyebrow
            // 
            lblEyebrow.AutoSize = true;
            lblEyebrow.BackColor = Color.Transparent;
            lblEyebrow.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblEyebrow.ForeColor = Color.FromArgb(201, 162, 39);
            lblEyebrow.Location = new Point(42, 22);
            lblEyebrow.Name = "lblEyebrow";
            lblEyebrow.Size = new Size(220, 15);
            lblEyebrow.TabIndex = 0;
            lblEyebrow.Text = "LẬP TRÌNH MẠNG CĂN BẢN";
            // 
            // flowMenu
            // 
            flowMenu.Location = new Point(40, 160);
            flowMenu.Name = "flowMenu";
            flowMenu.Size = new Size(920, 330);
            flowMenu.TabIndex = 1;
            // 
            // btnExit
            // 
            btnExit.Location = new Point(380, 530);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(240, 46);
            btnExit.TabIndex = 2;
            btnExit.Text = "THOÁT CHƯƠNG TRÌNH";
            btnExit.Click += btnExit_Click;
            // 
            // lblFooter
            // 
            lblFooter.AutoSize = true;
            lblFooter.Font = new Font("Segoe UI", 9F);
            lblFooter.ForeColor = Color.FromArgb(107, 114, 128);
            lblFooter.Location = new Point(42, 606);
            lblFooter.Name = "lblFooter";
            lblFooter.Size = new Size(200, 15);
            lblFooter.TabIndex = 3;
            lblFooter.Text = "NT106.R14.1   |   MSSV 25521233";
            // 
            // Mainform
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(247, 244, 236);
            ClientSize = new Size(1000, 640);
            Controls.Add(lblFooter);
            Controls.Add(btnExit);
            Controls.Add(flowMenu);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Mainform";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Lab 02 - Bảng điều hướng";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlHeader;
        private Panel pnlGold;
        private Label lblSubtitle;
        private Label lblTitle;
        private Label lblEyebrow;
        private FlowLayoutPanel flowMenu;
        private Button btnExit;
        private Label lblFooter;
    }
}
