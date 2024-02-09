using Application.Common.Interfaces;
using Infrastructure.Common.Interfaces;
using Infrastructure.Common.System;
using Infrastructure.ModemCommunication;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IModem, Modem>();
        services.AddSingleton<IComPort, ComPort>();
        services.AddScoped<IModemService, ModemService>();
        services.AddScoped<IModemServiceFactory, ModemServiceFactory>();
        return services;
    }
}
