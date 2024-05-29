namespace MlffSiteService.Interface;

public interface IDetectionMatchResult
{
    string SiteId { get; }

    IRfidTagDetectionResult? RfidTagDetectionResult { get; set; }

    IAnprDetectionResult? AnprDetectionResult { get; set; }

    ISpeedDetectionResult? SpeedDetectionResult { get; set; }

    DateTime DateMatched { get; set; }
}