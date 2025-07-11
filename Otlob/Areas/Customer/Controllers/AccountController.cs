using Otlob.Core.ViewModel;

namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IImageService imageService;
        private readonly IUserServices userServices;
        private readonly IAddressService addressService;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 RoleManager<IdentityRole> roleManager,
                                 IImageService imageService,
                                 IUserServices userServices,
                                 IAddressService addressService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.imageService = imageService;
            this.userServices = userServices;
            this.addressService = addressService;
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
            if (!ModelState.IsValid)
            {
                return View(userVM);
            }

            var applicatioUser = new ApplicationUser { UserName = userVM.UserName, Email = userVM.Email, PhoneNumber = userVM.PhoneNumber };

            var result = await userManager.CreateAsync(applicatioUser, userVM.Password);                
                
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(applicatioUser, SD.customer);

                var userAddress = addressService.AddUserAddress(userVM.Address, applicatioUser.Id);
                  
                if (!userAddress)
                {
                    return View(userVM);
                }

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

        public IActionResult Login()
        {
            signInManager.SignOutAsync();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            var user = await userManager.FindByEmailAsync(loginVM.Email);

            var finalResult = await userManager.CheckPasswordAsync(user!, loginVM.Password);

            if (finalResult)
            {              
                await userManager.ResetAccessFailedCountAsync(user!);
                var claims = AddUserClaims(user!);
                await signInManager.SignInWithClaimsAsync(user!, loginVM.RememberMe, claims);
                return await CheckOnUserRoll(user!);
            }
            else
            {                
                await userManager.AccessFailedAsync(user!);             
                ModelState.AddModelError("", "There is invalid user name or password");
                return View(loginVM);
            }                
        }

        private async Task<IActionResult> CheckOnUserRoll(ApplicationUser user)
        {
            if (await userManager.IsInRoleAsync(user, SD.superAdminRole))
            {
                return RedirectToAction("Index", "Home", new { Area = "SuperAdmin" });
            }
            else if (await userManager.IsInRoleAsync(user, SD.restaurantAdmin))
            {
                return RedirectToAction("Index", "Home", new { Area = "ResturantAdmin" });
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Area = "Customer" });
            }
        }

        private List<Claim> AddUserClaims(ApplicationUser user)
        {
            List<Claim> claims = new();
            claims.Add(new Claim(SD.restaurantId, user.RestaurantId.ToString()));
            if (user.Image is not null)
                claims.Add(new Claim(SD.userImageProfile, user.Image!));

            return claims;
        }

        public IActionResult UserProfile()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            ProfileVM userProfileDetails = userServices.GetUserProfileVmDetails(userId);

            if (userProfileDetails is null)
            {
                return RedirectToAction("Login");
            }

            return View(userProfileDetails);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UserProfile(ProfileVM profileVM)
        {
            if (!ModelState.IsValid)
            {
                return View(profileVM);
            }

            ApplicationUser? user = await userManager.GetUserAsync(User);

            if (user is null)
            {
                return RedirectToAction("Login");
            }

            user = userServices.UpdateUserInfo(user, profileVM);

            var updateUserInfo = await userManager.UpdateAsync(user);

            if (updateUserInfo.Succeeded)
            {
                TempData["Success"] = "Your profile info updated successfully.";
                return RedirectToAction("UserProfile");
            }
            
            foreach (var errorInfo in updateUserInfo.Errors)
            {
                ModelState.AddModelError("", errorInfo.Description);
            }

            return View(profileVM);                       
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UserProfilePicture(IFormFile image)
        {
            ApplicationUser? user = await userManager.GetUserAsync(User);

            if (user is null)
            {
                return RedirectToAction("Login");
            }

            var isImageUploaded = imageService.UploadImage(image!);

            if (!isImageUploaded.IsSuccess)
            {                
                TempData["Error"] = isImageUploaded.Message!;
                return RedirectToAction("UserProfile");
            }

            var isOldImageDeleted = imageService.DeleteOldImageIfExist(user.Image!);

            if (!isOldImageDeleted.IsSuccess)
            {
                TempData["Error"] = isOldImageDeleted.Message!;
                return RedirectToAction("UserProfile");
            }

            userServices.UpdateUserImage(user!, isImageUploaded.ImageUrl);

            TempData["Success"] = "Your profile picture updated successfully.";
            return RedirectToAction("UserProfile");        
        }

        public IActionResult ChangePassword() => View();


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM passwordVM)
        {
            var user = await userManager.GetUserAsync(User);

            if (user is null)
            {
                ModelState.AddModelError("", "There is no user");
                RedirectToAction("Index", "Home");
            }

            var result = await userManager.CheckPasswordAsync(user!, passwordVM.OldPassword);

            if (!result || passwordVM.OldPassword == passwordVM.NewPassword)
            {
                ModelState.AddModelError("", "Wrond Password Entered");
                return View();
            }

            var finalRes = await userManager.ChangePasswordAsync(user!, passwordVM.OldPassword, passwordVM.NewPassword);

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
