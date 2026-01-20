using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using System.Threading.Tasks;

namespace My_program.Views.helper
{
    public static class ShowConfirmationDialogHelper
    {
        public static async Task<bool> ShowConfirmationDialog(Window owner, string message)
        {
            bool result = false;
            
            var dialog = new Window
            {
                Title = "ຢືນຢັນ",
                Width = 400,
                Height = 220,
                CanResize = false,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = new StackPanel
                {
                    Margin = new Avalonia.Thickness(20),
                    Spacing = 20,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "⚠️",
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
                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Spacing = 10,
                            Children =
                            {
                                new Button
                                {
                                    Content = "ຕົກລົງ",
                                    Width = 100,
                                    Height = 35,
                                    Background = new SolidColorBrush(Color.Parse("#DC3545")),
                                    Foreground = new SolidColorBrush(Colors.White),
                                    FontWeight = FontWeight.Bold
                                },
                                new Button
                                {
                                    Content = "ຍົກເລີກ",
                                    Width = 100,
                                    Height = 35,
                                    Background = new SolidColorBrush(Color.Parse("#6C757D")),
                                    Foreground = new SolidColorBrush(Colors.White),
                                    FontWeight = FontWeight.Bold
                                }
                            }
                        }
                    }
                }
            };

            if (dialog.Content is StackPanel stackPanel && stackPanel.Children[2] is StackPanel buttonPanel)
            {
                if (buttonPanel.Children[0] is Button confirmButton)
                {
                    confirmButton.Click += (_, _) =>
                    {
                        result = true;
                        dialog.Close();
                    };
                }

                if (buttonPanel.Children[1] is Button cancelButton)
                {
                    cancelButton.Click += (_, _) =>
                    {
                        result = false;
                        dialog.Close();
                    };
                }
            }

            await dialog.ShowDialog(owner);
            return result;
        }
    }
}
