using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using RepositoryPatternWithUOW.Core.Models;
using Utility;

namespace Otlob.Areas.Restaurants.Controllers
{
    [Area("ResturantAdmin"),Authorize(Roles = SD.restaurantAdmin)]
    public class RestaurantController : Controller
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public RestaurantController(IUnitOfWorkRepository unitOfWorkRepository,
                                    UserManager<ApplicationUser> userManager)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
            this.userManager = userManager;
        }
        
        public async Task<IActionResult> Edit()
        {
            var user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                var resturant = _unitOfWorkRepository.Restaurants.GetOne(expression: r => r.Id == user.Resturant_Id, tracked: false);                
                return View(resturant);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(Restaurant restaurant, IFormFile Logo)
        {

            var oldResturantInfo = _unitOfWorkRepository.Restaurants.GetOne(expression: r => r.Id == restaurant.Id, tracked: false);
            
            if (restaurant.Name != oldResturantInfo?.Name || restaurant.Email != oldResturantInfo.Email || restaurant.Address != oldResturantInfo.Address)
            {
                ModelState.AddModelError("", "You can't change your resturaant [ Name or Email or Address ]");
                return View(restaurant);
            }

            if (ModelState.IsValid)
            {
                if (Logo != null)
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

                    if (oldResturantInfo.Logo != null)
                    {
                        var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\resturantLogo", oldResturantInfo.Logo);

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
                    restaurant.Logo = oldResturantInfo.Logo;
                }

                restaurant.AcctiveStatus = oldResturantInfo.AcctiveStatus;
                 _unitOfWorkRepository.Restaurants.Edit(restaurant);
                _unitOfWorkRepository.SaveChanges();

                TempData["Success"] = "Your resturant info updated Successfully";

                return RedirectToAction("Index", "Home");
            }

            restaurant.Logo = oldResturantInfo.Logo;

            return View(restaurant);
        }
    }
}