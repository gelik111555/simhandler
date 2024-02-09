namespace Application.Common.Interfaces;

public interface ILoggerForUserInterface
{
    void AddMessage(string comPort, string message);
    void ClearLog(string comPort);
}
