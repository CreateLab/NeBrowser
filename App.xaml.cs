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
                var sp = Program.ServiceCollection.BuildServiceProvider();
                desktop.MainWindow = sp.GetService<MainWindow>();
                desktop.MainWindow.DataContext = sp.GetServices<MainWindowViewModel>();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}