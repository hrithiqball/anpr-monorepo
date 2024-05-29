using MlffWebApi.Database.DbContexts;
using MlffWebApi.DTO.LicensePlateRecognition;
using MlffWebApi.Exceptions;
using MlffWebApi.Interfaces;
using MlffWebApi.Interfaces.ANPR;
using MlffWebApi.Interfaces.Repository;
using MlffWebApi.Models;
using Microsoft.EntityFrameworkCore;
using MlffWebApi.Models.ANPR;

namespace MlffWebApi.Repositories;

public class LicensePlateRecognitionRepository : ILicensePlateRecognitionRepository
{
    private readonly MlffDbContext _dbContext;
    private readonly ILogger<LicensePlateRecognitionRepository> _logger;

    public LicensePlateRecognitionRepository(MlffDbContext context, ILoggerFactory loggerFactory)
    {
        _dbContext = context;
        _logger = loggerFactory.CreateLogger<LicensePlateRecognitionRepository>();
    }

    public async Task<IPaginationResult<IEnumerable<ILicensePlateRecognition>>> GetRecords(
        GetLicensePlateRecognitionListRequestDto requestDto,
        CancellationToken cancellationToken)
    {
        try
        {
            if (requestDto is null)
            {
                throw new ArgumentNullException(nameof(requestDto));
            }

            var query = _dbContext.license_plate_recognitions.AsQueryable();

            if (requestDto.DetectionDateFrom.HasValue)
            {
                query = query.Where(t => t.date_detection >= requestDto.DetectionDateFrom.Value);
            }

            if (requestDto.DetectionDateTo.HasValue)
            {
                query = query.Where(t => t.date_detection <= requestDto.DetectionDateTo.Value);
            }

            IQueryable<license_plate_recognition> tempQuery1 = null;

            foreach (var siteId in requestDto.SiteIdList)
            {
                var subQuery = query.Where(t => t.site_id.ToLower().Contains(siteId.ToLower()));
                tempQuery1 = tempQuery1 == null ? subQuery : tempQuery1.Union(subQuery);
            }

            IQueryable<license_plate_recognition> tempQuery2 = null;
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
                case LicensePlateRecognitionSortByOptions.DETECTION_DATE:
                    finalQuery = requestDto.IsAscending
                        ? finalQuery.OrderBy(t => t.date_detection)
                        : finalQuery.OrderByDescending(t => t.date_detection);
                    break;
                case LicensePlateRecognitionSortByOptions.PLATE_NUMBER:
                    finalQuery = requestDto.IsAscending
                        ? finalQuery.OrderBy(t => t.plate_number)
                        : finalQuery.OrderByDescending(t => t.plate_number);
                    break;
                default:
                    var validOptions = Enum.GetNames(typeof(LicensePlateRecognitionSortByOptions));
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

            return new PaginationResult<IEnumerable<ILicensePlateRecognition>>
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

    public async Task<ILicensePlateRecognition> AddRecordIfNotExist(ILicensePlateRecognition record,
        CancellationToken cancellationToken)
    {
        if (record is null)
        {
            throw new ArgumentNullException(nameof(record));
        }

        var target = await _dbContext.license_plate_recognitions.FirstOrDefaultAsync(t =>
            t.site_id == record.SiteId && t.plate_number == record.PlateNumber &&
            t.date_detection == record.DetectionDate, cancellationToken);

        await AddOrInsertLicensePlateRecognition(cancellationToken, record, ref target);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return target.ToBusinessModel();
    }

    public async Task<(IList<ILicensePlateRecognition> updatedRecords, string details)> AddRecordsIfNotExist(
        ICollection<ILicensePlateRecognition> records,
        CancellationToken cancellationToken)
    {
        if (records is null)
        {
            throw new ArgumentNullException(nameof(records));
        }

        var maxDetectionDate = records.MaxBy(t => t.DetectionDate)?.DetectionDate;
        var minDetectionDate = records.MinBy(t => t.DetectionDate)?.DetectionDate;
        var targets =
            _dbContext.license_plate_recognitions.Where(t =>
                t.date_detection >= minDetectionDate && t.date_detection <= maxDetectionDate);

        var resultList = new List<ILicensePlateRecognition>();
        var updateRow = 0;
        var insertRow = 0;

        foreach (var record in records)
        {
            var target = targets.FirstOrDefault(t =>
                t.site_id == record.SiteId &&
                t.plate_number == record.PlateNumber &&
                t.date_detection == record.DetectionDate);

            var isInsert = target == null;

            await AddOrInsertLicensePlateRecognition(cancellationToken, record, ref target);

            if (isInsert)
            {
                insertRow++;
            }
            else
            {
                updateRow++;
            }

            resultList.Add(target.ToBusinessModel());
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        var details = $"{insertRow} rows inserted, {updateRow} rows updated.";

        return (resultList, details);
    }

    private Task AddOrInsertLicensePlateRecognition(CancellationToken cancellationToken,
        ILicensePlateRecognition record,
        ref license_plate_recognition target)
    {
        if (target == null)
        {
            target = record.ToDatabaseModel();
            _dbContext.license_plate_recognitions.AddAsync(target, cancellationToken);
        }
        else
        {
            target.site_id = record.SiteId;
            target.vehicle_image_path = record.VehicleImagePath;
            target.plate_image_path = record.PlateImagePath;
            target.plate_number = record.PlateNumber;
            target.bbox_top = record.BoundingBox?.Top;
            target.bbox_left = record.BoundingBox?.Left;
            target.bbox_height = record.BoundingBox?.Height;
            target.bbox_width = record.BoundingBox?.Width;
            target.confidence_lpd = record.ConfidenceOnPlateDetection;
            target.confidence_ocr = record.ConfidenceOnPlateRecognition;
            target.date_detection = record.DetectionDate;
            target.date_created = DateTime.Now;
        }

        return Task.CompletedTask;
    }

    public async Task<bool> DeleteRecord(Guid uid, CancellationToken cancellationToken)
    {
        var target =
            await _dbContext.license_plate_recognitions.FirstOrDefaultAsync(t => t.uid == uid, cancellationToken);

        if (target is null)
        {
            throw new KeyNotFoundException($"License plate recognition record not found. Uid: {uid}");
        }

        _dbContext.license_plate_recognitions.Remove(target);

        return await _dbContext.SaveChangesAsync(cancellationToken) == 1;
    }

    public async Task<int> DeleteRecords(ISite site, DateTime start, DateTime end,
        CancellationToken cancellationToken)
    {
        var targets = _dbContext.license_plate_recognitions.Where(t =>
            t.site_id == site.Id && t.date_detection >= start && t.date_detection <= end);

        _dbContext.license_plate_recognitions.RemoveRange(targets);

        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}