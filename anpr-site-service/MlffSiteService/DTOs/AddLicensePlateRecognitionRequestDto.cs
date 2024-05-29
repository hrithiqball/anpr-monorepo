using System.ComponentModel.DataAnnotations;
using MlffSiteService.Interface;
using MlffSiteService.Models;
using MlffSiteService.Models.Exceptions;

namespace MlffSiteService.DTOs;

public class AddLicensePlateRecognitionRequestDto
{

    public AddLicensePlateRecognitionRequestDto(params IAnprDetectionResult[] collections)
    {
        var siteId = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.SITE_ID) ??
                     throw new MissingEnvironmentVariableException(Constants.EnvironmentVariables.SITE_ID);

        foreach (var anpr in collections)
        {
            Data.Add(new LicensePlateRecognitionDetail
            {
                DetectionDate = anpr.Timestamp,
                SiteId = siteId,
                CameraId = anpr.CameraId,
                VehicleImagePath = anpr.VehicleImagePath,
                PlateImagePath = anpr.PlateImagePath,
                PlateNumber = anpr.PlateNumber,
                BoundingBox = anpr.BoundingBox,
                ConfidenceOnPlateNumberAccuracy = anpr.Confidence
            });
        }
    }

    public IList<LicensePlateRecognitionDetail> Data { get; } = new List<LicensePlateRecognitionDetail>();

    public class LicensePlateRecognitionDetail
    {
        [Required]
        public DateTime DetectionDate { get; init; }

        [Required]
        public string SiteId { get; init; } = string.Empty;

        [Required]
        public string VehicleImagePath { get; init; } = string.Empty;
        public string CameraId { get; init; } = string.Empty;

        public string PlateImagePath { get; init; } = string.Empty;

        [Required]
        public string PlateNumber { get; init; } = string.Empty;

        public IBoundingBox? BoundingBox { get; init; }

        public decimal? ConfidenceOnPlateDetection { get; init; }

        [Required]
        public decimal? ConfidenceOnPlateNumberAccuracy { get; init; }
    }
}