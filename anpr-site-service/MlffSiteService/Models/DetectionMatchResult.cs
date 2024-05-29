using MlffSiteService.Interface;
using MlffSiteService.Models.Exceptions;

namespace MlffSiteService.Models;

public class DetectionMatchResult : IDetectionMatchResult
{

    public DetectionMatchResult()
    {
        SiteId = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.SITE_ID) ??
                 throw new MissingEnvironmentVariableException(nameof(Constants.EnvironmentVariables.SITE_ID));
    }

    public string SiteId { get; }

    public IRfidTagDetectionResult? RfidTagDetectionResult { get; set; }

    public IAnprDetectionResult? AnprDetectionResult { get; set; }

    public ISpeedDetectionResult? SpeedDetectionResult { get; set; }

    public DateTime DateMatched { get; set; }
}