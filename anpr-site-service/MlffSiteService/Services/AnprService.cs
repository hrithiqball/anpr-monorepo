using System.Net;
using CliWrap;
using MlffSiteService.Extensions;
using MlffSiteService.Interface;
using MlffSiteService.Models;
using MlffSiteService.Models.Exceptions;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Exceptions;
using MQTTnet.Packets;
using ENV_KEY = MlffSiteService.Models.Constants.EnvironmentVariables;
using Timer = System.Timers.Timer;

namespace MlffSiteService.Services;

#pragma warning disable CS8618
#pragma warning disable CS8602
public class AnprService : IAnprService
{
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly ILogger<AnprService> _logger;

    private IMqttClient _mqttClient;
    private MqttClientOptions _mqttClientOptions;
    private int _retryConnectionAttempt;

    public AnprServiceConfig Config { get; }

    public AnprService(ILogger<AnprService> logger)
    {
        _logger = logger;
        _cancellationTokenSource = new CancellationTokenSource();
        Config = new AnprServiceConfig();

        if (Config.IsEnable)
        {
            InitMqttClient();
        }
    }

    private void InitMqttClient()
    {
        if (Config.MqttServerIp == null)
        {
            throw new MissingEnvironmentVariableException(Constants.EnvironmentVariables.ANPR_MQTT_SERVER_IP);
        }

        var mqttFactory = new MqttFactory();
        _mqttClient = mqttFactory.CreateMqttClient();
        _mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(Config.MqttServerIp.ToString(), Config.MqttServerPort)
            .WithTimeout(TimeSpan.FromSeconds(5))
            .Build();

        _mqttClient.ConnectedAsync += MqttClientOnConnectedAsync;
        _mqttClient.ApplicationMessageReceivedAsync += MqttClientOnApplicationMessageReceivedAsync;
        _mqttClient.DisconnectedAsync += MqttClientOnDisconnectAsync;
    }

    public event Action<IAnprDetectionResult>? OnAnprDetectionResult;

    public void Dispose()
    {
        if (!Config.IsEnable)
        {
            return;
        }

        _cancellationTokenSource.Dispose();
        _mqttClient.Dispose();
    }

    public async Task StartAsync()
    {
        if (!Config.IsEnable)
        {
            _logger.LogInformation("ANPR service disabled.");
            return;
        }

        try
        {
            if (_mqttClient is null)
            {
                throw new NullReferenceException("Unable to initiate MQTT client");
            }

            if (_mqttClient.IsConnected)
            {
                _logger.LogCritical("mqttClient Connected");
                return;
            }
            
            // is not connected
            await _mqttClient.ConnectAsync(_mqttClientOptions, _cancellationTokenSource.Token);
        }
        catch (MqttCommunicationException)
        {
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }

    private async Task MqttClientOnConnectedAsync(MqttClientConnectedEventArgs arg)
    {
        _retryConnectionAttempt = 0;
        _logger.LogInformation("Connected to MQTT broker.");
        var _ = await TryMountAnprImageNetworkDirectory();
        var subscribeTasks = new List<Task>();
        foreach (var topic in Config.MqttTopicList)
        {
            var subscribeTask = _mqttClient.SubscribeAsync(new MqttTopicFilter { Topic = topic }, _cancellationTokenSource.Token);
            subscribeTasks.Add(subscribeTask);
        }

        Task.WaitAll(subscribeTasks.ToArray());
    }

    private Task MqttClientOnDisconnectAsync(MqttClientDisconnectedEventArgs arg)
    {
        if (_retryConnectionAttempt == 0)
        {
            _logger.LogError("MQTT broker disconnected.");
        }

        StartDelayedRetry();
        return Task.CompletedTask;
    }

    private Task MqttClientOnApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        try
        {
            var payload = arg.ApplicationMessage.ConvertPayloadToString();

            _logger.LogDebug("Message payload: {payload}", payload);

            if (string.IsNullOrEmpty(payload))
            {
                _logger.LogWarning("MQTT message payload is empty");
                return Task.FromException<Exception>(new Exception("MQTT message payload is empty."));
            }

            // dispatch message to its subscriber
            var anprEngineType = Environment.GetEnvironmentVariable(ENV_KEY.ANPR_ENGINE_TYPE);
            if (anprEngineType == Constants.AnprEngineTypes.RECOANPR)
            {
                HandleRecoAnprApplicationMessage(payload);
            }
            else
            {
                return Task.FromException(
                    new NotImplementedException($"Handler for {anprEngineType} is not implemented."));
            }

            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            return Task.FromException<Exception>(e);
        }
    }

    private async Task<bool> TryMountAnprImageNetworkDirectory()
    {
        try
        {
            var uncPath = Config.ImageNetworkDirectoryPath;
            var username = Config.ImageNetworkDirectoryUsername;
            var password = Config.ImageNetworkDirectoryPassword;

            // try to mount the network drive
            if (OperatingSystem.IsWindows())
            {
                uncPath = Path.GetFullPath(uncPath);

                var isMounted = IsNetworkDriveMounted(uncPath, string.Empty, out var mountedPath);

                if (isMounted)
                {
                    _logger.LogInformation("Mounted {networkDrive} to {mountedPath}", uncPath, mountedPath);
                    Environment.SetEnvironmentVariable(ENV_KEY.ANPR_IMAGE_MOUNTED_NETWORK_PATH, mountedPath);
                    return true;
                }


                var cli = Cli.Wrap("net")
                    .WithArguments(args =>
                        args.Add("use")
                            .Add(uncPath)
                            .Add($"/user:{username}")
                            .Add(password)
                    );

                _logger.LogInformation("Try mounting network drive with net use. Command: {command}", cli.ToString());

                var commandResult = await cli.ExecuteAsync();

                if (commandResult.ExitCode != 0)
                {
                    _logger.LogError("Unable to mount network drive. Command: {Command}", cli.ToString());
                    return false;
                }

                isMounted = IsNetworkDriveMounted(uncPath, string.Empty, out mountedPath);

                if (isMounted)
                {
                    _logger.LogInformation("Mounted {networkDrive} to {mountedPath}", uncPath, mountedPath);
                    Environment.SetEnvironmentVariable(ENV_KEY.ANPR_IMAGE_MOUNTED_NETWORK_PATH, mountedPath);
                    return true;
                }

                _logger.LogCritical("Command executed but still unable to mount {networkDrive}", uncPath);
                return false;
            }

            if (OperatingSystem.IsLinux())
            {
                const string virtualDirectory = "/mnt/anpr-image";
                Directory.CreateDirectory(virtualDirectory);

                var isMounted = IsNetworkDriveMounted(uncPath, virtualDirectory, out var mountedPath);

                if (isMounted)
                {
                    _logger.LogInformation("Mounted {networkDrive} to {mountedPath}", uncPath, mountedPath);
                    Environment.SetEnvironmentVariable(ENV_KEY.ANPR_IMAGE_MOUNTED_NETWORK_PATH, mountedPath);
                    return true;
                }

                var command = $@"mount.cifs {uncPath} {virtualDirectory} -o user={username},pass={password}";
                var cli = Cli.Wrap("/bin/bash")
                    .WithArguments("-c \" " + command + " \"");

                var commandResult = await cli.ExecuteAsync();

                if (commandResult.ExitCode != 0)
                {
                    _logger.LogError("Unable to mount network drive. Command: {Command}", command);
                    return false;
                }

                _logger.LogInformation("Try mounting network drive with mount.cifs. Command: {Command}", command);

                // await process.WaitForExitAsync();

                isMounted = IsNetworkDriveMounted(uncPath, virtualDirectory, out mountedPath);

                if (isMounted)
                {
                    _logger.LogInformation("Mounted {networkDrive} to {mountedPath}", uncPath, mountedPath);
                    Environment.SetEnvironmentVariable(ENV_KEY.ANPR_IMAGE_MOUNTED_NETWORK_PATH, mountedPath);
                    return true;
                }

                _logger.LogCritical("Command executed but still unable to mount {networkDrive}", uncPath);
                return false;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return false;
        }

        return false;
    }

    private bool IsNetworkDriveMounted(string networkPath,
        string virtualDirectory,
        out string mountedPath)
    {
        if (OperatingSystem.IsLinux())
        {
            var isMounted = Directory.Exists(virtualDirectory) &&
                            (Directory.GetDirectories(virtualDirectory).Length > 0 || Directory.GetFiles(virtualDirectory).Length > 0);

            mountedPath = isMounted ? virtualDirectory : string.Empty;
            _logger.LogCritical("Mounted path: " + mountedPath);
            return isMounted;
        }

        if (OperatingSystem.IsWindows())
        {
            var isMounted = Directory.Exists(networkPath);
            mountedPath = isMounted ? Path.GetFullPath(networkPath) : string.Empty;
            return isMounted;
        }

        mountedPath = string.Empty;
        return false;
    }

    private void HandleRecoAnprApplicationMessage(string payload)
    {
        var anpr = payload.Deserialize<AnprDetectionResultRecoAnpr>();

        if (anpr == null)
        {
            throw new InvalidOperationException(
                $"Unable to deserialize payload to {typeof(AnprDetectionResultRecoAnpr)}");
        }

        _logger.LogInformation("ANPR > \tDetection date: {timestamp}\n" +
                               "\t\tPlate number: {plateNumber}\n" +
                               "\t\tConfidence level: {confidenceLevel}\n" +
                               "\t\tProcess time: {processTime}\n" +
                               "\t\tVehicle image: {vehicleImage}\n" +
                               "\t\tPlate image: {plateImage}",
            anpr.Timestamp.ToString("dd/MM/yyyy HH:mm:ss.fff"),
            anpr.PlateNumber,
            anpr.Confidence,
            anpr.ProcessTime,
            anpr.VehicleImagePath,
            anpr.PlateImagePath);

        OnAnprDetectionResult?.Invoke(anpr);
    }

    private void StartDelayedRetry()
    {
        var timer = new Timer();
        timer.AutoReset = false;
        timer.Elapsed += async (_,
            _) =>
        {
            _retryConnectionAttempt++;
            if (_retryConnectionAttempt < 5)
            {
                _logger.LogWarning("Retry attempt #{attemptCount}. Retry connection to MQTT broker.", _retryConnectionAttempt);
            }
            else if (_retryConnectionAttempt == 5)
            {
                _logger.LogWarning("Retry attempt #{attemptCount}. Retry connection to MQTT broker. Reached max logging count.",
                    _retryConnectionAttempt);
            }

            await StartAsync();
        };

        timer.Interval = TimeSpan.FromSeconds(10).TotalMilliseconds;
        timer.Start();
    }
}

public class AnprServiceConfig
{
    public AnprServiceConfig()
    {
        IsEnable = bool.Parse(Environment.GetEnvironmentVariable(ENV_KEY.ENABLE_ANPR_SERVICE) ??
                              throw new MissingEnvironmentVariableException(ENV_KEY.ENABLE_ANPR_SERVICE));

        if (IsEnable)
        {
            AnprEngineType = Environment.GetEnvironmentVariable(ENV_KEY.ANPR_ENGINE_TYPE) ??
                             throw new MissingEnvironmentVariableException(ENV_KEY.ANPR_ENGINE_TYPE);

            MqttServerIp = IPAddress.Parse(Environment.GetEnvironmentVariable(ENV_KEY.ANPR_MQTT_SERVER_IP) ??
                                           throw new MissingEnvironmentVariableException(ENV_KEY.ANPR_MQTT_SERVER_IP));

            MqttServerPort = ushort.Parse(Environment.GetEnvironmentVariable(ENV_KEY.ANPR_MQTT_SERVER_PORT) ??
                                          throw new MissingEnvironmentVariableException(ENV_KEY.ANPR_MQTT_SERVER_PORT));

            var topics = Environment.GetEnvironmentVariable(ENV_KEY.ANPR_MQTT_TOPICS_SEPARATED_BY_COMMA);

            if (string.IsNullOrEmpty(topics))
            {
                throw new MissingEnvironmentVariableException(ENV_KEY.ANPR_MQTT_TOPICS_SEPARATED_BY_COMMA);
            }

            MqttTopicList = topics.Split(new[] { ',', ';' });

            ImageNetworkDirectoryPath = Environment.GetEnvironmentVariable(ENV_KEY.ANPR_IMAGE_NETWORK_PATH) ??
                                        throw new MissingEnvironmentVariableException(ENV_KEY.ANPR_IMAGE_NETWORK_PATH);

            ImageNetworkDirectoryUsername = Environment.GetEnvironmentVariable(ENV_KEY.ANPR_IMAGE_DIRECTORY_USERNAME) ??
                                            throw new MissingEnvironmentVariableException(ENV_KEY.ANPR_IMAGE_DIRECTORY_USERNAME);

            ImageNetworkDirectoryPassword = Environment.GetEnvironmentVariable(ENV_KEY.ANPR_IMAGE_DIRECTORY_PASSWORD) ??
                                            throw new MissingEnvironmentVariableException(ENV_KEY.ANPR_IMAGE_DIRECTORY_PASSWORD);
        }
    }

    public bool IsEnable { get; }

    public string AnprEngineType { get; } = string.Empty;

    public IPAddress? MqttServerIp { get; }

    public ushort MqttServerPort { get; }

    public IList<string> MqttTopicList { get; } = new List<string>();

    public string ImageNetworkDirectoryPath { get; } = string.Empty;

    public string ImageNetworkDirectoryUsername { get; } = string.Empty;

    public string ImageNetworkDirectoryPassword { get; } = string.Empty;

    public string ImageMountedNetworkPath => Environment.GetEnvironmentVariable(ENV_KEY.ANPR_IMAGE_MOUNTED_NETWORK_PATH) ?? string.Empty;
}