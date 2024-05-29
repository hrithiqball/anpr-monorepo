using MlffWebApi.DTO.Pagination;
using MlffWebApi.Interfaces;
using MlffWebApi.Interfaces.ANPR;

namespace MlffWebApi.DTO.Watchlist;

public class GetWatchlistRequestDto : PaginationRequestDto
{
    public WatchlistMonitorOptions MonitorOptions { get; set; } = WatchlistMonitorOptions.LicensePlate;
    public string Values { get; set; }
}

public class GetWatchlistResponseDto : PaginationResponseDto
{
    public IList<IWatchlist> Watchlists { get; set; }
}

public static class GetWatchlistExtension
{
    public static GetWatchlistResponseDto ToResponseDto(this IPaginationResult<IList<IWatchlist>> tmp)
    {
        return new GetWatchlistResponseDto
        {
            PageSize = tmp.PageSize,
            PageNumber = tmp.PageNumber,
            TotalCount = tmp.TotalCount,
            Watchlists = tmp.Data
        };
    }
}