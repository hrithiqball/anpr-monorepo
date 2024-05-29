using MlffWebApi.DTO.Watchlist;
using MlffWebApi.Interfaces.ANPR;

namespace MlffWebApi.Interfaces.Repository;

public interface IWatchlistRepository
{
    Task<IPaginationResult<IList<IWatchlist>>> GetWatchlistAsync(GetWatchlistRequestDto requestDto,
        CancellationToken cancellationToken);

    Task<IEnumerable<IWatchlist>> InsertWatchlistAsync(IEnumerable<AddWatchlistRequestDto.Watchlist> watchlists,
        string username, CancellationToken cancellationToken);

    Task<IWatchlist> UpdateWatchlist(Guid id, UpdateWatchlistRequestDto requestDto, CancellationToken cancellationToken);

    Task<bool> DeleteWatchlistAsync(Guid uid, CancellationToken cancellationToken);

    Task<IWatchlist> GetWatchlistByPlateNumberAsync(string plateNumber, CancellationToken cancellationToken);
}