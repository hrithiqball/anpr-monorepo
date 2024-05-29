namespace MlffWebApi;

internal static class Constants
{
    internal const string USE_AZURE_SIGNAL_R_SERVICE = "USE_AZURE_SIGNAL_R_SERVICE"; // optional, default false
    internal const string SIGNALR_CONNECTION_STRING = "SIGNALR_CONNECTION_STRING"; // conditional required
    internal const string DB_CONNECTION_STRING = "DB_CONNECTION_STRING"; // required
    internal const string IMAGE_OUTPUT_DIRECTORY = "IMAGE_OUTPUT_DIRECTORY"; // required
    internal const string ENABLE_SWAGGER_UI = "ENABLE_SWAGGER_UI"; // optional, default true
    internal const string USE_HTTPS_REDIRECTION = "USE_HTTPS_REDIRECTION"; // optional, default false
    internal const string ENABLE_POST_FULL_IMAGE = "ENABLE_POST_FULL_IMAGE";
    internal const string ENABLE_POST_LICENSE_PLATE = "ENABLE_POST_LICENSE_PLATE";
}