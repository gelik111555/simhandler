using Application.Common.Interfaces;

namespace Application.ModemProcessing.ModemOperation;

internal abstract class BaseModemOperation : IModemOperation
{
    private IModemOperation _next;
    public async Task HandleAsync(ContextModemOperation modemOperationContext)
    {
        LogBefore();
        await HandleOperationAsync(modemOperationContext);
        LogAfter();
        if (_next != null) await _next.HandleAsync(modemOperationContext);
    }

    public IModemOperation SetNext(IModemOperation modemOperation)
    {
        _next = modemOperation;
        return modemOperation;
    }
    protected abstract Task HandleOperationAsync(ContextModemOperation modemOperation);
    protected abstract void LogBefore();
    protected abstract void LogAfter();
}
