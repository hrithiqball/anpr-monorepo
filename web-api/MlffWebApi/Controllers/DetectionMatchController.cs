using Microsoft.AspNetCore.Mvc;
using MlffWebApi.DTO;
using MlffWebApi.DTO.DetectionMatch;
using MlffWebApi.Hubs;
using MlffWebApi.Interfaces;
using MlffWebApi.Interfaces.Repository;
using MlffWebApi.Models;

namespace MlffWebApi.Controllers;

[Route("api/match")]
public class DetectionMatchController : Controller
{
    private readonly IRepositoryFactory _repositoryFactory;
    private readonly DetectionHub _detectionHub;
    private readonly ILogger<DetectionMatchController> _logger;

    public DetectionMatchController(IRepositoryFactory repositoryFactory, DetectionHub detectionHub,
        ILogger<DetectionMatchController> logger)
    {
        _repositoryFactory = repositoryFactory;
        _detectionHub = detectionHub;
        _logger = logger;
    }

    /// <summary>
    /// Submit detection match, by batch
    /// </summary>
    /// <param name="requestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddDetectionMatch(
        [FromBody] AddDetectionMatchRequestDto requestDto,
        CancellationToken cancellationToken)
    {
        try
        {
            if (requestDto == null)
            {
                return BadRequest(new ApiResponse(StatusCodes.Status400BadRequest,
                    "No match payload is found. Payload should submit in request body as array."));
            }

            var siteList = await _repositoryFactory.SiteRepository.GetSites(cancellationToken);

            var convertedMatchRecords =
                requestDto.MatchDetails.Select(t => t.ToBusinessModel(siteList)).ToList();

            var (recognitions, details) =
                await _repositoryFactory.DetectionMatchRepository.AddRecords(
                    convertedMatchRecords, cancellationToken);

            var latestMatch = recognitions.MaxBy(t => t.DateMatched).ToLite();

            if (latestMatch != null)
            {
                if (!string.IsNullOrEmpty(latestMatch.PlateNumber))
                {
                    // get from watchlist
                    var watchlist = await _repositoryFactory.WatchlistRepository.GetWatchlistByPlateNumberAsync(
                        latestMatch.PlateNumber, cancellationToken);

                    latestMatch.IsInsideWatchlist = watchlist != null;
                }

                await _detectionHub.BroadcastDetectionMatched(latestMatch, cancellationToken);
            }

            return Ok(new ApiResponse<IEnumerable<IDetectionMatch>>(StatusCodes.Status200OK, details,
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
    /// Get match historical records
    /// </summary>
    /// <param name="requestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("list")]
    public async Task<IActionResult> GetDetectionMatches(
        [FromQuery] GetDetectionMatchListRequestDto requestDto, CancellationToken cancellationToken)
    {
        try
        {
            var records =
                await _repositoryFactory.DetectionMatchRepository.GetRecords(requestDto, cancellationToken);

            var response = records.ToResponseDto(requestDto);

            return Ok(new ApiResponse<GetDetectionMatchListResponseDto>(
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