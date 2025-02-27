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
        private readonly IEncryptionService encryptionService;

        public RelatedMealsInCartController(IOrderedMealsService orderedMealsService,
                                           IEncryptionService encryptionService)
        {
            this.orderedMealsService = orderedMealsService;
            this.encryptionService = encryptionService;
        }

        public IActionResult RelatedMeals(string id)
        {
            int cartId = encryptionService.DecryptId(id);   

            var orderedMealsVM = orderedMealsService.GetOrderedMealsVMToView(cartId);

            return View(orderedMealsVM);
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
                    return RedirectToAction("RelatedMeals", new { id = encryptionService.EncryptId(result.CartId.Value) });
            }            
        }

        public IActionResult DeleteOrderedMeal(string id)
        {
            OrderedMeals selectedOrderMeal = orderedMealsService.DeleteOrderedMeal(id);

            bool isMealsExistInCart = orderedMealsService.ThereIsAnyMealsInCart(selectedOrderMeal);

            if (selectedOrderMeal is null || !isMealsExistInCart)
            {
                return RedirectToAction("DeleteCart", "Cart", new { id = encryptionService.EncryptId(selectedOrderMeal.CartId) });
            }

            return RedirectToAction("RelatedMeals", new { id = encryptionService.EncryptId(selectedOrderMeal.CartId) });
        }
    }
}
