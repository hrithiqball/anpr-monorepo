namespace MlffSiteService.Interface;

public interface ISpeedDetectionResult
{
    DateTime Timestamp { get; }

    int Speed { get; }
}