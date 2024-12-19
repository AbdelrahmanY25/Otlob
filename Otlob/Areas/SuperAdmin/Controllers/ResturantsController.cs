using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;
using Otlob.EF.UnitOfWorkRepository;
using RepositoryPatternWithUOW.Core.Models;
using Utility;
using static NuGet.Packaging.PackagingConstants;

namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(Roles = SD.superAdminRole)]
    public class ResturantsController : Controller
    {       
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public ResturantsController(IUnitOfWorkRepository unitOfWorkRepository, UserManager<ApplicationUser> userManager)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.userManager = userManager;
        }
        public IActionResult ResturantDetails(int id)
        {
            var resturnat = unitOfWorkRepository.Restaurants.GetOne(expression: r => r.Id == id);

            return View(resturnat);
        }

        public IActionResult AcceptResturant(int id)
        {
            var res = unitOfWorkRepository.Restaurants.GetOne(expression: r => r.Id == id);
            if (res != null)
            {
                res.AcctiveStatus = AcctiveStatus.Acctive;
                unitOfWorkRepository.Restaurants.Edit(res);
                unitOfWorkRepository.SaveChanges();
            }

            var resturants = unitOfWorkRepository.Restaurants.Get(expression: r => r.AcctiveStatus != AcctiveStatus.Unaccepted);
            TempData["Success"] = "The Resturant Status is Active";
            return RedirectToAction("ActiveResturatns", "Home", resturants);
        }

        public IActionResult WarnResturant(int id)
        {
            var res = unitOfWorkRepository.Restaurants.GetOne(expression: r => r.Id == id);
            if (res != null)
            {
                res.AcctiveStatus = AcctiveStatus.Warning;
                unitOfWorkRepository.Restaurants.Edit(res);
                unitOfWorkRepository.SaveChanges();
            }

            TempData["Success"] = "The Warning Was Sent to Resturant";
            return RedirectToAction("ActiveResturatns", "Home");
        }

        public IActionResult BlockResturant(int id)
        {
            var res = unitOfWorkRepository.Restaurants.GetOne(expression: r => r.Id == id);
            if (res != null)
            {
                res.AcctiveStatus = AcctiveStatus.Block;
                unitOfWorkRepository.Restaurants.Edit(res);
                unitOfWorkRepository.SaveChanges();
            }

            TempData["Success"] = "The Resturant Account Was Blocked";
            return RedirectToAction("ActiveResturatns", "Home");
        }

        #region ResturantsProfiles
        public IActionResult ResturantProfile(int id)
        {
            var res = unitOfWorkRepository.Restaurants.GetOne(expression: r => r.Id == id);
            ViewBag.ResturantId = res.Id;
            return View(res);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ResturantProfile(Restaurant restaurant, IFormFile Logo)
        {
            var oldResturant = unitOfWorkRepository.Restaurants.GetOne(expression: r => r.Id == restaurant.Id, tracked: false);

            if (ModelState.IsValid)
            {
                if (Logo != null)
                {
                    if (Logo.Length > 0)
                    {
                        const long maxFileSize = 4 * 1024 * 1024;

                        if (Logo.Length == 0 || Logo.Length > maxFileSize)
                        {
                            ModelState.AddModelError("", "The file size exceeds the 4MB limit.");
                            return View(restaurant);
                        }

                        var allowedExtentions = new[] { ".png", ".jpg", ".jpeg" };
                        var logoExtention = Path.GetExtension(Logo.FileName).ToLowerInvariant();

                        if (!allowedExtentions.Contains(logoExtention))
                        {
                            ModelState.AddModelError("", "Invalid file type. Only .jpg, .jpeg, and .png are allowed.");
                            return View(restaurant);
                        }

                        if (oldResturant.Logo != null)
                        {
                            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\resturantLogo", oldResturant.Logo);

                            if (System.IO.File.Exists(oldPath))
                                System.IO.File.Delete(oldPath);
                        }

                        var logoName = Guid.NewGuid().ToString() + Path.GetExtension(Logo.FileName);
                        var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\resturantLogo", logoName);

                        using (var stream = System.IO.File.Create(logoPath))
                        {
                            Logo.CopyTo(stream);
                        }

                        restaurant.Logo = logoName;
                    }
                    else
                    {
                        restaurant.Logo = oldResturant.Logo;
                    }

                    restaurant.AcctiveStatus = oldResturant.AcctiveStatus;
                    unitOfWorkRepository.Restaurants.Edit(restaurant);
                    unitOfWorkRepository.SaveChanges();

                    TempData["Success"] = "Your resturant info updated Successfully";

                    return RedirectToAction("ResturantProfile");
                }
            }

            return View(oldResturant);
        }

        public IActionResult ResturantAdminProfile(int id)
        {
            var admin = unitOfWorkRepository.Users.GetOne(expression: a => a.Resturant_Id == id);

            if (admin != null)
            {
                var userProfile = new ProfileVM
                {
                    Email = admin.Email,
                    BirthDate = admin.BirthDate,
                    FirstName = admin.FirstName,
                    LastName = admin.LastName,
                    Gender = admin.Gender,
                    PhoneNumber = admin.PhoneNumber,
                    ProfilePicture = admin.ProfilePicture
                };

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
                    if (Request.Form.Files.Count > 0)
                    {
                        var file = Request.Form.Files.FirstOrDefault();

                        if (file != null)
                        {
                            const long maxFileSize = 4 * 1024 * 1024;
                            if (file.Length > maxFileSize)
                            {
                                ModelState.AddModelError("", "The file size exceeds the 4MB limit.");
                                return View(profileVM);
                            }

                            var allowedExtentions = new[] { ".png", ".jpg", ".jpeg" };
                            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

                            if (!allowedExtentions.Contains(fileExtension))
                            {
                                ModelState.AddModelError("", "Invalid file type. Only .jpg, .jpeg, and .png are allowed.");
                                return View(profileVM);
                            }

                            using (var memoryStream = new MemoryStream())
                            {
                                await file.CopyToAsync(memoryStream);
                                admin.ProfilePicture = memoryStream.ToArray();
                            }

                            var updateResult = await userManager.UpdateAsync(admin);

                            if (updateResult.Succeeded)
                            {
                                TempData["Success"] = "Profile Picture updated successfully.";
                                return RedirectToAction("ResturantAdminProfile");
                            }

                            foreach (var error in updateResult.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }

                    if (admin.Email != profileVM.Email || admin.FirstName != profileVM.FirstName || admin.LastName != profileVM.LastName || admin.PhoneNumber != profileVM.PhoneNumber || admin.Gender != profileVM.Gender || admin.BirthDate != profileVM.BirthDate)
                    {
                        admin.FirstName = profileVM.FirstName;
                        admin.LastName = profileVM.LastName;
                        admin.BirthDate = profileVM.BirthDate;
                        admin.Gender = profileVM.Gender;
                        admin.PhoneNumber = profileVM.PhoneNumber;
                        admin.Email = profileVM.Email;

                        var result = await userManager.UpdateAsync(admin);

                        if (result.Succeeded)
                        {
                            TempData["Success"] = "User profile updated successfully.";
                            return RedirectToAction("ResturantAdminProfile");
                        }
                    }
                }
            }

            return View(profileVM);
        }

        #endregion ResturantsProfiles

        #region ResturantsMeals
        public IActionResult ResturantMeals(int id)
        {
            var meals = unitOfWorkRepository.Meals.Get(expression: m => m.RestaurantId == id);
            ViewBag.ResturantId = id;
            return View(meals);
        }

        public IActionResult AddMeal(int resId)
        {
            ViewBag.ResturantId = resId;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMeal(MealVm mealVm, IFormFile ImageUrl, int resId)
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

                var meal = new Meal
                {
                    Name = mealVm.Name,
                    Description = mealVm.Description,
                    Price = mealVm.Price,
                    Category = mealVm.Category,
                    IsAvailable = mealVm.IsAvailable,
                    ImageUrl = mealVm.ImageUrl,
                    RestaurantId = resId
                };

                unitOfWorkRepository.Meals.Create(meal);
                unitOfWorkRepository.SaveChanges();

                TempData["Success"] = "Your New Meal Added Successfully";

                return Redirect($"/SuperAdmin/Resturants/ResturantMeals/{resId}");
            }

            return View(mealVm);
        }

        public IActionResult MealDetails(int id, int resId)
        {
            var meal = unitOfWorkRepository.Meals.GetOne(expression: m => m.Id == id);
            ViewBag.ResturantId = resId;
            return View(meal);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MealDetails(Meal meal, IFormFile ImageUrl, int resId)
        {
            var oldMeal = unitOfWorkRepository.Meals.GetOne(expression: m => m.Id == meal.Id, tracked: false);

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

                meal.RestaurantId = resId;

                unitOfWorkRepository.Meals.Edit(meal);
                unitOfWorkRepository.SaveChanges();

                TempData["Success"] = "Your Old Meal Updated Successfully";

                return Redirect($"/SuperAdmin/Resturants/ResturantMeals/{resId}");
            }

            return View(meal);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMeal(int id, int resId)
        {
            var meal = unitOfWorkRepository.Meals.GetOne(expression: m => m.Id == id);

            if (meal != null)
            {
                unitOfWorkRepository.Meals.Delete(meal);
                unitOfWorkRepository.SaveChanges();
            }

            TempData["Success"] = "Choosed meal was delleted";
            return Redirect($"/SuperAdmin/Resturants/ResturantMeals/{resId}");
        }

        #endregion ResturantsMeals

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
