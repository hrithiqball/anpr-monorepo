using MlffWebApi.Interfaces;
using MlffWebApi.Interfaces.ANPR;
using MlffWebApi.Interfaces.RFID;
using MlffWebApi.Interfaces.Speed;

namespace MlffWebApi.DTO.DetectionMatch;

public class AddDetectionMatchRequestDto
{
    public IEnumerable<DetectionMatchDetail> MatchDetails { get; set; } = Enumerable.Empty<DetectionMatchDetail>();

    public class DetectionMatchDetail
    {
        public string SiteId { get; set; }
        public string TagId { get; set; }
        public string PlateNumber { get; set; }
        public int? Speed { get; set; }
        public DateTime DateMatched { get; set; }
        public string VehicleImagePath { get; set; }
        public string PlateImagePath { get; set; }
    }
}

public static class DetectionMatchExtensions
{
    public static IDetectionMatch ToBusinessModel(
        this AddDetectionMatchRequestDto.DetectionMatchDetail detail, IEnumerable<ISite> sites)
    {
        var site = sites.FirstOrDefault(t =>
            string.Equals(t.Id, detail.SiteId, StringComparison.CurrentCultureIgnoreCase)) ?? throw new KeyNotFoundException($"Site {detail.SiteId} not found.");

        return new Models.DetectionMatch
        {
            Uid = default,
            SiteId = site.Id,
            TagId = detail.TagId,
            PlateNumber = detail.PlateNumber,
            Speed = detail.Speed,
            DateMatched = detail.DateMatched,
            DateCreated = DateTime.Now,
            VehicleImagePath = detail.VehicleImagePath,
            PlateImagePath = detail.PlateImagePath,
        };
    }
}