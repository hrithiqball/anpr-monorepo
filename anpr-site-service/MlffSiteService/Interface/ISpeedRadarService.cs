using MlffSiteService.Services;

namespace MlffSiteService.Interface;

public interface ISpeedRadarService : IDisposable
{
    public SpeedRadarServiceConfig Config { get; }

    Task StartAsync();

    event Action<ISpeedDetectionResult> OnSpeedDetected;
}