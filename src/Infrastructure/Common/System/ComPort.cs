using Application.Common.Interfaces;

namespace Infrastructure.Common.System;

public class ComPort : IComPort
{
    public IEnumerable<string> GetComPorts()
    {
        return ModemLib.Services.ComPortService.GetAllPorts();
    }
}
