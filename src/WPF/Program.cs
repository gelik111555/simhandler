using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using Serilog;

namespace WPF;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        var app = new App();
        //app.InitializeComponent();
        app.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] Args)
    {
        return Host.CreateDefaultBuilder(Args)
           .UseContentRoot(App.CurrentDirectory)
           .ConfigureAppConfiguration((host, cfg) => cfg
               .SetBasePath(App.CurrentDirectory)
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true))
            .UseSerilog((context, services, configuration) =>
                {
                    configuration
                        .ReadFrom.Configuration(context.Configuration)
                        .MinimumLevel.Information()
                        .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                        .WriteTo.Console();
                })
           .ConfigureServices(App.ConfigureServices);
    }
}
