namespace MlffWebApi.Interfaces.Speed;

public interface ISpeedDetectionLite
{
    string SiteId { get; set; }
    public decimal Speed { get; set; }
    public DateTime DetectionDate { get; set; }
}

public interface ISpeedDetection : ISpeedDetectionLite
{
    Guid Uid { get; set; }
    DateTime CreatedDate { get; set; }
}