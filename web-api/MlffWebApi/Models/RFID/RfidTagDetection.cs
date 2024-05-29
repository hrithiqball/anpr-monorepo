using MlffWebApi.Database.DbContexts;
using MlffWebApi.Interfaces.RFID;

namespace MlffWebApi.Models.RFID;

public class RfidTagDetectionLite : IRfidTagDetectionLite
{
    public string SiteId { get; set; }
    public string TagId { get; set; }
    public DateTime DetectionDate { get; set; }
}

public class RfidTagDetection : RfidTagDetectionLite, IRfidTagDetection
{
    public Guid Uid { get; set; }
    public DateTime CreatedDate { get; set; }
}

public static class RfidTagDetectionExtension
{
    public static IRfidTagDetection ToBusinessModel(this rfid_detection tag)
    {
        return new RfidTagDetection
        {
            Uid = tag.uid,
            TagId = tag.tag_id,
            SiteId = tag.site_id,
            DetectionDate = tag.date_detection,
            CreatedDate = tag.date_created
        };
    }

    public static IRfidTagDetectionLite ToLite(this IRfidTagDetection tag)
    {
        return new RfidTagDetectionLite
        {
            SiteId = tag.SiteId,
            TagId = tag.TagId,
            DetectionDate = tag.DetectionDate
        };
    }

    public static rfid_detection ToDatabaseModel(this IRfidTagDetection tag)
    {
        return new rfid_detection
        {
            uid = Guid.NewGuid(),
            tag_id = tag.TagId,
            site_id = tag.SiteId,
            date_detection = tag.DetectionDate,
            date_created = DateTime.Now
        };
    }
}