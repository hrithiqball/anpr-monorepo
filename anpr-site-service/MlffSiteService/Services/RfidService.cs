using System.Net;
using Impinj.OctaneSdk;
using MlffSiteService.Interface;
using MlffSiteService.Models;
using MlffSiteService.Models.Exceptions;
using ENV_KEY = MlffSiteService.Models.Constants.EnvironmentVariables;
using Timer = System.Timers.Timer;

namespace MlffSiteService.Services;

#pragma warning disable CS8618
#pragma warning disable CS8602
public class RfidReaderService : IRfidReaderService
{
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly ILogger<RfidReaderService> _logger;

    private ImpinjReader _rfidReader;
    private int _retryConnectionAttempt;
    public bool _readerModeValue;

    public RfidReaderServiceConfig Config { get; }

    public RfidReaderService(ILogger<RfidReaderService> logger)
    {
        _logger = logger;
        _cancellationTokenSource = new CancellationTokenSource();
        Config = new RfidReaderServiceConfig();

        if (Config.IsEnable)
        {
            InitRfidReader();
        }
    }

    private void InitRfidReader()
    {
        _rfidReader = new ImpinjReader(Config.ReaderIP.ToString(), "RFID Reader");
        _rfidReader.ConnectAsyncComplete += OnRfidReaderConnectComplete;
        _rfidReader.ConnectionLost += OnRfidReaderDisconnected;
        _rfidReader.TagsReported += OnRfidTagDetected;
    }

    public event Action<IList<RfidTagDetectionResult>>? OnRfidTagDetectionResult;
    
    public Task StartAsync()
    {
        if (!Config.IsEnable)
        {
            _logger.LogInformation("RFID reader service disabled.");
            return Task.CompletedTask;
        }

        try
        {
            _rfidReader.ConnectAsync();
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return Task.FromException(e);
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource.Dispose();
        _rfidReader.Stop();
        _rfidReader.Disconnect();
    }


    private void OnRfidReaderConnectComplete(ImpinjReader reader,
        ConnectAsyncResult result,
        string errormessage)
    {
        // Impinj SDK fire this event after connect action complete.
        // So, we need to check is the reader connected

        if (!_rfidReader.IsConnected)
        {
            OnRfidReaderDisconnected(reader);
            return;
        }

        _retryConnectionAttempt = 0;
        _logger.LogInformation("Connected to RFID reader.");

        //var antennas = _rfidReader.QuerySettings();
        //Settings settingRfid = _rfidReader.QueryDefaultSettings();

        var settings = _rfidReader.QueryDefaultSettings();
        settings.AutoStart.Mode = AutoStartMode.Immediate;
        settings.AutoStop.Mode = AutoStopMode.None;
        settings.Report.IncludeFirstSeenTime = true;
        settings.Report.IncludeLastSeenTime = true;
        settings.Report.IncludeSeenCount = true;
        settings.Report.IncludeAntennaPortNumber = true;
        settings.Report.IncludeFastId = true;
        settings.Report.IncludeChannel = true;
        settings.Report.IncludeCrc = true;
        settings.Report.IncludeDopplerFrequency = true;
        settings.Report.IncludeGpsCoordinates = true;
        settings.Report.IncludeTxPower = true;
        settings.Report.IncludePeakRssi = true;
        settings.Keepalives.Enabled = true;
        settings.Keepalives.PeriodInMs = 5000;
        settings.Keepalives.LinkDownThreshold = 5;
        settings.SearchMode = SearchMode.TagFocus;
        settings.RfMode = Config.ReaderMode;
        settings.Session = 1;
        _rfidReader.ApplySettings(settings);
    }

    private void OnRfidReaderDisconnected(ImpinjReader reader)
    {
        if (_retryConnectionAttempt == 0)
        {
            _logger.LogError("RFID reader disconnected.");
        }

        StartDelayedRetry();
    }

    private void OnRfidTagDetected(ImpinjReader reader,
        TagReport report)
    {
        var timestamp = DateTime.Now;
        IList<RfidTagDetectionResult> detectionResults = new List<RfidTagDetectionResult>();

        foreach (var tag in report.Tags)
        {
            _logger.LogInformation("RFID >\tDetection date: {detectionDate}\n" +
                                   "\t\tAntenna Port: {port}\n" +
                                   "\t\tTag ID: {tagId}\n" +
                                   "\t\tEPC: {epc}",
                timestamp.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                tag.AntennaPortNumber,
                tag.Tid,
                tag.Epc
            );
            
            detectionResults.Add(new RfidTagDetectionResult(timestamp, tag.Tid.ToString(), tag.AntennaPortNumber, tag.PeakRssiInDbm));
        }

        OnRfidTagDetectionResult?.Invoke(detectionResults);
    }

    private void StartDelayedRetry()
    {
        var timer = new Timer();
        timer.AutoReset = false;
        timer.Elapsed += async (sender,
            args) =>
        {
            _retryConnectionAttempt++;
            if (_retryConnectionAttempt < 5)
            {
                _logger.LogWarning("Retry attempt #{attemptCount}. Retry connection to RFID reader.", _retryConnectionAttempt);
            }
            else if (_retryConnectionAttempt == 5)
            {
                _logger.LogWarning("Retry attempt #{attemptCount}. Retry connection to RFID reader. Reached max logging count.",
                    _retryConnectionAttempt);
            }

            await StartAsync();
        };

        timer.Interval = TimeSpan.FromSeconds(10).TotalMilliseconds;
        timer.Start();
    }
}

public class RfidReaderServiceConfig
{
    public RfidReaderServiceConfig()
    {
        IsEnable = bool.Parse(Environment.GetEnvironmentVariable(ENV_KEY.ENABLE_RFID_SERVICE) ??
                              throw new MissingEnvironmentVariableException(ENV_KEY.ENABLE_ANPR_SERVICE));

        if (!IsEnable)
        {
            return;
        }

        ReaderIP = IPAddress.Parse(Environment.GetEnvironmentVariable(ENV_KEY.RFID_READER_IP) ??
                                   throw new MissingEnvironmentVariableException(ENV_KEY.RFID_READER_IP));

        ReaderMode = uint.Parse(Environment.GetEnvironmentVariable(ENV_KEY.READER_MODE) ?? "4");
    }

    public bool IsEnable { get; }

    public IPAddress? ReaderIP { get; }

    public uint ReaderMode { get; }

}