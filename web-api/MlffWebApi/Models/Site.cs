using MlffWebApi.Database.DbContexts;
using MlffWebApi.DTO.Site;
using MlffWebApi.Interfaces;
using MlffWebApi.Interfaces.User;

namespace MlffWebApi.Models;

public class Site : ISite, IUserCreatable, IUserModifiable
{
    public string Id { get; set; }
    public string LocationName { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? KilometerMarker { get; set; }
    public string CreatedBy { get; set; }
    public DateTime DateCreated { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime DateModified { get; set; }
}

public class SiteLite : ISiteLite
{
    public string Id { get; set; }
    public string LocationName { get; set; }
    public decimal? KilometerMarker { get; set; }
    public string CameraId { get; set; }
    public string SpeedRadarId { get; set; }
    public string RfidReaderId { get; set; }
}

public static class SiteExtension
{
    public static ISite ToBusinessModel(this site t)
    {
        return new Site
        {
            Id = t.id,
            LocationName = t.location_name?.Trim(),
            Latitude = t.latitude,
            Longitude = t.longitude,
            KilometerMarker = t.kilometer_marker,
            CreatedBy = t.created_by,
            DateCreated = t.date_created,
            ModifiedBy = t.modified_by,
            DateModified = t.date_modified
        };
    }

    public static site ToDatabaseModel(this AddSiteRequestDto.Site t, string username)
    {
        return new site
        {
            id = t.Id,
            location_name = t.LocationName,
            latitude = t.Coordinate?.Latitude,
            longitude = t.Coordinate?.Longitude,
            kilometer_marker = t.KilometerMarker,
            created_by = username,
            date_created = DateTime.Now,
            date_modified = DateTime.Now,
            modified_by = username
        };
    }

    public static ISiteLite ToLite(this ISite t)
    {
        return new SiteLite
        {
            Id = t.Id,
            LocationName = t.LocationName,
            KilometerMarker = t.KilometerMarker
        };
    }
}