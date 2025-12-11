namespace Otlob.Services;

public class RestaurantProgressStatus(IUnitOfWorkRepository unitOfWorkRepository) : IRestaurantProgressStatus
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    
    public ProgressStatus GetRestaurantProgressStatus(int restaurantId)
    {
        return _unitOfWorkRepository.Restaurants
                    .GetOneWithSelect(
                        expression: r => r.Id == restaurantId,
                        tracked: false,
                        selector: r => r.ProgressStatus
                    );
    }

    public void ChangeRestaurantProgressStatus(int restaurantId, ProgressStatus status)
    {
        var restaurant = _unitOfWorkRepository.Restaurants.GetOneWithSelect(
            expression: r => r.Id == restaurantId,
            selector: r => new Restaurant
            {
                Id = r.Id,
                ProgressStatus = r.ProgressStatus
            }
        );

        restaurant!.ProgressStatus = status;

        _unitOfWorkRepository.Restaurants.ModifyProperty(restaurant, r => r.ProgressStatus);
        _unitOfWorkRepository.SaveChanges();
    }
}
