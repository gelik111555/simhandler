namespace Application.Common.Exceptions;

public class EmptyAnswerFromModemException : Exception
{
    public EmptyAnswerFromModemException()
        : base()
    {
    }

    public EmptyAnswerFromModemException(string message)
        : base(message)
    {
    }

    public EmptyAnswerFromModemException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

}
