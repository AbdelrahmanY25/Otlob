using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.IServices;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;
using Utility;
using Otlob.Core.Services;

namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly IImageService imageService;
        private readonly IUserServices userServices;
        private readonly IIdEncryptionService encryptionService;

        public AccountController(UserManager<ApplicationUser> userManager,
                                  SignInManager<ApplicationUser> signInManager,
                                  RoleManager<IdentityRole> roleManager,
                                  IUnitOfWorkRepository unitOfWorkRepository,
                                  IImageService imageService,
                                  IUserServices userServices,
                                  IIdEncryptionService encryptionService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.imageService = imageService;
            this.userServices = userServices;
            this.encryptionService = encryptionService;
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
                var applicatioUser = new ApplicationUser { UserName = userVM.UserName, Email = userVM.Email, PhoneNumber = userVM.PhoneNumber };

                var result = await userManager.CreateAsync(applicatioUser, userVM.Password);                
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(applicatioUser, SD.customer);

                    var userAddress = new AddressController(unitOfWorkRepository, userManager, encryptionService);                    
                    userAddress.AddUserAddress(userVM.Address, applicatioUser.Id);
                   
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

                        return await CheckOnUserRoll(userDb);
                    }
                    else
                        ModelState.AddModelError("", "There is invalid user name or password");
                }
                else
                    ModelState.AddModelError("", "There is invalid user name or password");
            }

            return View(loginVM);
        }

        private async Task<IActionResult> CheckOnUserRoll(ApplicationUser user)
        {
            if (await userManager.IsInRoleAsync(user, SD.superAdminRole))
            {
                return RedirectToAction("Index", "Home", new { area = "SuperAdmin" });
            }
            else if (await userManager.IsInRoleAsync(user, SD.restaurantAdmin))
            {
                return RedirectToAction("Index", "Home", new { area = "ResturantAdmin" });
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }
        }

        public async Task<IActionResult> Profile()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);

            if (user != null)
            {
                var userProfile = ProfileVM.MapToProfileVM(user);
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
                    user = userServices.ValidateUserInfo(user, profileVM);

                    var updateUserInfo = await userManager.UpdateAsync(user);

                    if (!imageService.UploadUserProfilePicture(Request.Form.Files)) return RedirectToAction("Profile");

                    var image = Request.Form.Files.FirstOrDefault();
                    var resOfValidation = imageService.ValidateImageSizeAndExtension(image);

                    if (resOfValidation is string error)
                    {
                        ModelState.AddModelError("", error);
                        profileVM.ProfilePicture = user.ProfilePicture;
                        return View(profileVM);
                    }

                    if (!await imageService.CopyImageToMemoryStream(image, user))
                    {
                        ModelState.AddModelError("", "Error in uploading image");
                        return View(profileVM);
                    }

                    updateUserInfo = await userManager.UpdateAsync(user);

                    if (updateUserInfo.Succeeded)
                    {
                        TempData["Success"] = "Your profile info updated successfully.";
                        return RedirectToAction("Profile");                        
                    }
                    
                    foreach (var errorInfo in updateUserInfo.Errors)
                    {
                        ModelState.AddModelError("", errorInfo.Description);
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
       
        public IActionResult Logout()
        {
            signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
