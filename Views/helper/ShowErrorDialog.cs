using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using System.Threading.Tasks;

namespace My_program.Views.helper
{
    public static class ShowErrorDialogHelper
    {
        public static async Task ShowErrorDialog(Window owner, string message)
        {
            var dialog = new Window
            {
                Title = "ແຈ້ງເຕືອນ",
                Width = 350,
                Height = 200,
                Content = new StackPanel
                {
                    Margin = new Avalonia.Thickness(20),
                    Spacing = 15,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "❌",
                            FontSize = 40,
                            HorizontalAlignment = HorizontalAlignment.Center
                        },
                        new TextBlock
                        {
                            Text = message,
                            FontSize = 13,
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
                            Background = new SolidColorBrush(Color.Parse("#b22222")),
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
