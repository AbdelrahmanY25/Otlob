using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IServices;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.ViewModel;

namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin")]
    public class RestaurantMealsController : Controller
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly IImageService imageService;

        public RestaurantMealsController(IUnitOfWorkRepository unitOfWorkRepository,
                                         IImageService imageService)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.imageService = imageService;
        }
        public IActionResult ResturantMeals(int id)
        {
            var meals = unitOfWorkRepository.Meals.Get(expression: m => m.RestaurantId == id);
            ViewBag.ResturantId = id;
            return View(meals);
        }

        public IActionResult AddMeal(int resId)
        {
            ViewBag.ResturantId = resId;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMeal(MealVm mealVM, IFormFile imageUrl, int resId)
        {
            if (ModelState.IsValid)
            {
                var resOfValidation = imageService.ValidateImageSizeAndExtension(imageUrl);

                if (resOfValidation is string errorMsg)
                {
                    ModelState.AddModelError("", errorMsg);
                    ViewBag.ResturantId = resId;
                    return View(mealVM);
                }

                var fileName = imageService.CreateNewImageExtention(imageUrl, "wwwroot\\images\\meals");

                if (fileName is null)
                {
                    ModelState.AddModelError("", "Error in uploading image");
                    ViewBag.ResturantId = resId;
                    return View(mealVM);
                }

                mealVM.ImageUrl = fileName;

                var meal = MealVm.MapToMeal(mealVM, resId);

                unitOfWorkRepository.Meals.Create(meal);
                unitOfWorkRepository.SaveChanges();

                return BackToMealsView("Your New Meal Added Successfully", resId);
            }

            ViewBag.ResturantId = resId;
            return View(mealVM);
        }

        public IActionResult MealDetails(int id, int resId)
        {
            var meal = unitOfWorkRepository.Meals.GetOne(expression: m => m.Id == id);
            var mealVM = MealVm.MaptoMealVm(meal);
            return View(mealVM);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MealDetails(MealVm mealVM, IFormFile imageUrl)
        {
            var oldMeal = unitOfWorkRepository.Meals.GetOne(expression: m => m.Id == mealVM.Id, tracked: false);

            if (ModelState.IsValid)
            {
                var resOfValidation = imageService.ValidateImageSizeAndExtension(imageUrl);

                if (resOfValidation is string errorMsg)
                {
                    ModelState.AddModelError("", errorMsg);
                    ViewBag.ResturantId = oldMeal.RestaurantId;
                    return View(mealVM);
                }

                var resOfDeleteOldImage = imageService.DelteOldImage(oldMeal.ImageUrl, "wwwroot\\images\\meals");

                if (!resOfDeleteOldImage)
                {
                    ModelState.AddModelError("", "Error in deleting old image");
                    ViewBag.ResturantId = oldMeal.RestaurantId;
                    return View(mealVM);
                }

                var fileName = imageService.CreateNewImageExtention(imageUrl, "wwwroot\\images\\meals");

                if (fileName is null)
                {
                    ModelState.AddModelError("", "Error in uploading new image");
                    ViewBag.ResturantId = oldMeal.RestaurantId;
                    return View(mealVM);
                }

                mealVM.ImageUrl = fileName;

                var newMeal = MealVm.MapToMeal(mealVM, oldMeal);

                unitOfWorkRepository.Meals.Edit(newMeal);
                unitOfWorkRepository.SaveChanges();

                return BackToMealsView("Your Old Meal Updated Successfully", oldMeal.RestaurantId);
            }

            ViewBag.ResturantId = oldMeal.RestaurantId;
            return View(mealVM);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMeal(int id, int resId)
        {
            var meal = unitOfWorkRepository.Meals.GetOne(expression: m => m.Id == id);

            if (meal != null)
            {
                unitOfWorkRepository.Meals.Delete(meal);
                unitOfWorkRepository.SaveChanges();
            }

            TempData["Success"] = "Choosed meal was delleted";
            return Redirect($"/SuperAdmin/RestaurantMeals/ResturantMeals/{resId}");
        }
        
        private IActionResult BackToMealsView(string msg, int? resId)
        {
            TempData["Success"] = msg;
            return Redirect($"/SuperAdmin/RestaurantMeals/ResturantMeals/{resId}");
        }
    }
}
