using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NeBrowser.Helpers;
using NeBrowser.ViewModels;
using NeBrowser.Views;
using Serilog;
using ILogger = Serilog.ILogger;

namespace NeBrowser
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static readonly IServiceCollection ServiceCollection = new ServiceCollection();
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
            InitLogger();
            ServiceCollection.AddSingleton<MainWindow, MainWindow>();
            ServiceCollection.AddTransient<Setting, Setting>();
            ServiceCollection.AddSingleton<MainWindowViewModel, MainWindowViewModel>();
            ServiceCollection.AddSingleton<SettingWindowViewModel, SettingWindowViewModel>();
            //ServiceCollection.AddSingleton<PaletteHelper, PaletteHelper>();
        }

        private static void InitLogger()
        {
           
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel
                .Debug()
                .WriteTo
                .Console()
                .WriteTo
                .File(Path.Combine(PathConstant.FolderPath,"log.txt"),
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true)
                .CreateLogger();
        }
        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug()
                .UseReactiveUI()
                .With(new X11PlatformOptions
                {
                    UseDBusMenu = true
                });
    }
}