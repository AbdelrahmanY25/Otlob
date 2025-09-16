namespace Otlob.Services;

public class CategoriesService(IUnitOfWorkRepository unitOfWorkRepository) : ICategoriesService
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

    public IQueryable<Category> GetAllCategories()
    {
        return _unitOfWorkRepository.Categories.Get(tracked: false)!;
    }
}
