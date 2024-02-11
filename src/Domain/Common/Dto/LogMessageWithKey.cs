namespace Domain.Common.Dto;


//TODO: Need to delete
public class LogMessageWithKey
{
    public LogMessageWithKey(string comPort)
    {
        ComPort = comPort;
    }
    public LogMessageWithKey(string message, string comPort)
    {
        Message = message;
        ComPort = comPort;
    }
    public string? Message { get; }
    public string ComPort { get; }
}
