namespace Otlob.ApiServices;

public interface IApiSearchService
{
    ApiResult<SearchResponse> Search(string query);
    ApiResult<IQueryable<RestaurantSearchResponse>> SearchByCategory(string categoryName);
}
