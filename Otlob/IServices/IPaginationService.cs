using Otlob.Core.ViewModel;

namespace Otlob.IServices
{
    public interface IPaginationService
    {
        PaginationVM<T> PaginateItems<T>(IQueryable<T> items, int pageSize, int currentPageNumber) where T : class;
    }
}