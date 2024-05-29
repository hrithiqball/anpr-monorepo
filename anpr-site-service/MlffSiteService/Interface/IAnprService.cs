using MlffSiteService.Services;

namespace MlffSiteService.Interface;

public interface IAnprService : IDisposable
{
    AnprServiceConfig Config { get; }

    event Action<IAnprDetectionResult> OnAnprDetectionResult;

    Task StartAsync();
}