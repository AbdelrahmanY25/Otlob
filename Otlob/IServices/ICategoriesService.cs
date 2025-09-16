namespace Otlob.IServices;

public interface ICategoriesService
{
    IQueryable<Category> GetAllCategories();
}
