using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Material.Styles.Themes;
using Microsoft.Extensions.DependencyInjection;
using NeBrowser.ViewModels;
using NeBrowser.Views;

namespace NeBrowser
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static readonly ServiceCollection ServiceCollection = new ServiceCollection();
        public static  ServiceProvider ServiceProvider;
        public static void Main(string[] args)
        {
            InitCollection();
            ServiceProvider = ServiceCollection.BuildServiceProvider();
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

        private static void InitCollection()
        {
            ServiceCollection.AddSingleton<MainWindow, MainWindow>();
            ServiceCollection.AddTransient<Setting, Setting>();
            ServiceCollection.AddSingleton<MainWindowViewModel, MainWindowViewModel>();
            ServiceCollection.AddSingleton<SettingWindowViewModel, SettingWindowViewModel>();
            ServiceCollection.AddSingleton<PaletteHelper, PaletteHelper>();
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug()
                .UseReactiveUI();
    }
}