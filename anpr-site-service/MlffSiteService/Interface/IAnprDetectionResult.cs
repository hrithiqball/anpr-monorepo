namespace MlffSiteService.Interface;

public interface IAnprDetectionResult
{
    DateTime Timestamp { get; set; }

    string VehicleImagePath { get; set; }

    string PlateImagePath { get; set; }

    IBoundingBox BoundingBox { get; }

    string PlateNumber { get; set; }

    int Confidence { get; set; }

    string CameraId { get; set; }
}

public interface IBoundingBox
{
    int Top { get; set; }

    int Left { get; set; }

    int Width { get; set; }

    int Height { get; set; }
}