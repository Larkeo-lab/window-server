using Avalonia.Controls;
using System.Linq;

namespace My_program.Views
{
    public partial class Navbar : UserControl
    {
        private MainForm? _mainForm;

        public Navbar()
        {
            InitializeComponent();
        }

        public Navbar(string status, MainForm mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
            SetVisibilityByStatus(status);
            SetupButtons();
        }

        private void SetVisibilityByStatus(string status)
        {
            // ถ้า status ไม่ใช่ "Admin" ให้ซ่อนปุ่มเหล่านี้
            bool isAdmin = status.Equals("Admin", System.StringComparison.OrdinalIgnoreCase);

            var btnData = this.FindControl<Button>("toolStripMenuItemData");
            var btnOrder = this.FindControl<Button>("toolStripMenuItemOrder");
            var btnReport = this.FindControl<Button>("toolStripMenuItemReport");

            if (btnData != null)
                btnData.IsVisible = isAdmin;

            if (btnOrder != null)
                btnOrder.IsVisible = isAdmin;

            if (btnReport != null)
                btnReport.IsVisible = isAdmin;
        }

        private void SetupButtons()
        {
            // Setup Home button
            var btnHome = this.FindControl<Button>("toolStripMenuItemHome");
            if (btnHome != null)
            {
                btnHome.Click += BtnHome_Click;
            }

            // Setup Logout button
            var btnLogout = this.FindControl<Button>("toolStripMenuItemLogout");
            if (btnLogout != null)
            {
                btnLogout.Click += BtnLogout_Click;
            }

            // Setup Profile button
            var btnProfile = this.FindControl<Button>("toolStripMenuItemProfile");
            if (btnProfile != null)
            {
                btnProfile.Click += BtnProfile_Click;
            }

            // Setup Brand Management menu item
            var menuBrandManagement = this.FindControl<MenuItem>("menuBrandManagement");
            if (menuBrandManagement != null)
            {
                menuBrandManagement.Click += MenuBrandManagement_Click;
            }
        }

        private void BtnHome_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // เรียก ShowPage ของ MainForm
            _mainForm?.ShowPage(new Home());
        }

        private void BtnProfile_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // เรียก ShowPage ของ MainForm พร้อมส่งข้อมูล id, name, status
            if (_mainForm != null)
            {
                var profilesPage = new Profiles(_mainForm.emp_id);
                _mainForm.ShowPage(profilesPage);
            }
        }

        private void MenuBrandManagement_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // เรียก ShowPage ของ MainForm พร้อมแสดงหน้า branMagement
            _mainForm?.ShowPage(new branMagement());
        }

        private async void BtnLogout_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // หา parent window
            var parentWindow = this.VisualRoot as Window;
            if (parentWindow != null)
            {
                // แสดง confirmation dialog
                bool confirmed = await Helpers.ShowConfirmDialog.Show(parentWindow, "ທ່ານຕ້ອງການອອກຈາກລະບົບບໍ່?");
                
                if (confirmed)
                {
                    // สร้าง login window ใหม่
                    var loginWindow = new Login();
                    loginWindow.Show();
                    
                    // ปิด window ปัจจุบัน (MainForm)
                    parentWindow.Close();
                }
            }
        }
    }
}
