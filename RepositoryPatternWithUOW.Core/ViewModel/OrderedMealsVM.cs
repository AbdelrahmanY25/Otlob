namespace Otlob.Core.ViewModel
{
    public class OrderedMealsVM
    {
        public int Id { get; set; }
        public int MealId { get; set; }
        public int CartId { get; set; }

        [Required,Range(1, 99)]
        public int Quantity { get; set; }

        [Required, Range(1, 50000), Precision(18, 4)]
        public decimal PricePerMeal { get; set; }
        public string MealName { get; set; } = null!;
        public string? MealDescription { get; set; }
        public string? Image { get; set; }

        public static IEnumerable<OrderedMealsVM> MappToOrderedMealsVMCollection(IQueryable<OrderedMeals> orderedMeals)
        {
            List<OrderedMealsVM> orderedMealsVMs = new List<OrderedMealsVM>();
            
            foreach (var item in orderedMeals)
            {
                orderedMealsVMs.Add(new OrderedMealsVM
                {
                    Id = item.Id,
                    MealId = item.MealId,
                    CartId = item.CartId,
                    Quantity = item.Quantity,
                    PricePerMeal = item.PricePerMeal,
                    MealName = item.Meal.Name,
                    MealDescription = item.Meal.Description,
                    Image = item.Meal.Image
                });    
            }

            return orderedMealsVMs;
        }
    }
}
