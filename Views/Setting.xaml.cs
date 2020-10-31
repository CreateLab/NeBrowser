using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NeBrowser.Views
{
    public class Setting : Window
    {
        public Setting()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}