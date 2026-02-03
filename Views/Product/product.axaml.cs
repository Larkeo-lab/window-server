using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
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
            
            InitializeComponent();
            
            LoadCategoriesIntoComboBox();
            LoadBrandsIntoComboBox();
            ApplyNumberFormatting();
            LoadDataFromDatabase();

            //buttonAdd
            var buttonAdd = this.Find<Button>("buttonAdd");
            if (buttonAdd != null)
            {
                buttonAdd.Click += buttonAdd_Click;
                Console.WriteLine("✅ buttonAdd ຖືກຜູກກັບເຫດການແລ້ວ");
            }

            //buttonCancel
            var buttonCancel = this.Find<Button>("buttonCancel");
            if (buttonCancel != null)
            {
                buttonCancel.Click += buttonCancel_Click;
                Console.WriteLine("✅ buttonCancel ຖືກຜູກກັບເຫດການແລ້ວ");
            }
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
                
                string sql = "SELECT barcode, product_name, unit, quantity, quantity_min, cost_price, retail_price, p.brand_id, brand_name, p.category_id, category_name, status FROM product p JOIN brand b ON p.brand_id = b.brand_id JOIN category c ON p.category_id = c.category_id ORDER BY barcode ASC";
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
                            brand_id = reader.GetInt32("brand_id"),
                            brand_name = reader.GetString("brand_name"),
                            category_id = reader.GetInt32("category_id"),
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
                    dataGrid.DoubleTapped += DgProducts_DoubleTapped;
                    dataGrid.SelectionChanged += DgProducts_SelectionChanged;
                    Console.WriteLine($"✅ แสดงข้อมูลสินค้าใน DataGrid แล้ว");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading product data: {ex.Message}");
            }
        }

        //buttonAdd
        private async void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgProducts.SelectedItem != null)
                {
                    var window = (Window)this.VisualRoot;
                    await My_program.Helpers.ShowDialog.ShowError(window, "ຖ້າເລືອກແລ້ວບໍ່ສາມາດເພີ່ມໄດ້");
                    return;
                }
                // Find controls locally to avoid conflict with generated fields
                var txtPro_id = this.Find<TextBox>("txtPro_id");
                var txtProName = this.Find<TextBox>("txtProName");
                var textUnit = this.Find<TextBox>("textUnit");
                var textQty = this.Find<TextBox>("textQty");
                var txtQtyMin = this.Find<TextBox>("txtQtyMin");
                var textCostPrice = this.Find<TextBox>("textCost_price");
                var textRetailPrice = this.Find<TextBox>("textRetail_price");
                var comboBrand = this.Find<ComboBox>("comboBrand");
                var comboCategory = this.Find<ComboBox>("comboCategory");
                var comboStatus = this.Find<ComboBox>("comboStatus");

                if (string.IsNullOrWhiteSpace(txtPro_id?.Text) ||
                    string.IsNullOrWhiteSpace(txtProName?.Text) ||
                    string.IsNullOrWhiteSpace(textUnit?.Text) ||
                    string.IsNullOrWhiteSpace(textQty?.Text) ||
                    string.IsNullOrWhiteSpace(txtQtyMin?.Text) ||
                    string.IsNullOrWhiteSpace(textCostPrice?.Text) ||
                    string.IsNullOrWhiteSpace(textRetailPrice?.Text) ||
                    comboBrand?.SelectedItem == null ||
                    comboCategory?.SelectedItem == null ||
                    comboStatus?.SelectedItem == null)
                {
                    var window = (Window)this.VisualRoot;
                    await My_program.Helpers.ShowDialog.ShowError(window, "ກະລຸນາປ້ອນຂໍ້ມູນໄຫ້ຄົບ");
                    return;
                }
                // หา TextBox ที่ต้องการใส่ NumberFormatter

                
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

                try
                {
                    var window = (Window)this.VisualRoot;

                    // ตรวจสอบรหัสซ้ำ
                    var con = new Connection_db();
                    await con.connectdb.OpenAsync();
                    
                    string checkSql = "SELECT COUNT(*) FROM product WHERE barcode = @barcode";
                    MySqlCommand checkCmd = new MySqlCommand(checkSql, con.connectdb);
                    checkCmd.Parameters.AddWithValue("@barcode", txtPro_id.Text);
                    int count = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());

                    if (count > 0)
                    {
                        con.connectdb.Close();
                        await My_program.Helpers.ShowDialog.ShowError(window, "ລະຫັດສິນຄ້ານີ້ມີໃນລະບົບແລ້ວ");
                        return;
                    }

                    // เพิ่มข้อมูลลงฐานข้อมูล
                    string insertSql = @"INSERT INTO product VALUES (
                                        @barcode, 
                                        @product_name, 
                                        @unit, 
                                        @quantity, 
                                        @quantity_min, 
                                        @cost_price, 
                                        @retail_price, 
                                        @brand_id, 
                                        @category_id, 
                                        @status
                                    )";
                    
                    MySqlCommand insertCmd = new MySqlCommand(insertSql, con.connectdb);
                    insertCmd.Parameters.AddWithValue("@barcode", txtPro_id.Text);
                    insertCmd.Parameters.AddWithValue("@product_name", txtProName.Text);
                    insertCmd.Parameters.AddWithValue("@unit", textUnit.Text);
                    insertCmd.Parameters.AddWithValue("@quantity", int.Parse(textQty.Text.Replace(",", "")));
                    insertCmd.Parameters.AddWithValue("@quantity_min", int.Parse(txtQtyMin.Text.Replace(",", "")));
                    insertCmd.Parameters.AddWithValue("@cost_price", decimal.Parse(textCostPrice.Text.Replace(",", "")));
                    insertCmd.Parameters.AddWithValue("@retail_price", decimal.Parse(textRetailPrice.Text.Replace(",", "")));
                    
                    var selectedBrand = (BrandModel)comboBrand.SelectedItem;
                    insertCmd.Parameters.AddWithValue("@brand_id", selectedBrand.Id);

                    var selectedCategory = (CategoryModel)comboCategory.SelectedItem;
                    insertCmd.Parameters.AddWithValue("@category_id", selectedCategory.Id);

                    var selectedStatus = (ComboBoxItem)comboStatus.SelectedItem;
                    insertCmd.Parameters.AddWithValue("@status", selectedStatus.Content.ToString());

                    await insertCmd.ExecuteNonQueryAsync();
                    con.connectdb.Close();

                    // โหลดข้อมูลใหม่เพื่ออัพเดท DataGrid
                    LoadDataFromDatabase();
                    ClearInputData(); // ล้างข้อมูลหลังจากเพิ่มสำเร็จ
                    
                    Console.WriteLine($"✅ Added product: {txtProName.Text}");
                    await ShowSuccessDialogHelper.ShowSuccessDialog(window, "ເພີ່ມຂໍ້ມູນສຳເລັດ");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error adding product: {ex.Message}");
                    var window = (Window)this.VisualRoot;
                    if (window != null)
                    {
                        await My_program.Helpers.ShowDialog.ShowError(window, $"ເກີດຂໍ້ຜິດພາດ: {ex.Message}");
                    }
                }
                // insert data
                

                

                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading product data: {ex.Message}");
            }
        }

        //buttonCancel
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            ClearInputData();
        }

        private void DgProducts_DoubleTapped(object? sender, TappedEventArgs e)
        {
            ClearInputData();
        }

        private void ClearInputData()
        {
            try
            {
                // Clear DataGrid Selection
                var dgProducts = this.Find<DataGrid>("dgProducts");
                if (dgProducts != null) dgProducts.SelectedItem = null;

                // Clear TextBoxes
                var txtPro_id = this.Find<TextBox>("txtPro_id");
                if (txtPro_id != null) 
                {
                    txtPro_id.Text = string.Empty;
                    txtPro_id.IsReadOnly = false;
                }

                var txtProName = this.Find<TextBox>("txtProName");
                if (txtProName != null) txtProName.Text = string.Empty;
                
                var textUnit = this.Find<TextBox>("textUnit");
                if (textUnit != null) textUnit.Text = string.Empty;

                var textQty = this.Find<TextBox>("textQty");
                if (textQty != null) textQty.Text = string.Empty;

                var txtQtyMin = this.Find<TextBox>("txtQtyMin");
                if (txtQtyMin != null) txtQtyMin.Text = string.Empty;

                var textCostPrice = this.Find<TextBox>("textCost_price");
                if (textCostPrice != null) textCostPrice.Text = string.Empty;

                var textRetailPrice = this.Find<TextBox>("textRetail_price");
                if (textRetailPrice != null) textRetailPrice.Text = string.Empty;

                // Clear ComboBoxes
                var comboBrand = this.Find<ComboBox>("comboBrand");
                if (comboBrand != null) comboBrand.SelectedItem = null;

                var comboCategory = this.Find<ComboBox>("comboCategory");
                if (comboCategory != null) comboCategory.SelectedItem = null;

                var comboStatus = this.Find<ComboBox>("comboStatus");
                if (comboStatus != null) comboStatus.SelectedItem = null;
                
                Console.WriteLine("✅ ยกเลิกการเลือกและล้างข้อมูลเรียบร้อย");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error clearing input data: {ex.Message}");
            }
        }

        private void DgProducts_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            try 
            {
                var dgProducts = this.Find<DataGrid>("dgProducts");
                if (dgProducts?.SelectedItem is ProductModel selectedProduct)
                {
                    // Find controls
                    var txtPro_id = this.Find<TextBox>("txtPro_id");
                    var txtProName = this.Find<TextBox>("txtProName");
                    var textUnit = this.Find<TextBox>("textUnit");
                    var textQty = this.Find<TextBox>("textQty");
                    var txtQtyMin = this.Find<TextBox>("txtQtyMin");
                    var textCostPrice = this.Find<TextBox>("textCost_price");
                    var textRetailPrice = this.Find<TextBox>("textRetail_price");
                    var comboBrand = this.Find<ComboBox>("comboBrand");
                    var comboCategory = this.Find<ComboBox>("comboCategory");
                    var comboStatus = this.Find<ComboBox>("comboStatus");

                    // Set TextBoxes
                    if (txtPro_id != null) 
                    {
                        txtPro_id.Text = selectedProduct.barcode;
                        txtPro_id.IsReadOnly = true;
                    }
                    if (txtProName != null) txtProName.Text = selectedProduct.product_name;
                    if (textUnit != null) textUnit.Text = selectedProduct.unit;
                    if (textQty != null) textQty.Text = selectedProduct.quantity.ToString("N0");
                    if (txtQtyMin != null) txtQtyMin.Text = selectedProduct.quantity_min.ToString("N0");
                    if (textCostPrice != null) textCostPrice.Text = selectedProduct.cost_price.ToString("N2");
                    if (textRetailPrice != null) textRetailPrice.Text = selectedProduct.retail_price.ToString("N2");

                    // Set ComboBox selections
                    if (comboBrand != null && comboBrand.ItemsSource != null)
                    {
                        foreach (var item in comboBrand.ItemsSource)
                        {
                            if (item is BrandModel brand && brand.Id == selectedProduct.brand_id)
                            {
                                comboBrand.SelectedItem = item;
                                break;
                            }
                        }
                    }

                    if (comboCategory != null && comboCategory.ItemsSource != null)
                    {
                        foreach (var item in comboCategory.ItemsSource)
                        {
                            if (item is CategoryModel category && category.Id == selectedProduct.category_id)
                            {
                                comboCategory.SelectedItem = item;
                                break;
                            }
                        }
                    }

                    if (comboStatus != null)
                    {
                        foreach (var item in comboStatus.Items)
                        {
                            if (item is ComboBoxItem comboItem && comboItem.Content?.ToString() == selectedProduct.status)
                            {
                                comboStatus.SelectedItem = item;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error selecting product: {ex.Message}");
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
    public int brand_id { get; set; }
    public string brand_name { get; set; } = string.Empty;
    public int category_id { get; set; }
    public string category_name { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;
}
