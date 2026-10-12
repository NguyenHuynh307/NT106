namespace Lab02
{
    partial class Lab02_Bai06
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
            SuspendLayout();
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(255, 246, 232);
            ClientSize = new Size(1360, 860);
            MinimumSize = new Size(1100, 640);
            WindowState = FormWindowState.Maximized;
            Name = "Lab02_Bai06";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Bài 06 - Hôm nay ăn gì?";
            Load += Lab02_Bai06_Load;
            ResumeLayout(false);
        }

        #endregion
    }
}
