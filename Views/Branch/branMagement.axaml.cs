using Avalonia.Controls;
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
        }

        private async void LoadDataFromDatabase()
        {
            Brands = new ObservableCollection<BrandModel>();
            
            var con = new Connection_db();

            try
            {
                // เปิดการเชื่อมต่อ
                await con.connectdb.OpenAsync();
                
                string sql = "SELECT brand_id, brand_name FROM brand ORDER BY brand_id DESC";
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading data: {ex.Message}");
                
                // ถ้าเกิด error ให้ใช้ข้อมูลตัวอย่าง
                Brands = new ObservableCollection<BrandModel>
                {
                    new BrandModel { Id = 1, Index = 1, BrandName = "ຊີຊະນຸ (ຂໍ້ມູນຕົວຢ່າງ)" },
                    new BrandModel { Id = 2, Index = 2, BrandName = "ແວບສີ (ຂໍ້ມູນຕົວຢ່າງ)" }
                };
                
                var dataGrid = this.Find<DataGrid>("dgBrands");
                if (dataGrid != null)
                {
                    dataGrid.ItemsSource = Brands;
                }
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
