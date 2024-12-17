using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;
using RepositoryPatternWithUOW.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Utility;

namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IUnitOfWorkRepository unitOfWorkRepository;

        public AccountController(UserManager<ApplicationUser> userManager,
                                  SignInManager<ApplicationUser> signInManager,
                                  RoleManager<IdentityRole> roleManager,
                                  IUnitOfWorkRepository unitOfWorkRepository)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<IActionResult> Register()
        {
            if (roleManager.Roles.IsNullOrEmpty())
            {
                await roleManager.CreateAsync(new(SD.superAdminRole));
                await roleManager.CreateAsync(new(SD.restaurantAdmin));
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
                    PhoneNumber = userVM.PhoneNumber
                };

                var result = await userManager.CreateAsync(applicatioUser, userVM.Password);                
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(applicatioUser, SD.customer);
                    var userAddress = new Address
                    {
                        CustomerAddres = userVM.Address,
                        ApplicationUserId = applicatioUser.Id,
                    };

                    unitOfWorkRepository.Addresses.Create(userAddress);
                    unitOfWorkRepository.SaveChanges();

                    await signInManager.SignInAsync(applicatioUser, isPersistent: false);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(userVM);
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

                        if (await userManager.IsInRoleAsync(userDb, SD.superAdminRole))
                        {
                            return RedirectToAction("Index", "Home", new { area = "SuperAdmin" });
                        }
                        else if (await userManager.IsInRoleAsync(userDb, SD.restaurantAdmin))
                        {
                            return RedirectToAction("Index", "Home", new { area = "ResturantAdmin" });
                        }
                        else if (await userManager.IsInRoleAsync(userDb, SD.customer))
                        {
                            return RedirectToAction("Index", "Home", new { area = "Customer" });
                        }
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
            var user = await userManager.FindByNameAsync(User.Identity.Name);

            if (user != null)
            {
                var userProfile = new ProfileVM
                {
                    Email = user.Email,
                    BirthDate = user.BirthDate,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
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
                                user.ProfilePicture = memoryStream.ToArray();
                            }

                            var updateResult = await userManager.UpdateAsync(user);

                            if (updateResult.Succeeded)
                            {
                                TempData["Success"] = "Profile Picture updated successfully.";
                                return RedirectToAction("Profile");
                            }

                            foreach (var error in updateResult.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }

                    if (user.Email != profileVM.Email || user.FirstName != profileVM.FirstName || user.LastName != profileVM.LastName || user.PhoneNumber != profileVM.PhoneNumber || user.Gender != profileVM.Gender || user.BirthDate != profileVM.BirthDate)
                    {
                        user.FirstName = profileVM.FirstName;
                        user.LastName = profileVM.LastName;
                        user.BirthDate = profileVM.BirthDate;
                        user.Gender = profileVM.Gender;
                        user.PhoneNumber = profileVM.PhoneNumber;
                        user.Email = profileVM.Email;
                        var result = await userManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            TempData["Success"] = "Profile updated successfully.";
                            return RedirectToAction("Profile");
                        }
                    }
                }                
            }

            return View(profileVM);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM passwordVM)
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                ModelState.AddModelError("", "User is no user");
                RedirectToAction("Index", "Home");
            }

            var result = await userManager.CheckPasswordAsync(user, passwordVM.OldPassword);

            if (!result || passwordVM.OldPassword == passwordVM.NewPassword)
            {
                ModelState.AddModelError("", "Wrond Password Entered");
                return View();
            }

            var finalRes = await userManager.ChangePasswordAsync(user, passwordVM.OldPassword, passwordVM.NewPassword);

            if (finalRes.Succeeded)
            {
                TempData["Success"] = "Your Password updated successfully.";
                return RedirectToAction("Profile");
            }

            return View();
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
       
        public IActionResult Logout()
        {
            signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
