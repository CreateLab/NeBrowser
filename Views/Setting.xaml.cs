using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NeBrowser.ViewModels;

namespace NeBrowser.Views
{
    public class Setting : Window
    {
        public Setting(SettingWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}