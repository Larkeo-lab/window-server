using Avalonia.Controls;

namespace My_program.Views // แก้ตรงนี้ให้ตรงกับ Namespace ใน x:Class
{
    public partial class MainForm : Window // ชื่อคลาสต้องตรงกัน
    {
        public string emp_id;
        public string emp_name;
        public string emp_status;
        
        public MainForm(string id, string name, string status)
        {
            InitializeComponent();
            this.emp_id = id;
            this.emp_name = name;
            this.emp_status = status;

            // สร้าง Navbar พร้อมส่ง status และส่ง reference ของ MainForm
            var navbar = new Navbar(status, this);
            DockPanel.SetDock(navbar, Dock.Top);

            // เพิ่ม Navbar เข้าไปใน DockPanel ที่ตำแหน่งแรก
            var mainDockPanel = this.FindControl<DockPanel>("MainDockPanel");
            if (mainDockPanel != null)
            {
                mainDockPanel.Children.Insert(0, navbar);
            }

            // แสดงหน้า home เป็นค่าเริ่มต้น
            ShowPage(new Home());
        }

        // Method สำหรับแสดงหน้าต่างๆ ใน ContentControl
        public void ShowPage(UserControl page)
        {
            var contentArea = this.FindControl<ContentControl>("MainContentArea");
            if (contentArea != null)
            {
                contentArea.Content = page;
            }
        }
    }
}