using MlffWebApi.DTO.Pagination;
using MlffWebApi.Interfaces;
using MlffWebApi.Interfaces.Speed;
using Newtonsoft.Json;

namespace MlffWebApi.DTO.SpeedRadar;

/// <summary>
/// Options to sort speed radar list
/// </summary>
public enum SpeedRadarSortByOptions
{
    /// <summary>
    /// Sort by id
    /// </summary>
    ID,
    /// <summary>
    /// Sort by display name
    /// </summary>
    DISPLAY_NAME,
    /// <summary>
    /// Sort by tag
    /// </summary>
    TAG,
    /// <summary>
    /// Sort by modified date
    /// </summary>
    DATE_MODIFIED
}

/// <summary>
/// Get camera list request model
/// </summary>
public class GetSpeedRadarListRequestDto : PaginationRequestDto
{
    /// <summary>
    /// Sort by. Default ID
    /// </summary>
    public SpeedRadarSortByOptions SortBy { get; set; } = SpeedRadarSortByOptions.ID;
    
    /// <summary>
    /// Sort order
    /// </summary>
    public bool IsAscending { get; set; } = true;
}

/// <summary>
/// Parameters of speed radar list request
/// </summary>
public class GetSpeedRadarListResponseDto : PaginationResponseDto
{
    /// <summary>
    /// Sort by in string
    /// </summary>
    [JsonProperty(PropertyName = "sortBy", Order = 5)]
    public string SortByString => SortBy.ToString();
    
    /// <summary>
    /// Sort by enum, JSON ignored
    /// </summary>
    [JsonIgnore]
    internal SpeedRadarSortByOptions SortBy { get; set; }

    /// <summary>
    /// Sort order
    /// </summary>
    [JsonProperty(Order = 6)]
    public bool IsAscending { get; set; }

    /// <summary>
    /// Speed radar details
    /// </summary>
    [JsonProperty(Order = 7)]
    public IList<ISpeedRadar> SpeedRadars { get; set; }
}

internal static class GetSpeedRadarListExtension
{
    public static GetSpeedRadarListResponseDto ToResponseDto(
        this IPaginationResult<IList<ISpeedRadar>> paginationResult, GetSpeedRadarListRequestDto filter)
    {
        var dto = new GetSpeedRadarListResponseDto
        {
            TotalCount = paginationResult.TotalCount,
            PageNumber = paginationResult.PageNumber,
            PageSize = paginationResult.PageSize,
            SortBy = filter.SortBy,
            IsAscending = filter.IsAscending,
            SpeedRadars = paginationResult.Data
        };

        return dto;
    }
}