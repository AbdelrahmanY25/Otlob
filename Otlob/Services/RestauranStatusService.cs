namespace Otlob.Services;

public class RestauranStatusService(IUnitOfWorkRepository unitOfWorkRepository, IDataProtectionProvider dataProtectionProvider) : IRestauranStatusService
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public Result ChangeRestauranStatus(string id, AcctiveStatus status)
    {
        int restaurantId = int.Parse(_dataProtector.Unprotect(id));

        Restaurant? restaurant = _unitOfWorkRepository
            .Restaurants
            .GetOneWithSelect(
                expression: r => r.Id == restaurantId,
                selector: r => new Restaurant
                {
                    Id = r.Id,
                    AcctiveStatus = r.AcctiveStatus
                }
            );

        if (restaurant is null)
        {
            return Result.Failure(RestaurantErrors.NotFound);
        }

        restaurant.AcctiveStatus = status;
        _unitOfWorkRepository.Restaurants.ModifyProperty(restaurant, r => r.AcctiveStatus);
        _unitOfWorkRepository.SaveChanges();

        //TODO: Send an Email to Owner about Status Change
        //      if pending he ready to add his meals and branches
        //      if accepted he now on production start to gain money
        //      if rejected inform him about the rejection

        return Result.Success();
    }
}
