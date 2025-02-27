namespace Otlob.Services.Results
{
    public class MealQuantityResult
    {
        public HandleMealQuantityProcess Status { get; init; }
        public int? CartId { get; init; }
    }

    public enum HandleMealQuantityProcess
    {
        SomeThingWrong,
        DeleteMeal,
        Success
    }

    public enum MealQuantity
    {
        Increase,
        Decrease
    }
}
