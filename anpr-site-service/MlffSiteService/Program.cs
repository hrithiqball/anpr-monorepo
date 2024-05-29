using System.Net;
using MlffSiteService.Interface;
using MlffSiteService.Models;
using MlffSiteService.Services;
using ENV_KEY = MlffSiteService.Models.Constants.EnvironmentVariables;

namespace MlffSiteService;

public class Program
{
    private static ILogger? _logger;
    private static IHostApplicationLifetime? _hostApplicationLifetime;

    private static readonly IDictionary<string, string> EnvironmentVariables = new Dictionary<string, string>();

    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddLogging(options =>
                {
                    options.ClearProviders();
                    options.AddSimpleConsole(c =>
                    {
                        c.TimestampFormat = "[dd/MM/yyyy HH:mm:ss.fff] ";
                    });

                    var isLogDebug =
                        Environment.GetEnvironmentVariable(ENV_KEY.ENABLE_DEBUG_LOG)?.ToLower() is ("true" or "1");

                    options.SetMinimumLevel(isLogDebug ? LogLevel.Debug : LogLevel.Information);
                });

                services.AddSingleton<IAnprService, AnprService>();
                services.AddSingleton<ISpeedRadarService, SpeedRadarService>();
                services.AddSingleton<IRfidReaderService, RfidReaderService>();
                services.AddSingleton<IMasterModeratorService, MasterModeratorService>();
                services.AddSingleton<IMlffWebApiManager, MlffWebApiManager>();
                services.AddHttpClient<MlffWebApiManager>();

                services.AddMemoryCache();
            })
            .Build();

        _logger = host?.Services?.GetService<ILogger<Program>>();
        _hostApplicationLifetime = host?.Services?.GetService<IHostApplicationLifetime>();

        if (host is null)
        {
            _logger?.LogError("Failed to build host");
            return;
        }

        // Check all the necessary environment variables
        if (!CheckNecessaryEnvironmentVariables())
        {
            _logger?.LogCritical("Errors in environment variables configuration, please check again the settings.");
            return;
        }

        InitApplicationLifetimeHandlers(host);
        await host.RunAsync();
    }


    private static void InitApplicationLifetimeHandlers(IHost host)
    {
        _hostApplicationLifetime?.ApplicationStarted.Register(() =>
        {
            StartBackgroundServices(host).ConfigureAwait(true);
        });

        _hostApplicationLifetime?.ApplicationStopped.Register(() =>
        {
            DisposeBackgroundServices(host);
        });
    }


    private static async Task StartBackgroundServices(IHost host)
    {
        var masterModeratorService = host.Services.GetService<IMasterModeratorService>();

        if (masterModeratorService is null)
        {
            return;
        }

        await masterModeratorService.StartAsync();
    }

    private static void DisposeBackgroundServices(IHost host)
    {
        var masterModeratorService = host.Services.GetService<IMasterModeratorService>();
        masterModeratorService?.Dispose();
    }

    private static bool CheckNecessaryEnvironmentVariables()
    {
        IList<string> errorMessages = new List<string>();
        var isAllValid = true;

        #region general

        CheckEnvironmentVariableExistence(ENV_KEY.ENABLE_DEBUG_LOG, ref errorMessages, ref isAllValid);
        CheckEnvironmentVariableExistence(ENV_KEY.SITE_ID, ref errorMessages, ref isAllValid);
        CheckEnvironmentVariableExistence(ENV_KEY.MLFF_WEB_API_BASE_URL, ref errorMessages, ref isAllValid, out var mlffWebApiBaseUrl);

        if (!Uri.TryCreate(mlffWebApiBaseUrl, UriKind.RelativeOrAbsolute, out _))
        {
            errorMessages.Add($"{ENV_KEY.MLFF_WEB_API_BASE_URL} is not a valid URI.");
            isAllValid = false;
        }

        #endregion

        #region anpr

        CheckEnvironmentVariableExistence(ENV_KEY.ENABLE_ANPR_SERVICE, ref errorMessages, ref isAllValid, out var isEnableAnpr);
        if (!string.IsNullOrEmpty(isEnableAnpr) && isEnableAnpr is "true" or "1")
        {
            CheckEnvironmentVariableExistence(ENV_KEY.ANPR_ENGINE_TYPE, ref errorMessages, ref isAllValid, out var anprEngineType);
            CheckEnvironmentVariableExistence(ENV_KEY.ANPR_MQTT_SERVER_IP, ref errorMessages, ref isAllValid, out var anprMqttServerIp);
            CheckEnvironmentVariableExistence(ENV_KEY.ANPR_MQTT_SERVER_PORT, ref errorMessages, ref isAllValid, out var anprMqttServerPort);
            CheckEnvironmentVariableExistence(ENV_KEY.ANPR_MQTT_TOPICS_SEPARATED_BY_COMMA, ref errorMessages, ref isAllValid);
            CheckEnvironmentVariableExistence(ENV_KEY.ANPR_IMAGE_NETWORK_PATH, ref errorMessages, ref isAllValid);
            CheckEnvironmentVariableExistence(ENV_KEY.ANPR_IMAGE_DIRECTORY_USERNAME, ref errorMessages, ref isAllValid);
            CheckEnvironmentVariableExistence(ENV_KEY.ANPR_IMAGE_DIRECTORY_PASSWORD, ref errorMessages, ref isAllValid);

            switch (anprEngineType)
            {
                default:
                    errorMessages.Add($"Invalid {ENV_KEY.ANPR_ENGINE_TYPE} option. Valid options are RECOANPR.");
                    isAllValid = false;
                    break;
                case Constants.AnprEngineTypes.RECOANPR:
                    break;
            }

            if (!string.IsNullOrEmpty(anprMqttServerIp) && !IPAddress.TryParse(anprMqttServerIp, out _))
            {
                errorMessages.Add($"{ENV_KEY.ANPR_MQTT_SERVER_IP} is not a valid IP address.");
                isAllValid = false;
            }

            if (!string.IsNullOrEmpty(anprMqttServerPort) &&
                (!ushort.TryParse(anprMqttServerPort, out var parsedAnprMqttServerPort) ||
                 parsedAnprMqttServerPort is not (> ushort.MinValue and < ushort.MaxValue)))
            {
                errorMessages.Add($"{ENV_KEY.ANPR_MQTT_SERVER_PORT} is not a valid port number.");
                isAllValid = false;
            }
        }

        #endregion

        #region speed radar

        CheckEnvironmentVariableExistence(ENV_KEY.ENABLE_SPEED_RADAR_SERVICE, ref errorMessages, ref isAllValid, out var isEnableSpeedRadar);
        if (!string.IsNullOrEmpty(isEnableSpeedRadar) && isEnableSpeedRadar is "true" or "1")
        {
            CheckEnvironmentVariableExistence(ENV_KEY.SPEED_RADAR_IP, ref errorMessages, ref isAllValid, out var speedRadarTcpServerIp);
            CheckEnvironmentVariableExistence(ENV_KEY.SPEED_RADAR_PORT, ref errorMessages, ref isAllValid, out var speedRadarTcpServerPort);

            if (!string.IsNullOrEmpty(speedRadarTcpServerIp) && !IPAddress.TryParse(speedRadarTcpServerIp, out _))
            {
                errorMessages.Add($"{ENV_KEY.SPEED_RADAR_IP} is not a valid IP address.");
                isAllValid = false;
            }

            if (!string.IsNullOrEmpty(speedRadarTcpServerPort) &&
                (!ushort.TryParse(speedRadarTcpServerPort, out var parsedSpeedRadarTcpServerPort) ||
                 parsedSpeedRadarTcpServerPort is not (> ushort.MinValue and < ushort.MaxValue)))
            {
                errorMessages.Add($"{ENV_KEY.SPEED_RADAR_PORT} not a valid port number.");
                isAllValid = false;
            }
        }

        #endregion

        #region rfid

        CheckEnvironmentVariableExistence(ENV_KEY.ENABLE_RFID_SERVICE, ref errorMessages, ref isAllValid, out var isEnableRfid);
        if (!string.IsNullOrEmpty(isEnableRfid) && isEnableRfid is "true" or "1")
        {
            CheckEnvironmentVariableExistence(ENV_KEY.RFID_READER_IP, ref errorMessages, ref isAllValid, out var rfidReaderIp);
            if (!string.IsNullOrEmpty(rfidReaderIp) && !IPAddress.TryParse(rfidReaderIp, out _))
            {
                errorMessages.Add($"{ENV_KEY.RFID_READER_IP} is not a valid IP address.");
                isAllValid = false;
            }
        }

        #endregion

        if (!isAllValid)
        {
            _logger?.LogCritical("\t{errorMessages}", string.Join("\n\t", errorMessages));
        }
        else
        {
            var x = EnvironmentVariables.Select(t => $"{t.Key}={t.Value}");
            _logger?.LogInformation("\t{environmentVariables}", string.Join("\n\t", x));
        }

        return isAllValid;
    }

    private static void CheckEnvironmentVariableExistence(string key,
        ref IList<string> errorMessages,
        ref bool isAllValid,
        out string value)
    {
        value = Environment.GetEnvironmentVariable(key) ?? string.Empty;
        if (!string.IsNullOrEmpty(value))
        {
            EnvironmentVariables.Add(key, value);
            return;
        }

        errorMessages.Add($"Missing environment variable {key}");
        isAllValid = false;
    }

    private static void CheckEnvironmentVariableExistence(string key,
        ref IList<string> errorMessages,
        ref bool isAllValid)
    {
        CheckEnvironmentVariableExistence(key, ref errorMessages, ref isAllValid, out _);
    }
}