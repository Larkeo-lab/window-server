using System;
using Avalonia.Controls;

namespace My_program.Views
{
    public partial class Profiles : UserControl
    {
        public string EmpId { get; private set; }

        public Profiles()
        {
            InitializeComponent();
            EmpId = "";
        }

        public Profiles(string empId)
        {
            InitializeComponent();  
            EmpId = empId;

            // แสดงข้อมูลใน Console เพื่อตรวจสอบว่าได้รับข้อมูลถูกต้อง
            Console.WriteLine($"Profile loaded - ID: {EmpId}");
        }
    }
}
