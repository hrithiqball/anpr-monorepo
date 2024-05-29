using MlffWebApi.Interfaces.ANPR;

namespace MlffWebApi.DTO.Watchlist;

public class AddWatchlistRequestDto
{
    public ICollection<Watchlist> Watchlists { get; set; }

    public class Watchlist
    {
        /// <summary>
        /// Monitor options. 1 - license plate, 2 - speed
        /// </summary>
        public WatchlistMonitorOptions MonitorOption { get; set; } = WatchlistMonitorOptions.LicensePlate;

        /// <summary>
        /// Can be number or text. Number for speed, text for license plate. Refer to <see cref="MonitorOption"/>
        /// </summary>
        public string Value { get; set; }
        public string Remarks { get; set; }
        public string TagColor { get; set; }
    }
}