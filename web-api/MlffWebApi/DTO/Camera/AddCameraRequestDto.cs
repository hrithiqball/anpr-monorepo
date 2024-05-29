using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MlffWebApi.Models;

namespace MlffWebApi.DTO.Camera;

public class AddCameraRequestDto
{
    public IList<Camera> Cameras { get; set; }

    /// <summary>
    /// This model is just for frontend XHR to map, only limit to those properties that frontend allowed to pass in
    /// </summary>
    public class Camera
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string DisplayName { get; set; }
        public string Tag { get; set; } 
    }
}
