using Microsoft.EntityFrameworkCore;
using MlffWebApi.Database.DbContexts;
using MlffWebApi.DTO.SpeedDetection;
using MlffWebApi.Exceptions;
using MlffWebApi.Interfaces;
using MlffWebApi.Interfaces.Repository;
using MlffWebApi.Interfaces.RFID;
using MlffWebApi.Interfaces.Speed;
using MlffWebApi.Models;
using MlffWebApi.Models.Speed;

namespace MlffWebApi.Repositories;

public class SpeedDetectionRepository : ISpeedDetectionRepository
{
    private readonly MlffDbContext _context;
    private readonly ILogger<SpeedDetectionRepository> _logger;

    public SpeedDetectionRepository(MlffDbContext context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = loggerFactory.CreateLogger<SpeedDetectionRepository>();
    }

    public async Task<IPaginationResult<IEnumerable<ISpeedDetection>>> GetRecords(
        GetSpeedDetectionListRequestDto requestDto,
        CancellationToken cancellationToken)
    {
        try
        {
            if (requestDto is null)
            {
                throw new ArgumentNullException(nameof(requestDto));
            }

            var query = _context.speed_detections.AsQueryable();

            if (requestDto.DetectionDateFrom.HasValue)
            {
                query = query.Where(t => t.date_detection >= requestDto.DetectionDateFrom.Value);
            }

            if (requestDto.DetectionDateTo.HasValue)
            {
                query = query.Where(t => t.date_detection <= requestDto.DetectionDateTo.Value);
            }

            if (requestDto.SpeedMinimum.HasValue)
            {
                query = query.Where(t => t.speed_kmh >= requestDto.SpeedMinimum);
            }

            if (requestDto.SpeedMaximum.HasValue)
            {
                query = query.Where(t => t.speed_kmh <= requestDto.SpeedMaximum);
            }

            switch (requestDto.SortBy)
            {
                case SpeedDetectionSortByOptions.DETECTION_DATE:
                    query = requestDto.IsAscending
                        ? query.OrderBy(t => t.date_detection)
                        : query.OrderByDescending(t => t.date_detection);
                    break;
                case SpeedDetectionSortByOptions.SPEED:
                    query = requestDto.IsAscending
                        ? query.OrderBy(t => t.speed_kmh)
                        : query.OrderByDescending(t => t.speed_kmh);
                    break;
                default:
                    var validOptions = Enum.GetNames(typeof(SpeedDetectionSortByOptions));
                    throw new ArgumentOutOfRangeException($"Invalid sort options. Valid options are " +
                                                          $"{string.Join(",", validOptions.Take(validOptions.Length - 1))} " +
                                                          $"or {validOptions.Last()} ");
            }

            var totalCount = query.Count();
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

            var data = await query
                .Skip(skipIndex)
                .Take(requestDto.PageSize)
                .Select(t => t.ToBusinessModel())
                .ToListAsync(cancellationToken);

            return new PaginationResult<IEnumerable<ISpeedDetection>>
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

    public async Task<ISpeedDetection> AddRecord(ISpeedDetection record, CancellationToken cancellationToken)
    {
        try
        {
            var target = record.ToDatabaseModel();
            await _context.speed_detections.AddAsync(target, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return target.ToBusinessModel();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<(IList<ISpeedDetection> updatedRecords, string details)> AddRecords(
        ICollection<ISpeedDetection> records, CancellationToken cancellationToken)
    {
        try
        {
            var resultList = new List<ISpeedDetection>();
            foreach (var record in records)
            {
                var target = record.ToDatabaseModel();
                await _context.speed_detections.AddAsync(target, cancellationToken);
                resultList.Add(target.ToBusinessModel());
            }

            var rowAddedCount = await _context.SaveChangesAsync(cancellationToken);

            var details = $"{rowAddedCount} rows inserted";
            return (resultList, details);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }
}