using System.Net;
using MlffSiteService.Interface;
using MlffSiteService.Models;
using MlffSiteService.Models.Exceptions;
using Nager.TcpClient;
using ENV_KEY = MlffSiteService.Models.Constants.EnvironmentVariables;
using Timer = System.Timers.Timer;

namespace MlffSiteService.Services;

#pragma warning disable CS8618
#pragma warning disable CS8602
public class SpeedRadarService : ISpeedRadarService
{
    private readonly CancellationTokenSource _cancellationTokenSource;

    private readonly ILogger<SpeedRadarService> _logger;
    private TcpClient _tcpClient;

    private int _retryConnectionAttempt;

    public SpeedRadarServiceConfig Config { get; }
    
    public SpeedRadarService(ILogger<SpeedRadarService> logger)
    {
        _logger = logger;
        _cancellationTokenSource = new CancellationTokenSource();
        Config = new SpeedRadarServiceConfig();

        if (Config.IsEnable)
        {
            InitTcpClient();
        }
    }

    private void InitTcpClient()
    {
        _tcpClient = new TcpClient();
        _tcpClient.Connected += OnTcpClientConnected;
        _tcpClient.Disconnected += OnTcpClientDisconnected;
        _tcpClient.DataReceived += OnTcpClientDataReceived;
    }

    public event Action<ISpeedDetectionResult>? OnSpeedDetected;


    public async Task StartAsync()
    {
        if (!Config.IsEnable)
        {
            _logger.LogInformation("Speed radar service disabled.");
            return;
        }

        try
        {
            if (_tcpClient.IsConnected)
            {
                return;
            }


            var isConnected = await _tcpClient.ConnectAsync(Config.SpeedRadarTcpServerIp.ToString(), Config.SpeedRadarTcpServerPort,

                _cancellationTokenSource.Token);

            if (!isConnected)
            {
                OnTcpClientDisconnected();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }

    public void Dispose()
    {
        if (!Config.IsEnable)
        {
            return;
        }

        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _tcpClient.Dispose();
    }

    private void OnTcpClientConnected()
    {
        _retryConnectionAttempt = 0;
        _logger.LogInformation("Connected to speed radar TCP server.");
    }

    private void OnTcpClientDisconnected()
    {
        if (_retryConnectionAttempt == 0)
        {
            _logger.LogError("Speed radar TCP server disconnected.");
        }

        StartDelayedRetry();
    }

    private void OnTcpClientDataReceived(byte[] content)
    {
        var timestamp = DateTime.Now;

        // speed data always will have 1 byte data only
        if (content.Length == 1)
        {
            var speed = content[0];
            // speed data
            _logger.LogInformation("Radar > \tDetection Date: {detectionDate}\n" +
                                   "\t\tSpeed: {speed} km/h",
                timestamp.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                speed);

            OnSpeedDetected?.Invoke(new SpeedDetectionResult(timestamp, speed));
        }
        else
        {
            _logger.LogDebug($"Unknown content: {string.Join(",", content.Select(t => t.ToString("X")))}");
        }
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
                _logger.LogWarning("Retry attempt #{attemptCount}. Retry connection to TCP server.", _retryConnectionAttempt);
            }
            else if (_retryConnectionAttempt == 5)
            {
                _logger.LogWarning("Retry attempt #{attemptCount}. Retry connection to TCP server. Reached max logging count.", _retryConnectionAttempt);
            }

            await StartAsync();
        };

        timer.Interval = TimeSpan.FromSeconds(10).TotalMilliseconds;
        timer.Start();
    }
}

public class SpeedRadarServiceConfig
{
    public SpeedRadarServiceConfig()
    {
        IsEnable = bool.Parse(Environment.GetEnvironmentVariable(ENV_KEY.ENABLE_SPEED_RADAR_SERVICE) ??
                              throw new MissingEnvironmentVariableException(ENV_KEY.ENABLE_SPEED_RADAR_SERVICE));

        if (IsEnable)
        {
            SpeedRadarTcpServerIp = IPAddress.Parse(Environment.GetEnvironmentVariable(ENV_KEY.SPEED_RADAR_IP) ??
                                                    throw new MissingEnvironmentVariableException(ENV_KEY.SPEED_RADAR_IP));

            SpeedRadarTcpServerPort = ushort.Parse(Environment.GetEnvironmentVariable(ENV_KEY.SPEED_RADAR_PORT) ??
                                                   throw new MissingEnvironmentVariableException(ENV_KEY.SPEED_RADAR_PORT));
        }
    }

    public bool IsEnable { get; }

    public IPAddress? SpeedRadarTcpServerIp { get; }

    public ushort SpeedRadarTcpServerPort { get; }
}