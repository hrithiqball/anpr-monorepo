using MlffWebApi.Database.DbContexts;
using MlffWebApi.Interfaces.Speed;

namespace MlffWebApi.Models.Speed;

public class SpeedDetectionLite : ISpeedDetectionLite
{
    public string SiteId { get; set; }
    public decimal Speed { get; set; }
    public DateTime DetectionDate { get; set; }
}

public class SpeedDetection : SpeedDetectionLite, ISpeedDetection
{
    public Guid Uid { get; set; }
    public DateTime CreatedDate { get; set; }
}

public static class SpeedDetectionExtension
{
    public static ISpeedDetection ToBusinessModel(this speed_detection tag)
    {
        return new SpeedDetection()
        {
            Uid = tag.uid,
            Speed = tag.speed_kmh,
            SiteId = tag.site_id,
            DetectionDate = tag.date_detection,
            CreatedDate = tag.date_created
        };
    }

    public static ISpeedDetectionLite ToLite(this ISpeedDetection speed)
    {
        return speed;
    }

    public static speed_detection ToDatabaseModel(this ISpeedDetection tag)
    {
        return new speed_detection
        {
            uid = Guid.NewGuid(),
            speed_kmh = tag.Speed,
            site_id = tag.SiteId,
            date_detection = tag.DetectionDate,
            date_created = DateTime.Now
        };
    }
}