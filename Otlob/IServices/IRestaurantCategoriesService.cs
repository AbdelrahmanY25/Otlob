namespace Otlob.IServices;

public interface IRestaurantCategoriesService
{
    IQueryable<Category>? GetAll();
    IQueryable<Category>? GetCategoriesByRestaurantId(int restaurantId);
    void SetCategoriesToRestaurant(List<int> categories, int restaurantId);
}
