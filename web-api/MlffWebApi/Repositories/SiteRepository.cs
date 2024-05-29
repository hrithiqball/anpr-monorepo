using Microsoft.EntityFrameworkCore;
using MlffWebApi.Database.DbContexts;
using MlffWebApi.DTO.Site;
using MlffWebApi.Exceptions;
using MlffWebApi.Interfaces;
using MlffWebApi.Interfaces.Repository;
using MlffWebApi.Models;

namespace MlffWebApi.Repositories;

public class SiteRepository : ISiteRepository
{
    private readonly MlffDbContext _context;
    private readonly ILogger<SiteRepository> _logger;

    public SiteRepository(MlffDbContext context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = loggerFactory.CreateLogger<SiteRepository>();
    }

    public async Task<ISite> GetSite(string id, CancellationToken cancellationToken)
    {
        try
        {
            return await _context.sites
                .Where(t => t.id.ToLower() == id.ToLower())
                .Select(t => t.ToBusinessModel())
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<IList<ISite>> GetSites(CancellationToken cancellationToken)
    {
        try
        {
            return await _context.sites
                .Select(t => t.ToBusinessModel())
                .ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<IPaginationResult<IList<ISite>>> GetSites(GetSiteListRequestDto requestDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.sites
                .AsQueryable();

            switch (requestDto.SortBy)
            {
                case SiteSortByOptions.ID:
                    query = requestDto.IsAscending
                        ? query.OrderBy(t => t.id)
                        : query.OrderByDescending(t => t.id);
                    break;
                case SiteSortByOptions.LOCATION_NAME:
                    query = requestDto.IsAscending
                        ? query.OrderBy(t => t.location_name)
                        : query.OrderByDescending(t => t.location_name);
                    break;
                case SiteSortByOptions.KILOMETER_MARKER:
                    query = requestDto.IsAscending
                        ? query.OrderBy(t => t.kilometer_marker)
                        : query.OrderByDescending(t => t.kilometer_marker);
                    break;
                case SiteSortByOptions.DATE_MODIFIED:
                    query = requestDto.IsAscending
                        ? query.OrderBy(t => t.date_modified)
                        : query.OrderByDescending(t => t.date_modified);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Invalid sort options.");
            }

            var totalCount = query.Count();
            var totalPage = (int)Math.Ceiling(totalCount / (decimal)requestDto.PageSize);
            var pageNumber = 0;
            if (requestDto.PageNumber < 0)
            {
                pageNumber = totalPage - Math.Abs(requestDto.PageNumber) + 1;
            }
            else
            {
                pageNumber = requestDto.PageNumber;
            }

            var skipIndex = (pageNumber - 1) * requestDto.PageSize;

            if (skipIndex < 0)
            {
                throw new PaginationOutOfRangeException(
                    "Invalid page number, page number is not within available page range");
            }

            if (totalCount == 0)
            {
                return new PaginationResult<IList<ISite>>
                {
                    TotalCount = totalCount,
                    PageSize = requestDto.PageSize,
                    PageNumber = pageNumber,
                    Data = new List<ISite>()
                };
            }

            if (pageNumber > totalPage)
            {
                throw new PaginationOutOfRangeException("Page number exceed total page available.");
            }

            var data = await query
                .Skip(skipIndex)
                .Take(requestDto.PageSize)
                .Select(t => t.ToBusinessModel())
                .ToListAsync(cancellationToken);

            return new PaginationResult<IList<ISite>>
            {
                TotalCount = totalCount,
                PageSize = requestDto.PageSize,
                PageNumber = pageNumber,
                Data = data
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<IEnumerable<ISite>> AddSite(IEnumerable<AddSiteRequestDto.Site> sites, string currentUsername,
        CancellationToken cancellationToken)
    {
        try
        {
            if (sites is null)
            {
                throw new ArgumentNullException(nameof(sites));
            }

            var resultList = new List<ISite>();
            
            foreach (var site in sites)
            {
                var target = await InsertSite(site, currentUsername, cancellationToken);
                resultList.Add(target);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return resultList;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<ISite> AddSite(AddSiteRequestDto.Site site, string currentUsername,
        CancellationToken cancellationToken)
    {
        try
        {
            if (site is null)
            {
                throw new ArgumentNullException(nameof(site));
            }

            var target = await InsertSite(site, currentUsername, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return target;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    private async Task<ISite> InsertSite(AddSiteRequestDto.Site site, string currentUsername, CancellationToken cancellationToken)
    {
        var target = await _context.sites.FirstOrDefaultAsync(t => t.id == site.Id, cancellationToken);

        if (target == null)
        {
            target = site.ToDatabaseModel(currentUsername);
            await _context.sites.AddAsync(target, cancellationToken);
        }
        else
        {
            throw new RepeatedUniqueValueException($"Site {site.Id} is existed");
        }

        return target.ToBusinessModel();
    }

    public async Task<ISite> UpdateSite(string id, UpdateSiteRequestDto requestDto, string currentUser,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (requestDto is null)
            {
                throw new ArgumentNullException(nameof(requestDto));
            }

            var target = await _context.sites.FirstOrDefaultAsync(t => t.id == id,cancellationToken);

            if (target == null)
            {
                throw new KeyNotFoundException($"Site {id} not found.");
            }

            target.kilometer_marker = requestDto.KilometerMarker;
            target.location_name = requestDto.LocationName;
            target.latitude = requestDto.Coordinate?.Latitude;
            target.longitude = requestDto.Coordinate?.Longitude;
            target.modified_by = currentUser;
            target.date_modified = DateTime.Now;

            _context.sites.Update(target);
            await _context.SaveChangesAsync(cancellationToken);
            return target.ToBusinessModel();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<bool> DeleteSite(string id, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var target = await _context.sites.FindAsync(id);

            if (target == null)
            {
                throw new KeyNotFoundException($"Site {id} not found.");
            }

            _context.sites.Remove(target);
            var rowChanged = await _context.SaveChangesAsync(cancellationToken);
            return rowChanged == 1;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }
}