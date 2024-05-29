using Microsoft.EntityFrameworkCore;
using MlffWebApi.Database.DbContexts;
using MlffWebApi.DTO.RfidDetection;
using MlffWebApi.Exceptions;
using MlffWebApi.Interfaces;
using MlffWebApi.Interfaces.Repository;
using MlffWebApi.Interfaces.RFID;
using MlffWebApi.Models;
using MlffWebApi.Models.RFID;

namespace MlffWebApi.Repositories;

public class RfidDetectionRepository : IRfidDetectionRepository
{
    private readonly MlffDbContext _context;
    private readonly ILogger<RfidDetectionRepository> _logger;

    public RfidDetectionRepository(MlffDbContext context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = loggerFactory.CreateLogger<RfidDetectionRepository>();
    }

    public async Task<IPaginationResult<IEnumerable<IRfidTagDetection>>> GetRecords(
        GetRfidTagDetectionListRequestDto requestDto, CancellationToken cancellationToken)
    {
        try
        {
            if (requestDto is null)
            {
                throw new ArgumentNullException(nameof(requestDto));
            }

            var query = _context.rfid_detections.AsQueryable();

            if (requestDto.DetectionDateFrom.HasValue)
            {
                query = query.Where(t => t.date_detection >= requestDto.DetectionDateFrom.Value);
            }

            if (requestDto.DetectionDateTo.HasValue)
            {
                query = query.Where(t => t.date_detection <= requestDto.DetectionDateTo.Value);
            }

            IQueryable<rfid_detection> tempQuery1 = null;

            foreach (var siteId in requestDto.SiteIdList)
            {
                var subQuery = query.Where(t => t.site_id.ToLower().Contains(siteId.ToLower()));
                tempQuery1 = tempQuery1 == null ? subQuery : tempQuery1.Union(subQuery);
            }

            IQueryable<rfid_detection> tempQuery2 = null;
            foreach (var numberPlate in requestDto.TagIdList)
            {
                var subQuery = query.Where(t => t.tag_id.ToLower().Contains(numberPlate.ToLower()));
                tempQuery2 = tempQuery2 == null ? subQuery : tempQuery2.Union(subQuery);
            }

            var finalQuery = query;
            if (tempQuery1 is not null) finalQuery = finalQuery.Intersect(tempQuery1);
            if (tempQuery2 is not null) finalQuery = finalQuery.Intersect(tempQuery2);

            switch (requestDto.SortBy)
            {
                case RfidTagDetectionSortByOptions.DETECTION_DATE:
                    finalQuery = requestDto.IsAscending
                        ? finalQuery.OrderBy(t => t.date_detection)
                        : finalQuery.OrderByDescending(t => t.date_detection);
                    break;
                case RfidTagDetectionSortByOptions.TAG_ID:
                    finalQuery = requestDto.IsAscending
                        ? finalQuery.OrderBy(t => t.tag_id)
                        : finalQuery.OrderByDescending(t => t.tag_id);
                    break;
                default:
                    var validOptions = Enum.GetNames(typeof(RfidTagDetectionSortByOptions));
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

            return new PaginationResult<IEnumerable<IRfidTagDetection>>
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

    public async Task<IRfidTagDetection> AddRecordIfNotExist(IRfidTagDetection record,
        CancellationToken cancellationToken)
    {
        try
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            var target = await _context.rfid_detections.FirstOrDefaultAsync(t =>
                t.site_id == record.SiteId &&
                t.tag_id == record.TagId &&
                t.date_detection == record.DetectionDate, cancellationToken);

            await InsertOrUpdateRfidTag(record, cancellationToken, ref target);

            await _context.SaveChangesAsync(cancellationToken);

            return target.ToBusinessModel();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<(IList<IRfidTagDetection> updatedRecords, string details)> AddRecordsIfNotExist(
        ICollection<IRfidTagDetection> records, CancellationToken cancellationToken)
    {
        try
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            var maxDetectionDate = records.MaxBy(t => t.DetectionDate)?.DetectionDate;
            var minDetectionDate = records.MinBy(t => t.DetectionDate)?.DetectionDate;
            var targets = _context.rfid_detections.Where(t =>
                t.date_detection >= minDetectionDate && t.date_detection <= maxDetectionDate);

            var resultList = new List<IRfidTagDetection>();
            var updateRow = 0;
            var insertRow = 0;

            foreach (var record in records)
            {
                var target = targets.FirstOrDefault(t =>
                    t.site_id == record.SiteId &&
                    t.tag_id == record.TagId &&
                    t.date_detection == record.DetectionDate);

                await InsertOrUpdateRfidTag(record, cancellationToken, ref target);

                if (target == null)
                {
                    insertRow++;
                }
                else
                {
                    updateRow++;
                }

                resultList.Add(target.ToBusinessModel());
            }

            await _context.SaveChangesAsync(cancellationToken);

            var details = $"{insertRow} rows inserted, {updateRow} rows updated.";

            return (resultList, details);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    private Task InsertOrUpdateRfidTag(IRfidTagDetection record, CancellationToken cancellationToken,
        ref rfid_detection target)
    {
        if (target == null)
        {
            target = record.ToDatabaseModel();
            _context.rfid_detections.AddAsync(target, cancellationToken);
        }
        else
        {
            target.site_id = record.SiteId;
            target.tag_id = record.TagId;
            target.date_detection = record.DetectionDate;
            target.date_created = DateTime.Now;
        }

        return Task.CompletedTask;
    }
}