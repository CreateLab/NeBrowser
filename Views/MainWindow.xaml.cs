using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NeBrowser.ViewModels;

namespace NeBrowser.Views
{
    public class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel model)
        {
            InitializeComponent();
            DataContext = model;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}