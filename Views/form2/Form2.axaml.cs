using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Threading.Tasks;

namespace My_program.Views
{
    public partial class Form2 : Window
    {
        public Form2()
        {
            InitializeComponent();
        }

        // ຄຳນວນ
        private void CalculateButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                // Price
                if (string.IsNullOrEmpty(PriceTextBox.Text))
                {
                    ShowError("ກະລຸນາໃສ່ລາຄາສິນຄ້າ!");
                    PriceTextBox.BorderBrush = Brushes.Red;
                    Dispatcher.UIThread.Post(() => PriceTextBox.Focus());
                    return;
                }
                if (!double.TryParse(PriceTextBox.Text, out double price))
                {
                    ShowError("ກະລຸນາໃສ່ລາຄາເປັນຕົວເລກ!");
                    PriceTextBox.BorderBrush = Brushes.Red;
                    Dispatcher.UIThread.Post(() => PriceTextBox.Focus());
                    return;
                }
                PriceTextBox.BorderBrush = Brushes.Black;

                // Quantity
                if (string.IsNullOrEmpty(QuantityTextBox.Text))
                {
                    ShowError("ກະລຸນາໃສ່ຈຳນວນສິນຄ້າ!");
                    QuantityTextBox.BorderBrush = Brushes.Red;
                    Dispatcher.UIThread.Post(() => QuantityTextBox.Focus());
                    return;
                }
                if (!int.TryParse(QuantityTextBox.Text, out int quantity))
                {
                    ShowError("ກະລຸນາໃສ່ຈຳນວນເປັນຕົວເລກ!");
                    QuantityTextBox.BorderBrush = Brushes.Red;
                    Dispatcher.UIThread.Post(() => QuantityTextBox.Focus());
                    return;
                }
                QuantityTextBox.BorderBrush = Brushes.Black;

                double total = price * quantity;
                TotalTextBox.Text = total.ToString("0.00");

                CalculateChange();
            }
            catch (Exception ex)
            {
                ShowError($"ເກີດຂໍ້ຜິດພາດ: {ex.Message}");
            }
        }

        // ລ້າງຂໍ້ມູນ
        private async void ClearButton_Click(object? sender, RoutedEventArgs e)
        {
            bool shouldClear = await ShowConfirmDialog("ທ່ານແນ່ໃຈບໍ່ວ່າຈະລ້າງຂໍໍ້ມູນ?");
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
                    if (double.TryParse(TotalTextBox.Text, out double total) &&
                        double.TryParse(PaymentTextBox.Text, out double payment))
                    {
                        if (payment < total)
                        {
                            ChangeTextBox.Text = "ເງິນຊຳລະບໍ່ພໍ່";
                        }
                        else
                        {
                            double change = payment - total;
                            ChangeTextBox.Text = change.ToString("0.00");
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


        // dialog ຢືນຢັນ
        private async Task<bool> ShowConfirmDialog(string message)
        {
            bool result = false;

            var dialog = new Window
            {
                Title = "ຢືນຢັນ",
                Width = 350,
                Height = 180,
                Content = new StackPanel
                {
                    Margin = new Avalonia.Thickness(20),
                    Spacing = 15,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = message,
                            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                        },
                        new StackPanel
                        {
                            Orientation = Avalonia.Layout.Orientation.Horizontal,
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                            Spacing = 10,
                            Children =
                            {
                                new Button
                                {
                                    Content = "ຕົກລົງ",
                                    Width = 80,
                                    Background = new SolidColorBrush(Color.Parse("#1e4d7b"))
                                },
                                new Button
                                {
                                    Content = "ຍົກເລີກ",
                                    Width = 80,
                                    Background = new SolidColorBrush(Color.Parse("#b22222"))
                                }
                            }
                        }
                    }
                }
            };

            if (dialog.Content is StackPanel stackPanel && stackPanel.Children[1] is StackPanel buttonPanel)
            {
                if (buttonPanel.Children[0] is Button okButton)
                    okButton.Click += (_, _) => { result = true; dialog.Close(); };

                if (buttonPanel.Children[1] is Button cancelButton)
                    cancelButton.Click += (_, _) => { result = false; dialog.Close(); };
            }

            await dialog.ShowDialog(this);
            return result;
        }

        // dialog ແຈ້ງເຕືອນ
        private async void ShowError(string message)
        {
            var dialog = new Window
            {
                Title = "ແຈ້ງເຕືອນ",
                Width = 300,
                Height = 150,
                Content = new StackPanel
                {
                    Margin = new Avalonia.Thickness(20),
                    Spacing = 15,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = message,
                            TextWrapping = Avalonia.Media.TextWrapping.Wrap
                        },
                        new Button
                        {
                            Content = "ຕົກລົງ",
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                        }
                    }
                }
            };

            if (dialog.Content is StackPanel stackPanel && stackPanel.Children[1] is Button okButton)
                okButton.Click += (_, _) => dialog.Close();

            await dialog.ShowDialog(this);
        }
    }
}
