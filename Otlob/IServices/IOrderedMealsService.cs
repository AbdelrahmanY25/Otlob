namespace Otlob.IServices
{
    public interface IOrderedMealsService
    {
        OrderedMeals? GetOrderedMealById(int orderedMealId);
        OrderedMeals? GetMealFromCart(int mealId);
        OrderedMeals AddOrderedMeals(OrderedMealsVM orderedMealsVM, Cart userCart);
        IEnumerable<OrderedMeals> GetOrderedMealsDetails(int cartId);
        IEnumerable<OrderedMeals> GetOrderedMealsWithMealsDetails(int cartId);
        IEnumerable<OrderedMealsVM> GetOrderedMealsVMToView(int cartId);
        bool AddQuantityToOrderedMeals(OrderedMeals orderedMeal, OrderedMealsVM orderedMealsVM);
        OrderedMeals DeleteOrderedMeal(string id);
        bool ThereIsAnyMealsInCart(OrderedMeals selectedOrderMeal);
        bool CheckWhenUserAddAnotherMeal(OrderedMealsVM orderedMealsVM, Cart usercart);
        decimal CalculateTotalMealsPrice(int cartId);
        MealQuantityResult EditOrderedMealsQuantity(string id, MealQuantity mealQuantity);
    }
}