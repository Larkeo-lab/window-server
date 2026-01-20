using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using My_program.Views.helper;
using MySql.Data.MySqlClient;

namespace My_program.Views
{
    public partial class Product : UserControl
    {
        public ObservableCollection<CategoryModel>? Categories { get; set; }

        public Product()
        {
            InitializeComponent();
            
        }
    }
}
