namespace Otlob.IServices
{
    public interface IPaginationService
    {
        PaginationVM<T> PaginateItems<T>(IQueryable<T> items, int pageSize, int currentPageNumber, object? element = null) where T : class;
    }
}