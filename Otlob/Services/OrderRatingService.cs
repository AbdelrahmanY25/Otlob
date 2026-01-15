namespace Otlob.Services;

public class OrderRatingService(IUnitOfWorkRepository unitOfWorkRepository, IHttpContextAccessor httpContextAccessor,
                                IRestaurantRatingAnlyticsService restaurantRatingAnlyticsService) : IOrderRatingService
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IRestaurantRatingAnlyticsService _restaurantRatingAnlyticsService = restaurantRatingAnlyticsService;

    public Result<RatingResponse> GetOrderForRating(int orderId)
    {
        string userId = _httpContextAccessor.HttpContext!.User.GetUserId()!;

        var order = _unitOfWorkRepository.Orders.GetOne(
            expression: o => o.Id == orderId,
            includeProps: [o => o.Restaurant, o => o.Rating]
        );

        if (order is null)
            return Result.Failure<RatingResponse>(RatingErrors.OrderNotFound);

        if (order.UserId != userId)
            return Result.Failure<RatingResponse>(RatingErrors.Unauthorized);

        if (order.Status != OrderStatus.Delivered)
            return Result.Failure<RatingResponse>(RatingErrors.OrderNotDelivered);

        if (order.Rating is not null)
            return Result.Failure<RatingResponse>(RatingErrors.AlreadyRated);

        return Result.Success(new RatingResponse
        {
            OrderId = order.Id,
            RestaurantName = order.Restaurant.Name,
            RestaurantImage = order.Restaurant.Image,
            OrderDate = order.OrderDate,
            TotalPrice = order.TotalPrice,
            IsRated = false
        });
    }

    public Result<RatingResponse> GetRating(int orderId)
    {
        string userId = _httpContextAccessor.HttpContext!.User.GetUserId()!;

        var order = _unitOfWorkRepository.Orders.GetOne(
            expression: o => o.Id == orderId,
            includeProps: [o => o.Restaurant, o => o.Rating]
        );

        if (order is null)
            return Result.Failure<RatingResponse>(RatingErrors.OrderNotFound);

        if (order.UserId != userId)
            return Result.Failure<RatingResponse>(RatingErrors.Unauthorized);

        if (order.Rating is null)
            return Result.Failure<RatingResponse>(RatingErrors.RatingNotFound);

        return Result.Success(new RatingResponse
        {
            OrderId = order.Id,
            RestaurantName = order.Restaurant.Name,
            RestaurantImage = order.Restaurant.Image,
            OrderDate = order.OrderDate,
            TotalPrice = order.TotalPrice,
            GoodFood = order.Rating.GoodFood,
            FastDelivery = order.Rating.FastDelivery,
            GreatPacking = order.Rating.GreatPacking,
            FreshFood = order.Rating.FreshFood,
            GoodPortionSize = order.Rating.GoodPortionSize,
            FriendlyDelivery = order.Rating.FriendlyDelivery,
            WorthThePrice = order.Rating.WorthThePrice,
            Comment = order.Rating.Comment,
            RatedAt = order.Rating.RatedAt,
            IsRated = true
        });
    }

    public Result<RatingResponse> GetRatingForAdmin(int orderId)
    {
        var order = _unitOfWorkRepository.Orders.GetOne(
            expression: o => o.Id == orderId,
            includeProps: [o => o.Restaurant, o => o.Rating]
        );

        if (order is null)
            return Result.Failure<RatingResponse>(RatingErrors.OrderNotFound);

        if (order.Rating is null)
            return Result.Failure<RatingResponse>(RatingErrors.RatingNotFound);

        return Result.Success(new RatingResponse
        {
            OrderId = order.Id,
            RestaurantName = order.Restaurant.Name,
            RestaurantImage = order.Restaurant.Image,
            OrderDate = order.OrderDate,
            TotalPrice = order.TotalPrice,
            GoodFood = order.Rating.GoodFood,
            FastDelivery = order.Rating.FastDelivery,
            GreatPacking = order.Rating.GreatPacking,
            FreshFood = order.Rating.FreshFood,
            GoodPortionSize = order.Rating.GoodPortionSize,
            FriendlyDelivery = order.Rating.FriendlyDelivery,
            WorthThePrice = order.Rating.WorthThePrice,
            Comment = order.Rating.Comment,
            RatedAt = order.Rating.RatedAt,
            IsRated = true
        });
    }

    public Result SubmitRating(RatingRequest request)
    {
        string userId = _httpContextAccessor.HttpContext!.User.GetUserId()!;

        var order = _unitOfWorkRepository.Orders.GetOne(
            expression: o => o.Id == request.OrderId,
            includeProps: [o => o.Rating]
        );

        if (order is null)
            return Result.Failure(RatingErrors.OrderNotFound);

        if (order.UserId != userId)
            return Result.Failure(RatingErrors.Unauthorized);

        if (order.Status != OrderStatus.Delivered)
            return Result.Failure(RatingErrors.OrderNotDelivered);

        if (order.Rating is not null)
            return Result.Failure(RatingErrors.AlreadyRated);

        bool hasAnyTag = request.GoodFood || request.FastDelivery || request.GreatPacking ||
                         request.FreshFood || request.GoodPortionSize || request.FriendlyDelivery ||
                         request.WorthThePrice;

        if (!hasAnyTag)
            return Result.Failure(RatingErrors.NoTagsSelected);

        decimal rateScore = GetRateScore(request);

        using var transaction = _unitOfWorkRepository.BeginTransaction();

        try
        {
            var rating = new OrderRating
            {
                OrderId = request.OrderId,
                GoodFood = request.GoodFood,
                FastDelivery = request.FastDelivery,
                GreatPacking = request.GreatPacking,
                FreshFood = request.FreshFood,
                GoodPortionSize = request.GoodPortionSize,
                FriendlyDelivery = request.FriendlyDelivery,
                WorthThePrice = request.WorthThePrice,
                Comment = request.Comment?.Trim(),
                RatedAt = DateTime.Now
            };

            _unitOfWorkRepository.OrderRatings.Add(rating);
            _unitOfWorkRepository.SaveChanges();

            _restaurantRatingAnlyticsService.UpdateRate(order.RestaurantId, rateScore);

            transaction.Commit();

            return Result.Success();
        }
        catch
        {
            transaction.Rollback();
            return Result.Failure(RatingErrors.SubmissionFailed);
        }
    }

    private static decimal GetRateScore (RatingRequest request)
    {
        int selectedTags = 0;

        if (request.GoodFood)
            selectedTags += 1;
        if (request.FastDelivery)
            selectedTags += 1;
        if (request.GreatPacking)
            selectedTags += 1;
        if (request.FreshFood)
            selectedTags += 1;
        if (request.GoodPortionSize)
            selectedTags += 1;
        if (request.FriendlyDelivery)
            selectedTags += 1;
        if (request.WorthThePrice)
            selectedTags += 1;

        // Map the number of selected tags (0..7) to an integer rating between 1 and 5.
        // Compute a ratio [0..1], scale to [0..4], round to nearest integer, then add 1 => [1..5]
        decimal ratio = selectedTags / 7m;
        decimal scaled = ratio * 4m; // range 0..4
        decimal rounded = Math.Round(scaled, 0, MidpointRounding.AwayFromZero);
        decimal finalScore = rounded + 1m;

        // Clamp to [1,5]
        if (finalScore < 1m) finalScore = 1m;
        if (finalScore > 5m) finalScore = 5m;

        return finalScore;
    }
}
