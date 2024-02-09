using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.ModemView;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.ModemProcessing;

public class MainDataGridHandler : IMainDataGridHandler
{
    private readonly IModemServiceFactory _modemServiceFactory;
    private readonly ILogger<MainDataGridHandler> _logger;
    private readonly IModemViewDataManager _modemViewDataManager;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IComPort _comPort;
    private IEnumerable<string> _comPorts;
    private readonly bool _isDevelopment = true;
    private readonly string _developmentComPort = "COM103";
    private ICollection<Dictionary<ModemViewData, IModemService>> _modemViewAndService;

    public MainDataGridHandler(IModemServiceFactory modemServiceFactory,
        ILogger<MainDataGridHandler> logger,
        IModemViewDataManager modemViewDataManager,
        IHostApplicationLifetime hostApplicationLifetime,
        IComPort comPort)
    {
        _modemServiceFactory = modemServiceFactory;
        _logger = logger;
        _modemViewDataManager = modemViewDataManager;
        _hostApplicationLifetime = hostApplicationLifetime;
        _comPort = comPort;
    }
    public async Task Handle()
    {
        try
        {
            GetComPorts();
            FillingViewDataManager();
            await StartUpModemsHandle();
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex.Message);
            throw;
        }
    }
    private void GetComPorts()
    {
        _comPorts = _comPort.GetComPorts();
    }
    private void FillingViewDataManager()
    {
        _modemViewDataManager.AddComPortsView(_comPorts);
    }

    private async Task StartUpModemsHandle()
    {
        if (_isDevelopment)
        {
            await StartModem(_developmentComPort);
        }
        else
        {
            foreach (var comPort in _comPorts)
            {
                await StartModem(comPort);
            }
        }
    }

    private async Task StartModem(string comPort)
    {
        await Task.Run(async () =>
        {
            var modemService = await CreateModemServices(comPort);
            ChainModemHandle chainModemHandle = new(modemService, _hostApplicationLifetime);
            await chainModemHandle.StartUp();
        });
    }
    private async Task<IModemService> CreateModemServices(string comPort)
    {
        return await _modemServiceFactory.CreateAsync(comPort);
    }
}
