using System.Net;
using Microsoft.AspNetCore.Mvc;
using MlffWebApi.DTO;
using MlffWebApi.DTO.IP;
using MlffWebApi.Exceptions;
using MlffWebApi.Hubs;
using MlffWebApi.Interfaces.Repository;
using MlffWebApi.Interfaces.PublicIP;
using MlffWebApi.Models.PublicIP;
using Microsoft.AspNetCore.Diagnostics;

namespace MlffWebApi.Controllers;

[Route("api/public-ip")]
public class PublicIPController : ApiBaseController
{
    private readonly IRepositoryFactory _repositoryFactory;
    private readonly ILogger<PublicIPController> _logger;
    private readonly DetectionHub _detectionHub;

    public PublicIPController(
        IRepositoryFactory repositoryFactory,
        ILogger<PublicIPController> logger,
        DetectionHub detectionHub
    ) : base(logger)
    {
        _repositoryFactory = repositoryFactory;
        _logger = logger;
        _detectionHub = detectionHub;
    }

    /// <summary>
    /// Post public IP from site
    /// </summary>
    /// <param name="requestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddPublicIPRecognition(
        [FromBody] AddPublicIPDto requestDto,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (requestDto == null)
            {
                return BadRequest(
                    new ApiResponse(
                        StatusCodes.Status400BadRequest,
                        "Public IP could not be retrieve"
                    )
                );
            }

            var siteList = await _repositoryFactory.SiteRepository.GetSites(cancellationToken);
            var convertedRecognitionRecords = requestDto.Data
                .Select(t => t.ToBusinessModel(siteList))
                .ToList();

            var (recognitions, details) =
                await _repositoryFactory.PublicIPRepository.AddRecordsIfNotExist(
                    convertedRecognitionRecords,
                    cancellationToken
                );

            var latestDetection = recognitions.MaxBy(t => t.DateUpdate).ToLite();

            if (latestDetection != null)
            {
                await _detectionHub.BroadcastPublicIP(latestDetection, cancellationToken);
            }

            return Ok(
                new ApiResponse<IEnumerable<IPublicIPRecognition>>(
                    StatusCodes.Status200OK,
                    details,
                    recognitions
                )
            );
        }
        catch (KeyNotFoundException e)
        {
            return BadRequest(new ApiResponse(StatusCodes.Status400BadRequest, e.Message));
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ApiResponse(StatusCodes.Status500InternalServerError, e.Message)
            );
        }
    }

    /// <summary>
    /// Get public IP based on site latest one
    /// </summary>
    /// <param name="siteId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("/api/get-public-api")]
    public async Task<IActionResult> GetPublicIPRecognition(
        string siteId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (string.IsNullOrEmpty(siteId))
            {
                return BadRequest(
                    new ApiResponse(
                        StatusCodes.Status400BadRequest,
                        $"Property {nameof(siteId)} is required."
                    )
                );
            }

            var dataReceived = await _repositoryFactory.PublicIPRepository.GetPublicIP(
                siteId,
                cancellationToken
            );

            if (dataReceived == null)
            {
                return Ok(
                    new ApiResponse(StatusCodes.Status200OK, $"Get by site id {siteId}. Not found.")
                );
            }

            return Ok(
                new ApiResponse<IPublicIPRecognition>(
                    StatusCodes.Status200OK,
                    $"Get by {nameof(siteId)}",
                    dataReceived
                )
            );
        }
        catch (InvalidOperationException e)
        {
            return HandleBadRequest(e, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ApiResponse(StatusCodes.Status500InternalServerError, e.Message)
            );
        }
    }
    //[HttpGet("list")]
    //public async Task<IActionResult> GetRfidTagDetections(
    //    [FromQuery] GetRfidTagDetectionListRequestDto requestDto,
    //    CancellationToken cancellationToken
    //)
    //{
    //    try
    //    {
    //        var records = await _repositoryFactory.RfidDetectionRepository.GetRecords(
    //            requestDto,
    //            cancellationToken
    //        );

    //        var response = records.ToResponseDto(requestDto);

    //        return Ok(
    //            new ApiResponse<GetRfidTagDetectionListResponseDto>(
    //                StatusCodes.Status200OK,
    //                response
    //            )
    //        );
    //    }
    //    catch (Exception e)
    //    {
    //        _logger.LogError(e, e.Message);
    //        return StatusCode(
    //            StatusCodes.Status500InternalServerError,
    //            new ApiResponse(StatusCodes.Status500InternalServerError, e.Message)
    //        );
    //    }
    //}
}
