namespace Otlob.Services;

public class RestaurantRatingAnlyticsService(IUnitOfWorkRepository unitOfWorkRepository) : IRestaurantRatingAnlyticsService
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

    public void Add(int restaurantId)
    {
        bool isExists = _unitOfWorkRepository.RestaurantRatingAnlytics
            .IsExist(r => r.RestaurantId == restaurantId);

        if (!isExists)
        {
            _unitOfWorkRepository.RestaurantRatingAnlytics
                .Add(new RestaurantRatingAnlytic{ RestaurantId = restaurantId });

            _unitOfWorkRepository.SaveChanges();
        }
    }

    public void UpdateRate(int restaurantId, decimal rate)
    {
        Add(restaurantId);

        var restaurantRatings = _unitOfWorkRepository.RestaurantRatingAnlytics
            .GetOne(expression: r => r.RestaurantId == restaurantId)!;

        restaurantRatings.Score += rate;
        restaurantRatings.RatesCount += 1;

        _unitOfWorkRepository.SaveChanges();

        var restaurant = _unitOfWorkRepository.Restaurants
            .GetOne(expression: r => r.Id == restaurantId)!;

        restaurant.Rating = restaurantRatings.AverageRate;

        _unitOfWorkRepository.Restaurants.Update(restaurant);
        _unitOfWorkRepository.SaveChanges();        
    }

    public RestaurantRatingAnlyticsResponse GetByRestaurantId(int restaurantId)
    {
        Add(restaurantId);

        var restaurantRatings = _unitOfWorkRepository.RestaurantRatingAnlytics
            .GetOneWithSelect(
                expression: r => r.RestaurantId == restaurantId,
                tracked: false,
                selector: r => new RestaurantRatingAnlyticsResponse
                {
                    ScoreRate = r.Score,
                    RatingsCount = r.RatesCount,
                    AverageRate = r.AverageRate
                }
            )!;

        return restaurantRatings;
    }

    public IEnumerable<RestaurantRatingAnlyticsResponse> GetAll()
    {
        var rates = _unitOfWorkRepository.RestaurantRatingAnlytics.Get()!
            .Select(r => new RestaurantRatingAnlyticsResponse
            {
                ScoreRate = r.Score,
                RatingsCount = r.RatesCount,
                AverageRate = r.AverageRate
            })
            .ToList();

        return rates is not null ? rates : [];
    }
}
