using Application.Common.Interfaces;
using Application.ModemProcessing;
using Microsoft.Extensions.Configuration;
using Application.Common.Models.ModemView;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IModemViewDataManager, ModemViewDataManager>();
        services.AddScoped<IMainDataGridHandler, MainDataGridHandler>();
        return services;
    }
}
