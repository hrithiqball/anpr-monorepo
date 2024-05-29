using MlffWebApi.DTO.LicensePlateRecognition;
using MlffWebApi.DTO.Pagination;
using MlffWebApi.Interfaces;
using MlffWebApi.Interfaces.ANPR;
using Newtonsoft.Json;

namespace MlffWebApi.DTO.DetectionMatch;

public enum DetectionMatchSortByOptions
{
    DATE_MATCHED,
    SITE_ID,
    PLATE_NUMBER,
    SPEED,
    TAG_ID
}

public class GetDetectionMatchListRequestDto : PaginationRequestDto
{
    public DetectionMatchSortByOptions SortBy { get; set; } =
        DetectionMatchSortByOptions.DATE_MATCHED;

    public bool IsAscending { get; set; } = false;

    /// <summary>
    /// Optional. Site ids, split by semicolon(;), comma(,). Supports partial search and case insensitive
    /// </summary>
    public string SiteIds { get; set; } = string.Empty;

    internal ICollection<string> SiteIdList => string.IsNullOrEmpty(SiteIds)
        ? Enumerable.Empty<string>().ToArray()
        : SiteIds.Split(new char[] { ';', ',' });

    /// <summary>
    /// Optional. Starting detection timestamp
    /// </summary>
    public DateTime? MatchedDateFrom { get; set; }

    /// <summary>
    /// Optional. Ending detection timestamp
    /// </summary>
    public DateTime? MatchedDateTo { get; set; }

    /// <summary>
    /// Optional. Number plates, split by semicolon(;), comma(,). Supports partial search and case insensitive
    /// </summary>
    public string NumberPlates { get; set; } = string.Empty;

    internal ICollection<string> NumberPlateList => string.IsNullOrEmpty(NumberPlates)
        ? Enumerable.Empty<string>().ToArray()
        : NumberPlates.Split(new char[] { ';', ',' });

    /// <summary>
    /// Optional. Minimum speed
    /// </summary>
    public int? MinSpeed { get; set; }

    /// <summary>
    /// Optional. Maximum speed
    /// </summary>
    public int? MaxSpeed { get; set; }

    /// <summary>
    /// Optional. Tag IDs, split by semicolon(;), comma(,). Supports partial search and case insensitive
    /// </summary>
    public string TagIds { get; set; } = string.Empty;

    internal ICollection<string> TagIdList => string.IsNullOrEmpty(TagIds)
        ? Enumerable.Empty<string>().ToArray()
        : TagIds.Split(new char[] { ',', ';' });
}

public class GetDetectionMatchListResponseDto : PaginationResponseDto
{
    [JsonProperty("sortBy", Order = 5)]
    public string SortByString => SortBy.ToString();

    [JsonIgnore]
    public DetectionMatchSortByOptions SortBy { get; set; }

    [JsonProperty(Order = 6)]
    public bool IsAscending { get; set; }

    [JsonProperty(Order = 7)]
    public IEnumerable<IDetectionMatch> DetectionMatches { get; set; }
}

public static class GetDetectionMatchListExtension
{
    public static GetDetectionMatchListResponseDto ToResponseDto(
        this IPaginationResult<IEnumerable<IDetectionMatch>> paginationResult,
        GetDetectionMatchListRequestDto filter)
    {
        var dto = new GetDetectionMatchListResponseDto
        {
            TotalCount = paginationResult.TotalCount,
            PageNumber = paginationResult.PageNumber,
            PageSize = paginationResult.PageSize,
            SortBy = filter.SortBy,
            IsAscending = filter.IsAscending,
            DetectionMatches = paginationResult.Data
        };

        return dto;
    }
}