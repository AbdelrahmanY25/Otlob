namespace Otlob.Core.Contracts.MealPriceHistory;

public class MealPriceHistoryResponse
{
    public int Id { get; init; }
    public decimal Price { get; init; }
    public int MealId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}