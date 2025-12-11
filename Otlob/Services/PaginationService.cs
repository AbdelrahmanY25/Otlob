namespace Otlob.Services;

public class PaginationService : IPaginationService
{
    public PaginationVM<T> PaginateItems<T>(IQueryable<T> items, int pageSize, int currentPageNumber, object? element = null) where T : class
    {
        int totalItems = items.Count();

        int pageCount = (int)Math.Ceiling((decimal)totalItems / pageSize);

        if (currentPageNumber < 1 || currentPageNumber > pageCount)
        {
            currentPageNumber = 1;
        }

        items = items.Skip((currentPageNumber - 1) * pageSize).Take(pageSize);

        PaginationVM<T> viewModel = new()
        {
            Items = items,
            PageCount = pageCount,
            CurrentPageNumber = currentPageNumber,
            TotalItems = totalItems
        };

        if (element is not null)
        {
            viewModel.UnieqElement = element;
        }

        return viewModel;
    }
}
