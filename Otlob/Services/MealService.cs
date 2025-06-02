namespace Otlob.Services
{
    public class MealService : IMealService
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly IImageService imageService;
        private readonly IMealPriceHistoryService mealPriceHistoryService;

        public MealService(IUnitOfWorkRepository unitOfWorkRepository, IImageService imageService, IMealPriceHistoryService mealPriceHistoryService)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.imageService = imageService;
            this.mealPriceHistoryService = mealPriceHistoryService;
        }

        public IQueryable<MealVm> ViewMealsVmToRestaurantAdminSummary(int restaurantlId)
        {
            IQueryable<MealVm>? mealsVM = unitOfWorkRepository.Meals
                .GetAllWithSelect
                 (
                    expression: m => m.RestaurantId == restaurantlId,
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

        public MealVm GetMealVM(int mealId)
        {
            var meal = unitOfWorkRepository.Meals.GetOne(expression: m => m.Id == mealId);
            return MealVm.MaptoMealVm(meal);
        }

        public Meal GetMeal(int mealId) => unitOfWorkRepository.Meals.GetOne(expression: m => m.Id == mealId);

        public Meal GetMealNameAndImage(int mealId) => unitOfWorkRepository.Meals.GetOneWithSelect(selector: m => new Meal { Id = m.Id, Name = m.Name, Image = m.Image},expression: m => m.Id == mealId);

        public async Task<string> AddMeal(MealVm mealVM, int restaurantId, IFormFileCollection image)
        {
            string isImageUploaded = await imageService.UploadImage(image, imageUrl: mealVM);

            if (isImageUploaded is string)
            {
                return isImageUploaded;
            }

            var meal = MealVm.MapToMeal(mealVM, restaurantId);

            unitOfWorkRepository.Meals.Create(meal);
            unitOfWorkRepository.SaveChanges();

            mealPriceHistoryService.AddMealPriceHistory(meal.Id, meal.Price);

            return null;
        }

        public async Task<string> EditMeal(MealVm mealVM, int mealId, IFormFileCollection image)
        {            
            var oldMeal = GetMeal(mealId);

            if (oldMeal.Price != mealVM.Price)
            {
                mealPriceHistoryService.UpdateMealPriceHistory(mealId, mealVM.Price);
            }

            if (image.Count <= 0)
            {

                var newMeal = MealVm.MapToMeal(mealVM, oldMeal);

                unitOfWorkRepository.Meals.Edit(newMeal);

                unitOfWorkRepository.SaveChanges();

                return null;
            }

            var newMealWithImage = MealVm.MapToMeal(mealVM, oldMeal);

            string isImageUploaded = await imageService.UploadImage(image, imageUrl: mealVM);

            if (isImageUploaded is string)
            {
                return isImageUploaded;
            }

            newMealWithImage.Image = mealVM.Image;

            unitOfWorkRepository.Meals.Edit(newMealWithImage);
            
            unitOfWorkRepository.SaveChanges();

            return null;
        }

        public IQueryable<MealVm> GetDeletedMeals(int restaurantId)
        {
            IQueryable<MealVm>? mealVms = unitOfWorkRepository.Meals.GetAllWithSelect
                 (
                    expression: m => m.RestaurantId == restaurantId && EFCore.Property<bool>(m, "IsDeleted"),
                    tracked: false,
                    ignoreQueryFilter: true,
                    selector: m => new MealVm
                    {
                        MealVmId = m.Id,
                        Name = m.Name,
                        Image = m.Image,
                        Price = m.Price,                        
                    }
                 );

            return mealVms;
        }

        public bool DeleteMeal(int mealId)
        {
            unitOfWorkRepository.Meals.SoftDelete(expression: m => m.Id == mealId);
            unitOfWorkRepository.SaveChanges();
            return true;
        }

        public bool UnDeleteMeal(int mealId)
        {
            unitOfWorkRepository.Meals.UnSoftDelete(expression: m => m.Id == mealId);
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
