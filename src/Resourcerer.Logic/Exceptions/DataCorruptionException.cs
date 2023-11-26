namespace Resourcerer.Logic.Exceptions;

public sealed class DataCorruptionException : Exception
{
    public DataCorruptionException() : base("Data corrupted")
    {
    }

    public DataCorruptionException(string message)
        : base(message)
    {
    }

    public DataCorruptionException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
