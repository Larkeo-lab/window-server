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
            buttonCancel.Click -= ButtonCancel_Click;
            buttonCancel.Click += ButtonCancel_Click;
            
            // ผูก event handler สำหรับ DataGrid SelectionChanged
            dgBrands.SelectionChanged -= DgBrands_SelectionChanged;
            dgBrands.SelectionChanged += DgBrands_SelectionChanged;
        }
        
        private void ButtonCancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // ล้างข้อมูลใน TextBox
            txtBrandName.Clear();
            
            // ยกเลิกการเลือกแถวในตาราง
            dgBrands.SelectedItem = null;
            
            Console.WriteLine("🔄 ยกเลิกการเลือก - พร้อมเพิ่มข้อมูลใหม่");
        }
        
        private void DgBrands_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            // ตรวจสอบว่ามีการเลือกแถวหรือไม่
            if (dgBrands.SelectedItem is BrandModel selectedBrand)
            {
                // นำ BrandName ไปแสดงใน TextBox
                txtBrandName.Text = selectedBrand.BrandName;
                Console.WriteLine($"📌 เลือกยี่ห้อ: {selectedBrand.BrandName} (ID: {selectedBrand.Id})");
            }
        }
        
        private async void ButtonAdd_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // หา parent window ก่อนเพื่อใช้ในทั้ง method
            var parentWindow = TopLevel.GetTopLevel(this) as Window;
            
            // ตรวจสอบว่ามีการเลือกแถวในตารางหรือไม่ (ถ้าเลือกแล้วต้องใช้ปุ่มแก้ไข ไม่ใช่เพิ่ม)
            if (dgBrands.SelectedItem != null)
            {
                if (parentWindow != null)
                {
                    await ShowErrorDialogHelper.ShowErrorDialog(parentWindow, "ເມື່ອທ່ານເລືອກຂໍ້ມູນໃນຕາຕະລາງ ຈະບໍ່ສາມາດກົດປຸ່ມເພີ່ມໄດ້\n" +
                    "ກະລຸນາເລືອກກົດປຸ່ມ ແກ້ໄຂ, ລືບ ຫຼື ຍົກເລີກ ເທົ່ານັ້ນ");
                }
                return;
            }
            
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
            
            // ตรวจสอบว่ามีการเลือกแถวในตารางหรือไม่
            if (dgBrands.SelectedItem == null)
            {
                if (parentWindow != null)
                {
                    await ShowErrorDialogHelper.ShowErrorDialog(parentWindow, "ກະລຸນາເລືອກຂໍ້ມູນໃນຕາຕະລາງກ່ອນ");
                }
                return;
            }
            
            // ตรวจสอบว่ามีข้อมูลใน TextBox หรือไม่
            if (string.IsNullOrWhiteSpace(txtBrandName.Text))
            {
                if (parentWindow != null)
                {
                    await ShowErrorDialogHelper.ShowErrorDialog(parentWindow, "ກະລຸນາເລື່ອກຂໍ້ມູນໃນຕາລາງກ່ອນ");
                }
                return;
            }
            
            try
            {
                // ดึงข้อมูลที่เลือก
                var selectedBrand = dgBrands.SelectedItem as BrandModel;
                if (selectedBrand == null) return;
                
                // อัพเดทข้อมูลในฐานข้อมูล
                var con = new Connection_db();
                await con.connectdb.OpenAsync();
                string sql = "UPDATE brand SET brand_name = @brand_name WHERE brand_id = @brand_id";
                MySqlCommand cmd = new MySqlCommand(sql, con.connectdb);
                cmd.Parameters.AddWithValue("@brand_name", txtBrandName.Text);
                cmd.Parameters.AddWithValue("@brand_id", selectedBrand.Id);
                
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                con.connectdb.Close();
                
                if (rowsAffected > 0)
                {
                    if (parentWindow != null)
                    {
                        await ShowSuccessDialogHelper.ShowSuccessDialog(parentWindow, "ແກ້ໄຂຂໍ້ມູນສຳເລັດ");
                    }
                    
                    // โหลดข้อมูลใหม่เพื่ออัพเดท DataGrid
                    LoadDataFromDatabase();
                    
                    Console.WriteLine($"✅ Updated brand ID {selectedBrand.Id}: {txtBrandName.Text}");
                }
                else
                {
                    if (parentWindow != null)
                    {
                        await ShowErrorDialogHelper.ShowErrorDialog(parentWindow, "ບໍ່ສາມາດແກ້ໄຂຂໍ້ມູນໄດ້");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error updating brand: {ex.Message}");
                if (parentWindow != null)
                {
                    await ShowErrorDialogHelper.ShowErrorDialog(parentWindow, $"ເກີດຂໍ້ຜິດພາດ: {ex.Message}");
                }
            }
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
