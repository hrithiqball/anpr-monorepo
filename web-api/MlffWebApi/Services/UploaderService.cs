using Microsoft.Extensions.Logging;
using MlffWebApi.Controllers;
using MlffWebApi.Interfaces.Services;

namespace MlffWebApi.Services;

public class UploaderService : IUploaderService
{
    private readonly string _outputDirectory;
    private readonly string _enablePlateImage;
    private readonly string _enableFullImage;
    private readonly bool _enablePlateImageBool;
    private readonly bool _enableFullImageBool;

    private readonly ILogger<SpeedDetectionController> _logger;

    public UploaderService()
    {
        _outputDirectory = Environment.GetEnvironmentVariable(Constants.IMAGE_OUTPUT_DIRECTORY);
        _enableFullImage =
            Environment.GetEnvironmentVariable(Constants.ENABLE_POST_FULL_IMAGE) ?? "FALSE";
        _enablePlateImage =
            Environment.GetEnvironmentVariable(Constants.ENABLE_POST_LICENSE_PLATE) ?? "FALSE";
        _enableFullImageBool = string.Equals(
            _enableFullImage,
            "true",
            StringComparison.OrdinalIgnoreCase
        );
        _enablePlateImageBool = string.Equals(
            _enablePlateImage,
            "true",
            StringComparison.OrdinalIgnoreCase
        );

        if (string.IsNullOrEmpty(_outputDirectory))
        {
            throw new ArgumentException(
                $"No environment variable \"{Constants.IMAGE_OUTPUT_DIRECTORY}\" found"
            );
        }

        if (!Directory.Exists(_outputDirectory))
        {
            Directory.CreateDirectory(_outputDirectory);
        }
    }

    public async Task<string> WriteImageAsync(
        IFormFile form,
        string filename,
        CancellationToken cancellationToken
    )
    {
        var format = GetFileFormat(form);
        filename = Path.GetFileNameWithoutExtension(filename) + format;
        var outputPath = Path.Combine(_outputDirectory, filename);

        await using var fileStream = new FileStream(
            outputPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.ReadWrite
        );
        await form.OpenReadStream().CopyToAsync(fileStream, cancellationToken);

        _logger.LogCritical($"/images/{filename}");
        return $"/images/{filename}";
    }

    public async Task<string> WriteImageVehicleAsync(
        IFormFile form,
        string filename,
        CancellationToken cancellationToken
    )
    {
        _logger.LogCritical("HERE PINPOINT 2");
        _logger.LogCritical(_enableFullImageBool.ToString());

        var format = GetFileFormat(form);
        filename = Path.GetFileNameWithoutExtension(filename) + format;
        var outputPath = Path.Combine(_outputDirectory, filename);

        _logger.LogCritical("HERE PINPOINT 3");

        if (_enableFullImageBool)
        {
            _logger.LogCritical("HERE PINPOINT FINAL");
            await using var fileStream = new FileStream(
                outputPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.ReadWrite
            );
            await form.OpenReadStream().CopyToAsync(fileStream, cancellationToken);

            return $"/images/{filename}";
        }
        else
        {
            return "Vehicle images were disabled in environment config";
        }
    }

    public async Task<string> WriteImagePlateAsync(
        IFormFile form,
        string filename,
        CancellationToken cancellationToken
    )
    {
        var format = GetFileFormat(form);
        filename = Path.GetFileNameWithoutExtension(filename) + format;
        var outputPath = Path.Combine(_outputDirectory, filename);

        if (_enablePlateImageBool)
        {
            await using var fileStream = new FileStream(
                outputPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.ReadWrite
            );
            await form.OpenReadStream().CopyToAsync(fileStream, cancellationToken);

            return $"/images/{filename}";
        }
        else
        {
            return "Plate images were disabled in environment config";
        }
    }

    private string GetFileFormat(IFormFile form)
    {
        return Path.GetExtension(form.FileName);
    }
}

//chatgpt solution
//public async Task<string> WriteImageAsync(IFormFile form, string filename, string enableFlag, CancellationToken cancellationToken)
//{
//    var format = GetFileFormat(form);
//    bool enabled = true;
//    filename = Path.GetFileNameWithoutExtension(filename) + format;
//    var outputPath = Path.Combine(_outputDirectory, filename);

//    if (!string.IsNullOrEmpty(enableFlag))
//    {
//        if (bool.TryParse(enableFlag, out bool value))
//        {
//            enabled = value;
//        }
//        else if (enableFlag.ToLower() == "true")
//        {
//            enabled = true;
//        }
//    }

//    if (enabled)
//    {
//        await using var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
//        await form.OpenReadStream().CopyToAsync(fileStream, cancellationToken);
//    }

//    return $"/images/{filename}";
//}

//public async Task<string> WriteImageVehicleAsync(IFormFile form, string filename, CancellationToken cancellationToken)
//{
//    var enablePostFullImage = Environment.GetEnvironmentVariable(Constants.ENABLE_POST_FULL_IMAGE);
//    return await WriteImageAsync(form, filename, enablePostFullImage, cancellationToken);
//}

//public async Task<string> WriteImagePlateAsync(IFormFile form, string filename, CancellationToken cancellationToken)
//{
//    var enablePostLicensePlate = Environment.GetEnvironmentVariable(Constants.ENABLE_POST_LICENSE_PLATE);
//    return await WriteImageAsync(form, filename, enablePostLicensePlate, cancellationToken);
//}
