using MlffSiteService.Interface;
using MlffSiteService.Models;
using MlffSiteService.Models.Exceptions;

namespace MlffSiteService.DTOs;

public class AddDetectionMatchRequestDto
{
    public AddDetectionMatchRequestDto(params IDetectionMatchResult[] matches)
    {
        var siteId = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.SITE_ID) ??
                     throw new MissingEnvironmentVariableException(Constants.EnvironmentVariables.SITE_ID);

        foreach (var match in matches)
        {
            if (match == null)
            {
                continue;
            }

            MatchDetails.Add(new DetectionMatchDetail
            {
                SiteId = siteId,
                TagId = match.RfidTagDetectionResult?.TagId ?? string.Empty,
                Speed = match.SpeedDetectionResult?.Speed,
                PlateNumber = match.AnprDetectionResult?.PlateNumber ?? string.Empty,
                DateMatched = match.DateMatched,
                VehicleImagePath = match.AnprDetectionResult?.VehicleImagePath ?? string.Empty,
                PlateImagePath = match.AnprDetectionResult?.PlateImagePath ?? string.Empty,
            });
        }
    }

    public IList<DetectionMatchDetail> MatchDetails { get; set; } = Enumerable.Empty<DetectionMatchDetail>().ToList();

    public class DetectionMatchDetail
    {
        public string SiteId { get; set; } = string.Empty;

        public string TagId { get; set; } = string.Empty;

        public string PlateNumber { get; set; } = string.Empty;

        public int? Speed { get; set; }

        public DateTime DateMatched { get; set; }
        public string VehicleImagePath { get; set; } = string.Empty;
        public string PlateImagePath { get; set; } = string.Empty;
    }
}