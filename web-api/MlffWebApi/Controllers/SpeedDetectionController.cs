using Microsoft.AspNetCore.Mvc;
using MlffWebApi.DTO;
using MlffWebApi.DTO.SpeedDetection;
using MlffWebApi.Hubs;
using MlffWebApi.Interfaces.Repository;
using MlffWebApi.Interfaces.Speed;
using MlffWebApi.Models.Speed;

namespace MlffWebApi.Controllers;

[Route("api/speed-detection")]
public class SpeedDetectionController : ApiBaseController
{
    private readonly IRepositoryFactory _repositoryFactory;
    private readonly ILogger<SpeedDetectionController> _logger;
    private readonly DetectionHub _detectionHub;

    public SpeedDetectionController(IRepositoryFactory repositoryFactory, ILogger<SpeedDetectionController> logger,
        DetectionHub detectionHub) :
        base(logger)
    {
        _repositoryFactory = repositoryFactory;
        _logger = logger;
        _detectionHub = detectionHub;
    }


    /// <summary>
    /// Add speed detection
    /// </summary>
    /// <param name="requestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddSpeedDetection([FromBody] AddSpeedDetectionRequestDto requestDto,
        CancellationToken cancellationToken)
    {
        try
        {
            if (requestDto == null)
            {
                return BadRequest(new ApiResponse(StatusCodes.Status400BadRequest,
                    "No speed detection payload is found. Payload should submit in request body as array."));
            }

            var siteList = await _repositoryFactory.SiteRepository.GetSites(cancellationToken);

            var convertedDetectionRecords =
                requestDto.Data.Select(t => t.ToBusinessModel(siteList)).ToList();

            var (recognitions, details) =
                await _repositoryFactory.SpeedDetectionRepository.AddRecords(
                    convertedDetectionRecords, cancellationToken);


            var latestDetection = recognitions.MaxBy(t => t.DetectionDate).ToLite();

            if (latestDetection != null)
            {
                await _detectionHub.BroadcastSpeedDetected(latestDetection, cancellationToken);
            }

            return Ok(new ApiResponse<IEnumerable<ISpeedDetection>>(StatusCodes.Status200OK, details,
                recognitions));
        }
        catch (KeyNotFoundException e)
        {
            return BadRequest(new ApiResponse(StatusCodes.Status400BadRequest, e.Message));
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse(StatusCodes.Status500InternalServerError, e.Message));
        }
    }

    /// <summary>
    /// Get speed detections
    /// </summary>
    /// <param name="requestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("list")]
    public async Task<IActionResult> GetSpeedDetections(
        [FromQuery] GetSpeedDetectionListRequestDto requestDto, CancellationToken cancellationToken)
    {
        try
        {
            var records = await _repositoryFactory.SpeedDetectionRepository.GetRecords(requestDto, cancellationToken);

            var response = records.ToResponseDto(requestDto);

            return Ok(new ApiResponse<GetSpeedDetectionListResponseDto>(StatusCodes.Status200OK, response));
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse(StatusCodes.Status500InternalServerError, e.Message));
        }
    }
}