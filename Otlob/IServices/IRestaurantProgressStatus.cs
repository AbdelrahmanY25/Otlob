namespace Otlob.IServices;

public interface IRestaurantProgressStatus
{
    ProgressStatus GetRestaurantProgressStatus(int restaurantId);
    void ChangeRestaurantProgressStatus(int restaurantId, ProgressStatus status);
}
