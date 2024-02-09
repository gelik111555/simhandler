using Application.Common.Interfaces;
using Infrastructure.Common.Interfaces;
using Microsoft.Extensions.Logging;
using ModemLib.Domain.Interfaces;

namespace Infrastructure.ModemCommunication;

public class ModemServiceFactory : IModemServiceFactory
{
    private readonly ILogger<ModemServiceFactory> _logger;
    private readonly CancellationToken _cancellationToken;
    private readonly IModem _modem;
     
    public ModemServiceFactory(
        ILogger<ModemServiceFactory> logger,
        CancellationToken cancellationToken,
        IModem modem)
    {
        _logger = logger;
        _cancellationToken = cancellationToken;
        _modem = modem;
    }
    public async Task<IModemService> CreateAsync(string comPort)
    {
        IModemSignal signal = new ModemSignal();
        IModemBase modemBase = await _modem.GetModemAsync(comPort, _logger, _cancellationToken);
        return new ModemService(modemBase, signal, _cancellationToken);
    }
}
