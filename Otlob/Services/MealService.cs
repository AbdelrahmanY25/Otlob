namespace Otlob.Services
{
    public class MealService : IMealService
    {
        private readonly IImageService imageService;
        private readonly IDataProtector dataProtector;
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly IMealPriceHistoryService mealPriceHistoryService;

        public MealService(IImageService imageService,
                           IUnitOfWorkRepository unitOfWorkRepository,
                           IDataProtectionProvider dataProtectionProvider,
                           IMealPriceHistoryService mealPriceHistoryService)
        {
            this.imageService = imageService;
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.mealPriceHistoryService = mealPriceHistoryService;
            this.dataProtector = dataProtectionProvider.CreateProtector("SecureData");
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
                        Key = dataProtector.Protect(m.Id.ToString()),
                        Name = m.Name,
                        Image = m.Image,
                        Price = m.Price,
                    }
                 );

            return mealsVM!;
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

            return mealsVM!;
        }

        public MealVm GetMealVM(int mealId)
        {
            var meal = unitOfWorkRepository.Meals.GetOne(expression: m => m.Id == mealId, ignoreQueryFilter: true);
            return MealVm.MaptoMealVm(meal!);
        }

        public Meal GetMeal(int mealId) => unitOfWorkRepository.Meals.GetOne(expression: m => m.Id == mealId)!;

        public Meal GetMealNameAndImage(int mealId) => unitOfWorkRepository
                .Meals
                .GetOneWithSelect(selector: m => new Meal { Id = m.Id, Name = m.Name, Image = m.Image}, expression: m => m.Id == mealId)!;

        public string AddMeal(MealVm mealVM, int restaurantId, IFormFile image)
        {
            var isImageUploaded = imageService.UploadImage(image)!;

            if (!isImageUploaded.IsSuccess)
            {
                return isImageUploaded.Message!;
            }

            Meal meal = FillMealData(mealVM, restaurantId, isImageUploaded.ImageUrl);

            unitOfWorkRepository.Meals.Create(meal);
            unitOfWorkRepository.SaveChanges();

            mealPriceHistoryService.AddMealPriceHistory(meal.Id, meal.Price);

            return null!;
        }        

        public string EditMeal(MealVm mealVM, int mealId)
        {            
            Meal oldMeal = GetMeal(mealId);

            if (oldMeal.Price != mealVM.Price)
            {
                mealPriceHistoryService.UpdateMealPriceHistory(mealId, mealVM.Price);
            }

            bool thereIsNewData = ThereIsNewData(mealVM, oldMeal);

            if (!thereIsNewData)
            {
                mealVM.Image = oldMeal.Image;
                return "No new data to upload it";
            }

            UpdateMealData(mealVM, oldMeal);

            unitOfWorkRepository.Meals.Edit(oldMeal);      
            unitOfWorkRepository.SaveChanges();

            return null!;
        }

        public Meal GetMealImageById(int mealId)
        {
            Meal meal = unitOfWorkRepository
                .Meals
                .GetOneWithSelect(
                    expression: m => m.Id == mealId,
                    selector: m => new Meal
                    {
                        Id = m.Id,
                        Image = m.Image
                    }
                )!;

            return meal;
        }

        public void UpdateMealImage(Meal meal, string imageUrl)
        {
            meal.Image = imageUrl;
            unitOfWorkRepository.Meals.ModifyProperty(meal, r => r.Image!);
            unitOfWorkRepository.SaveChanges();
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
                        Key = dataProtector.Protect(m.Id.ToString()),
                        Name = m.Name,
                        Image = m.Image,
                        Price = m.Price,                        
                    }
                 );

            return mealVms!;
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
            if (!filter.IsNullOrEmpty() && filter.ToLower() != "all")
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

        private bool ThereIsNewData(MealVm newMeal, Meal oldMeal)
        {
            bool result = newMeal.Name == oldMeal.Name ?
                          false : newMeal.Description == oldMeal.Description ?
                          false : newMeal.Category == oldMeal.Category ?
                          false : newMeal.IsNewMeal == oldMeal.IsNewMeal ?
                          false : newMeal.IsTrendingMeal == newMeal.IsTrendingMeal ?
                          false : newMeal.NumberOfServings == oldMeal.NumberOfServings ?
                          false : newMeal.IsAvailable == oldMeal.IsAvailable;

            return result;                
        }

        private Meal FillMealData(MealVm mealVm, int restaurantId, string imageUrl)
        {
            return new Meal
            {
                Image = imageUrl,
                Name = mealVm.Name,
                Price = mealVm.Price,
                Category = mealVm.Category,
                RestaurantId = restaurantId,
                IsNewMeal = mealVm.IsNewMeal,
                IsAvailable = mealVm.IsAvailable,
                Description = mealVm.Description,
                IsTrendingMeal = mealVm.IsTrendingMeal,
                NumberOfServings = mealVm.NumberOfServings
            };
        }

        private void UpdateMealData(MealVm mealVm, Meal oldMeal)
        {
            oldMeal.Name = mealVm.Name;
            oldMeal.Price = mealVm.Price;
            oldMeal.Category = mealVm.Category;
            oldMeal.IsNewMeal = mealVm.IsNewMeal;
            oldMeal.IsAvailable = mealVm.IsAvailable;
            oldMeal.Description = mealVm.Description;
            oldMeal.IsTrendingMeal = mealVm.IsTrendingMeal;
            oldMeal.NumberOfServings = mealVm.NumberOfServings;
        }
    }
}
