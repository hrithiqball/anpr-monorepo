using MlffWebApi.Interfaces;
using MlffWebApi.Models;

namespace MlffWebApi.DTO.Site;

public class AddSiteRequestDto
{
    public IList<Site> Sites { get; set; }
    
    public class Site
    {
        public string Id { get; set; }
        public string LocationName { get; set; }
        public Coordinate Coordinate { get; set; }
        public decimal? KilometerMarker { get; set; }
    }
}