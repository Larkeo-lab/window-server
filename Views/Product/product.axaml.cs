using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using My_program.Views.helper;
using MySql.Data.MySqlClient;
using My_program.Helpers;

namespace My_program.Views
{
    public partial class Product : UserControl
    {
        public ObservableCollection<CategoryModel>? Categories { get; set; }
        public ObservableCollection<ProductModel>? Products { get; set; }

        public Product()
        {
            InitializeComponent();
            LoadCategoriesIntoComboBox();
            LoadBrandsIntoComboBox();
            ApplyNumberFormatting();
            LoadDataFromDatabase();
        }


        
        private async void LoadCategoriesIntoComboBox()
        {
            try
            {
                // เรียกใช้ CategoryService เพื่อโหลดข้อมูล
                var categoryList = await Core.service.CategoryService.LoadCategoriesAsync();

                // ผูกข้อมูลกับ ComboBox
                var comboCategory = this.Find<ComboBox>("comboCategory");
                if (comboCategory != null)
                {
                    comboCategory.ItemsSource = categoryList;
                    Console.WriteLine($"✅ แสดงข้อมูล {categoryList.Count} ประเภทใน ComboBox แล้ว");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading categories in Product view: {ex.Message}");
            }
        }
        
        private async void LoadBrandsIntoComboBox()
        {
            try
            {
                // เรียกใช้ BrandService เพื่อโหลดข้อมูล
                var brandList = await Core.service.BrandService.LoadBrandAsync();

                // ผูกข้อมูลกับ ComboBox
                var comboBrand = this.Find<ComboBox>("comboBrand");
                if (comboBrand != null)
                {
                    comboBrand.ItemsSource = brandList;
                    Console.WriteLine($"✅ แสดงข้อมูล {brandList.Count} ยี่ห้อใน ComboBox แล้ว");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading brands in Product view: {ex.Message}");
            }
        }
        
        private void ApplyNumberFormatting()
        {
            // หา TextBox ที่ต้องการใส่ NumberFormatter
            var textQty = this.Find<TextBox>("textQty");
            var txtQtyMin = this.Find<TextBox>("txtQtyMin");
            var textCostPrice = this.Find<TextBox>("textCost_price");
            var textRetailPrice = this.Find<TextBox>("textRetail_price");
            
            // ใช้ NumberFormatter กับแต่ละ TextBox
            if (textQty != null)
            {
                NumberFormatter.ApplyNumberComma(textQty, integerOnly: true); // จำนวนเต็มเท่านั้น
                Console.WriteLine("✅ ใช้ NumberFormatter กับ textQty แล้ว");
            }
            
            if (txtQtyMin != null)
            {
                NumberFormatter.ApplyNumberComma(txtQtyMin, integerOnly: true); // จำนวนเต็มเท่านั้น
                Console.WriteLine("✅ ใช้ NumberFormatter กับ txtQtyMin แล้ว");
            }
            
            if (textCostPrice != null)
            {
                NumberFormatter.ApplyNumberComma(textCostPrice, integerOnly: false); // รองรับทศนิยม
                Console.WriteLine("✅ ใช้ NumberFormatter กับ textCost_price แล้ว");
            }
            
            if (textRetailPrice != null)
            {
                NumberFormatter.ApplyNumberComma(textRetailPrice, integerOnly: false); // รองรับทศนิยม
                Console.WriteLine("✅ ใช้ NumberFormatter กับ textRetail_price แล้ว");
            }
        }

        private async void LoadDataFromDatabase()
        {
            Products = new ObservableCollection<ProductModel>();
            
            var con = new Connection_db();

            try
            {
                // เปิดการเชื่อมต่อ
                await con.connectdb.OpenAsync();
                
                string sql = "SELECT barcode, product_name, unit, quantity, quantity_min, cost_price, retail_price, brand_name, category_name, status FROM product p JOIN brand b ON p.brand_id = b.brand_id JOIN category c ON p.category_id = c.category_id ORDER BY barcode ASC";
                MySqlCommand cmd = new MySqlCommand(sql, con.connectdb);
                
                using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Products.Add(new ProductModel
                        {
                            barcode = reader.GetString("barcode"),
                            product_name = reader.GetString("product_name"),
                            unit = reader.GetString("unit"),
                            quantity = reader.GetInt32("quantity"),
                            quantity_min = reader.GetInt32("quantity_min"),
                            cost_price = reader.GetDecimal("cost_price"),
                            retail_price = reader.GetDecimal("retail_price"),
                            brand_name = reader.GetString("brand_name"),
                            category_name = reader.GetString("category_name"),
                            status = reader.GetString("status")
                        });
                    }
                }
                
                // ปิดการเชื่อมต่อ
                con.connectdb.Close();
                
                Console.WriteLine($"✅ โหลดข้อมูล {Products.Count} รายการสินค้าจากฐานข้อมูล");

                // ผูกข้อมูลกับ DataGrid
                var dataGrid = this.Find<DataGrid>("dgProducts");
                if (dataGrid != null)
                {
                    dataGrid.ItemsSource = Products;
                    Console.WriteLine($"✅ แสดงข้อมูลสินค้าใน DataGrid แล้ว");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading product data: {ex.Message}");
            }
        }
    }
}

// Model class สำหรับเก็บข้อมูลสินค้า
public class ProductModel
{
    public string barcode { get; set; } = string.Empty;
    public string product_name { get; set; } = string.Empty;
    public string unit { get; set; } = string.Empty;
    public int quantity { get; set; }
    public int quantity_min { get; set; }
    public decimal cost_price { get; set; }
    public decimal retail_price { get; set; }
    public string brand_name { get; set; } = string.Empty;
    public string category_name { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;
}
