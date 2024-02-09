namespace Application.Common.Exceptions;

public class ModemViewNotFound: Exception
{
    public ModemViewNotFound()
        : base()
    {
    }

    public ModemViewNotFound(string message)
        : base(message)
    {
    }

    public ModemViewNotFound(string message, Exception innerException)
        : base(message, innerException)
    {
    }

}
