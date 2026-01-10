namespace Otlob.Services;

public class RestaurantCategoriesService(IUnitOfWorkRepository unitOfWorkRepository) : IRestaurantCategoriesService
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

    public IQueryable<Category>? GetAll() =>
        _unitOfWorkRepository.Categories.Get(tracked: false);

    public IQueryable<Category>? GetCategoriesByRestaurantId(int restaurantId) =>
        _unitOfWorkRepository.RestaurantCategories
            .GetAllWithSelect(
                expression: c => c.RestaurantId == restaurantId,
                tracked: false,
                selector: c => c.Category
            );

    public void SetCategoriesToRestaurant(List<int> categoriesIds, int restaurantId)
    {
        var existingCategories = _unitOfWorkRepository.RestaurantCategories
            .Get(expression: rc => rc.RestaurantId == restaurantId)?.ToList();


        // TODO: Enhance the performance by Except linq method
        if (existingCategories != null)
        {
            foreach (var category in existingCategories)
            {
                _unitOfWorkRepository.RestaurantCategories.HardDelete(category);
            }
        }

        // TODO: Enhance the performance by using Bulk Insert
        foreach (var categoryId in categoriesIds)
        {
            RestaurantCategory restaurantCategory = new() { CategoryId = categoryId, RestaurantId = restaurantId };
            _unitOfWorkRepository.RestaurantCategories.Add(restaurantCategory);
        }
    }
}
