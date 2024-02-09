using Application.Common.Interfaces;

namespace Application.Common.Models;

public class LoggerForUserInterface : ILoggerForUserInterface
{
    private readonly IModemViewDataManager _comPortViewManager;

    public LoggerForUserInterface(IModemViewDataManager comPortViewManager)
    {
        _comPortViewManager = comPortViewManager;
    }
    public void AddMessage(string comPort, string message)
    {
        _comPortViewManager.GetComPortView(comPort).ViewLogger.Add(message);
    }

    public void ClearLog(string comPort)
    {
        _comPortViewManager.GetComPortView(comPort).ViewLogger.Clear();
    }
}
