using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IServices;
using Otlob.Core.Models;
using Otlob.IServices;
using Otlob.Services.Results;

namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class RelatedMealsInCartController : Controller
    {
        private readonly IOrderedMealsService orderedMealsService;
        private readonly ICartService cartService;
        private readonly IEncryptionService encryptionService;

        public RelatedMealsInCartController(IOrderedMealsService orderedMealsService,
                                            ICartService cartService,
                                            IEncryptionService encryptionService)
        {
            this.orderedMealsService = orderedMealsService;
            this.cartService = cartService;
            this.encryptionService = encryptionService;
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
            OrderedMeals selectedOrderMeal = orderedMealsService.DeleteOrderedMeal(id);

            bool isMealsExistInCart = orderedMealsService.ThereIsAnyMealsInCart(selectedOrderMeal);

            if (selectedOrderMeal is null || !isMealsExistInCart)
            {
                cartService.DeleteCart(selectedOrderMeal.CartId);
            }

            return RedirectToAction("Cart", "Cart");
        }
    }
}
