namespace Otlob.IServices;

public interface IBranchService
{
    IQueryable<BranchResponse> GetAllByRestaurantId(int restaurantId);
    Result<BranchResponse> GetById(string id);
    Result Add(BranchRequest request, int restaurantId);
    Result Update(string id, BranchRequest request);
    int GetRestaurantBranchesCountByRestaurantId(int restaurantId);
    Result Delete(string key);
}
