namespace Otlob.Core.Contracts.ViewModel;

public class MealPriceHistoryVM
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public int MealId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
