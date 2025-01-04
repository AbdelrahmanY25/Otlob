using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;
using Otlob.EF.UnitOfWorkRepository;
using RepositoryPatternWithUOW.Core.Models;
using Utility;

namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class BecomeaPartnerController : Controller
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public BecomeaPartnerController(IUnitOfWorkRepository unitOfWorkRepository, UserManager<ApplicationUser> userManager)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.userManager = userManager;
        }
        public IActionResult BecomeaPartner()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> BecomeaPartner(RegistResturantVM registresturant)
        {
            if (ModelState.IsValid)
            {
                var applicationUser = new ApplicationUser
                {
                    UserName = registresturant.ResUserName,
                    Email = registresturant.ResEmail,
                };

                var result = await userManager.CreateAsync(applicationUser, registresturant.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(applicationUser, SD.restaurantAdmin);

                    var resturant = new Restaurant
                    {
                        Name = registresturant.ResName,
                        Email = registresturant.ResEmail,
                        Address = registresturant.ResAddress.ToString(),
                        Phone = registresturant.ResPhone,
                        Description = registresturant.Description
                    };


                    unitOfWorkRepository.Restaurants.Create(resturant);
                    unitOfWorkRepository.SaveChanges();

                    var theresturant = unitOfWorkRepository.Restaurants.GetOne(expression: r => r.Name == registresturant.ResName, tracked: false);

                    if (theresturant != null)
                    {
                        applicationUser.Resturant_Id = theresturant.Id;
                        await userManager.UpdateAsync(applicationUser);
                    }

                    TempData["Success"] = "Resturant Account Created Succefully";

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(registresturant);
                }
            }
            return View(registresturant);
        }
    }
}
