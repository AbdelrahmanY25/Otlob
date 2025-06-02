namespace Otlob.Core.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IUnitOfWorkRepository.IUnitOfWorkRepository unitOfWorkRepository;
        private readonly IImageService imageService;
        private readonly IRestaurantFilterService restaurantFilterService;
        private readonly IEncryptionService encryptionService;
        private readonly IUserServices userServices;

        public RestaurantService(IUnitOfWorkRepository.IUnitOfWorkRepository unitOfWorkRepository,
                                 IImageService imageService,
                                 IRestaurantFilterService restaurantFilterService,
                                 IEncryptionService encryptionService,
                                 IUserServices userServices)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.imageService = imageService;
            this.restaurantFilterService = restaurantFilterService;
            this.encryptionService = encryptionService;
            this.userServices = userServices;
        }

        public IQueryable<RestaurantVM> GetAllRestaurantsJustMainInfo(RestaurantCategory? filter = null, AcctiveStatus[]? statuses = null)
        {
            var descriptionFilter = restaurantFilterService.RestaurantCategoryFilter(filter);
            var statusFilter = restaurantFilterService.RestaurantsStatusFilter(statuses);

            var combineFilters = PredicateBuilder.New<Restaurant>(true)
                .And(descriptionFilter)
                .And(statusFilter);

            var restaurantsVM = unitOfWorkRepository.Restaurants
                .GetAllWithSelect
                 (
                    expression: combineFilters,
                    tracked: false,
                    selector: r => new RestaurantVM
                    {
                        RestaurantVMId = r.Id,
                        AcctiveStatus = r.AcctiveStatus,
                        Name = r.Name,
                        Image = r.Image,
                        DeliveryDuration = r.DeliveryDuration,
                        DeliveryFee = r.DeliveryFee
                    }
                 );

            return restaurantsVM;
        }

        public RestaurantVM GetRestaurantJustMainInfo(int restaurantId)
        {
            var restaurantsVM = unitOfWorkRepository.Restaurants
                .GetOneWithSelect
                 (
                    expression: r => r.Id == restaurantId,
                    tracked: false,
                    selector: r => new RestaurantVM
                    {
                        Name = r.Name,
                        DeliveryFee = r.DeliveryFee,
                        AcctiveStatus = r.AcctiveStatus,
                        Image = r.Image,
                    }
                 );

            return restaurantsVM;
        }

        public RestaurantVM GetRestaurantDetailsById(int restaurantId)
        {
            var restaurant = unitOfWorkRepository.Restaurants
                .GetOne
                 (
                    expression: r => r.Id == restaurantId, tracked: false
                 );

            string? userId = userServices.GetUserIdByRestaurantId(restaurantId);

            var restaurantsVM = RestaurantVM.MapToRestaurantVMWithId(restaurant, userId);

            return restaurantsVM;
        }

        public async Task<string>? EditRestaurantProfileInfo(RestaurantVM restaurantVM, int restaurantId, IFormFileCollection image, bool ValidateData = true)
        {
            Restaurant? oldResturantInfo = unitOfWorkRepository.Restaurants.GetOne(expression: r => r.Id == restaurantId);

            if (ValidateData)
            {
                if (!restaurantFilterService.ValidateDataWhenEditRedtaurantProfile(restaurantVM, oldResturantInfo))
                {
                    return "You can't change your resturaant [ Name or Email or Address ]";
                }
            }

            if (image.Count > 0)
            {
                string? isImageUpdated = await imageService.UploadImage(image, restaurantVM);

                if (isImageUpdated is string)
                {
                    return isImageUpdated;
                }

                oldResturantInfo.Image = restaurantVM.Image;
            }


            Restaurant newRestaurant = restaurantVM.MapToRestaurant(oldResturantInfo);

            unitOfWorkRepository.Restaurants.Edit(newRestaurant);
            unitOfWorkRepository.SaveChanges();

            return null;
        }

        public bool ChangeRestauranStatus(string id, AcctiveStatus status)
        {
            int restaurantId = encryptionService.DecryptId(id);

            Restaurant? restaurant = unitOfWorkRepository.Restaurants.GetOne(expression: r => r.Id == restaurantId);

            if (restaurant is null)
            {
                return false;
            }

            restaurant.AcctiveStatus = status;

            unitOfWorkRepository.Restaurants.Edit(restaurant);
            unitOfWorkRepository.SaveChanges();

            return true;
        }

        public IQueryable<Restaurant>? GetDeletedRestaurants()
        {
            IQueryable<Restaurant>? restaurants = unitOfWorkRepository.Restaurants
                .Get(expression: r => EFCore.Property<bool>(r, "IsDeleted"),
                    tracked: false,
                    ignoreQueryFilter: true
                );

            if (restaurants is null)
            {
                return null;
            }

            return restaurants;
        }

        public bool DelteRestaurant(string id)
        {
            int restaurantId = encryptionService.DecryptId(id);      

            unitOfWorkRepository.Restaurants.SoftDelete(r => r.Id == restaurantId);

            unitOfWorkRepository.Users.SoftDelete(u => u.RestaurantId == restaurantId);

            unitOfWorkRepository.Meals.SoftDelete(expression: m => m.RestaurantId == restaurantId);

            unitOfWorkRepository.SaveChanges();

            return true;
        }

        public bool UnDelteRestaurant(string id)
        {
            int restaurantId = encryptionService.DecryptId(id);

            unitOfWorkRepository.Restaurants.UnSoftDelete(r => r.Id == restaurantId);

            unitOfWorkRepository.Users.UnSoftDelete(u => u.RestaurantId == restaurantId);

            unitOfWorkRepository.Meals.UnSoftDelete(expression: m => m.RestaurantId == restaurantId);
                       
            unitOfWorkRepository.SaveChanges();

            return true;
        }
    }
}
