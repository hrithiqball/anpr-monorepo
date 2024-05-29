using System.ComponentModel.DataAnnotations;
using MlffWebApi.Interfaces;
using MlffWebApi.Interfaces.Speed;

namespace MlffWebApi.DTO.SpeedDetection;

public class AddSpeedDetectionRequestDto
{
    public IEnumerable<SpeedDetectionDetail> Data { get; set; }

    public class SpeedDetectionDetail
    {
        [Required]
        public DateTime DetectionDate { get; set; }

        [Required]
        public string SiteId { get; set; }

        [Required]
        public int Speed { get; set; }
    }
}

public static class SpeedDetectionExtensions
{
    public static ISpeedDetection ToBusinessModel(
        this AddSpeedDetectionRequestDto.SpeedDetectionDetail detail, IEnumerable<ISite> sites)
    {
        var site = sites.FirstOrDefault(t =>
            string.Equals(t.Id, detail.SiteId, StringComparison.CurrentCultureIgnoreCase));

        if (site is null)
        {
            throw new KeyNotFoundException($"Site {detail.SiteId} not found.");
        }

        return new Models.Speed.SpeedDetection
        {
            Speed = detail.Speed,
            DetectionDate = detail.DetectionDate,
            SiteId = site.Id
        };
    }

}