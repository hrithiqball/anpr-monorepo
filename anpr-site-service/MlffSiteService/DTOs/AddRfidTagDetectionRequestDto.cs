using System.ComponentModel.DataAnnotations;
using MlffSiteService.Interface;
using MlffSiteService.Models;
using MlffSiteService.Models.Exceptions;

namespace MlffSiteService.DTOs;

public class AddRfidTagDetectionRequestDto
{
    public AddRfidTagDetectionRequestDto(params IRfidTagDetectionResult[] collections)
    {
        var siteId = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.SITE_ID) ??
                     throw new MissingEnvironmentVariableException(Constants.EnvironmentVariables.SITE_ID);

        foreach (var rfidTagDetection in collections)
        {
            Data.Add(new RfidTagDetectionDetail(rfidTagDetection.Timestamp, siteId, rfidTagDetection.TagId));
        }
    }

    public IList<RfidTagDetectionDetail> Data { get; set; } = new List<RfidTagDetectionDetail>();

    public class RfidTagDetectionDetail
    {
        public RfidTagDetectionDetail(DateTime detectionDate,
            string siteId,
            string tagId)
        {
            DetectionDate = detectionDate;
            SiteId = siteId;
            TagId = tagId;
        }

        [Required]
        public DateTime DetectionDate { get; }

        [Required]
        public string SiteId { get; }

        [Required]
        public string TagId { get; }
    }
}