using System.ComponentModel.DataAnnotations;
using MlffWebApi.Interfaces;
using MlffWebApi.Interfaces.ANPR;
using MlffWebApi.Models;
using MlffWebApi.Models.ANPR;

namespace MlffWebApi.DTO.LicensePlateRecognition;

public class AddLicensePlateRecognitionRequestDto
{
    public IEnumerable<LicensePlateRecognitionDetail> Data { get; set; }

    public class LicensePlateRecognitionDetail
    {
        [Required]
        public DateTime DetectionDate { get; set; }

        [Required]
        public string SiteId { get; set; }
        public string CameraId { get; set; }

        [Required]
        public string VehicleImagePath { get; set; }

        public string PlateImagePath { get; set; }

        [Required]
        public string PlateNumber { get; set; }

        public BoundingBox BoundingBox { get; set; }
        public decimal? ConfidenceOnPlateDetection { get; set; }

        [Required]
        public decimal? ConfidenceOnPlateNumberAccuracy { get; set; }
    }
}

public static class LicensePlateRecognitionExtensions
{
    public static ILicensePlateRecognition ToBusinessModel(
        this AddLicensePlateRecognitionRequestDto.LicensePlateRecognitionDetail detail, IEnumerable<ISite> sites)
    {
        var site = sites.FirstOrDefault(t =>
            string.Equals(t.Id, detail.SiteId, StringComparison.CurrentCultureIgnoreCase)) ?? throw new KeyNotFoundException($"Site {detail.SiteId} not found.");

        return new Models.ANPR.LicensePlateRecognition
        {
            VehicleImagePath = detail.VehicleImagePath,
            PlateImagePath = detail.PlateImagePath,
            PlateNumber = detail.PlateNumber,
            BoundingBox = detail.BoundingBox is not null
                ? new BoundingBox(detail.BoundingBox.Top, detail.BoundingBox.Left, detail.BoundingBox.Width,
                    detail.BoundingBox.Height)
                : null,
            ConfidenceOnPlateRecognition = detail.ConfidenceOnPlateNumberAccuracy,
            ConfidenceOnPlateDetection = detail.ConfidenceOnPlateDetection,
            SiteId = site.Id,
            CameraId = detail.CameraId,
            DetectionDate = detail.DetectionDate,
            DateCreated = DateTime.Now
        };
    }
}