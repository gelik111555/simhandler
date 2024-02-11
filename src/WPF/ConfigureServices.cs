using WPF;

namespace Microsoft.Extensions.DependencyInjection;
public static class ConfigureServices
{
    public static IServiceCollection AddUIServices(this IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
        return services;
    }
}
