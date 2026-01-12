using Avalonia.Controls;
using System.Collections.ObjectModel;

namespace My_program.Views
{
    public partial class branMagement : UserControl
    {
        public branMagement()
        {
            InitializeComponent();
            InitializeData();
        }

        private void InitializeData()
        {
            // สร้างข้อมูลตัวอย่างเพื่อแสดงใน DataGrid
            var brands = new ObservableCollection<BrandModel>
            {
                new BrandModel { Index = 1, BrandName = "ຊີຊະນຸ" },
                new BrandModel { Index = 2, BrandName = "ແວບສີ" },
                new BrandModel { Index = 3, BrandName = "ຄົວເສື້ອ" },
                new BrandModel { Index = 4, BrandName = "ເບເລາວ" }
            };

            // ผูกข้อมูลกับ DataGrid
            var dataGrid = this.FindControl<DataGrid>("dgBrands");
            if (dataGrid != null)
            {
                dataGrid.ItemsSource = brands;
            }
        }
    }

    // Model class สำหรับเก็บข้อมูลยี่ห้อ
    public class BrandModel
    {
        public int Index { get; set; }
        public string BrandName { get; set; } = string.Empty;
    }
}
