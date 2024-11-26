using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RepositoryPatternWithUOW.Core.Models;
using RepositoryPatternWithUOW.Core.ViewModel;
using System.ComponentModel.DataAnnotations;
using Utility;

namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController(UserManager<ApplicationUser> userManager,
                                  SignInManager<ApplicationUser> signInManager,
                                  RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }
        public async Task<IActionResult> Register()
        {
            if (roleManager.Roles.IsNullOrEmpty())
            {
                await roleManager.CreateAsync(new(SD.superAdminRole));
                await roleManager.CreateAsync(new(SD.resturanrAdmin));
                await roleManager.CreateAsync(new(SD.customer));
            }
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(ApplicationUserlVM userVM)
        {
            if (ModelState.IsValid)
            {
                var applicatioUser = new ApplicationUser
                {
                    UserName = userVM.UserName,
                    Email = userVM.Email,
                    Address = userVM.Address,
                    PhoneNumber = userVM.PhoneNumber
                };

                var result = await userManager.CreateAsync(applicatioUser, userVM.Password);
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(applicatioUser, SD.customer);
                    await signInManager.SignInAsync(applicatioUser, isPersistent: false);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Password", "Invalid Password");
                }
            }
            return View(userVM);
        }

        public IActionResult Login()
        {
            signInManager.SignOutAsync();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                    var userDb = await userManager.FindByEmailAsync(loginVM.UserName);
                    if (userDb != null)
                    {
                        var finalResult = await userManager.CheckPasswordAsync(userDb, loginVM.Password);

                        if (finalResult)
                        {
                            await signInManager.SignInAsync(userDb, loginVM.RememberMe);
                            return RedirectToAction("Index", "Home");
                        }
                        else
                            ModelState.AddModelError("", "There is invalid user name or password");
                    }
                    else
                        ModelState.AddModelError("", "There is invalid user name or password");
            }

            return View(loginVM);
        }
        public async Task<IActionResult> Profile()
        {
            var user = await userManager.GetUserAsync(User);

            if (user != null)
            {
                var userProfile = new ProfileVM
                {
                    BirthDate = user.BirthDate,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Gender = user.Gender,
                    PhoneNumber = user.PhoneNumber,
                    ProfilePicture = user.ProfilePicture
                };
                return View(userProfile);
            }
            return View(user);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileVM profileVM)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user != null)
                {
                    user.FirstName = profileVM.FirstName;
                    user.LastName = profileVM.LastName;
                    user.BirthDate = profileVM.BirthDate;
                    user.Gender = profileVM.Gender;
                    user.PhoneNumber = profileVM.PhoneNumber;
                
                    var result = await userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        TempData["Success"] = "Profile updated successfully.";
                        return RedirectToAction("Profile");
                    }
                }                
            }

            return View(profileVM);
        }
        public IActionResult SavedAddresses()
        {
            return View();
        }
        public IActionResult Orders()
        {
            return View();
        }

        public IActionResult Logout()
        {
            signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
