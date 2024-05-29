namespace MlffWebApi.Extensions;

public static class ConfigurationExtension
{
    public static string GetConnectionString(this IConfiguration configuration)
    {
        string connectionStr = configuration.GetSection($"ConnectionStrings").GetChildren().FirstOrDefault()?.Value ??
                               string.Empty;
        return connectionStr;
    }

    // public static string GetConnectionString(this IConfiguration configuration, string keyName)
    // {
    //     var section = configuration.GetSection($"ConnectionStrings:{keyName}");
    //
    //     if (!section.GetChildren()?.Any(t => t.Key == keyName)??true)
    //     {
    //         throw new NullReferenceException($"No connection string with key name {keyName}");
    //     }
    //     
    //     string connectionStr = section.Value;
    //     return connectionStr;
    // }
}