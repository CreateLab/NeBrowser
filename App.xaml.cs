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
                var mw = sp.GetService<MainWindow>();
                desktop.MainWindow = mw;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}