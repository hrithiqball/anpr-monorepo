using MlffSiteService.Interface;

namespace MlffSiteService.Models;

public class SpeedDetectionResult : ISpeedDetectionResult
{
    public SpeedDetectionResult(DateTime timestamp,
        int speed)
    {
        Timestamp = timestamp;
        Speed = speed;
    }

    public DateTime Timestamp { get; }

    public int Speed { get; }
}