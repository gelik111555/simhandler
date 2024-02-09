using Application.ModemProcessing.ModemOperation;

namespace Application.Common.Interfaces;

internal interface IModemOperation
{
    IModemOperation SetNext(IModemOperation modemOperation);
    Task HandleAsync(ContextModemOperation modemOperation);
}
