using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using My_program.Views.helper;
using MySql.Data.MySqlClient;
using My_program.Views;

namespace My_program.Core.service
{
    public class CategoryService
    {
        /// <summary>
        /// โหลดรายการ Category ทั้งหมดจากฐานข้อมูล
        /// </summary>
        /// <returns>ObservableCollection ของ CategoryModel</returns>
        public static async Task<ObservableCollection<CategoryModel>> LoadCategoriesAsync()
        {
            var categoryList = new ObservableCollection<CategoryModel>();
            var con = new Connection_db();

            try
            {
                // เปิดการเชื่อมต่อ
                await con.connectdb.OpenAsync();
                
                string sql = "SELECT category_id, category_name FROM category ORDER BY category_name ASC";
                MySqlCommand cmd = new MySqlCommand(sql, con.connectdb);
                
                using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        categoryList.Add(new CategoryModel
                        {
                            Id = reader.GetInt32("category_id"),
                            CategoryName = reader.GetString("category_name")
                        });
                    }
                }
                
                // ปิดการเชื่อมต่อ
                con.connectdb.Close();
                
                Console.WriteLine($"✅ โหลดข้อมูล {categoryList.Count} ประเภทสินค้าจาก CategoryService");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading categories from service: {ex.Message}");
            }

            return categoryList;
        }
    }
}
