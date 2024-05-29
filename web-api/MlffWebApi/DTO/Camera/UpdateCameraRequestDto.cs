using MlffWebApi.Models;

namespace MlffWebApi.DTO.Camera;

public class UpdateCameraRequestDto
{
    public string DisplayName { get; set; }
    public string Tag { get; set; }
}