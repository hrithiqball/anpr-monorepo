namespace MlffWebApi.Exceptions;

public class PaginationOutOfRangeException : Exception
{
    public override string Message { get; }

    public PaginationOutOfRangeException(string message)
    {
        Message = message;
    }
}