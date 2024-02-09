using ModemLib.Domain.Interfaces;
using ModemLib.Models.Modems;

namespace Infrastructure.Common.Interfaces;

internal interface IModemSignal
{
    public Task<string> GetSignalAsync(IModemBase modem, CancellationToken cancellationToken);
}
