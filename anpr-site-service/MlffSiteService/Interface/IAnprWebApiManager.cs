using MlffSiteService.DTOs;

namespace MlffSiteService.Interface;

public interface IMlffWebApiManager
{
    Task<ApiResponse<UploadAnprImageResponseDto>> UploadImageAsync(
        FileInfo vehicleImage,
        FileInfo plateImage,
        string imageId,
        string plateNumber,
        CancellationToken cancellationToken
    );

    Task<ApiResponse> PostRecoAnprResultAsync(
        IEnumerable<IAnprDetectionResult> anprDetection,
        CancellationToken cancellationToken
    );

    Task<ApiResponse> PostSpeedDetectionAsync(
        IEnumerable<ISpeedDetectionResult> speedDetection,
        CancellationToken cancellationToken
    );

    Task<ApiResponse> PostRfidTagDetectionAsync(
        IEnumerable<IRfidTagDetectionResult> speedDetection,
        CancellationToken cancellationToken
    );

    Task<ApiResponse> PostDetectionMatchAsync(
        IEnumerable<IDetectionMatchResult> matchResults,
        CancellationToken cancellationToken
    );

    Task<ApiResponse> PostPublicIPAsync(string publicIPString, CancellationToken cancellationToken);
}
