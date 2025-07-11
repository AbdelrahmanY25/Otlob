namespace Otlob.Core.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IUserServices userServices;
        private readonly IDataProtector dataProtector;
        private readonly IRestaurantFilterService restaurantFilterService;
        private readonly IUnitOfWorkRepository.IUnitOfWorkRepository unitOfWorkRepository;


        public RestaurantService(IUserServices userServices,
                                 IDataProtectionProvider dataProtectionProvider,
                                 IRestaurantFilterService restaurantFilterService,
                                 IUnitOfWorkRepository.IUnitOfWorkRepository unitOfWorkRepository)
        {
            this.userServices = userServices;
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.restaurantFilterService = restaurantFilterService;
            dataProtector = dataProtectionProvider.CreateProtector("SecureData");
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
                        Key = dataProtector.Protect(r.Id.ToString()),
                        AcctiveStatus = r.AcctiveStatus,
                        Name = r.Name!,
                        Image = r.Image,
                        DeliveryDuration = r.DeliveryDuration,
                        DeliveryFee = r.DeliveryFee,
                    }
                 );

            return restaurantsVM!;
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
                        Name = r.Name!,
                        DeliveryFee = r.DeliveryFee,
                        AcctiveStatus = r.AcctiveStatus,
                        Image = r.Image,
                    }
                 );

            return restaurantsVM!;
        }

        public RestaurantVM GetRestaurantVMDetailsById(int restaurantId)
        {
            var restaurantsVM = unitOfWorkRepository.Restaurants
                .GetOneWithSelect
                 (
                    expression: r => r.Id == restaurantId,
                    tracked: false,
                    ignoreQueryFilter: true,
                    selector: r => new RestaurantVM
                    {
                        Key = dataProtector.Protect(r.Id.ToString()),
                        Name = r.Name!,
                        Address = r.Address!,
                        Phone = r.Phone!,
                        Email = r.Email!,
                        Description = r.Description!,
                        DeliveryDuration = r.DeliveryDuration,
                        DeliveryFee = r.DeliveryFee,
                        AcctiveStatus = r.AcctiveStatus,
                        Category = r.Category,
                        Image = r.Image
                    }
                 );

            string userId = userServices.GetUserIdByRestaurantId(restaurantId)!;

            restaurantsVM!.UserId = userId;

            return restaurantsVM;
        }

        public string EditRestaurantProfileInfo(RestaurantVM restaurantVM, int restaurantId, bool ValidateData = true)
        {
            Restaurant? oldResturantInfo = unitOfWorkRepository.Restaurants.GetOne(expression: r => r.Id == restaurantId);

            if (ValidateData)
            {
                if (!restaurantFilterService.ValidateDataWhenEditRedtaurantProfile(restaurantVM, oldResturantInfo!))
                {
                    return "You can't change your resturaant [ Name or Email or Address ]";
                }
            }
            
            UpdateRestaurantData(restaurantVM, oldResturantInfo!);

            unitOfWorkRepository.Restaurants.Edit(oldResturantInfo!);            
            unitOfWorkRepository.SaveChanges();

            return null!;
        }

        public Restaurant GetRestaurantImageById(int restaurantId)
        {
            Restaurant restaurant = unitOfWorkRepository
                    .Restaurants
                    .GetOneWithSelect(
                        expression: r => r.Id == restaurantId,
                        selector: r => new Restaurant
                        {
                            Id = r.Id,
                            Image = r.Image
                        }
                    )!;

            return restaurant;
        }

        public void UpdateRestaurantImage(Restaurant restaurant, string imageUrl)
        {
            restaurant.Image = imageUrl;
            unitOfWorkRepository.Restaurants.ModifyProperty(restaurant, r => r.Image!);
            unitOfWorkRepository.SaveChanges();
        }

        public bool ChangeRestauranStatus(string id, AcctiveStatus status)
        {
            int restaurantId = int.Parse(dataProtector.Unprotect(id));

            Restaurant? restaurant = unitOfWorkRepository
                .Restaurants
                .GetOneWithSelect(
                    expression: r => r.Id == restaurantId,
                    selector: r => new Restaurant
                    {
                        Id = r.Id,
                        AcctiveStatus = r.AcctiveStatus
                    }
                );

            if (restaurant is null)
            {
                return false;
            }

            restaurant.AcctiveStatus = status;
            unitOfWorkRepository.Restaurants.ModifyProperty(restaurant, r => r.AcctiveStatus);
            unitOfWorkRepository.SaveChanges();

            return true;
        }

        public IQueryable<RestaurantVM>? GetDeletedRestaurants()
        {
            IQueryable<RestaurantVM>? restaurants = unitOfWorkRepository
                    .Restaurants
                    .GetAllWithSelect(
                        expression: r => EFCore.Property<bool>(r, "IsDeleted"),
                        tracked: false,
                        ignoreQueryFilter: true,
                        selector: r => new RestaurantVM
                        {
                            Key = dataProtector.Protect(r.Id.ToString()),
                            Name = r.Name!,
                            Image = r.Image,
                        }
                    );

            if (restaurants is null)
            {
                return null;
            }

            return restaurants;
        }

        public bool DelteRestaurant(string id)
        {
            int restaurantId = int.Parse(dataProtector.Unprotect(id));

            unitOfWorkRepository.Restaurants.SoftDelete(r => r.Id == restaurantId);

            unitOfWorkRepository.Users.SoftDelete(u => u.RestaurantId == restaurantId);

            unitOfWorkRepository.Meals.SoftDelete(expression: m => m.RestaurantId == restaurantId);

            unitOfWorkRepository.Orders.SoftDelete(o => o.RestaurantId == restaurantId);

            unitOfWorkRepository.SaveChanges();

            return true;
        }

        public bool UnDelteRestaurant(string id)
        {
            int restaurantId = int.Parse(dataProtector.Unprotect(id));

            unitOfWorkRepository.Restaurants.UnSoftDelete(r => r.Id == restaurantId);

            unitOfWorkRepository.Users.UnSoftDelete(u => u.RestaurantId == restaurantId);

            unitOfWorkRepository.Meals.UnSoftDelete(expression: m => m.RestaurantId == restaurantId);

            unitOfWorkRepository.Orders.UnSoftDelete(o => o.RestaurantId == restaurantId);

            unitOfWorkRepository.SaveChanges();

            return true;
        }

        private void UpdateRestaurantData(RestaurantVM restaurantVM, Restaurant oldRestaurant)
        {
            oldRestaurant.Name = restaurantVM.Name;
            oldRestaurant.Address = restaurantVM.Address;
            oldRestaurant.Phone = restaurantVM.Phone;
            oldRestaurant.Email = restaurantVM.Email;
            oldRestaurant.Description = restaurantVM.Description;
            oldRestaurant.DeliveryDuration = restaurantVM.DeliveryDuration;
            oldRestaurant.DeliveryFee = restaurantVM.DeliveryFee;
        }
    }
}
