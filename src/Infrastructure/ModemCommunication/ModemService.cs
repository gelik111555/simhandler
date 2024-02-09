using Application.Common.Interfaces;
using Infrastructure.Common.Interfaces;
using ModemLib.Domain.Interfaces;

namespace Infrastructure.ModemCommunication;

public class ModemService : IModemService
{
    private readonly IModemSignal _signal;
    private readonly CancellationToken _cancellationToken;
    private IModemBase _modemBase;
    internal ModemService(
        IModemBase modemBase,
        IModemSignal signal,
        CancellationToken cancellationToken)
    {
        _modemBase = modemBase;
        _signal = signal ?? throw new ArgumentNullException(nameof(signal));
        _cancellationToken = cancellationToken;
    }

    public string GetName()
    {
        return _modemBase.ModemModel;
    }

    public async Task<string> GetSignalLevelAsync()
    {
        return await _signal.GetSignalAsync(_modemBase, _cancellationToken);
    }
}
