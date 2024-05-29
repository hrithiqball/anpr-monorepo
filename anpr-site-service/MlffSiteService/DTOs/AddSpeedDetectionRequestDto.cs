using System.ComponentModel.DataAnnotations;
using MlffSiteService.Interface;
using MlffSiteService.Models;
using MlffSiteService.Models.Exceptions;

namespace MlffSiteService.DTOs;

public class AddSpeedDetectionRequestDto
{
    public AddSpeedDetectionRequestDto(params ISpeedDetectionResult[] collections)
    {
        var siteId = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.SITE_ID) ??
                     throw new MissingEnvironmentVariableException(Constants.EnvironmentVariables.SITE_ID);

        foreach (var speedDetection in collections)
        {
            Data.Add(new SpeedDetectionDetail(speedDetection.Timestamp, siteId, speedDetection.Speed));
        }
    }

    public IList<SpeedDetectionDetail> Data { get; } = new List<SpeedDetectionDetail>();

    public class SpeedDetectionDetail
    {
        public SpeedDetectionDetail(DateTime detectionDate,
            string siteId,
            int speed)
        {
            DetectionDate = detectionDate;
            SiteId = siteId;
            Speed = speed;
        }

        [Required]
        public DateTime DetectionDate { get; set; }

        [Required]
        public string SiteId { get; set; }

        [Required]
        public int Speed { get; set; }
    }
}