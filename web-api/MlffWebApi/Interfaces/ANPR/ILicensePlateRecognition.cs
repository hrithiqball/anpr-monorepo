namespace MlffWebApi.Interfaces.ANPR;

public interface ILicensePlateRecognitionLite
{
    string SiteId { get; set; }
    string CameraId { get; set; }
    string VehicleImagePath { get; set; }
    string PlateImagePath { get; set; }
    string PlateNumber { get; set; }
    DateTime DetectionDate { get; set; }
    bool IsInsideWatchlist { get; set; }
}

public interface ILicensePlateRecognition : ILicensePlateRecognitionLite
{
    Guid Uid { get; set; }
    IBoundingBox BoundingBox { get; set; }
    decimal? ConfidenceOnPlateRecognition { get; set; }
    decimal? ConfidenceOnPlateDetection { get; set; }
    DateTime DateCreated { get; set; }
}