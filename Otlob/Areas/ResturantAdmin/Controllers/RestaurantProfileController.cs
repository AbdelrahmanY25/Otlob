using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Services;
using Otlob.Core.IServices;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;
using RepositoryPatternWithUOW.Core.Models;
using Utility;

namespace Otlob.Areas.Restaurants.Controllers
{
    [Area("ResturantAdmin"),Authorize(Roles = SD.restaurantAdmin)]
    public class RestaurantProfileController : Controller
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IImageService imageService;

        public RestaurantProfileController(IUnitOfWorkRepository unitOfWorkRepository,
                                           UserManager<ApplicationUser> userManager,
                                           IImageService imageService)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.userManager = userManager;
            this.imageService = imageService;
        }
        
        public async Task<IActionResult> EditRestaurantProfile()
        {
            var user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                var resturant = unitOfWorkRepository.Restaurants.GetOne(expression: r => r.Id == user.Resturant_Id, tracked: false);                
                var resturantVM = RestaurantVM.MapToRestaurantVM(resturant);
                return View(resturantVM);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRestaurantProfile(RestaurantVM restaurantVM, IFormFile logo)
        {
            var user = await userManager.GetUserAsync(User);

            if (user is null) return RedirectToAction("Index", "Home");

            var oldResturantInfo = unitOfWorkRepository.Restaurants.GetOne(expression: r => r.Id == user.Resturant_Id, tracked: false);
            
            if (!ValidateOnCreticalData(restaurantVM, oldResturantInfo))
            {
                ModelState.AddModelError("", "You can't change your resturaant [ Name or Email or Address ]");
                return View(restaurantVM);
            }

            if (ModelState.IsValid)
            {                
                var resOfValidation = imageService.ValidateImageSizeAndExtension(logo);

                if (resOfValidation is string error)
                {
                    ModelState.AddModelError("", error);
                    restaurantVM.Logo = oldResturantInfo.Logo;
                    return View(restaurantVM);
                }

                var resOfDeleteOldImage = imageService.DelteOldImage(oldResturantInfo.Logo, "wwwroot\\images\\resturantLogo");

                if (!resOfDeleteOldImage)
                {
                    ModelState.AddModelError("", "Error in deleting old image");
                    restaurantVM.Logo = oldResturantInfo.Logo;
                    return View(restaurantVM);
                }

                var logoName = imageService.CreateNewImageExtention(logo, "wwwroot\\images\\resturantLogo");                

                if (logoName is null)
                {
                    ModelState.AddModelError("", "Error in uploading new image");
                    restaurantVM.Logo = oldResturantInfo.Logo;
                    return View(restaurantVM);
                }
    
                restaurantVM.Logo = logoName;

                var newRestaurant = RestaurantVM.MapToRestaurant(restaurantVM, oldResturantInfo);

                unitOfWorkRepository.Restaurants.Edit(newRestaurant);
                unitOfWorkRepository.SaveChanges();

                TempData["Success"] = "Your resturant info updated Successfully";

                return RedirectToAction("Index", "Home");
            }

            restaurantVM.Logo = oldResturantInfo.Logo;

            return View(restaurantVM);
        }

        private bool ValidateOnCreticalData(RestaurantVM restaurantVM, Restaurant oldResturantInfo)
        {
            return restaurantVM.Name == oldResturantInfo?.Name &&
                   restaurantVM.Email == oldResturantInfo.Email &&
                   restaurantVM.Address == oldResturantInfo.Address;
        }        
    }
}