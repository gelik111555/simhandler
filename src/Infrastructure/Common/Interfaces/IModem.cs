using Microsoft.Extensions.Logging;
using ModemLib.Domain.Interfaces;

namespace Infrastructure.Common.Interfaces;

public interface IModem
{
    public Task<IModemBase> GetModemAsync(string comPort, ILogger logger, CancellationToken cancellationToken);
}
