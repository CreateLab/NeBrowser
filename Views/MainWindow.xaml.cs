using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NeBrowser.ViewModels;

namespace NeBrowser.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}