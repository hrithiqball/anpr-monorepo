using MlffWebApi.Database.DbContexts;
using MlffWebApi.DTO.Watchlist;
using MlffWebApi.Exceptions;
using MlffWebApi.Interfaces;
using MlffWebApi.Interfaces.ANPR;
using MlffWebApi.Interfaces.Repository;
using MlffWebApi.Models;
using Microsoft.EntityFrameworkCore;
using MlffWebApi.DTO.Site;

namespace MlffWebApi.Repositories;

public class WatchlistRepository : IWatchlistRepository
{
    private readonly MlffDbContext _context;
    private readonly ILogger<WatchlistRepository> _logger;

    public WatchlistRepository(MlffDbContext context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = loggerFactory.CreateLogger<WatchlistRepository>();
    }

    public async Task<IPaginationResult<IList<IWatchlist>>> GetWatchlistAsync(
        GetWatchlistRequestDto requestDto,
        CancellationToken cancellationToken
    )
    {
        var query = _context.watchlists.AsQueryable();
        if (requestDto.MonitorOptions == WatchlistMonitorOptions.LicensePlate)
        {
            query = query.Where(t => t.monitor_option == (int)WatchlistMonitorOptions.LicensePlate);
            if (!string.IsNullOrEmpty(requestDto.Values))
            {
                query = query.Where(t => requestDto.Values.ToLower().Contains(t.value.ToLower()));
            }
        }
        /*
        // currently not supported yet
        else if (requestDto.MonitorOptions == WatchlistMonitorOptions.Speed)
        {
            query = query.Where(t => t.monitor_option == (int)WatchlistMonitorOptions.Speed);
            if (!string.IsNullOrEmpty(requestDto.Values))
            {
                var speed = int.Parse(requestDto.Values);
                query = query.Where(t => int.Parse(t.value) >= speed);
            }
        }*/

        var totalCount = query.Count();

        if (totalCount == 0)
        {
            return new PaginationResult<IList<IWatchlist>>
            {
                TotalCount = 0,
                PageSize = requestDto.PageSize,
                Data = Enumerable.Empty<IWatchlist>().ToList()
            };
        }

        var pageNumber = requestDto.PageNumber;
        var totalPage = (int)Math.Ceiling((decimal)totalCount / (decimal)requestDto.PageSize);
        var skipIndex = (requestDto.PageNumber - 1) * requestDto.PageSize;
        var takeSize = requestDto.PageSize;

        if (skipIndex < 0)
        {
            throw new PaginationOutOfRangeException(
                "Invalid page number, page number is not within available page range"
            );
        }

        if (pageNumber > totalPage)
        {
            throw new PaginationOutOfRangeException("Page number exceed total page available.");
        }

        var list = await query
            .OrderBy(t => t.value)
            .Skip(skipIndex)
            .Take(takeSize)
            .Select(t => t.ToBusinessModel())
            .ToListAsync(cancellationToken);

        return new PaginationResult<IList<IWatchlist>>
        {
            TotalCount = totalCount,
            PageSize = takeSize,
            PageNumber = pageNumber,
            Data = list
        };
    }

    public async Task<IEnumerable<IWatchlist>> InsertWatchlistAsync(
        IEnumerable<AddWatchlistRequestDto.Watchlist> watchlists,
        string username,
        CancellationToken cancellationToken
    )
    {
        if (watchlists is null)
        {
            throw new ArgumentNullException(nameof(watchlists));
        }

        var entities = watchlists.Select(t => t.ToDatabaseModel(username)).ToList();

        foreach (var entity in entities)
        {
            if (entity.monitor_option == (int)WatchlistMonitorOptions.LicensePlate)
            {
                if (_context.watchlists.Any(t => t.value.ToLower() == entity.value.ToLower()))
                {
                    throw new RepeatedUniqueValueException(
                        $"License plate {entity.value} existed."
                    );
                }

                await _context.watchlists.AddAsync(entity, cancellationToken);
            }
        }

        await _context.watchlists.AddRangeAsync(entities, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entities.Select(t => t.ToBusinessModel());
    }

    public async Task<IWatchlist> UpdateWatchlist(
        Guid id,
        UpdateWatchlistRequestDto requestDto,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (requestDto == null)
            {
                throw new ArgumentNullException(nameof(requestDto));
            }

            var target = await _context.watchlists.FirstOrDefaultAsync(
                t => t.uid == id,
                cancellationToken
            ) ?? throw new KeyNotFoundException($"Watchlist {id} not found.");

            target.value = requestDto.Value;
            target.remarks = requestDto.Remarks;
            target.tag_color = requestDto.TagColor;


            _context.watchlists.Update(target);
            await _context.SaveChangesAsync(cancellationToken);
            return target.ToBusinessModel();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<bool> DeleteWatchlistAsync(Guid uid, CancellationToken cancellationToken)
    {
        var target = _context.watchlists.Where(t => t.uid == uid) ?? throw new KeyNotFoundException($"Watchlist {uid} not found.");
        _context.watchlists.RemoveRange(target);
        var rowChanges = await _context.SaveChangesAsync(cancellationToken);
        return rowChanges == 1;
    }

    public async Task<IWatchlist> GetWatchlistByPlateNumberAsync(
        string plateNumber,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrEmpty(plateNumber))
        {
            throw new ArgumentNullException(nameof(plateNumber));
        }

        var watchlist = await _context.watchlists.FirstOrDefaultAsync(
            t =>
                t.monitor_option == (int)WatchlistMonitorOptions.LicensePlate
                && t.value == plateNumber,
            cancellationToken
        );

        return watchlist?.ToBusinessModel() ?? null;
    }
}
