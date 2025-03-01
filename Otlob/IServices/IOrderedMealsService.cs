﻿using Otlob.Core.Models;
using Otlob.Core.ViewModel;
using Otlob.Services.Results;

namespace Otlob.IServices
{
    public interface IOrderedMealsService
    {
        OrderedMeals? GetOrderedMealById(int orderedMealId);
        OrderedMeals? GetMealFromCart(int mealId);
        OrderedMeals AddOrderedMeals(OrderedMealsVM orderedMealsVM, Cart userCart);
        IEnumerable<OrderedMealsVM> GetOrderedMealsVMToView(int cartId);
        bool AddQuantityToOrderedMeals(OrderedMeals orderedMeal, OrderedMealsVM orderedMealsVM);
        OrderedMeals DeleteOrderedMeal(string id);
        bool ThereIsAnyMealsInCart(OrderedMeals selectedOrderMeal);
        bool CheckWhenUserAddAnotherMeal(OrderedMealsVM orderedMealsVM, Cart usercart);
        decimal CalculateTotalMealsPrice(CartVM cart);
        MealQuantityResult EditOrderedMealsQuantity(string id, MealQuantity mealQuantity);
    }
}