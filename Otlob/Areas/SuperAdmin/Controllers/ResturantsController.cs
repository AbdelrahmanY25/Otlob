using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IServices;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Otlob.Core.Services;
using Otlob.Core.ViewModel;
using RepositoryPatternWithUOW.Core.Models;
using Utility;

namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(Roles = SD.superAdminRole)]
    public class ResturantsController : Controller
    {       
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IImageService imageService;
        private readonly IUserServices userServices;

        public ResturantsController(IUnitOfWorkRepository unitOfWorkRepository,
                                    UserManager<ApplicationUser> userManager, 
                                    IImageService imageService,
                                    IUserServices userServices)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.userManager = userManager;
            this.imageService = imageService;
            this.userServices = userServices;
        }
        public IActionResult ResturantDetails(int id)
        {
            var resturnat = unitOfWorkRepository.Restaurants.GetOne(expression: r => r.Id == id);

            return View(resturnat);
        }        

        #region ResturantsProfiles
        public IActionResult ResturantProfile(int id)
        {
            var res = unitOfWorkRepository.Restaurants.GetOne(expression: r => r.Id == id);
            if (res == null) return RedirectToAction("ActiveResturatns", "Home");
            var resVM = RestaurantVM.MapToRestaurantVM(res);
            ViewBag.ResturantId = res.Id;
            return View(resVM);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ResturantProfile(RestaurantVM restaurantVM, int resId, IFormFile logo)
        {
            var oldResturant = unitOfWorkRepository.Restaurants.GetOne(expression: r => r.Id == resId, tracked: false);

            if (ModelState.IsValid)
            {
                var resOfValidation = imageService.ValidateImageSizeAndExtension(logo);

                if (resOfValidation is string error)
                {
                    ModelState.AddModelError("", error);
                    restaurantVM.Logo = oldResturant.Logo;
                    ViewBag.ResturantId = resId;
                    return View(restaurantVM);
                }                

                var resOfDeleteOldImage = imageService.DelteOldImage(oldResturant.Logo, "wwwroot\\images\\resturantLogo");

                if (!resOfDeleteOldImage)
                {
                    ModelState.AddModelError("", "Error in deleting old image");
                    restaurantVM.Logo = oldResturant.Logo;
                    ViewBag.ResturantId = resId;
                    return View(restaurantVM);
                }

                var logoName = imageService.CreateNewImageExtention(logo, "wwwroot\\images\\resturantLogo");

                if (logoName is null)
                {
                    ModelState.AddModelError("", "Error in uploading new image");
                    restaurantVM.Logo = oldResturant.Logo;
                    ViewBag.ResturantId = resId;
                    return View(restaurantVM);
                }

                restaurantVM.Logo = logoName;

                var restaurant = RestaurantVM.MapToRestaurant(restaurantVM, oldResturant);

                unitOfWorkRepository.Restaurants.Edit(restaurant);
                unitOfWorkRepository.SaveChanges();

                TempData["Success"] = "Your resturant info updated Successfully";

                return RedirectToAction("ResturantProfile", resId);
            }

            ViewBag.ResturantId = resId;
            restaurantVM.Logo = oldResturant.Logo;
            return View(restaurantVM);
        }

        public IActionResult ResturantAdminProfile(int id)
        {
            var admin = unitOfWorkRepository.Users.GetOne(expression: a => a.Resturant_Id == id);

            if (admin != null)
            {
                var userProfile = ProfileVM.MapToProfileVM(admin);

                ViewBag.ResturantId = admin.Resturant_Id;

                return View(userProfile);
            }

            return View(admin);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResturantAdminProfile(ProfileVM profileVM, int resId)
        {
            var admin = unitOfWorkRepository.Users.GetOne(expression: a => a.Resturant_Id == resId);

            if (ModelState.IsValid)
            {
                if (admin != null)
                {                 
                    admin = userServices.ValidateUserInfo(admin, profileVM);

                    var updateAdminInfo = await userManager.UpdateAsync(admin);

                    if(!imageService.UploadUserProfilePicture(Request.Form.Files)) return RedirectToAction("ResturantAdminProfile", resId);

                    var profilePic = Request.Form.Files.FirstOrDefault();

                    var resOfValidation = imageService.ValidateImageSizeAndExtension(profilePic);

                    if (resOfValidation is string error)
                    {
                        ModelState.AddModelError("", error);
                        profileVM.ProfilePicture = admin.ProfilePicture;
                        ViewBag.ResturantId = resId;
                        return View(profileVM);
                    }

                    if (!await imageService.CopyImageToMemoryStream(profilePic, admin))
                    {
                        ModelState.AddModelError("", "Error in uploading image");
                        ViewBag.ResturantId = resId;
                        return View(profileVM);
                    }
                    
                    updateAdminInfo = await userManager.UpdateAsync(admin);

                    if (updateAdminInfo.Succeeded)
                    {
                        TempData["Success"] = "User profile updated successfully.";
                        return RedirectToAction("ResturantAdminProfile", resId);
                    }


                    foreach (var errorInfo in updateAdminInfo.Errors)
                    {
                        ModelState.AddModelError("", errorInfo.Description);
                    }
                }
            }

            ViewBag.ResturantId = resId;
            profileVM.ProfilePicture = admin.ProfilePicture;
            return View(profileVM);
        }

        #endregion ResturantsProfiles       

        #region ResturantsOrders

        public IActionResult CurrentResturantOrders(int id, int pageNumber = 1)
        {
            var orders = unitOfWorkRepository.Orders.Get([o => o.ApplicationUser, o => o.Restaurant], expression: o => o.RestaurantId == id && o.Status != OrderStatus.Delivered);
            var resturant = unitOfWorkRepository.Orders.GetOne([o => o.Restaurant], expression: o => o.RestaurantId == id);

            if (orders != null)
                orders = orders.OrderByDescending(o => o.OrderDate);

            ViewBag.ResId = id;

            if (orders.Count() == 0)
                ViewBag.MostExpensiveOrder = 0.0;
            else
                ViewBag.MostExpensiveOrder = orders.Max(O => O.OrderPrice) + resturant.Restaurant.DeliveryFee;

            const int pageSize = 9;
            double pageCount = Math.Ceiling((double)orders.Count() / pageSize);


            if (pageNumber - 1 < pageCount)
            {
                orders = orders.Skip((pageNumber - 1) * pageSize).Take(pageSize).OrderByDescending(o => o.OrderDate);

                ViewBag.Count = pageCount;
                pageNumber = Math.Clamp(pageNumber, 1, (int)pageCount);
                ViewBag.PageNumber = pageNumber;

                return View(orders);
            }

            return View(orders);
        }
        
        public IActionResult DeliveredOrders(int id, int pageNumber = 1)
        {
            var orders = unitOfWorkRepository.Orders.Get([o => o.ApplicationUser], expression: o => o.RestaurantId == id && o.Status == OrderStatus.Delivered);
            var resturant = unitOfWorkRepository.Orders.GetOne([o => o.Restaurant], expression: o => o.RestaurantId == id);

            if (orders != null)
                orders = orders.OrderByDescending(o => o.OrderDate);

            ViewBag.ResId = id;

            if (orders.Count() == 0)
                ViewBag.MostExpensiveDeliveredOrder = 0.0;
            else
                ViewBag.MostExpensiveOrder = orders.Max(O => O.OrderPrice) + resturant.Restaurant.DeliveryFee;

            const int pageSize = 9;
            double pageCount = Math.Ceiling((double)orders.Count() / pageSize);

            if (pageNumber - 1 < pageCount)
            {
                orders = orders.Skip((pageNumber - 1) * pageSize).Take(pageSize);

                ViewBag.Count = pageCount;
                pageNumber = Math.Clamp(pageNumber, 1, (int)pageCount);
                ViewBag.PageNumber = pageNumber;

                return View(orders);
            }

            return View(orders);
        }

        public IActionResult OrderDetails(int id)
        {
            var order = unitOfWorkRepository.Orders.GetOne(expression: o => o.Id == id);

            var meals = unitOfWorkRepository.MealsInOrder.Get([m => m.Meal], expression: m => m.CartInOrderId == order.CartInOrderId);

            var mealsPrice = meals.Sum(m => m.Meal.Price * m.Quantity);

            var resturant = unitOfWorkRepository.Restaurants.GetOne(expression: o => o.Id == order.RestaurantId);

            ViewBag.OrderDetails = order;
            ViewBag.SubPrice = mealsPrice;
            ViewBag.DeliveryFee = resturant.DeliveryFee;

            return View(meals);
        }

        #endregion ResturantsOrders

        #region ResturantComplaints

        public IActionResult ResturantComplaints(int id)
        {
            var complaints = unitOfWorkRepository.UserComplaints.Get(expression: c => c.RestaurantId == id);
            return View(complaints);
        }

        #endregion ResturantComplaints
    }
}
