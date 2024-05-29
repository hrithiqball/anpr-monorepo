using System.Text;
using MlffSiteService.DTOs;
using MlffSiteService.Extensions;
using MlffSiteService.Interface;
using MlffSiteService.Models;
using MlffSiteService.Models.Exceptions;

namespace MlffSiteService.Services;

/// <summary>
///     This class is to manage all MLFF Web API
/// </summary>
public class MlffWebApiManager : IMlffWebApiManager
{
    private const string URL_POST_UPLOAD_ANPR_IMAGES = "api/upload/anpr";
    private const string URL_POST_ANPR_DETECTION = "api/license-plate-recognition";
    private const string URL_POST_SPEED_DETECTION = "api/speed-detection";
    private const string URL_POST_RFID_TAG_DETECTION = "api/rfid-tag-detection";
    private const string URL_POST_DETECTION_MATCH = "api/match";
    private const string URL_POST_PUBLIC_IP = "api/public-ip";
    private const string MEDIA_TYPE_JSON = "application/json";
    private readonly HttpClient _httpClient;
    private readonly ILogger<MlffWebApiManager> _logger;

    public MlffWebApiManager(HttpClient httpClient,
        ILogger<MlffWebApiManager> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        // web api http client
        var baseUrl = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.MLFF_WEB_API_BASE_URL) ??
                      throw new MissingEnvironmentVariableException(Constants.EnvironmentVariables.MLFF_WEB_API_BASE_URL);

        _httpClient.BaseAddress = new Uri(baseUrl);
    }

    public async Task<ApiResponse<UploadAnprImageResponseDto>> UploadImageAsync(FileInfo vehicleImage,
        FileInfo plateImage,
        string imageId,
        string plateNumber,
        CancellationToken cancellationToken)
    {
        await using var vehicleImageFileStream = new FileStream(vehicleImage.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        await using var plateImageFileStream = new FileStream(plateImage.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        _logger.LogCritical("Vehicle Image file stream" + vehicleImageFileStream);
        _logger.LogCritical("Plate Image file stream" + plateImageFileStream);

        var formContent = new MultipartFormDataContent();
        formContent.Add(new StringContent(plateNumber), "numberPlate");
        formContent.Add(new StringContent(imageId), "imageId");
        formContent.Add(new StreamContent(vehicleImageFileStream), "vehicleImage", vehicleImage.Name);
        formContent.Add(new StreamContent(plateImageFileStream), "plateImage", plateImage.Name);
        _logger.LogCritical("Form Content"+formContent.ToString());

        try
        {
            var httpResponse = await _httpClient.PostAsync(URL_POST_UPLOAD_ANPR_IMAGES, formContent, cancellationToken);
            var responseBodyString = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            var responseBody = responseBodyString.Deserialize<ApiResponse<UploadAnprImageResponseDto>>();

            if (responseBody is null)
            {
                throw new JsonDeserializeException(typeof(ApiResponse<UploadAnprImageResponseDto>), responseBodyString);
            }

            _logger.LogCritical("Image was written and sent");
            return responseBody;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<ApiResponse> PostRecoAnprResultAsync(IEnumerable<IAnprDetectionResult> anpr,
        CancellationToken cancellationToken)
    {
        var requestDto = new AddLicensePlateRecognitionRequestDto(anpr.ToArray());
        var requestBodyString = requestDto.Serialize();
        try
        {
            var httpResponse = await _httpClient.PostAsync(URL_POST_ANPR_DETECTION,
                new StringContent(requestBodyString, Encoding.UTF8, MEDIA_TYPE_JSON),
                cancellationToken);

            var responseBodyString = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            var responseBody = responseBodyString.Deserialize<ApiResponse>();

            if (responseBody is null)
            {
                throw new JsonDeserializeException(typeof(ApiResponse), responseBodyString);
            }

            return responseBody;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<ApiResponse> PostPublicIPAsync(string publicIPString, CancellationToken cancellationToken)
    {
        var requestDto = new AddPublicIPRequestDto(publicIPString);
        var requestBodyString = requestDto.Serialize();

        try
        {
            var httpResponse = await _httpClient.PostAsync(URL_POST_PUBLIC_IP, new StringContent(requestBodyString, Encoding.UTF8, MEDIA_TYPE_JSON), cancellationToken);
            var responseBodyString = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            var responseBody = responseBodyString.Deserialize<ApiResponse>();

            if (responseBody is null)
            {
                throw new JsonDeserializeException(typeof(ApiResponse), responseBodyString);
            }

            return responseBody;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<ApiResponse> PostSpeedDetectionAsync(IEnumerable<ISpeedDetectionResult> speedDetection,
        CancellationToken cancellationToken)
    {
        var requestDto = new AddSpeedDetectionRequestDto(speedDetection.ToArray());
        var requestBodyString = requestDto.Serialize();
        try
        {
            var httpResponse = await _httpClient.PostAsync(URL_POST_SPEED_DETECTION,
                new StringContent(requestBodyString, Encoding.UTF8, MEDIA_TYPE_JSON),
                cancellationToken);
            var responseBodyString = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            var responseBody = responseBodyString.Deserialize<ApiResponse>();

            if (responseBody is null)
            {
                throw new JsonDeserializeException(typeof(ApiResponse), responseBodyString);
            }

            return responseBody;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<ApiResponse> PostRfidTagDetectionAsync(IEnumerable<IRfidTagDetectionResult> tagDetection,
        CancellationToken cancellationToken)
    {
        var requestDto = new AddRfidTagDetectionRequestDto(tagDetection.ToArray());
        var requestBodyString = requestDto.Serialize();
        try
        {
            var httpResponse = await _httpClient.PostAsync(URL_POST_RFID_TAG_DETECTION,
                new StringContent(requestBodyString, Encoding.UTF8, MEDIA_TYPE_JSON),
                cancellationToken);
            var responseBodyString = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            var responseBody = responseBodyString.Deserialize<ApiResponse>();

            if (responseBody is null)
            {
                throw new JsonDeserializeException(typeof(ApiResponse), responseBodyString);
            }

            return responseBody;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<ApiResponse> PostDetectionMatchAsync(IEnumerable<IDetectionMatchResult> matchResults, CancellationToken cancellationToken)
    {
        var requestDto = new AddDetectionMatchRequestDto(matchResults.ToArray());
        var requestBodyString = requestDto.Serialize();
        try
        {
            var httpResponse = await _httpClient.PostAsync(URL_POST_DETECTION_MATCH,
                new StringContent(requestBodyString, Encoding.UTF8, MEDIA_TYPE_JSON),
                cancellationToken);
            var responseBodyString = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            var responseBody = responseBodyString.Deserialize<ApiResponse>();

            if (responseBody is null)
            {
                throw new JsonDeserializeException(typeof(ApiResponse), responseBodyString);
            }

            return responseBody;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }
}