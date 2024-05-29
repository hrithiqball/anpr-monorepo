using MlffWebApi.Interfaces.ANPR;
using MlffWebApi.Interfaces.RFID;
using MlffWebApi.Interfaces.Speed;

namespace MlffWebApi.Interfaces;

public interface IDetectionMatch : IDetectionMatchLite
{
    Guid Uid { get; set; }
    DateTime DateMatched { get; set; }
    DateTime DateCreated { get; set; }
    bool Verified { get; set; }
    bool? Correctness { get; set; }
}

public interface IDetectionMatchLite
{
    string SiteId { get; set; }
    string TagId { get; set; }
    string PlateNumber { get; set; }
    int? Speed { get; set; }
    bool IsInsideWatchlist { get; set; }
    string VehicleImagePath { get; set; }
    string PlateImagePath { get; set; }
}