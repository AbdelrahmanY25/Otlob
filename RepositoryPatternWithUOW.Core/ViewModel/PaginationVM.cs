namespace Otlob.Core.ViewModel
{
    public class PaginationVM<T> where T : class
    {
        public IQueryable<T> Items { get; set; }
        public int PageCount { get; set; }
        public int CurrentPageNumber { get; set; }
        public int totalItems { get; set; }
        public bool HasPreviousPage => CurrentPageNumber > 1;
        public bool HasNextPage => CurrentPageNumber < PageCount;
    }
}
