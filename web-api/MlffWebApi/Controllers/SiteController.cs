using System.Net;
using Microsoft.AspNetCore.Mvc;
using MlffWebApi.DTO;
using MlffWebApi.DTO.Site;
using MlffWebApi.Exceptions;
using MlffWebApi.Interfaces;
using MlffWebApi.Interfaces.Repository;

namespace MlffWebApi.Controllers;

public class SiteController : ApiBaseController
{
    private readonly ILogger<SiteController> _logger;
    private readonly IRepositoryFactory _repositoryFactory;

    public SiteController(ILogger<SiteController> logger, IRepositoryFactory repositoryFactory) : base(logger)
    {
        _logger = logger;
        _repositoryFactory = repositoryFactory;
    }

    /// <summary>
    /// Get site
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetSite(string id, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new ApiResponse(StatusCodes.Status400BadRequest,
                    $"Property {nameof(id)} is required."));
            }

            var site = await _repositoryFactory.SiteRepository.GetSite(id, cancellationToken);

            if (site == null)
            {
                return Ok(new ApiResponse(StatusCodes.Status200OK, $"Get by id {id}. Not found."));
            }

            return Ok(new ApiResponse<ISite>(StatusCodes.Status200OK, $"Get by {nameof(id)}", site));
        }
        catch (InvalidOperationException e)
        {
            return HandleBadRequest(e, e.InnerException?.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse(StatusCodes.Status500InternalServerError, e.Message));
        }
    }


    /// <summary>
    /// Get site list
    /// </summary>
    /// <param name="requestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("list")]
    public async Task<IActionResult> GetSiteList([FromQuery] GetSiteListRequestDto requestDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var sites = await _repositoryFactory.SiteRepository.GetSites(requestDto, cancellationToken);

            var responseBody = sites.ToResponseDto(requestDto);

            return Ok(new ApiResponse<GetSiteListResponseDto>(HttpStatusCode.OK, responseBody));
        }
        catch (PaginationOutOfRangeException e)
        {
            return HandleBadRequest(e, e.Message);
        }
        catch (InvalidOperationException e)
        {
            return HandleBadRequest(e, e.InnerException?.Message);
        }
        catch (ArgumentOutOfRangeException e)
        {
            return HandleBadRequest(e, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse(StatusCodes.Status500InternalServerError, e.Message));
        }
    }

    /// <summary>
    /// Add site
    /// </summary>
    /// <param name="requestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddSite([FromBody] AddSiteRequestDto requestDto,
        CancellationToken cancellationToken)
    {
        // TODO: change to current user;
        var currentUsername = "HttpRequest"; // HttpContext.Connection.RemoteIpAddress.ToString();

        try
        {
            var sites = await _repositoryFactory.SiteRepository.AddSite(requestDto.Sites,
                currentUsername, cancellationToken);

            return Ok(new ApiResponse<IList<ISite>>(HttpStatusCode.OK, sites.ToList()));
        }
        catch (InvalidOperationException e)
        {
            return HandleBadRequest(e, e.InnerException?.Message);
        }
        catch (RepeatedUniqueValueException e)
        {
            return HandleBadRequest(e, e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse(StatusCodes.Status500InternalServerError, e.Message));
        }
    }

    /// <summary>
    /// Update site
    /// </summary>
    /// <param name="id"></param>
    /// <param name="requestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<IActionResult> UpdateSite([FromQuery] string id, UpdateSiteRequestDto requestDto,
        CancellationToken cancellationToken)
    {
        // TODO: change to current user;
        var currentUsername = "HttpRequest";

        try
        {
            var site = await _repositoryFactory.SiteRepository.UpdateSite(id, requestDto, currentUsername,
                cancellationToken);

            return Ok(new ApiResponse<ISite>(StatusCodes.Status200OK, site));
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
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse(StatusCodes.Status500InternalServerError, e.Message));
        }
    }

    /// <summary>
    /// Delete site
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete]
    public async Task<IActionResult> DeleteSite([FromQuery] string id, CancellationToken cancellationToken)
    {
        try
        {
            if (await _repositoryFactory.SiteRepository.DeleteSite(id, cancellationToken))
            {
                return Ok(new ApiResponse(StatusCodes.Status200OK));
            }
            else
            {
                return StatusCode(StatusCodes.Status406NotAcceptable);
            }
        }
        catch (InvalidOperationException e)
        {
            return HandleBadRequest(e, e.InnerException?.Message);
        }
        catch (KeyNotFoundException e)
        {
            return HandleBadRequest(e, e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse(StatusCodes.Status500InternalServerError, e.Message));
        }
    }
}