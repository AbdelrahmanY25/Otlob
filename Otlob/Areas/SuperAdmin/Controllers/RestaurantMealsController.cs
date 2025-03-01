﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IServices;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;
using Otlob.IServices;
using Otlob.Services;

namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin")]
    public class RestaurantMealsController : Controller
    {
        private readonly IMealService mealService;
        private readonly IEncryptionService encryptionService;

        public RestaurantMealsController(IMealService mealService,
                                         IEncryptionService encryptionService)
        {
            this.mealService = mealService;
            this.encryptionService = encryptionService;
        }
        public IActionResult RestaurantMeals(string id)
        {
            int restaurantId = encryptionService.DecryptId(id);
            HttpContext.Session.SetString("restaurantId", encryptionService.EncryptId(restaurantId));
            var mealsVM = mealService.ViewMealsVmToRestaurantAdminSummary(restaurantId);
            return View(mealsVM);
        }

        public IActionResult AddMeal() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMeal(MealVm mealVM)
        {
            if (!ModelState.IsValid)
            {
                return View(mealVM);
            }

            int restaurantId = encryptionService.DecryptId(HttpContext.Session.GetString("restaurantId"));

            var isMealadded = await mealService.AddMeal(mealVM, restaurantId, Request.Form.Files);

            if (isMealadded is string)
            {
                ModelState.AddModelError("", isMealadded);
                return View(mealVM);
            }

            return RedirectToAction("RestaurantMeals", "RestaurantMeals", new { id = encryptionService.EncryptId(restaurantId) });
        }

        public IActionResult MealDetails(string id)
        {
            var mealId = encryptionService.DecryptId(id);

            var mealVM = mealService.GetMealVM(mealId);

            HttpContext.Session.SetString("MealId", encryptionService.EncryptId(mealId));

            return View(mealVM);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MealDetails(MealVm mealVM)
        {
            int mealId = encryptionService.DecryptId(HttpContext.Session.GetString("MealId"));

            if (!ModelState.IsValid)
            {
                return View(mealVM);
            }

            string isMealAdded = await mealService.EditMeal(mealVM, mealId, Request.Form.Files);

            if (isMealAdded is string)
            {
                ModelState.AddModelError("", isMealAdded);
                return View(mealVM);
            }

            int restaurantId = encryptionService.DecryptId(HttpContext.Session.GetString("restaurantId"));
            return RedirectToAction("RestaurantMeals", "RestaurantMeals", new { id = encryptionService.EncryptId(restaurantId) });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult DeleteMeal(string id)
        {
            int mealId = encryptionService.DecryptId(id);
            mealService.DeleteMeal(mealId);
            int restaurantId = encryptionService.DecryptId(HttpContext.Session.GetString("restaurantId"));
            return RedirectToAction("RestaurantMeals", "RestaurantMeals", new { id = encryptionService.EncryptId(restaurantId) });
        }        
    }
}
