using MlffWebApi.Extensions;
using MlffWebApi.Interfaces;

namespace MlffWebApi.Models;

public class Coordinate : ICoordinate
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    public Coordinate()
    {
    }

    public Coordinate(decimal latitude, decimal longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public Coordinate(double latitude, double longitude)
    {
        Latitude = latitude.ToDecimal();
        Longitude = longitude.ToDecimal();
    }
}