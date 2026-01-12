using Avalonia.Controls;
using My_program.Views.helper;
using System;
using System.Threading.Tasks;
using System.Data; // เพิ่มบรรทัดนี้
using MySql.Data.MySqlClient;

namespace My_program.Views
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            this.Opened += async (s, e) => await TestDatabaseConnection();
            this.Opened += (s, e) => InitializeUI();
        }

        private void InitializeUI()
        {
            // ผูก event handler สำหรับปุ่ม Login
            var loginButton = this.FindControl<Button>("ButtonLogin");
            if (loginButton != null)
            {
                loginButton.Click += LoginButton_Click;
            }

            // ผูก event handler สำหรับ Checkbox แสดงรหัสผ่าน
            var showPasswordCheckBox = this.FindControl<CheckBox>("ShowPassword");
            if (showPasswordCheckBox != null)
            {
                showPasswordCheckBox.IsCheckedChanged += ShowPassword_IsCheckedChanged;
            }

            // ผูก event handler สำหรับลบข้อมูล
            var clearDataTextBlock = this.FindControl<TextBlock>("ClearDataTextBlock");
            if (clearDataTextBlock != null)
            {
                clearDataTextBlock.PointerPressed += ClearData_PointerPressed;
            }
        }

        private async void LoginButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var usernameTextBox = this.FindControl<TextBox>("TextUsername");
            var passwordTextBox = this.FindControl<TextBox>("textPassword");

            if (usernameTextBox == null || passwordTextBox == null) return;

            string username = usernameTextBox.Text?.Trim() ?? "";
            string password = passwordTextBox.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await ShowErrorDialogHelper.ShowErrorDialog(this, "ກະລຸນາໃສ່ບັນຊີ ແລະ ລະຫັດຜ່ານ");
                return;
            }

            try
            {
                var db = new Connection_db();
                // ใน Connection_db ต้องเปลี่ยนตัวแปร connectdb ให้เป็น MySqlConnection ด้วย
                using (var con = db.connectdb)
                {
                    if (con.State == ConnectionState.Closed)
                        await con.OpenAsync();

                    string sql = "SELECT emp_id, emp_name, status FROM employee WHERE username=@username AND password=@password";

                    using (var command = new MySqlCommand(sql, con)) // เปลี่ยนเป็น MySqlCommand
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", Encryptor.MD5Hash(password));

                        using (var rd = await command.ExecuteReaderAsync())
                        {
                            if (await rd.ReadAsync())
                            {
                                MainForm mf = new MainForm(rd.GetString(0), rd.GetString(1), rd.GetString(2));
                                mf.Show();
                                this.Close();
                            }
                            else
                            {
                                await ShowErrorDialogHelper.ShowErrorDialog(this, "ບັນຊີເຂົ້າໃຊ້ ແລະ ລະຫັດຜ່ານບໍ່ຖືກຕ້ອງ!");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await ShowErrorDialogHelper.ShowErrorDialog(this, "Error: " + ex.Message);
            }

            ClearInputFields();
        }

        private void ClearData_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            ClearInputFields();
            Console.WriteLine("ລ້າງຂໍ້ມູນແລ້ວ");
        }

        private void ClearInputFields()
        {
            var usernameTextBox = this.FindControl<TextBox>("TextUsername");
            var passwordTextBox = this.FindControl<TextBox>("textPassword");
            var showPasswordCheckBox = this.FindControl<CheckBox>("ShowPassword");

            if (usernameTextBox != null)
                usernameTextBox.Clear();

            if (passwordTextBox != null)
                passwordTextBox.Clear();

            if (showPasswordCheckBox != null)
                showPasswordCheckBox.IsChecked = false;
        }

        private void ShowPassword_IsCheckedChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var showPasswordCheckBox = sender as CheckBox;
            var passwordTextBox = this.FindControl<TextBox>("textPassword");

            if (passwordTextBox != null && showPasswordCheckBox != null)
            {
                if (showPasswordCheckBox.IsChecked == true)
                {
                    passwordTextBox.PasswordChar = '\0';  // ສະແດງລະຫັດຜ່ານ
                }
                else
                {
                    passwordTextBox.PasswordChar = '●';   // ເຊື່ອງລະຫັດຜ່ານ
                }
            }
        }

        private async Task TestDatabaseConnection()
        {
            try
            {
                var db = new Connection_db();
                await db.TestConnection();
                await ShowSuccessDialogHelper.ShowSuccessDialog(this, "ເຊື່ອມຕໍ່ຖານຂໍ້ມູນສຳເລັດ!");
            }
            catch (Exception ex)
            {
                await ShowErrorDialogHelper.ShowErrorDialog(this, ex.Message);
            }
        }
    }
}