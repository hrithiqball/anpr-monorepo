using System.ComponentModel.DataAnnotations;
using MlffWebApi.Extensions;

namespace MlffWebApi.DTO.Pagination;

public class PaginationRequestDto
{
    /// <summary>
    /// Number of record per page.
    /// </summary>
    public int PageSize { get; init; } = 20;

    /// <summary>
    /// Page number, begin with 1, negative is allowed for reverse direction. For example: -1 for last page 
    /// </summary>
    public int PageNumber { get; init; } = 1;
}

public class PaginationResponseDto : PaginationRequestDto
{
    /// <summary>
    /// Total count. For pagination
    /// </summary>
    public int TotalCount { get; set; } = 0;

    /// <summary>
    /// Last page number
    /// </summary>
    public int TotalPage => Math.Ceiling(TotalCount.ToDouble() / PageSize.ToDouble()).ToInt();
}