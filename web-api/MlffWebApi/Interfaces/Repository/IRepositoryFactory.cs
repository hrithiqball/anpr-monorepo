namespace MlffWebApi.Interfaces.Repository;

public interface IRepositoryFactory
{
    #region repositories

    ISiteRepository SiteRepository { get; }
    ILicensePlateRecognitionRepository LicensePlateRecognitionRepository { get; }
    IWatchlistRepository WatchlistRepository { get; }
    IRfidDetectionRepository RfidDetectionRepository { get; }
    ISpeedDetectionRepository SpeedDetectionRepository { get; }
    IDetectionMatchRepository DetectionMatchRepository { get; }
    IPublicIPRepository PublicIPRepository { get; }

    #endregion
}