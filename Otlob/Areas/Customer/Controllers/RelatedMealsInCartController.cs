using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IUnitOfWorkRepository;

namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class RelatedMealsInCartController : Controller
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;

        public RelatedMealsInCartController(IUnitOfWorkRepository unitOfWorkRepository)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
        }

        public IActionResult RelatedMeals(int resId)
        {
            var meals = unitOfWorkRepository.OrderedMeals.Get([m => m.Meal, m => m.Cart], expression: o => o.RestaurantId == resId);
            return View(meals);
        }

        public IActionResult IncreaseMealQuantity(int mealId, int cartId)
        {
            var selectedMeal = unitOfWorkRepository.OrderedMeals.GetOne(expression: e => e.MealId == mealId && e.CartId == cartId);

            if (selectedMeal != null)
            {
                if (selectedMeal.Quantity < 99)
                {
                    selectedMeal.Quantity++;
                    unitOfWorkRepository.OrderedMeals.Edit(selectedMeal);
                    unitOfWorkRepository.SaveChanges();
                }
            }

            string url = $"/customer/RelatedMealsInCart/RelatedMeals?resId={selectedMeal.RestaurantId}";
            return Redirect(url);
        }

        public IActionResult DecreaseMealQuantity(int mealId, int cartId)
        {
            var selectedMeal = unitOfWorkRepository.OrderedMeals.GetOne(expression: e => e.MealId == mealId && e.CartId == cartId);
            var selectedCart = unitOfWorkRepository.Carts.GetOne(expression: c => c.Id == cartId);

            if (selectedMeal != null)
            {
                if (selectedMeal.Quantity < 99)
                {
                    selectedMeal.Quantity--;
                    unitOfWorkRepository.OrderedMeals.Edit(selectedMeal);
                    unitOfWorkRepository.SaveChanges();
                }
                if (selectedMeal.Quantity == 0)
                {
                    unitOfWorkRepository.OrderedMeals.Delete(selectedMeal);
                    unitOfWorkRepository.SaveChanges();
                    var allOrders = unitOfWorkRepository.OrderedMeals.Get(expression: o => o.RestaurantId == selectedCart.ResturantId && o.MealId == mealId && o.CartId == cartId);

                    if (allOrders?.Count() == 0)
                    {
                        unitOfWorkRepository.Carts.Delete(selectedCart);
                        unitOfWorkRepository.SaveChanges();
                        return RedirectToAction("Cart", "Cart");
                    }
                }
            }

            string url = $"/customer/RelatedMealsInCart/RelatedMeals?resId={selectedMeal.RestaurantId}";
            return Redirect(url);
        }

        public IActionResult DeleteMeal(int mealId, int cartId)
        {
            var selectedMeal = unitOfWorkRepository.OrderedMeals.GetOne(expression: e => e.MealId == mealId && e.CartId == cartId);
            var selectedCart = unitOfWorkRepository.Carts.GetOne(expression: c => c.Id == cartId);

            if (selectedMeal != null)
            {
                unitOfWorkRepository.OrderedMeals.Delete(selectedMeal);
                unitOfWorkRepository.SaveChanges();


                var allOrders = unitOfWorkRepository.OrderedMeals.Get(expression: o => o.RestaurantId == selectedCart.ResturantId && o.MealId == mealId && o.CartId == cartId);

                if (allOrders?.Count() == 0)
                {
                    unitOfWorkRepository.Carts.Delete(selectedCart);
                    unitOfWorkRepository.SaveChanges();
                }
                return RedirectToAction("Cart", "Cart");
            }

            string url = $"/customer/RelatedMealsInCart/RelatedMeals?resId={selectedMeal.RestaurantId}";
            return Redirect(url);
        }
    }
}
