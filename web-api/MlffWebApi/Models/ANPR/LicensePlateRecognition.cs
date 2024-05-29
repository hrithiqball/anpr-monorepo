using MlffWebApi.Database.DbContexts;
using MlffWebApi.Interfaces.ANPR;

namespace MlffWebApi.Models.ANPR;

public class LicensePlateRecognitionLite : ILicensePlateRecognitionLite
{
    public string SiteId { get; set; }
    public string VehicleImagePath { get; set; }
    public string PlateImagePath { get; set; }
    public string PlateNumber { get; set; }
    public DateTime DetectionDate { get; set; }
    public bool IsInsideWatchlist { get; set; } = false;
    public string CameraId { get; set; }
}

public class LicensePlateRecognition : LicensePlateRecognitionLite, ILicensePlateRecognition
{
    public Guid Uid { get; set; }
    public IBoundingBox BoundingBox { get; set; }
    public decimal? ConfidenceOnPlateRecognition { get; set; }
    public decimal? ConfidenceOnPlateDetection { get; set; }
    public DateTime DateCreated { get; set; }
}

public class BoundingBox : IBoundingBox
{
    public int? Top { get; set; }
    public int? Left { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }

    public BoundingBox(int? top, int? left, int? width, int? height)
    {
        Top = top;
        Left = left;
        Width = width;
        Height = height;
    }

    public BoundingBox()
    {
    }
}

public static class LicensePlateRecognitionExtension
{
    public static ILicensePlateRecognition ToBusinessModel(this license_plate_recognition lpr)
    {
        return new LicensePlateRecognition
        {
            Uid = lpr.uid,
            VehicleImagePath = lpr.vehicle_image_path,
            PlateImagePath = lpr.plate_image_path,
            PlateNumber = lpr.plate_number,
            CameraId = lpr.camera_id,
            BoundingBox = new BoundingBox(lpr.bbox_top, lpr.bbox_left, lpr.bbox_width, lpr.bbox_height),
            ConfidenceOnPlateRecognition = lpr.confidence_ocr,
            ConfidenceOnPlateDetection = lpr.confidence_lpd,
            DetectionDate = lpr.date_detection,
            SiteId = lpr.site_id,
            DateCreated = lpr.date_created,
        };
    }

    public static ILicensePlateRecognitionLite ToLite(this ILicensePlateRecognition lpr)
    {
        return lpr;
    }

    public static license_plate_recognition ToDatabaseModel(this ILicensePlateRecognition lpr)
    {
        return new license_plate_recognition
        {
            uid = Guid.NewGuid(),
            vehicle_image_path = lpr.VehicleImagePath,
            plate_image_path = lpr.PlateImagePath,
            plate_number = lpr.PlateNumber,
            bbox_top = lpr.BoundingBox?.Top,
            bbox_left = lpr.BoundingBox?.Left,
            bbox_height = lpr.BoundingBox?.Height,
            bbox_width = lpr.BoundingBox?.Width,
            confidence_lpd = lpr.ConfidenceOnPlateDetection,
            confidence_ocr = lpr.ConfidenceOnPlateRecognition,
            date_detection = lpr.DetectionDate,
            site_id = lpr.SiteId,
            camera_id = lpr.CameraId,
            date_created = DateTime.Now
        };
    }
}