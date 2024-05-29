using MlffWebApi.Interfaces.ANPR;
using MlffWebApi.Interfaces.Hubs;
using MlffWebApi.Interfaces.RFID;
using MlffWebApi.Interfaces.Speed;
using Microsoft.AspNetCore.SignalR;
using MlffWebApi.Interfaces;
using MlffWebApi.Interfaces.PublicIP;

namespace MlffWebApi.Hubs;

public class DetectionHub : Hub<IDetectionHub>
{
    private readonly ILogger<DetectionHub> _logger;
    private IHubContext<DetectionHub, IDetectionHub> _context;

    public DetectionHub(ILogger<DetectionHub> logger, IHubContext<DetectionHub, IDetectionHub> context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task BroadcastLicensePlateDetected(ILicensePlateRecognitionLite lpr,
        CancellationToken cancellationToken)
    {
        var message = Newtonsoft.Json.JsonConvert.SerializeObject(lpr);
        try
        {
            await _context.Clients?.All?.LicensePlateDetected(lpr);
            _logger.LogInformation($"{nameof(IDetectionHub.LicensePlateDetected)}> Message: {message}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }

    public async Task BroadcastSpeedDetected(ISpeedDetectionLite speed,
        CancellationToken cancellationToken)
    {
        var message = Newtonsoft.Json.JsonConvert.SerializeObject(speed);
        try
        {
            await _context.Clients?.All?.SpeedDetected(speed);
            _logger.LogInformation($"{nameof(IDetectionHub.SpeedDetected)}> Message: {message}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }

    public async Task BroadcastRfidTagDetected(IRfidTagDetectionLite rfidTag,
        CancellationToken cancellationToken)
    {
        var message = Newtonsoft.Json.JsonConvert.SerializeObject(rfidTag);
        try
        {
            await _context.Clients?.All?.RfidTagDetected(rfidTag);
            _logger.LogInformation($"{nameof(IDetectionHub.RfidTagDetected)}> Message: {message}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }

    public async Task BroadcastDetectionMatched(IDetectionMatchLite match,
        CancellationToken cancellationToken)
    {
        var message = Newtonsoft.Json.JsonConvert.SerializeObject(match);
        try
        {
            await _context.Clients?.All?.DetectionMatched(match);
            _logger.LogInformation($"{nameof(IDetectionHub.DetectionMatched)}> Message: {message}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }

    public async Task BroadcastPublicIP(IPublicIPRecognition publicIP, CancellationToken cancellationToken)
    {
        var message = Newtonsoft.Json.JsonConvert.SerializeObject(publicIP);
        try
        {
            await _context.Clients?.All?.PublicIPRetrieved(publicIP);
            _logger.LogCritical($"{nameof(IDetectionHub.PublicIPRetrieved)}> Message: {message}");
        } catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation($"SignalR client connected. Connection id: {Context.ConnectionId}");
        return base.OnConnectedAsync();
    }
}