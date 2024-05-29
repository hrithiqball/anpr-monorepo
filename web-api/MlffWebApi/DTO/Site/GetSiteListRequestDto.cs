using MlffWebApi.DTO.Pagination;
using MlffWebApi.Interfaces;
using Newtonsoft.Json;

namespace MlffWebApi.DTO.Site;

public enum SiteSortByOptions
{
    ID,
    LOCATION_NAME,
    KILOMETER_MARKER,
    DATE_MODIFIED
}

public class GetSiteListRequestDto : PaginationRequestDto
{
    public SiteSortByOptions SortBy { get; set; } = SiteSortByOptions.ID;
    public bool IsAscending { get; set; } = true;
}

public class GetSiteListResponseDto : PaginationResponseDto
{
    [JsonProperty(PropertyName = "sortBy", Order = 5)]
    public string SortByString => SortBy.ToString();
    
    [JsonIgnore]
    public SiteSortByOptions SortBy { get; set; }

    [JsonProperty(Order = 6)]
    public bool IsAscending { get; set; }

    [JsonProperty(Order = 7)]
    public IList<ISite> Sites { get; set; }
}

public static class GetSiteListExtension
{
    public static GetSiteListResponseDto ToResponseDto(
        this IPaginationResult<IList<ISite>> paginationResult, GetSiteListRequestDto filter)
    {
        var dto = new GetSiteListResponseDto
        {
            TotalCount = paginationResult.TotalCount,
            PageNumber = paginationResult.PageNumber,
            PageSize = paginationResult.PageSize,
            SortBy = filter.SortBy,
            IsAscending = filter.IsAscending,
            Sites = paginationResult.Data
        };

        return dto;
    }
}