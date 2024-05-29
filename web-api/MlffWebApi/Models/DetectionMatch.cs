using MlffWebApi.Database.DbContexts;
using MlffWebApi.Interfaces;

namespace MlffWebApi.Models;

public class DetectionMatch : DetectionMatchLite, IDetectionMatch
{
    public Guid Uid { get; set; }
    public DateTime DateCreated { get; set; }
    public bool Verified { get; set; } = false;
    public bool? Correctness { get; set; }
}

public class DetectionMatchLite : IDetectionMatchLite
{
    public string SiteId { get; set; }
    public string TagId { get; set; }
    public string PlateNumber { get; set; }
    public int? Speed { get; set; }
    public DateTime DateMatched { get; set; }
    public bool IsInsideWatchlist { get; set; } = false;
    public string VehicleImagePath { get; set; }
    public string PlateImagePath { get; set; }
}

public static class DetectionMatchExtension
{
    public static IDetectionMatch ToBusinessModel(this detection_match match)
    {
        return new DetectionMatch
        {
            Uid = match.uid,
            SiteId = match.site_id,
            TagId = match.tag_id,
            Speed = match.speed,
            PlateNumber = match.plate_number,
            DateMatched = match.date_matched,
            DateCreated = match.date_created,
            Verified = match.verified,
            Correctness = match.correctness,
            VehicleImagePath = match.vehicle_image_path,
            PlateImagePath = match.plate_image_path,
        };
    }

    public static IDetectionMatchLite ToLite(this IDetectionMatch match)
    {
        return new DetectionMatchLite
        {
            SiteId = match.SiteId,
            TagId = match.TagId,
            PlateNumber = match.PlateNumber,
            Speed = match.Speed,
            DateMatched = match.DateMatched,
            VehicleImagePath = match.VehicleImagePath,
            PlateImagePath= match.PlateImagePath,
        };
    }

    public static detection_match ToDatabaseModel(this IDetectionMatch match, bool isForUpdate)
    {
        return new detection_match
        {
            uid = isForUpdate ? match.Uid : Guid.NewGuid(),
            site_id = match.SiteId,
            tag_id = string.IsNullOrEmpty(match.TagId) ? null : match.TagId,
            speed = match.Speed,
            plate_number = string.IsNullOrEmpty(match.PlateNumber) ? null : match.PlateNumber,
            date_matched = match.DateMatched,
            date_created = DateTime.Now,
            correctness = match.Correctness,
            verified = match.Verified,
            vehicle_image_path = match.VehicleImagePath,
            plate_image_path = match.PlateImagePath,
        };
    }
}