using Avalonia.Controls;
using Avalonia.Media;
using System.Threading.Tasks;

namespace My_program.Helpers
{
    public static class ShowDialog
    {
        /// <summary>
        /// แสดง dialog แจ้งเตือน
        /// </summary>
        public static async Task ShowError(Window parent, string message)
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

            await dialog.ShowDialog(parent);
        }
    }
}
