using MlffWebApi.Interfaces.ANPR;
using MlffWebApi.Interfaces.RFID;
using MlffWebApi.Interfaces.Speed;

namespace MlffWebApi.Interfaces;

public interface ISite:ISiteLite
{
    decimal? Latitude { get; set; }
    decimal? Longitude { get; set; }
}

public interface ISiteLite
{
    string Id { get; set; }
    string LocationName { get; set; }
    decimal? KilometerMarker { get; set; }
}