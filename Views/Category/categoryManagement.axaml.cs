using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using My_program.Views.helper;
using MySql.Data.MySqlClient;

namespace My_program.Views
{
    public partial class categoryManagement : UserControl
    {
        public ObservableCollection<CategoryModel>? Categories { get; set; }

        public categoryManagement()
        {
            InitializeComponent();
            LoadDataFromDatabase();
            
            // ผูก event handlers สำหรับปุ่มต่างๆ (ใช้ -= ก่อนเพื่อป้องกันการ register ซ้ำ)
            buttonAdd.Click -= ButtonAdd_Click;
            buttonAdd.Click += ButtonAdd_Click;
            buttonEdit.Click -= ButtonEdit_Click;
            buttonEdit.Click += ButtonEdit_Click;
            buttonDelete.Click -= ButtonDelete_Click;
            buttonDelete.Click += ButtonDelete_Click;
            buttonCancel.Click -= ButtonCancel_Click;
            buttonCancel.Click += ButtonCancel_Click;
            
            // ผูก event handler สำหรับ DataGrid SelectionChanged
            dgCategories.SelectionChanged -= DgCategories_SelectionChanged;
            dgCategories.SelectionChanged += DgCategories_SelectionChanged;
            
            // ผูก event handler สำหรับ txtSearch TextChanged
            txtSearch.TextChanged -= TxtSearch_TextChanged;
            txtSearch.TextChanged += TxtSearch_TextChanged;
        }
        
        private void ButtonCancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // ล้างข้อมูลใน TextBox
            txtCategoryName.Clear();
            
            // ยกเลิกการเลือกแถวในตาราง
            dgCategories.SelectedItem = null;
            
            Console.WriteLine("🔄 ยกเลิกการเลือก - พร้อมเพิ่มข้อมูลใหม่");
        }
        
        private void DgCategories_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            // ตรวจสอบว่ามีการเลือกแถวหรือไม่
            if (dgCategories.SelectedItem is CategoryModel selectedCategory)
            {
                // นำ CategoryName ไปแสดงใน TextBox
                txtCategoryName.Text = selectedCategory.CategoryName;
                Console.WriteLine($"📌 เลือกປະເພດ: {selectedCategory.CategoryName} (ID: {selectedCategory.Id})");
            }
        }
        
        private async void ButtonAdd_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // หา parent window ก่อนเพื่อใช้ในทั้ง method
            var parentWindow = TopLevel.GetTopLevel(this) as Window;
            
            // ตรวจสอบว่ามีการเลือกแถวในตารางหรือไม่ (ถ้าเลือกแล้วต้องใช้ปุ่มแก้ไข ไม่ใช่เพิ่ม)
            if (dgCategories.SelectedItem != null)
            {
                if (parentWindow != null)
                {
                    await ShowErrorDialogHelper.ShowErrorDialog(parentWindow, "ເມື່ອທ່ານເລືອກຂໍ້ມູນໃນຕາຕະລາງ ຈະບໍ່ສາມາດກົດປຸ່ມເພີ່ມໄດ້\n" +
                    "ກະລຸນາເລືອກກົດປຸ່ມ ແກ້ໄຂ, ລືບ ຫຼື ຍົກເລີກ ເທົ່ານັ້ນ");
                }
                return;
            }
            
            // ตรวจสอบว่ามีข้อมูลใน TextBox หรือไม่
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
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
                string sql = "INSERT INTO category (category_name) VALUES (@category_name)";
                MySqlCommand cmd = new MySqlCommand(sql, con.connectdb);
                cmd.Parameters.AddWithValue("@category_name", txtCategoryName.Text);
                await cmd.ExecuteNonQueryAsync();
                con.connectdb.Close();
                
                if (parentWindow != null)
                {
                    await ShowSuccessDialogHelper.ShowSuccessDialog(parentWindow, "ເພີ່ມຂໍ້ມູນສຳເລັດ");
                }
                
                // โหลดข้อมูลใหม่เพื่ออัพเดท DataGrid
                LoadDataFromDatabase();
                
                Console.WriteLine($"✅ Added category: {txtCategoryName.Text}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error adding category: {ex.Message}");
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
            if (dgCategories.SelectedItem == null)
            {
                if (parentWindow != null)
                {
                    await ShowErrorDialogHelper.ShowErrorDialog(parentWindow, "ກະລຸນາເລືອກຂໍ້ມູນໃນຕາຕະລາງກ່ອນ");
                }
                return;
            }
            
            // ตรวจสอบว่ามีข้อมูลใน TextBox หรือไม่
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                if (parentWindow != null)
                {
                    await ShowErrorDialogHelper.ShowErrorDialog(parentWindow, "ກະລຸນາປ້ອນຂໍ້ມູນ");
                }
                return;
            }
            
            try
            {
                // ดึงข้อมูลที่เลือก
                var selectedCategory = dgCategories.SelectedItem as CategoryModel;
                if (selectedCategory == null) return;
                
                // อัพเดทข้อมูลในฐานข้อมูล
                var con = new Connection_db();
                await con.connectdb.OpenAsync();
                string sql = "UPDATE category SET category_name = @category_name WHERE category_id = @category_id";
                MySqlCommand cmd = new MySqlCommand(sql, con.connectdb);
                cmd.Parameters.AddWithValue("@category_name", txtCategoryName.Text);
                cmd.Parameters.AddWithValue("@category_id", selectedCategory.Id);
                
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
                    
                    Console.WriteLine($"✅ Updated category ID {selectedCategory.Id}: {txtCategoryName.Text}");
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
                Console.WriteLine($"❌ Error updating category: {ex.Message}");
                if (parentWindow != null)
                {
                    await ShowErrorDialogHelper.ShowErrorDialog(parentWindow, $"ເກີດຂໍ້ຜິດພາດ: {ex.Message}");
                }
            }
        }

        private async void ButtonDelete_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // หา parent window ก่อนเพื่อใช้ในทั้ง method
            var parentWindow = TopLevel.GetTopLevel(this) as Window;
            
            // ตรวจสอบว่ามีการเลือกแถวในตารางหรือไม่
            if (dgCategories.SelectedItem == null)
            {
                if (parentWindow != null)
                {
                    await ShowErrorDialogHelper.ShowErrorDialog(parentWindow, "ກະລຸນາເລືອກຂໍ້ມູນໃນຕາຕະລາງກ່ອນ");
                }
                return;
            }
            
            // ดึงข้อมูลที่เลือก
            var selectedCategory = dgCategories.SelectedItem as CategoryModel;
            if (selectedCategory == null) return;
            
            // แสดงหน้าต่างยืนยัน
            if (parentWindow != null)
            {
                var result = await ShowConfirmationDialogHelper.ShowConfirmationDialog(parentWindow, $"ຕ້ອງການລືບປະເພດ '{selectedCategory.CategoryName}' ແທ້ບໍ່?");
                if (!result)
                {
                    return;
                }
            }
            
            try
            {
                // ลบข้อมูลจากฐานข้อมูล
                var con = new Connection_db();
                await con.connectdb.OpenAsync();
                string sql = "DELETE FROM category WHERE category_id = @category_id";
                MySqlCommand cmd = new MySqlCommand(sql, con.connectdb);
                cmd.Parameters.AddWithValue("@category_id", selectedCategory.Id);
                
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                con.connectdb.Close();
                
                if (rowsAffected > 0)
                {
                    if (parentWindow != null)
                    {
                        await ShowSuccessDialogHelper.ShowSuccessDialog(parentWindow, "ລືບຂໍ້ມູນສຳເລັດ");
                    }
                    
                    // โหลดข้อมูลใหม่เพื่ออัພเดท DataGrid
                    LoadDataFromDatabase();
                    
                    Console.WriteLine($"✅ Deleted category ID {selectedCategory.Id}: {selectedCategory.CategoryName}");
                }
                else
                {
                    if (parentWindow != null)
                    {
                        await ShowErrorDialogHelper.ShowErrorDialog(parentWindow, "ບໍ່ສາມາດລືບຂໍ້ມູນໄດ້");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error deleting category: {ex.Message}");
                if (parentWindow != null)
                {
                    await ShowErrorDialogHelper.ShowErrorDialog(parentWindow, $"ເກີດຂໍ້ຜິດພາດ: {ex.Message}");
                }
            }
        }

        private async void TxtSearch_TextChanged(object? sender, TextChangedEventArgs e)
        {
            // ดึงค่าจาก txtSearch
            string searchText = txtSearch.Text?.Trim() ?? "";
            
            // ถ้าค่าว่าง ให้โหลดข้อมูลทั้งหมด
            if (string.IsNullOrWhiteSpace(searchText))
            {
                LoadDataFromDatabase();
                return;
            }
            
            // ค้นหาข้อมูลจากฐานข้อมูล
            await SearchCategoriesFromDatabase(searchText);
        }
        
        private async Task SearchCategoriesFromDatabase(string searchText)
        {
            Categories = new ObservableCollection<CategoryModel>();
            
            var con = new Connection_db();

            try
            {
                // เปิดการเชื่อมต่อ
                await con.connectdb.OpenAsync();
                
                // ใช้ LIKE สำหรับค้นหา (ตัวหน้า ตัวหลัง หรือตรงกลางก็ได้) และไม่สนใจ case
                string sql = "SELECT category_id, category_name FROM category WHERE LOWER(category_name) LIKE LOWER(@searchText) ORDER BY category_id ASC";
                MySqlCommand cmd = new MySqlCommand(sql, con.connectdb);
                cmd.Parameters.AddWithValue("@searchText", $"%{searchText}%");
                
                using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                {
                    int index = 1;
                    while (await reader.ReadAsync())
                    {
                        Categories.Add(new CategoryModel
                        {
                            Id = reader.GetInt32("category_id"),
                            Index = index++,
                            CategoryName = reader.GetString("category_name")
                        });
                    }
                }
                
                // ปิดการเชื่อมต่อ
                con.connectdb.Close();
                
                Console.WriteLine($"🔍 พบข้อมูล {Categories.Count} รายการจากการค้นหา: {searchText}");

                // ผูกข้อมูลกับ DataGrid
                var dataGrid = this.Find<DataGrid>("dgCategories");
                if (dataGrid != null)
                {
                    dataGrid.ItemsSource = Categories;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error searching data: {ex.Message}");
            }
        }

        private async void LoadDataFromDatabase()
        {
            Categories = new ObservableCollection<CategoryModel>();
            
            var con = new Connection_db();

            try
            {
                // เปิดการเชื่อมต่อ
                await con.connectdb.OpenAsync();
                
                string sql = "SELECT category_id, category_name FROM category ORDER BY category_id ASC";
                MySqlCommand cmd = new MySqlCommand(sql, con.connectdb);
                
                using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                {
                    int index = 1;
                    while (await reader.ReadAsync())
                    {
                        Categories.Add(new CategoryModel
                        {
                            Id = reader.GetInt32("category_id"),
                            Index = index++,
                            CategoryName = reader.GetString("category_name")
                        });
                    }
                }
                
                // ปิดการเชื่อมต่อ
                con.connectdb.Close();
                
                Console.WriteLine($"✅ โหลดข้อมูล {Categories.Count} รายการจากฐานข้อมูล");

                // ผูกข้อมูลกับ DataGrid
                var dataGrid = this.Find<DataGrid>("dgCategories");
                if (dataGrid != null)
                {
                    dataGrid.ItemsSource = Categories;
                    Console.WriteLine($"✅ แสดงข้อมูลใน DataGrid แล้ว");
                }
                var txtCategoryName = this.Find<TextBox>("txtCategoryName");
                if (txtCategoryName != null)
                {
                    txtCategoryName.Text = string.Empty;
                    Console.WriteLine($"✅ ລ້າງຂໍ້ມູນໃນ TextBox ແລ້ວ");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading data: {ex.Message}");
            }
        }
    }

    // Model class สำหรับเก็บข้อมูลหมวดหมู่
    public class CategoryModel
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
