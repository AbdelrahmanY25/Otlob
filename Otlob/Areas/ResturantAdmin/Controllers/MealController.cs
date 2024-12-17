using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;
using RepositoryPatternWithUOW.Core.Models;
using System.Linq;

namespace Otlob.Areas.ResturantAdmin.Controllers
{
    [Area("ResturantAdmin")]
    public class MealController : Controller
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public MealController(IUnitOfWorkRepository unitOfWorkRepository,
                                 UserManager<ApplicationUser> userManager)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var resturnat =  await userManager.GetUserAsync(User);
            var meals = unitOfWorkRepository.Meals.Get(expression: m => m.RestaurantId == resturnat.Resturant_Id);

            return View(meals);
        }
        public IActionResult AddMeal()
        {           
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMeal(MealVm mealVm, IFormFile ImageUrl)
        {
            if (ModelState.IsValid)
            {
                if (ImageUrl != null)
                {
                    const long maxImageUrlSize = 4 * 1024 * 1024;

                    if (ImageUrl.Length > maxImageUrlSize)
                    {
                        ModelState.AddModelError("", "The ImageUrl size exceeds the 4MB limit.");
                        return View(mealVm);
                    }

                    var allowedExtentions = new[] { ".png", ".jpg", ".jpeg" };
                    var ImageUrlExtension = Path.GetExtension(ImageUrl.FileName).ToLowerInvariant();

                    if (!allowedExtentions.Contains(ImageUrlExtension))
                    {
                        ModelState.AddModelError("", "Invalid ImageUrl type. Only .jpg, .jpeg, and .png are allowed.");
                        return View(mealVm);
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageUrl.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\meals", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        ImageUrl.CopyTo(stream);
                    }

                    mealVm.ImageUrl = fileName;
                }

                var resturnat = await userManager.GetUserAsync(User);

                var meal = new Meal
                {
                    Name = mealVm.Name,
                    Description = mealVm.Description,
                    Price = mealVm.Price,
                    Category = mealVm.Category,
                    IsAvailable = mealVm.IsAvailable,
                    ImageUrl = mealVm.ImageUrl,
                    RestaurantId = resturnat.Resturant_Id
                };

                unitOfWorkRepository.Meals.Create(meal);
                unitOfWorkRepository.SaveChanges();

                var meals = unitOfWorkRepository.Meals.Get(expression: m => m.RestaurantId == resturnat.Resturant_Id);

                TempData["Success"] = "Your New Meal Added Successfully";

                return RedirectToAction("Index", meals);
            }

            return View(mealVm);
        }

        public IActionResult MealDetails(int id)
        {
            var meal = unitOfWorkRepository.Meals.GetOne(expression: m => m.Id == id);

            return View(meal);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MealDetails(Meal meal, IFormFile ImageUrl)
        {
            var oldMeal = unitOfWorkRepository.Meals.GetOne(expression: m => m.Id ==  meal.Id, tracked: false); 

            if (ModelState.IsValid)
            {
                if (ImageUrl != null)
                {
                    const long maxImageUrlSize = 4 * 1024 * 1024;

                    if (ImageUrl.Length > maxImageUrlSize)
                    {
                        ModelState.AddModelError("", "The ImageUrl size exceeds the 4MB limit.");
                        return View(meal);
                    }

                    var allowedExtentions = new[] { ".png", ".jpg", ".jpeg" };
                    var ImageUrlExtension = Path.GetExtension(ImageUrl.FileName).ToLowerInvariant();

                    if (!allowedExtentions.Contains(ImageUrlExtension))
                    {
                        ModelState.AddModelError("", "Invalid ImageUrl type. Only .jpg, .jpeg, and .png are allowed.");
                        return View(meal);
                    }

                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\meals", oldMeal.ImageUrl);

                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageUrl.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\meals", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        ImageUrl.CopyTo(stream);
                    }

                    meal.ImageUrl = fileName;
                }
                else
                {
                    meal.ImageUrl = oldMeal.ImageUrl;
                }

                var resturnat = await userManager.GetUserAsync(User);

                meal.RestaurantId = resturnat.Resturant_Id;

                unitOfWorkRepository.Meals.Edit(meal);
                unitOfWorkRepository.SaveChanges();


                var meals = unitOfWorkRepository.Meals.Get(expression: m => m.RestaurantId == resturnat.Resturant_Id);

                TempData["Success"] = "Your Old Meal Updated Successfully";

                return RedirectToAction("Index", meals);
            }

            return View(meal);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMeal(int id)
        {
            var resturnat = await userManager.GetUserAsync(User);
            var meals = unitOfWorkRepository.Meals.Get(expression: m => m.RestaurantId == resturnat.Resturant_Id);
            var meal = unitOfWorkRepository.Meals.GetOne(expression: m => m.Id == id);

            if (meal != null)
            {
                unitOfWorkRepository.Meals.Delete(meal);
                unitOfWorkRepository.SaveChanges();
            }

            return RedirectToAction("Index", meals);
        }
    }
}
