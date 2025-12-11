namespace Otlob.IServices;

public interface IMenuCategoryService
{
    Result<IQueryable<MenuCategoryResponse>>? GetAllByRestaurantId(int restaurantId);
    Result<MenuCategoryResponse>? GetById(int id);
    Result Add(int restaurantId, MenuCategoryRequest request);
    Result Update(string key, MenuCategoryRequest request);
    Result Delete(string key);
    Result UnDelete(string key);
    Result IsCategoryIdExists(int id);
}
