namespace MlffSiteService.Models.Exceptions;

public class JsonDeserializeException : Exception
{
    public JsonDeserializeException(Type type,
        string input)
    {
        Type = type;
        Input = input;
    }

    public Type Type { get; }

    public string Input { get; }

    public override string Message => $"Unable to deserialize to {Type}.\tContent: {Input}";
}