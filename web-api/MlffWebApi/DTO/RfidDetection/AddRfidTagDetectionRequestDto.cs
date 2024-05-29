using System.ComponentModel.DataAnnotations;
using MlffWebApi.Interfaces;
using MlffWebApi.Interfaces.RFID;
using MlffWebApi.Models.RFID;

namespace MlffWebApi.DTO.RfidDetection;

public class AddRfidTagDetectionRequestDto
{
    public IEnumerable<RfidTagDetectionDetail> Data { get; set; }

    public class RfidTagDetectionDetail
    {
        [Required]
        public DateTime DetectionDate { get; set; }

        [Required]
        public string SiteId { get; set; }

        [Required]
        public string TagId { get; set; }
    }
}

public static class RfidTagDetectionExtensions
{
    public static IRfidTagDetection ToBusinessModel(
        this AddRfidTagDetectionRequestDto.RfidTagDetectionDetail detail, IEnumerable<ISite> sites)
    {
        var site = sites.FirstOrDefault(t =>
            string.Equals(t.Id, detail.SiteId, StringComparison.CurrentCultureIgnoreCase));

        if (site is null)
        {
            throw new KeyNotFoundException($"Site {detail.SiteId} not found.");
        }

        return new RfidTagDetection
        {
            TagId = detail.TagId,
            DetectionDate = detail.DetectionDate,
            SiteId = detail.SiteId
        };
    }
}