using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.ObjectModel;
using My_program.Views.helper;
using MySql.Data.MySqlClient;

namespace My_program.Views
{
    public partial class branMagement : UserControl
    {
        public ObservableCollection<BrandModel>? Brands { get; set; }

        public branMagement()
        {
            InitializeComponent();
            LoadDataFromDatabase();
            
            // ผูก event handlers สำหรับปุ่มต่างๆ (ใช้ -= ก่อนเพื่อป้องกันการ register ซ้ำ)
            buttonAdd.Click -= ButtonAdd_Click;
            buttonAdd.Click += ButtonAdd_Click;
            buttonEdit.Click -= ButtonEdit_Click;
            buttonEdit.Click += ButtonEdit_Click;
        }
        
        private async void ButtonAdd_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // หา parent window ก่อนเพื่อใช้ในทั้ง method
            var parentWindow = TopLevel.GetTopLevel(this) as Window;
            
            // ตรวจสอบว่ามีข้อมูลใน TextBox หรือไม่
            if (string.IsNullOrWhiteSpace(txtBrandName.Text))
            {
                if (parentWindow != null)
                {
                    await ShowErrorDialogHelper.ShowErrorDialog(parentWindow, "ກະລຸນາປ້ອນຂໍ້ມູນ");
                }
                return;
            }

            try
            {
                // เพิ่มข้อมูลลงฐานข้อมูล
                var con = new Connection_db();
                await con.connectdb.OpenAsync();
                string sql = "INSERT INTO brand (brand_name) VALUES (@brand_name)";
                MySqlCommand cmd = new MySqlCommand(sql, con.connectdb);
                cmd.Parameters.AddWithValue("@brand_name", txtBrandName.Text);
                await cmd.ExecuteNonQueryAsync();
                con.connectdb.Close();
                
                if (parentWindow != null)
                {
                    await ShowSuccessDialogHelper.ShowSuccessDialog(parentWindow, "ເພີ່ມຂໍ້ມູນສຳເລັດ");
                }
                
                // โหลดข้อมูลใหม่เพื่ออัพเดท DataGrid
                LoadDataFromDatabase();
                
                Console.WriteLine($"✅ Added brand: {txtBrandName.Text}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error adding brand: {ex.Message}");
                if (parentWindow != null)
                {
                    await ShowErrorDialogHelper.ShowErrorDialog(parentWindow, $"ເກີດຂໍ້ຜິດພາດ: {ex.Message}");
                }
            }
        }
        
        private async void ButtonEdit_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // หา parent window ก่อนเพื่อใช้ในทั้ง method
            var parentWindow = TopLevel.GetTopLevel(this) as Window;
            
            // ตรวจสอบว่ามีข้อมูลใน TextBox หรือไม่
            if (string.IsNullOrWhiteSpace(txtBrandName.Text))
            {
                if (parentWindow != null)
                {
                    await ShowErrorDialogHelper.ShowErrorDialog(parentWindow, "ກະລຸນາປ້ອນຂໍ້ມູນ");
                }
                return;
            }
            
            // TODO: เพิ่มโค้ดสำหรับแก้ไขข้อมูลในฐานข้อมูล
            Console.WriteLine($"Editing brand: {txtBrandName.Text}");
        }

        private async void LoadDataFromDatabase()
        {
            Brands = new ObservableCollection<BrandModel>();
            
            var con = new Connection_db();

            try
            {
                // เปิดการเชื่อมต่อ
                await con.connectdb.OpenAsync();
                
                string sql = "SELECT brand_id, brand_name FROM brand ORDER BY brand_id ASC";
                MySqlCommand cmd = new MySqlCommand(sql, con.connectdb);
                
                using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                {
                    int index = 1;
                    while (await reader.ReadAsync())
                    {
                        Brands.Add(new BrandModel
                        {
                            Id = reader.GetInt32("brand_id"),
                            Index = index++,
                            BrandName = reader.GetString("brand_name")
                        });
                    }
                }
                
                // ปิดการเชื่อมต่อ
                con.connectdb.Close();
                
                Console.WriteLine($"✅ โหลดข้อมูล {Brands.Count} รายการจากฐานข้อมูล");

                // ผูกข้อมูลกับ DataGrid
                var dataGrid = this.Find<DataGrid>("dgBrands");
                if (dataGrid != null)
                {
                    dataGrid.ItemsSource = Brands;
                    Console.WriteLine($"✅ แสดงข้อมูลใน DataGrid แล้ว");
                }
                var txtBrandName = this.Find<TextBox>("txtBrandName");
                if (txtBrandName != null)
                {
                    txtBrandName.Text = string.Empty;
                    Console.WriteLine($"✅ ລ້າງຂໍ້ມູນໃນ TextBox ແລ້ວ");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading data: {ex.Message}");
            }
        }
    }

    // Model class สำหรับเก็บข้อมูลยี่ห้อ
    public class BrandModel
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public string BrandName { get; set; } = string.Empty;
    }
}
