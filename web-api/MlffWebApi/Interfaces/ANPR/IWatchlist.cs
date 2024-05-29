using MlffWebApi.Interfaces.User;

namespace MlffWebApi.Interfaces.ANPR;

public interface IWatchlist : IUserCreatable, IUserModifiable
{
    Guid Uid { get; set; }
    /// <summary>
    /// Monitor by license plate, or speed. Trigger when the detection meet the <see cref="Value"/> 
    /// </summary>
    WatchlistMonitorOptions MonitorOption { get; set; }
    /// <summary>
    /// Value that triggering the PUB/SUB scenario
    /// </summary>
    string Value { get; set; }
    string Remarks { get; set; }
    string TagColor { get; set; }
}

public enum WatchlistMonitorOptions
{
    LicensePlate = 1,
    //Speed = 2 // currently not supported yet
}