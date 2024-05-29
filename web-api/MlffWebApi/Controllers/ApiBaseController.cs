using Microsoft.AspNetCore.Mvc;
using MlffWebApi.DTO;

namespace MlffWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApiBaseController : ControllerBase
{
    private readonly ILogger _logger;
    public ApiBaseController(ILogger logger)
    {
        _logger = logger;
    }
    
    internal IActionResult HandleBadRequest(Exception e, string messageInResponse)
    {
        // when db offline, it will throw invalid operation exception here
        _logger.LogError(e, e.Message);
        return StatusCode(StatusCodes.Status400BadRequest,
            new ApiResponse(StatusCodes.Status400BadRequest, messageInResponse));
    }
}