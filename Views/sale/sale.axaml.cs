using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Threading.Tasks;

namespace My_program.Views;

public partial class SaleWindow : Window
{

    public SaleWindow()
    {
        InitializeComponent();
    }

    // ຟັງຊັ່ນຄິດໄລ່ (ເມື່ອກົດປຸ່ມ)
    private void CalculateButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            // ກວດສອບວ່າຊ່ອງລາຄາເປັນຄ່າວ່າງບໍ່
            if (string.IsNullOrEmpty(PriceTextBox.Text))
            {
                ShowError("ກະລຸນາໃສ່ລາຄາສິນຄ້າ!");
                return;
            }

            // ກວດສອບວ່າຊ່ອງຈຳນວນເປັນຄ່າວ່າງບໍ່
            if (string.IsNullOrEmpty(QuantityTextBox.Text))
            {
                ShowError("ກະລຸນາໃສ່ຈຳນວນສິນຄ້າ!");
                return;
            }

            // ດຶງຄ່າຈາກ TextBox ແລະ ກວດສອບວ່າເປັນຕົວເລກບໍ່
            if (!double.TryParse(PriceTextBox.Text, out double price))
            {
                ShowError("ກະລຸນາໃສ່ລາຄາທີ່ຖືກຕ້ອງ!");
                return;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int quantity))
            {
                ShowError("ກະລຸນາໃສ່ຈຳນວນທີ່ຖືກຕ້ອງ!");
                return;
            }

            // ຄິດໄລ່ລາຄາລວມ
            double total = price * quantity;
            // // ສະແດງຜົນ
            TotalTextBox.Text = total.ToString("0.00");

            // ຄິດໄລ່ເງີນທອນຖ້າມີການໃສ່ເງີນຊໍາລະ
            CalculateChange();
        }
        catch (Exception ex)
        {
            ShowError($"ເກີດຂໍ້ຜິດພາດ: {ex.Message}");
        }
    }

    // ຄິດໄລ່ເງີນທອນເມື່ອມີການປ່ຽນແປງເງີນຊໍາລະ (Real-time)
    private void PaymentTextBox_TextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        // ພິມຄ່າເງິນຊຳລະທີ່ກຳລັງປ້ອນອອກໄປ Real-time
        if (sender is TextBox textBox && !string.IsNullOrEmpty(textBox.Text))
        {
            Console.WriteLine($">>> ເງິນຊໍາລະ (Payment) ກຳລັງປ້ອນ: {textBox.Text}");
        }
        
        CalculateChange();
    }

    // ຟັງຊັ່ນຄິດໄລ່ເງີນທອນ
    private void CalculateChange()
    {
        try
        {
            if (!string.IsNullOrEmpty(TotalTextBox.Text) && 
                !string.IsNullOrEmpty(PaymentTextBox.Text))
            {
                if (double.TryParse(TotalTextBox.Text, out double total) &&
                    double.TryParse(PaymentTextBox.Text, out double payment))
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
        catch
        {
            ChangeTextBox.Text = "";
        }
    }

    // ຟັງຊັ່ນລ້າງຂໍ້ມູນ
    private async void ClearButton_Click(object? sender, RoutedEventArgs e)
    {
        // ຢືນຢັນກ່ອນລ້າງຂໍ້ມູນ
        bool shouldClear = await ShowConfirmDialog("ທ່ານແນ່ໃຈບໍ່ວ່າຈະລ້າງຂໍໍ້ມູນ?");
        if (!shouldClear)
            return;

        PriceTextBox.Text = "";
        QuantityTextBox.Text = "";
        TotalTextBox.Text = "";
        PaymentTextBox.Text = "";
        ChangeTextBox.Text = "";
        
        Console.WriteLine("\n🗑️  ລ້າງຂໍ້ມູນແລ້ວ (Data Cleared)\n");
    }

    // ສະແດງ dialog ຢືນຢັນ
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
                                Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#1e4d7b"))
                            },
                            new Button 
                            { 
                                Content = "ຍົກເລີກ",
                                Width = 80,
                                Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#b22222"))
                            }
                        }
                    }
                }
            }
        };
        
        // ຈັດການປຸ່ມ
        if (dialog.Content is StackPanel stackPanel && 
            stackPanel.Children[1] is StackPanel buttonPanel)
        {
            if (buttonPanel.Children[0] is Button okButton)
            {
                okButton.Click += (_, _) => { result = true; dialog.Close(); };
            }
            if (buttonPanel.Children[1] is Button cancelButton)
            {
                cancelButton.Click += (_, _) => { result = false; dialog.Close(); };
            }
        }

        await dialog.ShowDialog(this);
        return result;
    }

    // ສະແດງຂໍ້ຄວາມຜິດພາດ
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
        
        // ປິດ dialog ເມື່ອກົດປຸ່ມ ຕົກລົງ
        if (dialog.Content is StackPanel stackPanel && stackPanel.Children[1] is Button okButton)
        {
            okButton.Click += (_, _) => dialog.Close();
        }

        await dialog.ShowDialog(this);
        
        Console.WriteLine($"\n❌ ເກີດຂໍ້ຜິດພາດ (Error): {message}\n");
    }
}