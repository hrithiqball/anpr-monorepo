using MlffWebApi.Database.DbContexts;
using MlffWebApi.DTO.LicensePlateRecognition;
using MlffWebApi.DTO.Pagination;
using MlffWebApi.Interfaces;
using MlffWebApi.Interfaces.RFID;
using Newtonsoft.Json;

namespace MlffWebApi.DTO.RfidDetection;

public enum RfidTagDetectionSortByOptions
{
    DETECTION_DATE,
    TAG_ID
}

public class GetRfidTagDetectionListRequestDto : PaginationRequestDto
{
    public RfidTagDetectionSortByOptions SortBy { get; set; } = RfidTagDetectionSortByOptions.DETECTION_DATE;

    public bool IsAscending { get; set; } = false;

    /// <summary>
    /// Optional. Starting detection timestamp
    /// </summary>
    public DateTime? DetectionDateFrom { get; set; }

    /// <summary>
    /// Optional. Ending detection timestamp
    /// </summary>
    public DateTime? DetectionDateTo { get; set; }

    /// <summary>
    /// Optional. Site ids, split by semicolon(;), comma(,). Supports partial search and case insensitive
    /// </summary>
    public string SiteIds { get; set; }

    internal ICollection<string> SiteIdList => string.IsNullOrEmpty(SiteIds)
        ? Enumerable.Empty<string>().ToArray()
        : SiteIds.Split(new char[] { ';', ',' });
    
    /// <summary>
    /// Optional. Tag id, split by semicolon(;), comma(,). Supports partial search and case insensitive
    /// </summary>
    public string TagIds { get; set; }

    internal ICollection<string> TagIdList => string.IsNullOrEmpty(TagIds)
        ? Enumerable.Empty<string>().ToArray()
        : TagIds.Split(new char[] { ';', ',' });
}

public class GetRfidTagDetectionListResponseDto : PaginationResponseDto
{
    [JsonProperty("sortBy", Order = 5)]
    public string SortByString => SortBy.ToString();

    [JsonIgnore]
    internal RfidTagDetectionSortByOptions SortBy { get; set; }

    [JsonProperty(Order = 6)]
    public bool IsAscending { get; set; }

    [JsonProperty(Order = 7)]
    public IEnumerable<IRfidTagDetection> RfidTagDetections { get; set; }
}

public static class GetRfidTagDetectionListExtension
{
    public static GetRfidTagDetectionListResponseDto ToResponseDto(
        this IPaginationResult<IEnumerable<IRfidTagDetection>> paginationResult,
        GetRfidTagDetectionListRequestDto filter)
    {
        var dto = new GetRfidTagDetectionListResponseDto
        {
            TotalCount = paginationResult.TotalCount,
            PageNumber = paginationResult.PageNumber,
            PageSize = paginationResult.PageSize,
            SortBy = filter.SortBy,
            IsAscending = filter.IsAscending,
            RfidTagDetections = paginationResult.Data
        };

        return dto;
    }
}