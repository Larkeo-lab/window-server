using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Threading.Tasks;
using My_program.Helpers;

namespace My_program.Views
{
    public partial class Form3 : Window
    {
        public Form3()
        {
            InitializeComponent();
            
            // เรียกใช้ NumberFormatter สำหรับ TextBox ที่ต้องการแสดงเลขมีจุดคั่น
            // false = รองรับทศนิยม, true = จำนวนเต็มเท่านั้น
            NumberFormatter.ApplyNumberComma(PriceTextBox, false);  // รองรับทศนิยม
            NumberFormatter.ApplyNumberComma(QuantityTextBox, true);  // จำนวนเต็มเท่านั้น
            NumberFormatter.ApplyNumberComma(PaymentTextBox, false);  // รองรับทศนิยม
        }

        // ຄຳນວນ
        private async void CalculateButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                // Price
                if (string.IsNullOrEmpty(PriceTextBox.Text))
                {
                    await Helpers.ShowDialog.ShowError(this, "ກະລຸນາໃສ່ລາຄາສິນຄ້າ!");
                    PriceTextBox.BorderBrush = Brushes.Red;
                    Dispatcher.UIThread.Post(() => PriceTextBox.Focus());
                    return;
                }
                
                double price = NumberFormatter.ParseNumber(PriceTextBox.Text);
                if (price <= 0)
                {
                    await Helpers.ShowDialog.ShowError(this, "ກະລຸນາໃສ່ລາຄາເປັນຕົວເລກ!");
                    PriceTextBox.BorderBrush = Brushes.Red;
                    Dispatcher.UIThread.Post(() => PriceTextBox.Focus());
                    return;
                }
                PriceTextBox.BorderBrush = Brushes.Black;

                // Quantity
                if (string.IsNullOrEmpty(QuantityTextBox.Text))
                {
                    await Helpers.ShowDialog.ShowError(this, "ກະລຸນາໃສ່ຈຳນວນສິນຄ້າ!");
                    QuantityTextBox.BorderBrush = Brushes.Red;
                    Dispatcher.UIThread.Post(() => QuantityTextBox.Focus());
                    return;
                }
                
                int quantity = (int)NumberFormatter.ParseNumber(QuantityTextBox.Text);
                if (quantity <= 0)
                {
                    await Helpers.ShowDialog.ShowError(this, "ກະລຸນາໃສ່ຈຳນວນເປັນຕົວເລກ!");
                    QuantityTextBox.BorderBrush = Brushes.Red;
                    Dispatcher.UIThread.Post(() => QuantityTextBox.Focus());
                    return;
                }
                QuantityTextBox.BorderBrush = Brushes.Black;

                double total = price * quantity;
                TotalTextBox.Text = NumberFormatter.Format(total);

                CalculateChange();
            }
            catch (Exception ex)
            {
                await Helpers.ShowDialog.ShowError(this, $"ເກີດຂໍ້ຜິດພາດ: {ex.Message}");
            }
        }

        // ລ້າງຂໍ້ມູນ
        private async void ClearButton_Click(object? sender, RoutedEventArgs e)
        {
            bool shouldClear = await Helpers.ShowConfirmDialog.Show(this, "ທ່ານແນ່ໃຈບໍ່ວ່າຈະລ້າງຂໍໍ້ມູນ?");
            if (!shouldClear) return;

            PriceTextBox.Text = "";
            PriceTextBox.BorderBrush = Brushes.Black;
            QuantityTextBox.Text = "";
            QuantityTextBox.BorderBrush = Brushes.Black;
            TotalTextBox.Text = "";
            PaymentTextBox.Text = "";
            PaymentTextBox.BorderBrush = Brushes.Black;
            ChangeTextBox.Text = "";
        }

        // ຄຳນວນເງີນທອນ real-time
        private void PaymentTextBox_TextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
        {
            CalculateChange();
        }

        // ຄຳນວນເງີນທອນ
        private void CalculateChange()
        {
            try
            {
                if (!string.IsNullOrEmpty(TotalTextBox.Text) && !string.IsNullOrEmpty(PaymentTextBox.Text))
                {
                    double total = NumberFormatter.ParseNumber(TotalTextBox.Text);
                    double payment = NumberFormatter.ParseNumber(PaymentTextBox.Text);
                    
                    if (total > 0 && payment > 0)
                    {
                        if (payment < total)
                        {
                            ChangeTextBox.Text = "ເງິນຊຳລະບໍ່ພໍ່";
                        }
                        else
                        {
                            double change = payment - total;
                            ChangeTextBox.Text = NumberFormatter.Format(change);
                        }
                    }
                    else
                    {
                        ChangeTextBox.Text = "";
                    }
                }
                else
                {
                    ChangeTextBox.Text = "";
                }
            }
            catch
            {
                ChangeTextBox.Text = "";
            }
        }
    }
}
