using System.ComponentModel.DataAnnotations;
using MlffWebApi.DTO.Pagination;
using MlffWebApi.Exceptions;
using MlffWebApi.Interfaces;
using MlffWebApi.Interfaces.ANPR;
using Newtonsoft.Json;

namespace MlffWebApi.DTO.Camera;

/// <summary>
/// Options to sort camera list
/// </summary>
public enum CameraSortByOptions
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
public class GetCameraListRequestDto : PaginationRequestDto
{
    /// <summary>
    /// Sort by. Default ID
    /// </summary>
    public CameraSortByOptions SortBy { get; set; } = CameraSortByOptions.ID;
    
    /// <summary>
    /// Sort order
    /// </summary>
    public bool IsAscending { get; set; } = true;
}

/// <summary>
/// Parameters of camera list request
/// </summary>
public class GetCameraListResponseDto : PaginationResponseDto
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
    internal CameraSortByOptions SortBy { get; set; }

    /// <summary>
    /// Sort order
    /// </summary>
    [JsonProperty(Order = 6)]
    public bool IsAscending { get; set; }

    /// <summary>
    /// Camera details
    /// </summary>
    [JsonProperty(Order = 7)]
    public IList<ICamera> Cameras { get; set; }
}

internal static class GetCameraListExtension
{
    public static GetCameraListResponseDto ToResponseDto(
        this IPaginationResult<IList<ICamera>> paginationResult, GetCameraListRequestDto filter)
    {
        var dto = new GetCameraListResponseDto
        {
            TotalCount = paginationResult.TotalCount,
            PageNumber = paginationResult.PageNumber,
            PageSize = paginationResult.PageSize,
            SortBy = filter.SortBy,
            IsAscending = filter.IsAscending,
            Cameras = paginationResult.Data
        };

        return dto;
    }
}