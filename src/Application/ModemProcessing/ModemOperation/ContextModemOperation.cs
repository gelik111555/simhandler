using Application.Common.Interfaces;

namespace Application.ModemProcessing.ModemOperation;

public class ContextModemOperation
{
    public ContextModemOperation(IModemService modemService)
    {
        ModemService = modemService;
    }

    public IModemService ModemService { get; }
}
