using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab02
{
    public partial class Mainform : Form
    {
        private readonly (string Number, string Title, string Description, Func<Form> CreateForm)[] exercises =
        {
            ("01", "Ghi và đọc file", "input1.txt  →  output1.txt", () => new Lab02_Bai01()),
            ("02", "Thông tin file .txt", "Tên, dung lượng, dòng, từ, ký tự", () => new Lab02_Bai02()),
            ("03", "Tính biểu thức", "input3.txt  →  output3.txt", () => new Lab02_Bai03()),
            ("04", "Quản lý sinh viên", "input4.txt  →  output4.txt", () => new Lab02_Bai04()),
            ("05", "Quản lý phòng vé", "input5.txt  →  output5.txt", () => new Lab02_Bai05()),
            ("06", "Hôm nay ăn gì?", "SQLite  →  món ăn ngẫu nhiên", () => new Lab02_Bai06()),
            ("07", "Duyệt thư mục", "Ổ đĩa  →  xem nội dung tệp", () => new Lab02_Bai07()),
        };

        public Mainform()
        {
            InitializeComponent();
            pnlHeader.Paint += Theme.PaintHeaderBackground;
            Theme.StyleDanger(btnExit);
            BuildMenu();
        }

        private void BuildMenu()
        {
            foreach (var exercise in exercises)
            {
                ExerciseCard card = new ExerciseCard
                {
                    Number = exercise.Number,
                    Heading = exercise.Title,
                    Detail = exercise.Description,
                    Size = new Size(LogicalToDeviceUnits(210), LogicalToDeviceUnits(120)),
                    Margin = new Padding(LogicalToDeviceUnits(8))
                };

                Func<Form> createForm = exercise.CreateForm;
                card.Click += (sender, e) => createForm().Show();
                flowMenu.Controls.Add(card);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
