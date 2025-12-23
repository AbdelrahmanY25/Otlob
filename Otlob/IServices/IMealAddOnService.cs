namespace Otlob.IServices;

public interface IMealAddOnService
{
    Result<IQueryable<AddOnResponse>> GetAllByRestaurantId(int restaurantId);
    Result Add(AddOnRequest request, int restaurantId);
    Result Update(AddOnRequest request, string id);
    Result Delete(string id);
}
