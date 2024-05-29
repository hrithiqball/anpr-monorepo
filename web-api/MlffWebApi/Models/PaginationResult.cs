using MlffWebApi.Extensions;
using MlffWebApi.Interfaces;

namespace MlffWebApi.Models;

public class PaginationResult<T> : IPaginationResult<T>
{
    public int TotalCount { get; set; }
    public int TotalPage => Math.Ceiling(TotalCount.ToDouble() / PageSize.ToDouble()).ToInt();
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public T Data { get; set; }
}