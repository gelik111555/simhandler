namespace Application.Common.Exceptions;

public class ModemResponseException : Exception
{
    public ModemResponseException(string message) : base(message) { }
    public ModemResponseException(string message, Exception innerException)
       : base(message, innerException)
    {
    }
}
