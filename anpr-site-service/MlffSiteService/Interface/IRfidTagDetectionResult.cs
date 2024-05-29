namespace MlffSiteService.Interface;

public interface IRfidTagDetectionResult
{
    DateTime Timestamp { get; }

    string TagId { get; }

    ushort Antenna { get; }
}