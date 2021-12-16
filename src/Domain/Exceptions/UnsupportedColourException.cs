namespace Messaging.Domain.Exceptions;

public class UnsupportedColourException : Exception
{
    public UnsupportedColourException(string code)
        : base($"Colour \"{code}\" is unsupported.")
    {
    }

    public UnsupportedColourException()
    {
    }

    public UnsupportedColourException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
