using System.ComponentModel.DataAnnotations;
using MlffSiteService.Interface;
using MlffSiteService.Models;
using MlffSiteService.Models.Exceptions;

namespace MlffSiteService.DTOs;

public class AddPublicIPRequestDto
{
    public AddPublicIPRequestDto(string dynamicIP)
    {
        var siteId = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.SITE_ID) ??
                     throw new MissingEnvironmentVariableException(Constants.EnvironmentVariables.SITE_ID);

        DateTime dateUpdate = DateTime.Now;
        Data.Add(new PublicIPDetectionDetail(dateUpdate, siteId, dynamicIP));
    }

    public IList<PublicIPDetectionDetail> Data { get; } = new List<PublicIPDetectionDetail>();

    public class PublicIPDetectionDetail
    {
        public PublicIPDetectionDetail(DateTime dateUpdate, string siteId, string publicIPString)
        {
            DateUpdate = dateUpdate;
            SiteId = siteId;
            PublicIPString = publicIPString;
        }

        [Required]
        public DateTime DateUpdate { get; set; }

        [Required]
        public string SiteId { get; set; }

        [Required]
        public string PublicIPString { get; set; }
    }
}