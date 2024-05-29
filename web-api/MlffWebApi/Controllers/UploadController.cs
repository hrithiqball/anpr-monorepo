using MlffWebApi.DTO;
using MlffWebApi.DTO.Upload;
using MlffWebApi.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace MlffWebApi.Controllers;

public class UploadController : ApiBaseController
{
    private readonly ILogger<UploadController> _logger;
    private readonly IUploaderService _uploaderService;

    public UploadController(ILogger<UploadController> logger, IUploaderService uploaderService) : base(logger)
    {
        _logger = logger;
        _uploaderService = uploaderService;
    }

    /// <summary>
    /// Upload ANPR related images
    /// </summary>
    /// <param name="requestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("anpr")]
    public async Task<IActionResult> UploadAnprImage([FromForm] UploadAnprRequestDto requestDto,
        CancellationToken cancellationToken)
    {
        try
        {
            //todo fix the write image part to combine but use flag
            // var vehicleImagePath = await _uploaderService.WriteImageVehicleAsync(requestDto.VehicleImage,
            //     $"{requestDto.ImageId}_{requestDto.NumberPlate}_vehicle", cancellationToken);
            // var plateImagePath = await _uploaderService.WriteImagePlateAsync(requestDto.PlateImage,
            //     $"{requestDto.ImageId}_{requestDto.NumberPlate}_plate", cancellationToken);
            //var plateImagePathOld = await _uploaderService.WriteImageAsync(requestDto.PlateImage, $"{requestDto.ImageId}_{requestDto.NumberPlate}_plate", cancellationToken);
            var plateImagePathOld = await _uploaderService.WriteImageAsync(requestDto.VehicleImage,
                $"{requestDto.ImageId}_{requestDto.NumberPlate}_vehicle", cancellationToken);
            var vehicleImagePathOld = await _uploaderService.WriteImageAsync(requestDto.VehicleImage,
                $"{requestDto.ImageId}_{requestDto.NumberPlate}_vehicle", cancellationToken);

            var responseDto = new UploadAnprResponseDto
            {
                PlateImageUrl = plateImagePathOld,
                VehicleImageUrl = vehicleImagePathOld
            };
            
            _logger.LogCritical("DTO Response here");
            return Ok(new ApiResponse<UploadAnprResponseDto>(StatusCodes.Status200OK, responseDto));
        }
        catch (Exception e)
        {
            _logger.LogCritical("HERE PINPOINT ERROR DTO");
            _logger.LogError(e, e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse(StatusCodes.Status500InternalServerError, "Error in upload ANPR image"));
        }
    }
}