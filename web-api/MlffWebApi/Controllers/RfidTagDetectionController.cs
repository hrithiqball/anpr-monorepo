using Microsoft.AspNetCore.Mvc;
using MlffWebApi.DTO;
using MlffWebApi.DTO.RfidDetection;
using MlffWebApi.Hubs;
using MlffWebApi.Interfaces.Repository;
using MlffWebApi.Interfaces.RFID;
using MlffWebApi.Models.RFID;

namespace MlffWebApi.Controllers;

[Route("api/rfid-tag-detection")]
public class RfidTagDetectionController : ApiBaseController
{
    private readonly IRepositoryFactory _repositoryFactory;
    private readonly ILogger<RfidTagDetectionController> _logger;
    private readonly DetectionHub _detectionHub;

    public RfidTagDetectionController(IRepositoryFactory repositoryFactory, ILogger<RfidTagDetectionController> logger,
        DetectionHub detectionHub) :
        base(logger)
    {
        _repositoryFactory = repositoryFactory;
        _logger = logger;
        _detectionHub = detectionHub;
    }

    /// <summary>
    /// Add RFID tag detection
    /// </summary>
    /// <param name="requestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddRfidTagDetection([FromBody] AddRfidTagDetectionRequestDto requestDto,
        CancellationToken cancellationToken)
    {
        try
        {
            if (requestDto == null)
            {
                return BadRequest(new ApiResponse(StatusCodes.Status400BadRequest,
                    "No RFID tag detection payload is found. Payload should submit in request body as array."));
            }

            var siteList = await _repositoryFactory.SiteRepository.GetSites(cancellationToken);

            var convertedDetectionRecords =
                requestDto.Data.Select(t => t.ToBusinessModel(siteList)).ToList();

            var (recognitions, details) =
                await _repositoryFactory.RfidDetectionRepository.AddRecordsIfNotExist(
                    convertedDetectionRecords, cancellationToken);


            var latestDetection = recognitions.MaxBy(t => t.DetectionDate).ToLite();

            if (latestDetection != null)
            {
                await _detectionHub.BroadcastRfidTagDetected(latestDetection, cancellationToken);
            }

            return Ok(new ApiResponse<IEnumerable<IRfidTagDetection>>(StatusCodes.Status200OK, details,
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
    /// Get RFID tag detections
    /// </summary>
    /// <param name="requestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("list")]
    public async Task<IActionResult> GetRfidTagDetections(
        [FromQuery] GetRfidTagDetectionListRequestDto requestDto, CancellationToken cancellationToken)
    {
        try
        {
            var records = await _repositoryFactory.RfidDetectionRepository.GetRecords(requestDto, cancellationToken);

            var response = records.ToResponseDto(requestDto);

            return Ok(new ApiResponse<GetRfidTagDetectionListResponseDto>(StatusCodes.Status200OK, response));
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse(StatusCodes.Status500InternalServerError, e.Message));
        }
    }
}