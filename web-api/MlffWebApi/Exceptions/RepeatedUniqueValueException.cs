namespace MlffWebApi.Exceptions;

public class RepeatedUniqueValueException : Exception
{
    public override string Message { get; }

    public RepeatedUniqueValueException(string message)
    {
        Message = message;
    }
}