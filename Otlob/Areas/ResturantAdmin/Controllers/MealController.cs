using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IServices;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;

namespace Otlob.Areas.ResturantAdmin.Controllers
{
    [Area("ResturantAdmin")]
    public class MealController : Controller
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IImageService imageService;

        public MealController(IUnitOfWorkRepository unitOfWorkRepository,
                                 UserManager<ApplicationUser> userManager,
                                 IImageService imageService)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.userManager = userManager;
            this.imageService = imageService;
        }

        public async Task<IActionResult> Index()
        {
            var restaurnat =  await userManager.GetUserAsync(User);
            var meals = unitOfWorkRepository.Meals.Get(expression: m => m.RestaurantId == restaurnat.Resturant_Id);

            return View(meals);
        }
        public IActionResult AddMeal()
        {           
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMeal(MealVm mealVM, IFormFile imageUrl)
        {
            if (ModelState.IsValid)
            {
                var resOfValidation = imageService.ValidateImageSizeAndExtension(imageUrl);

                if (resOfValidation is string errorMsg)
                {
                    ModelState.AddModelError("", errorMsg);
                    return View(mealVM);
                }

                var fileName = imageService.CreateNewImageExtention(imageUrl, "wwwroot\\images\\meals");               

                mealVM.ImageUrl = fileName;

                var restaurnat = await userManager.GetUserAsync(User);

                var meal = MealVm.MapToMeal(mealVM, restaurnat);

                unitOfWorkRepository.Meals.Create(meal);
                unitOfWorkRepository.SaveChanges();

                return BackToMealsView("Your New Meal Added Successfully", restaurnat);                
            }

            return View(mealVM);
        }

        public IActionResult MealDetails(int id)
        {
            var meal = unitOfWorkRepository.Meals.GetOne(expression: m => m.Id == id);

            var mealVM = MealVm.MaptoMealVm(meal);

            return View(mealVM);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MealDetails(MealVm mealVM, IFormFile imageUrl)
        {
            var oldMeal = unitOfWorkRepository.Meals.GetOne(expression: m => m.Id ==  mealVM.Id, tracked: false); 

            if (ModelState.IsValid)
            {
                var resOfValidation = imageService.ValidateImageSizeAndExtension(imageUrl);

                if (resOfValidation is string errorMsg)
                {
                    ModelState.AddModelError("", errorMsg);
                    return View(mealVM);
                }

                if (oldMeal.ImageUrl != null)
                {
                    var resOfDeleteOldImage = imageService.DelteOldImage(oldMeal.ImageUrl, "wwwroot\\images\\meals");

                    if (!resOfDeleteOldImage)
                    {
                        ModelState.AddModelError("", "Error in deleting old image");
                        mealVM.ImageUrl = oldMeal.ImageUrl;
                        return View(mealVM);
                    }
                }

                var fileName = imageService.CreateNewImageExtention(imageUrl, "wwwroot\\images\\meals");

                if (fileName is null)
                {
                    ModelState.AddModelError("", "Error in uploading new image");
                    mealVM.ImageUrl = oldMeal.ImageUrl;
                    return View(mealVM);
                }

                mealVM.ImageUrl = fileName;                               

                var restaurnat = await userManager.GetUserAsync(User);

                var newMeal = MealVm.MapToMeal(mealVM, oldMeal);

                unitOfWorkRepository.Meals.Edit(newMeal);
                unitOfWorkRepository.SaveChanges();

                return BackToMealsView("Your Old Meal Updated Successfully", restaurnat);        
            }

            return View(mealVM);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMeal(int id)
        {
            var restaurnat = await userManager.GetUserAsync(User);
            var meals = unitOfWorkRepository.Meals.Get(expression: m => m.RestaurantId == restaurnat.Resturant_Id);
            var meal = unitOfWorkRepository.Meals.GetOne(expression: m => m.Id == id);

            if (meal != null)
            {
                unitOfWorkRepository.Meals.Delete(meal);
                unitOfWorkRepository.SaveChanges();
            }

            return RedirectToAction("Index", meals);
        }

        private IActionResult BackToMealsView(string msg, ApplicationUser restaurant)
        {
            var meals = unitOfWorkRepository.Meals.Get(expression: m => m.RestaurantId == restaurant.Resturant_Id);

            TempData["Success"] = msg;

            return RedirectToAction("Index", meals);
        }
    }
}
