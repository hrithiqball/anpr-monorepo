using MlffWebApi.Interfaces;
using MlffWebApi.Models;

namespace MlffWebApi.DTO.Site;

public class UpdateSiteRequestDto
{
    public string LocationName { get; set; }
    public Coordinate Coordinate { get; set; }
    public decimal? KilometerMarker { get; set; }
}