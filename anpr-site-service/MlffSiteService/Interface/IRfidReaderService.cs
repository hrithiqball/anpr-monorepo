using MlffSiteService.Models;
using MlffSiteService.Services;

namespace MlffSiteService.Interface;

public interface IRfidReaderService : IDisposable
{
    RfidReaderServiceConfig Config { get; }

    event Action<IList<RfidTagDetectionResult>> OnRfidTagDetectionResult;

    Task StartAsync();
}