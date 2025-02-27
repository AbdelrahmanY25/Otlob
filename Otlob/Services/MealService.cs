using Otlob.Core.IServices;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;
using Otlob.IServices;

namespace Otlob.Services
{
    public class MealService : IMealService
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly IImageService imageService;

        public MealService(IUnitOfWorkRepository unitOfWorkRepository, IImageService imageService)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.imageService = imageService;
        }

        public IQueryable<MealVm> ViewMealsVmToRestaurantAdminSummary(int RestaurantlId)
        {
            IQueryable<MealVm>? mealsVM = unitOfWorkRepository.Meals
                .GetAllWithSelect
                 (
                    expression: m => m.RestaurantId == RestaurantlId,
                    tracked: false,
                    selector: m => new MealVm
                    {
                        MealVmId = m.Id,
                        Name = m.Name,
                        Image = m.Image,
                        Price = m.Price,
                    }
                 );

            return mealsVM;
        }

        public IQueryable<MealVm> ViewAllMealsVm(int RestaurantlId)
        {
            IQueryable<MealVm>? mealsVM = unitOfWorkRepository.Meals
                .GetAllWithSelect
                 (
                    expression: m => m.RestaurantId == RestaurantlId && m.IsAvailable,
                    tracked: false,
                    selector: m => new MealVm
                    {
                        MealVmId = m.Id,
                        Name = m.Name,
                        Image = m.Image,
                        Price = m.Price,
                        Category = m.Category,
                        NumberOfServings = m.NumberOfServings,
                        IsNewMeal = m.IsNewMeal,
                        IsTrendingMeal = m.IsTrendingMeal,
                        Description = m.Description
                    }
                 );

            return mealsVM;
        }

        public MealVm GetMealVM(int MealId)
        {
            var meal = unitOfWorkRepository.Meals.GetOne(expression: m => m.Id == MealId);
            return MealVm.MaptoMealVm(meal);
        }

        public Meal GetMeal(int MealId) => unitOfWorkRepository.Meals.GetOne(expression: m => m.Id == MealId);

        public async Task<string> AddMeal(MealVm mealVM, int restaurantId, IFormFileCollection image)
        {
            string isImageUploaded = await imageService.UploadImage(image, imageProp: mealVM);

            if (isImageUploaded is string)
            {
                return isImageUploaded;
            }

            var meal = MealVm.MapToMeal(mealVM, restaurantId);

            unitOfWorkRepository.Meals.Create(meal);
            unitOfWorkRepository.SaveChanges();

            return null;
        }

        public async Task<string> EditMeal(MealVm mealVM, int mealId, IFormFileCollection image)
        {
            string isImageUploaded = await imageService.UploadImage(image, imageProp: mealVM);

            if (isImageUploaded is string)
            {
                return isImageUploaded;
            }

            var oldMeal = GetMeal(mealId);

            var newMeal = MealVm.MapToMeal(mealVM, oldMeal);

            unitOfWorkRepository.Meals.Edit(newMeal);
            unitOfWorkRepository.SaveChanges();

            return null;
        }

        public bool DeleteMeal(int mealId)
        {
            Meal meal = GetMeal(mealId);

            unitOfWorkRepository.Meals.Delete(meal);
            unitOfWorkRepository.SaveChanges();
            return true;
        }

        public IQueryable<MealVm> MealCategoryFilter(IQueryable<MealVm> meals, string filter)
        {
            if (!string.IsNullOrEmpty(filter) && filter.ToLower() != "all")
            {
                meals = filter.ToLower() switch
                {
                    "new" => meals.Where(m => m.IsNewMeal),
                    "trend" => meals.Where(m => m.IsTrendingMeal),
                    "main" => meals.Where(m => m.Category == MealCategory.MainCourse),
                    "grilled" => meals.Where(m => m.Category == MealCategory.Grill),
                    "desserts" => meals.Where(m => m.Category == MealCategory.Dessert),
                    "bakeries" => meals.Where(m => m.Description.Contains("Bakeries")),
                    "drink" => meals.Where(m => m.Description.Contains("Drink")),
                    _ => meals
                };
            }
            return meals;
        }
    }
}
