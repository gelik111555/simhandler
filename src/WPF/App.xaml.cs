using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Configuration;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    public static bool IsDesignMode { get; private set; } = true;

    private static IHost _Host;

    public static IHost Host => _Host ??= Program.CreateHostBuilder(Environment.GetCommandLineArgs()).Build();

    protected override async void OnStartup(StartupEventArgs e)
    {
        IsDesignMode = false;
        this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        var host = Host;
        base.OnStartup(e);
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                
            }
        }
        await host.StartAsync().ConfigureAwait(false);
        var mainWindow = host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        string errorMessage = string.Format("An unhandled exception occurred: {0}", e.Exception.Message);
        MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        var host = Host;
        await host.StopAsync().ConfigureAwait(false);
        host.Dispose();
        _Host.Dispose();
    }

    public static void ConfigureServices(HostBuilderContext host, IServiceCollection services)
    {
        services.AddApplicationServices(host.Configuration);
        services.AddInfrastructureServices(host.Configuration);
        services.AddUIServices();
    }

    public static string CurrentDirectory => IsDesignMode
        ? Path.GetDirectoryName(GetSourceCodePath())
        : Environment.CurrentDirectory;

    private static string GetSourceCodePath([CallerFilePath] string Path = null) => Path;
}
