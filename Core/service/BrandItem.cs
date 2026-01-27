using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using My_program.Views.helper;
using MySql.Data.MySqlClient;
using My_program.Views;

namespace My_program.Core.service
{
    public class BrandService
    {
        /// <summary>
        /// โหลดรายการ Brand ทั้งหมดจากฐานข้อมูล
        /// </summary>
        /// <returns>ObservableCollection ของ BrandModel</returns>
        public static async Task<ObservableCollection<BrandModel>> LoadBrandAsync()
        {
            var brandList = new ObservableCollection<BrandModel>();
            var con = new Connection_db();

            try
            {
                // เปิดการเชื่อมต่อ
                await con.connectdb.OpenAsync();
                
                string sql = "SELECT brand_id, brand_name FROM brand ORDER BY brand_name ASC";
                MySqlCommand cmd = new MySqlCommand(sql, con.connectdb);
                
                using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        brandList.Add(new BrandModel
                        {
                            Id = reader.GetInt32("brand_id"),
                            BrandName = reader.GetString("brand_name")
                        });
                    }
                }
                
                // ปิดการเชื่อมต่อ
                con.connectdb.Close();
                
                Console.WriteLine($"✅ โหลดข้อมูล {brandList.Count} ยี่ห้อสินค้าจาก BrandService");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading brands from service: {ex.Message}");
            }

            return brandList;
        }
    }
}
