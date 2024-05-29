using System.ComponentModel.DataAnnotations;
using MlffWebApi.Interfaces;
using MlffWebApi.Interfaces.PublicIP;
using MlffWebApi.Models.PublicIP;

namespace MlffWebApi.DTO.IP;

public class AddPublicIPDto
{
    public IEnumerable<PublicIPDetailRecognition> Data { get; set; }

    public class PublicIPDetailRecognition
    {
        [Required]
        public string SiteId { get; set; }

        [Required]
        public string PublicIPString { get; set; }

        [Required]
        public DateTime DateUpdate { get; set; }
    }
}

public static class PublicIPRecognitionExtensions
{
   

    public static IPublicIPRecognition ToBusinessModel(this AddPublicIPDto.PublicIPDetailRecognition detail, IEnumerable<ISite> sites)
    {
        var site = sites.FirstOrDefault(t => string.Equals(t.Id, detail.SiteId, StringComparison.CurrentCultureIgnoreCase));

        if (site is null)
        {
            throw new KeyNotFoundException($"Site {detail.SiteId} not found");
        }

        return new Models.PublicIP.PublicIPRecognition
        {
            SiteId = site.Id,
            DateUpdate = detail.DateUpdate,
            PublicIPString = detail.PublicIPString
        };
    }
}