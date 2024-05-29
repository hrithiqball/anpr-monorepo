using MlffWebApi.DTO;
using MlffWebApi.DTO.Watchlist;
using MlffWebApi.Interfaces.ANPR;
using MlffWebApi.Interfaces.Repository;
using Microsoft.AspNetCore.Mvc;
using MlffWebApi.Exceptions;

namespace MlffWebApi.Controllers;

public class WatchlistController : ApiBaseController
{
    private readonly IRepositoryFactory _repositoryFactory;
    private readonly ILogger<WatchlistController> _logger;

    public WatchlistController(
        IRepositoryFactory repositoryFactory,
        ILogger<WatchlistController> logger
    ) : base(logger)
    {
        _repositoryFactory = repositoryFactory;
        _logger = logger;
    }
    /// <summary>
    /// Get list of watchlist vehicles
    /// </summary>
    /// <param name="requestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("list")]
    public async Task<IActionResult> GetWatchlist(
        [FromQuery] GetWatchlistRequestDto requestDto,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var result = await _repositoryFactory.WatchlistRepository.GetWatchlistAsync(
                requestDto,
                cancellationToken
            );

            var responseDto = result.ToResponseDto();

            return Ok(
                new ApiResponse<GetWatchlistResponseDto>(StatusCodes.Status200OK, responseDto)
            );
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
    /// Update watchlist
    /// </summary>
    /// <param name="id"></param>
    /// <param name="requestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<IActionResult> UpdateWatchlist(
        [FromQuery] Guid id,
        UpdateWatchlistRequestDto requestDto,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var watchlist = await _repositoryFactory.WatchlistRepository.UpdateWatchlist(
                id,
                requestDto,
                cancellationToken
            );
            return Ok(new ApiResponse<IWatchlist>(StatusCodes.Status200OK, watchlist));
        }
        catch (InvalidOperationException e)
        {
            return HandleBadRequest(e, e.InnerException?.Message);
        }
        catch (RepeatedUniqueValueException e)
        {
            return HandleBadRequest(e, e.Message);
        }
        catch (KeyNotFoundException e)
        {
            return HandleBadRequest(e, e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ApiResponse(StatusCodes.Status500InternalServerError, e.Message)
            );
        }
    }
    /// <summary>
    /// Add new watchlist vehicle
    /// </summary>
    /// <param name="requestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddWatchlist(
        AddWatchlistRequestDto requestDto,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var currentUsername = "HttpRequest";

            var result = await _repositoryFactory.WatchlistRepository.InsertWatchlistAsync(
                requestDto.Watchlists,
                currentUsername,
                cancellationToken
            );
            return Ok(
                new ApiResponse<IEnumerable<IWatchlist>>(
                    StatusCodes.Status200OK,
                    "Watchlist added.",
                    result
                )
            );
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
    /// Delete watchlist vehicle
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete]
    public async Task<IActionResult> DeleteWatchlist(Guid uid, CancellationToken cancellationToken)
    {
        try
        {
            var isDeleted = await _repositoryFactory.WatchlistRepository.DeleteWatchlistAsync(
                uid,
                cancellationToken
            );

            if (isDeleted)
            {
                return Ok(new ApiResponse(StatusCodes.Status200OK, "Watchlist deleted"));
            }
            else
            {
                return Ok(new ApiResponse(StatusCodes.Status200OK, "Watchlist not found."));
            }
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
}
