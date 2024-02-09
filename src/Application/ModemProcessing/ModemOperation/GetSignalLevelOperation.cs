
namespace Application.ModemProcessing.ModemOperation;

internal class GetSignalLevelOperation : BaseModemOperation
{
    protected override async Task HandleOperationAsync(ContextModemOperation modemOperation)
    {
        await modemOperation.ModemService.GetSignalLevelAsync();
    }

    protected override void LogAfter()
    {
        
    }

    protected override void LogBefore()
    {
       
    }
}
