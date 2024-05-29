namespace MlffWebApi.Interfaces.Services;

public interface IUploaderService
{
    Task<string> WriteImageAsync(IFormFile form, string filename, CancellationToken cancellationToken);
    Task<string> WriteImageVehicleAsync(IFormFile form, string filename, CancellationToken cancellationToken);
    Task<string> WriteImagePlateAsync(IFormFile form, string filename, CancellationToken cancellationToken);
}