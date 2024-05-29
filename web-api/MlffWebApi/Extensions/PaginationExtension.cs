using MlffWebApi.DTO.Pagination;

namespace MlffWebApi.Extensions;

public static class PaginationExtension
{
    public static PaginationResponseDto ToPaginationResponseDto<T>(this IEnumerable<T> obj, PaginationRequestDto filter)
    {
        var dto = new PaginationResponseDto
        {
            TotalCount = obj.Count(),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
        return dto;
    }
}