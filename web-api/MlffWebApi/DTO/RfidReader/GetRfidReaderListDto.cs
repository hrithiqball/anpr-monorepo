using MlffWebApi.DTO.Pagination;
using MlffWebApi.Interfaces;
using MlffWebApi.Interfaces.RFID;
using Newtonsoft.Json;

namespace MlffWebApi.DTO.RfidReader;

/// <summary>
/// Options to sort camera list
/// </summary>
public enum RfidReaderSortByOptions
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
public class GetRfidReaderListRequestDto : PaginationRequestDto
{
    /// <summary>
    /// Sort by. Default ID
    /// </summary>
    public RfidReaderSortByOptions SortBy { get; set; } = RfidReaderSortByOptions.ID;
    
    /// <summary>
    /// Sort order
    /// </summary>
    public bool IsAscending { get; set; } = true;
}

/// <summary>
/// Parameters of camera list request
/// </summary>
public class GetRfidReaderListResponseDto : PaginationResponseDto
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
    internal RfidReaderSortByOptions SortBy { get; set; }

    /// <summary>
    /// Sort order
    /// </summary>
    [JsonProperty(Order = 6)]
    public bool IsAscending { get; set; }

    /// <summary>
    /// Camera details
    /// </summary>
    [JsonProperty(Order = 7)]
    public IList<IRfidReader> RfidReaders { get; set; }
}

internal static class GetRfidReaderListExtension
{
    public static GetRfidReaderListResponseDto ToResponseDto(
        this IPaginationResult<IList<IRfidReader>> paginationResult, GetRfidReaderListRequestDto filter)
    {
        var dto = new GetRfidReaderListResponseDto
        {
            TotalCount = paginationResult.TotalCount,
            PageNumber = paginationResult.PageNumber,
            PageSize = paginationResult.PageSize,
            SortBy = filter.SortBy,
            IsAscending = filter.IsAscending,
            RfidReaders = paginationResult.Data
        };

        return dto;
    }
}