namespace Otlob.Core.Contracts.ViewModel;

public class PaginationVM<T> where T : class
{
    public IQueryable<T> Items { get; set; } = default!;   
    public int PageCount { get; set; }
    public int CurrentPageNumber { get; set; }
    public int TotalItems { get; set; }
    public object? UnieqElement { get; set; }
    public bool HasPreviousPage => CurrentPageNumber > 1;
    public bool HasNextPage => CurrentPageNumber < PageCount;
}
