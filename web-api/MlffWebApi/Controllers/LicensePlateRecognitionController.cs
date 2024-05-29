using MlffWebApi.DTO;
using MlffWebApi.DTO.LicensePlateRecognition;
using MlffWebApi.Hubs;
using MlffWebApi.Interfaces.ANPR;
using MlffWebApi.Interfaces.Repository;
using MlffWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using MlffWebApi.Models.ANPR;

namespace MlffWebApi.Controllers;

[Route("api/license-plate-recognition")]
public class LicensePlateRecognitionController : ApiBaseController
{
    private readonly IRepositoryFactory _repositoryFactory;
    private readonly ILogger<LicensePlateRecognitionController> _logger;
    private readonly DetectionHub _detectionHub;

    public LicensePlateRecognitionController(IRepositoryFactory repositoryFactory,
        ILogger<LicensePlateRecognitionController> logger, DetectionHub detectionHub) : base(logger)
    {
        _repositoryFactory = repositoryFactory;
        _logger = logger;
        _detectionHub = detectionHub;
    }


    /// <summary>
    /// Submit LPR detection, by batch
    /// </summary>
    /// <param name="requestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddLicensePlateRecognition(
        [FromBody] AddLicensePlateRecognitionRequestDto requestDto,
        CancellationToken cancellationToken)
    {
        try
        {
            if (requestDto == null)
            {
                return BadRequest(new ApiResponse(StatusCodes.Status400BadRequest,
                    "No recognition payload is found. Payload should submit in request body as array."));
            }

            var siteList = await _repositoryFactory.SiteRepository.GetSites(cancellationToken);

            var convertedRecognitionRecords =
                requestDto.Data.Select(t => t.ToBusinessModel(siteList)).ToList();

            var (recognitions, details) =
                await _repositoryFactory.LicensePlateRecognitionRepository.AddRecordsIfNotExist(
                    convertedRecognitionRecords, cancellationToken);

            var latestDetection = recognitions.MaxBy(t => t.DetectionDate).ToLite();

            if (latestDetection != null)
            {
                // get from watchlist
                var watchlist = await _repositoryFactory.WatchlistRepository.GetWatchlistByPlateNumberAsync(
                    latestDetection.PlateNumber, cancellationToken);

                latestDetection.IsInsideWatchlist = watchlist != null;
                
                await _detectionHub.BroadcastLicensePlateDetected(latestDetection, cancellationToken);
            }

            return Ok(new ApiResponse<IEnumerable<ILicensePlateRecognition>>(StatusCodes.Status200OK, details,
                recognitions));
        }
        catch (KeyNotFoundException e)
        {
            return BadRequest(new ApiResponse(StatusCodes.Status400BadRequest, e.Message));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse(StatusCodes.Status500InternalServerError, e.Message));
        }
    }

    /// <summary>
    /// Get LPR detection historical records
    /// </summary>
    /// <param name="requestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("list")]
    public async Task<IActionResult> GetLicensePlateRecognitions(
        [FromQuery] GetLicensePlateRecognitionListRequestDto requestDto, CancellationToken cancellationToken)
    {
        try
        {
            var records =
                await _repositoryFactory.LicensePlateRecognitionRepository.GetRecords(requestDto, cancellationToken);

            var response = records.ToResponseDto(requestDto);

            return Ok(new ApiResponse<GetLicensePlateRecognitionListResponseDto>(
                StatusCodes.Status200OK, response));
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse(StatusCodes.Status500InternalServerError, e.Message));
        }
    }
}