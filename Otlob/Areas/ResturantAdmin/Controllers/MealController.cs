using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IServices;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;
using Otlob.IServices;
using Otlob.Services;

namespace Otlob.Areas.ResturantAdmin.Controllers
{
    [Area("ResturantAdmin")]
    public class MealController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMealService mealService;
        private readonly IEncryptionService encryptionService;
        private readonly IMealPriceHistoryService mealPriceHistoryService;

        public MealController(UserManager<ApplicationUser> userManager,
                                 IMealService mealService,
                                 IEncryptionService encryptionService,
                                 IMealPriceHistoryService mealPriceHistoryService)
        {
            this.userManager = userManager;
            this.mealService = mealService;
            this.encryptionService = encryptionService;
            this.mealPriceHistoryService = mealPriceHistoryService;
        }

        public async Task<IActionResult> Index()
        {
            var restaurnat =  await userManager.GetUserAsync(User);

            if (restaurnat == null)
            {
                return RedirectToAction("Login", "Account", new { Area = "Customer" });
            }

            var mealsVM = mealService.ViewMealsVmToRestaurantAdminSummary(restaurnat.RestaurantId);

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

            var restaurnat = await userManager.GetUserAsync(User);

            var isMealadded = await mealService.AddMeal(mealVM, restaurnat.RestaurantId, Request.Form.Files);

            if (isMealadded is string)
            {
                ModelState.AddModelError("", isMealadded);
                return View(mealVM);
            }

            return BackToMealsView("Your New Meal Added Successfully");                            
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

            return BackToMealsView("Your Old Meal Updated Successfully");        
        }

        public IActionResult MealPriceHistoryDetails(string id)
        {
            int mealId = encryptionService.DecryptId(id);

            var mealPriceHistories = mealPriceHistoryService.GetMealPriceHistories(mealId);

            return View(mealPriceHistories);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult DeleteMeal(string id)
        {
            int mealId = encryptionService.DecryptId(id);
            mealService.DeleteMeal(mealId);
            return RedirectToAction("Index");
        }

        private IActionResult BackToMealsView(string msg)
        {
            TempData["Success"] = msg;

            return RedirectToAction("Index");
        }
    }
}
