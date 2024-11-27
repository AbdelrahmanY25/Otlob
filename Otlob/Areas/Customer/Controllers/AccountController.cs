using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;
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
                                user.ProfilePicture = memoryStream.ToArray(); // Assuming ProfilePicture is a byte array
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

            if (!result)
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
