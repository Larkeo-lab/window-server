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

        public Product()
        {
            InitializeComponent();
            LoadCategoriesIntoComboBox();
            LoadBrandsIntoComboBox();
            ApplyNumberFormatting();
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
    }
}
