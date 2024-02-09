using Application.Common.Interfaces;
using Application.Common.Models;
using Application.ModemProcessing.ModemOperation;
using Microsoft.Extensions.Hosting;
using System.Threading;

namespace Application.ModemProcessing;

internal class ChainModemHandle
{
    private readonly IModemService _modemService;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private List<IModemOperation> _modemOperations = new();
    private IModemOperation _chainOperations;

    internal ChainModemHandle(IModemService modemService, IHostApplicationLifetime  hostApplicationLifetime)
    {
        _modemService = modemService;
        _hostApplicationLifetime = hostApplicationLifetime;
        AddModemOperationToList();
        CreateChainOperation();
    }
    internal async Task StartUp()
    {
        var context = new ContextModemOperation(_modemService);

        while (!_hostApplicationLifetime.ApplicationStopping.IsCancellationRequested)
        {
            try
            {
                await _chainOperations.HandleAsync(context);
            }
            catch (Exception ex)
            {

            }
        }
    }


    private void AddModemOperationToList()
    {
        _modemOperations.Add(new GetSignalLevelOperation());
    }

    private void CreateChainOperation()
    {
        IModemOperation previousOperation = null;
        foreach (var operation in _modemOperations)
        {
            previousOperation?.SetNext(operation);
            previousOperation = operation;
        }
        _chainOperations = previousOperation;
    }
}
