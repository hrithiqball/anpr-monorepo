using Microsoft.EntityFrameworkCore;
using MlffWebApi.Database.DbContexts;
using MlffWebApi.DTO.DetectionMatch;
using MlffWebApi.Exceptions;
using MlffWebApi.Interfaces;
using MlffWebApi.Interfaces.Repository;
using MlffWebApi.Models;

namespace MlffWebApi.Repositories;

public class DetectionMatchRepository : IDetectionMatchRepository
{
    private readonly MlffDbContext _dbContext;
    private readonly ILogger<DetectionMatchRepository> _logger;

    public DetectionMatchRepository(MlffDbContext context, ILoggerFactory loggerFactory)
    {
        _dbContext = context;
        _logger = loggerFactory.CreateLogger<DetectionMatchRepository>();
    }

    public async Task<(IList<IDetectionMatch> addedRecords, string details)> AddRecords(
        ICollection<IDetectionMatch> records,
        CancellationToken cancellationToken)
    {
        try
        {
            var resultList = new List<IDetectionMatch>();
            
            foreach (var match in records)
            {
                var target = match.ToDatabaseModel(false);
                await _dbContext.AddAsync(target, cancellationToken);
                var tmp = target.ToBusinessModel();
                tmp.PlateImagePath = match.PlateImagePath;
                tmp.VehicleImagePath = match.VehicleImagePath;
                resultList.Add(tmp);
            }

            var addCount = await _dbContext.SaveChangesAsync(cancellationToken);

            return (resultList, $"Added {addCount} detection match(es).");
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    public async Task<IPaginationResult<IEnumerable<IDetectionMatch>>> GetRecords(
        GetDetectionMatchListRequestDto requestDto,
        CancellationToken cancellationToken)
    {
        try
        {
            if (requestDto is null)
            {
                throw new ArgumentNullException(nameof(requestDto));
            }

            var query = _dbContext.detection_matches.AsQueryable();

            if (requestDto.MatchedDateFrom.HasValue)
            {
                query = query.Where(t => t.date_matched >= requestDto.MatchedDateFrom.Value);
            }

            if (requestDto.MatchedDateTo.HasValue)
            {
                query = query.Where(t => t.date_matched <= requestDto.MatchedDateTo.Value);
            }

            if (requestDto.MinSpeed.HasValue)
            {
                query = query.Where(t => t.speed >= requestDto.MinSpeed);
            }

            if (requestDto.MaxSpeed.HasValue)
            {
                query = query.Where(t => t.speed <= requestDto.MaxSpeed);
            }

            IQueryable<detection_match> tempQuery1 = null;

            foreach (var siteId in requestDto.SiteIdList)
            {
                var subQuery = query.Where(t => t.site_id.ToLower().Contains(siteId.ToLower()));
                tempQuery1 = tempQuery1 == null ? subQuery : tempQuery1.Union(subQuery);
            }

            IQueryable<detection_match> tempQuery2 = null;
            foreach (var numberPlate in requestDto.NumberPlateList)
            {
                var subQuery = query.Where(t => t.plate_number.ToLower().Contains(numberPlate.ToLower()));
                tempQuery2 = tempQuery2 == null ? subQuery : tempQuery2.Union(subQuery);
            }

            var finalQuery = query;
            if (tempQuery1 is not null) finalQuery = finalQuery.Intersect(tempQuery1);
            if (tempQuery2 is not null) finalQuery = finalQuery.Intersect(tempQuery2);

            switch (requestDto.SortBy)
            {
                case DetectionMatchSortByOptions.DATE_MATCHED:
                    finalQuery = requestDto.IsAscending
                        ? finalQuery.OrderBy(t => t.date_matched)
                        : finalQuery.OrderByDescending(t => t.date_matched);
                    break;
                case DetectionMatchSortByOptions.SITE_ID:
                    finalQuery = requestDto.IsAscending
                        ? finalQuery.OrderBy(t => t.site_id)
                        : finalQuery.OrderByDescending(t => t.site_id);
                    break;
                case DetectionMatchSortByOptions.PLATE_NUMBER:
                    finalQuery = requestDto.IsAscending
                        ? finalQuery.OrderBy(t => t.plate_number)
                        : finalQuery.OrderByDescending(t => t.plate_number);
                    break;
                case DetectionMatchSortByOptions.SPEED:
                    finalQuery = requestDto.IsAscending
                        ? finalQuery.OrderBy(t => t.speed)
                        : finalQuery.OrderByDescending(t => t.speed);
                    break;
                case DetectionMatchSortByOptions.TAG_ID:
                    finalQuery = requestDto.IsAscending
                        ? finalQuery.OrderBy(t => t.tag_id)
                        : finalQuery.OrderByDescending(t => t.tag_id);
                    break;
                default:
                    var validOptions = Enum.GetNames(typeof(DetectionMatchSortByOptions));
                    throw new ArgumentOutOfRangeException($"Invalid sort options. Valid options are " +
                                                          $"{string.Join(",", validOptions.Take(validOptions.Length - 1))} " +
                                                          $"or {validOptions.Last()} ");
            }

            var totalCount = finalQuery.Count();
            var totalPage = (int)Math.Ceiling(totalCount / (decimal)requestDto.PageSize);
            int pageNumber;
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

            if (totalCount > 0 && pageNumber > totalPage)
            {
                throw new PaginationOutOfRangeException("Page number exceed total page available.");
            }

            var data = await finalQuery
                .Skip(skipIndex)
                .Take(requestDto.PageSize)
                .Select(t => t.ToBusinessModel())
                .ToListAsync(cancellationToken);

            return new PaginationResult<IEnumerable<IDetectionMatch>>
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
}