using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using NeBrowser.ViewModels;
using NeBrowser.Views;

namespace NeBrowser
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Program.ServiceProvider.GetService<Setting>().DataContext = Program.ServiceProvider.GetService<SettingWindowViewModel>();
                desktop.MainWindow = Program.ServiceProvider.GetService<MainWindow>();
                desktop.MainWindow.DataContext = Program.ServiceProvider.GetService<MainWindowViewModel>();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}