using MlffSiteService.Interface;

namespace MlffSiteService.Models;

public class RfidTagDetectionResult : IRfidTagDetectionResult
{
    public RfidTagDetectionResult(DateTime timestamp,
        string tagId, ushort antenna, double rssi)
    {
        Timestamp = timestamp;
        TagId = tagId;
        Antenna = antenna;
        Rssi = rssi;
    }

    public DateTime Timestamp { get; }

    public string TagId { get; }

    public ushort Antenna { get; }

    public double Rssi { get; }
}