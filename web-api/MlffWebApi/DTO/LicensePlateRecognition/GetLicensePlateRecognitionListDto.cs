using System.Runtime.InteropServices;
using MlffWebApi.DTO.Pagination;
using MlffWebApi.Interfaces;
using MlffWebApi.Interfaces.ANPR;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Npgsql.Internal.TypeHandlers.NumericHandlers;

namespace MlffWebApi.DTO.LicensePlateRecognition;

public enum LicensePlateRecognitionSortByOptions
{
    DETECTION_DATE,
    PLATE_NUMBER
}

public class GetLicensePlateRecognitionListRequestDto : PaginationRequestDto
{
    public LicensePlateRecognitionSortByOptions SortBy { get; set; } =
        LicensePlateRecognitionSortByOptions.DETECTION_DATE;

    public bool IsAscending { get; set; } = false;

    /// <summary>
    /// Optional. Site ids, split by semicolon(;), comma(,). Supports partial search and case insensitive
    /// </summary>
    public string SiteIds { get; set; }

    internal ICollection<string> SiteIdList => string.IsNullOrEmpty(SiteIds)
        ? Enumerable.Empty<string>().ToArray()
        : SiteIds.Split(new char[] { ';', ',' });
    
    /// <summary>
    /// Optional. Starting detection timestamp
    /// </summary>
    public DateTime? DetectionDateFrom { get; set; }

    /// <summary>
    /// Optional. Ending detection timestamp
    /// </summary>
    public DateTime? DetectionDateTo { get; set; }

    /// <summary>
    /// Optional. Number plates, split by semicolon(;), comma(,). Supports partial search and case insensitive
    /// </summary>
    public string NumberPlates { get; set; }

    internal ICollection<string> NumberPlateList => string.IsNullOrEmpty(NumberPlates)
        ? Enumerable.Empty<string>().ToArray()
        : NumberPlates.Split(new char[] { ';', ',' });
}

public class GetLicensePlateRecognitionListResponseDto : PaginationResponseDto
{
    [JsonProperty("sortBy", Order = 5)]
    public string SortByString => SortBy.ToString();

    [JsonIgnore]
    public LicensePlateRecognitionSortByOptions SortBy { get; set; }

    [JsonProperty(Order = 6)]
    public bool IsAscending { get; set; }

    [JsonProperty(Order = 7)]
    public IEnumerable<ILicensePlateRecognition> LicensePlateRecognitions { get; set; }
}

public static class GetLicensePlateRecognitionListExtension
{
    public static GetLicensePlateRecognitionListResponseDto ToResponseDto(
        this IPaginationResult<IEnumerable<ILicensePlateRecognition>> paginationResult,
        GetLicensePlateRecognitionListRequestDto filter)
    {
        var dto = new GetLicensePlateRecognitionListResponseDto
        {
            TotalCount = paginationResult.TotalCount,
            PageNumber = paginationResult.PageNumber,
            PageSize = paginationResult.PageSize,
            SortBy = filter.SortBy,
            IsAscending = filter.IsAscending,
            LicensePlateRecognitions = paginationResult.Data
        };

        return dto;
    }
}