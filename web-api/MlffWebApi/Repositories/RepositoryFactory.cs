using MlffWebApi.Database.DbContexts;
using MlffWebApi.Interfaces.Repository;

namespace MlffWebApi.Repositories;

public class RepositoryFactory : IRepositoryFactory
{
    private readonly MlffDbContext _context;
    private readonly ILoggerFactory _loggerFactory;

    public ISiteRepository SiteRepository => new SiteRepository(_context, _loggerFactory);

    public ILicensePlateRecognitionRepository LicensePlateRecognitionRepository =>
        new LicensePlateRecognitionRepository(_context, _loggerFactory);

    public IRfidDetectionRepository RfidDetectionRepository => new RfidDetectionRepository(_context, _loggerFactory);
    public ISpeedDetectionRepository SpeedDetectionRepository => new SpeedDetectionRepository(_context, _loggerFactory);
    public IWatchlistRepository WatchlistRepository => new WatchlistRepository(_context, _loggerFactory);
    public IPublicIPRepository PublicIPRepository => new PublicIPRecognitionRepository(_context, _loggerFactory);

    public IDetectionMatchRepository DetectionMatchRepository =>
        new DetectionMatchRepository(_context, _loggerFactory);
    public RepositoryFactory(MlffDbContext context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _loggerFactory = loggerFactory;

        if (!context.Database.CanConnect())
        {
            throw new Exception("Database offline");
        }
    }
}