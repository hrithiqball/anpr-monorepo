namespace MlffWebApi.Interfaces;


public interface IPaginationResult<T>
{
    public int TotalCount { get; set; }
    public int TotalPage { get; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public T Data { get; set; }
}