namespace Otlob.IServices
{
    public interface IOrderedMealsService
    {
        CartDetails? GetOrderedMealById(int orderedMealId);
        CartDetails? GetMealFromCart(string mealId);
        CartDetails AddOrderedMeals(OrderedMealsVM orderedMealsVM, Cart userCart);
        IEnumerable<CartDetails> GetOrderedMealsDetails(int cartId);
        IEnumerable<CartDetails> GetOrderedMealsWithMealsDetails(int cartId);
        IEnumerable<OrderedMealsVM> GetOrderedMealsVMToView(int cartId);
        bool AddQuantityToOrderedMeals(CartDetails orderedMeal, OrderedMealsVM orderedMealsVM);
        CartDetails DeleteOrderedMeal(string id);
        bool ThereIsAnyMealsInCart(CartDetails selectedOrderMeal);
        bool CheckWhenUserAddAnotherMeal(OrderedMealsVM orderedMealsVM, Cart usercart);
        decimal CalculateTotalMealsPrice(int cartId);
        MealQuantityResult EditOrderedMealsQuantity(string id, MealQuantity mealQuantity);
    }
}