using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.ViewModel;
using Otlob.Core.Models;
using Utility;
using RepositoryPatternWithUOW.Core.Models;
using Microsoft.AspNetCore.Identity;
using Otlob.Core.IUnitOfWorkRepository;
using Microsoft.IdentityModel.Tokens;

namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(Roles = SD.superAdminRole)]
    public class RegisterController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IUnitOfWorkRepository unitOfWorkRepository;

        public RegisterController(UserManager<ApplicationUser> userManager,
                                  SignInManager<ApplicationUser> signInManager,
                                  RoleManager<IdentityRole> roleManager,
                                  IUnitOfWorkRepository unitOfWorkRepository)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.unitOfWorkRepository = unitOfWorkRepository;
        }

        public IActionResult RegistResturant()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegistResturant(RegistResturantVM registresturant)
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

        public IActionResult RegistSuperAdmin()
        {            
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistSuperAdmin(ApplicationUserlVM userVM)
        {
            if (ModelState.IsValid)
            {
                var applicatioUser = new ApplicationUser
                {
                    UserName = userVM.UserName,
                    Email = userVM.Email,
                    PhoneNumber = userVM.PhoneNumber
                };

                var result = await userManager.CreateAsync(applicatioUser, userVM.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(applicatioUser, SD.superAdminRole);
                    var userAddress = new Address
                    {
                        CustomerAddres = userVM.Address,
                        ApplicationUserId = applicatioUser.Id,
                    };
                    TempData["Success"] = "Super Admin Account Created Succefully";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Password", "Invalid Password");
                    return View(userVM);
                }
            }
            return View(userVM);
        }

    }
}
