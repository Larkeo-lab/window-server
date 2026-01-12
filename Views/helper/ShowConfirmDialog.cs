using Avalonia.Controls;
using Avalonia.Media;
using System.Threading.Tasks;

namespace My_program.Helpers
{
    public static class ShowConfirmDialog
    {
        /// <summary>
        /// แสดง dialog ยืนยัน
        /// </summary>
        public static async Task<bool> Show(Window parent, string message)
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

            await dialog.ShowDialog(parent);
            return result;
        }
    }
}
