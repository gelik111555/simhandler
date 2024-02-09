using Infrastructure.Common.Interfaces;
using Microsoft.Extensions.Logging;
using ModemLib.Domain.Interfaces;
using ModemLib.Factories.Modem;
using ModemLib.Models;
namespace Infrastructure.ModemCommunication;

public class Modem: IModem
{
    public async Task<IModemBase> GetModemAsync(string comPort, ILogger logger, CancellationToken cancellationToken)
    {
        ModemConfigurator modemConfigurator = new(comPort, logger, cancellationToken);
        return await new ModemFactory().CreateAsync(modemConfigurator);
    }
}
