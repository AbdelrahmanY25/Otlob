using Otlob.Core.IServices;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;
using Otlob.IServices;
using LinqKit;

namespace Otlob.Core.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IUnitOfWorkRepository.IUnitOfWorkRepository unitOfWorkRepository;
        private readonly IImageService imageService;
        private readonly IRestaurantFilterService restaurantFilterService;
        private readonly IEncryptionService encryptionService;

        public RestaurantService(IUnitOfWorkRepository.IUnitOfWorkRepository unitOfWorkRepository,
                                 IImageService imageService,
                                 IRestaurantFilterService restaurantFilterService,
                                 IEncryptionService encryptionService)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.imageService = imageService;
            this.restaurantFilterService = restaurantFilterService;
            this.encryptionService = encryptionService;
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

            var restaurantsVM = RestaurantVM.MapToRestaurantVMWithId(restaurant);

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
    }
}
