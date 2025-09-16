namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class RelatedMealsInCartController : Controller
    {
        private readonly ICartService cartService;
        private readonly IOrderedMealsService orderedMealsService;

        public RelatedMealsInCartController(ICartService cartService, IOrderedMealsService orderedMealsService)
        {
            this.cartService = cartService;
            this.orderedMealsService = orderedMealsService;
        }

        public IActionResult ChangeMealQuantity(string id, MealQuantity type)
        {
            MealQuantityResult result = orderedMealsService.EditOrderedMealsQuantity(id, type);

            switch (result.Status)
            {
                case HandleMealQuantityProcess.SomeThingWrong:
                    TempData["Error"] = "Failed to update quantity";
                    return RedirectToAction("Index", "Home");

                case HandleMealQuantityProcess.DeleteMeal:
                    return DeleteOrderedMeal(id);

                default:
                    return RedirectToAction("Cart", "Cart");
            }            
        }

        public IActionResult DeleteOrderedMeal(string id)
        {
            CartDetails selectedOrderMeal = orderedMealsService.DeleteOrderedMeal(id);

            bool isMealsExistInCart = orderedMealsService.ThereIsAnyMealsInCart(selectedOrderMeal);

            if (selectedOrderMeal is null || !isMealsExistInCart)
            {
                cartService.DeleteCart(selectedOrderMeal!.CartId);
            }

            return RedirectToAction("Cart", "Cart");
        }
    }
}
