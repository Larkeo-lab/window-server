using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using System.Threading.Tasks;

namespace My_program.Views.helper
{
    public static class ShowSuccessDialogHelper
    {
        public static async Task ShowSuccessDialog(Window owner, string message)
        {
            var dialog = new Window
            {
                Title = "ສຳເລັດ",
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
                            Text = "✅",
                            FontSize = 40,
                            HorizontalAlignment = HorizontalAlignment.Center
                        },
                        new TextBlock
                        {
                            Text = message,
                            FontSize = 14,
                            TextWrapping = TextWrapping.Wrap,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            TextAlignment = TextAlignment.Center
                        },
                        new Button
                        {
                            Content = "ຕົກລົງ",
                            Width = 100,
                            Height = 35,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Background = new SolidColorBrush(Color.Parse("#1e4d7b")),
                            Foreground = new SolidColorBrush(Colors.White)
                        }
                    }
                }
            };

            if (dialog.Content is StackPanel stackPanel && stackPanel.Children[2] is Button okButton)
            {
                okButton.Click += (_, _) => dialog.Close();
            }

            await dialog.ShowDialog(owner);
        }
    }
}
