using Otlob.Core.ViewModel;
using Otlob.IServices;

namespace Otlob.Services
{
    public class PaginationService : IPaginationService
    {
        public PaginationVM<T> PaginateItems<T>(IQueryable<T> items, int pageSize, int currentPageNumber) where T : class
        {
            int totalItems = items.Count();

            int pageCount = (int)Math.Ceiling((decimal)totalItems / pageSize);

            if (currentPageNumber < 1 || currentPageNumber > pageCount)
            {
                currentPageNumber = 1;
            }

            items = items.Skip((currentPageNumber - 1) * pageSize).Take(pageSize);

            PaginationVM<T> viewModel = new PaginationVM<T>
            {
                Items = items,
                PageCount = pageCount,
                CurrentPageNumber = currentPageNumber,
                totalItems = totalItems
            };

            return viewModel;
        }
    }
}
