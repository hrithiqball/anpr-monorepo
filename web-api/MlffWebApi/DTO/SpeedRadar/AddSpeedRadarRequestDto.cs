using System.ComponentModel.DataAnnotations;

namespace MlffWebApi.DTO.SpeedRadar;

public class AddSpeedRadarRequestDto
{
    public IList<SpeedRadar> SpeedRadars { get; set; }

    /// <summary>
    /// This model is just for frontend XHR to map, only limit to those properties that frontend allowed to pass in
    /// </summary>
    public class SpeedRadar
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string DisplayName { get; set; }
        public string Tag { get; set; } 
    }
}
