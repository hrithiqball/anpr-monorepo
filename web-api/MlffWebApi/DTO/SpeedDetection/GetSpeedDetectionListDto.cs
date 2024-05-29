using MlffWebApi.DTO.Pagination;
using MlffWebApi.DTO.RfidDetection;
using MlffWebApi.Interfaces;
using MlffWebApi.Interfaces.RFID;
using MlffWebApi.Interfaces.Speed;
using Newtonsoft.Json;

namespace MlffWebApi.DTO.SpeedDetection;

public enum SpeedDetectionSortByOptions
{
    DETECTION_DATE,
    SPEED
}

public class GetSpeedDetectionListRequestDto : PaginationRequestDto
{
    public SpeedDetectionSortByOptions SortBy { get; set; } = SpeedDetectionSortByOptions.DETECTION_DATE;

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
    /// Optional. Minimum speed
    /// </summary>
    public int? SpeedMinimum { get; set; }
    
    /// <summary>
    /// Optional. Maximum speed
    /// </summary>
    public int? SpeedMaximum { get; set; }
}

public class GetSpeedDetectionListResponseDto : PaginationResponseDto
{
    [JsonProperty("sortBy", Order = 5)]
    public string SortByString => SortBy.ToString();

    [JsonIgnore]
    internal SpeedDetectionSortByOptions SortBy { get; set; }

    [JsonProperty(Order = 6)]
    public bool IsAscending { get; set; }

    [JsonProperty(Order = 7)]
    public IEnumerable<ISpeedDetection> SpeedDetections { get; set; }
}

public static class GetSpeedDetectionListExtension
{
    public static GetSpeedDetectionListResponseDto ToResponseDto(
        this IPaginationResult<IEnumerable<ISpeedDetection>> paginationResult,
        GetSpeedDetectionListRequestDto filter)
    {
        var dto = new GetSpeedDetectionListResponseDto
        {
            TotalCount = paginationResult.TotalCount,
            PageNumber = paginationResult.PageNumber,
            PageSize = paginationResult.PageSize,
            SortBy = filter.SortBy,
            IsAscending = filter.IsAscending,
            SpeedDetections = paginationResult.Data
        };

        return dto;
    }
}