namespace MlffWebApi.Interfaces.RFID;

public interface IRfidTagDetectionLite
{
    string SiteId { get; set; }
    string TagId { get; set; }
    DateTime DetectionDate { get; set; }
}

public interface IRfidTagDetection : IRfidTagDetectionLite
{
    Guid Uid { get; set; }
    DateTime CreatedDate { get; set; }
}