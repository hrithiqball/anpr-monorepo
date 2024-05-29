namespace MlffSiteService.Models.Exceptions;

public class MissingEnvironmentVariableException : Exception
{
    public MissingEnvironmentVariableException(string key)
    {
        Key = key;
    }

    public string Key { get; }
}