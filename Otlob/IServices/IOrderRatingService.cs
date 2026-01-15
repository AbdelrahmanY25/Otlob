namespace Otlob.IServices;

public interface IOrderRatingService
{
    Result<RatingResponse> GetOrderForRating(int orderId);
    Result<RatingResponse> GetRating(int orderId);
    Result<RatingResponse> GetRatingForAdmin(int orderId);
    Result SubmitRating(RatingRequest request);
}
