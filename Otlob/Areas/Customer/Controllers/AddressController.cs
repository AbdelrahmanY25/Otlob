namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AddressController : Controller
    {
        private readonly IAddressService addressService;
        private readonly IDataProtector dataProtector;

        public AddressController(IAddressService addressService,
                                 IDataProtectionProvider dataProtectionProvider)
        {
            this.addressService = addressService;
            dataProtector = dataProtectionProvider.CreateProtector("SecureData");
        }
        public IActionResult SavedAddresses()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var addresses = addressService.GetUserAddressies(userId!);

            return View(addresses);
        }

        public IActionResult AddAddress() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult AddAddress(AddressVM addressVM)
        {
            if (!ModelState.IsValid) 
            {
                return View();
            }

            string? userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return RedirectToAction("Login", "Account");
            }

            string theResultOfAddingAddress = addressService.AddAddress(addressVM, userId)!;

            if (theResultOfAddingAddress is string)
            {
                return BackToIndexAddressPage("The address is already exist", true);
            }

            return BackToIndexAddressPage("Your New Address Added Successfully");
        }        
        
        public IActionResult UpdateAddress(string id)
        {
            int addressId = int.Parse(dataProtector.Unprotect(id));

            AddressVM? addressVM = addressService.GetOneAddress(addressId);

            if (addressVM is null)
            {
                return NotFound();
            }

            HttpContext.Session.SetString("AddressId", id);

            return View(addressVM);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult UpdateAddress(AddressVM addressVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            int addressId = int.Parse(dataProtector.Unprotect(HttpContext.Session.GetString("AddressId")!));

            AddressVM oldAddressVM = addressService.GetOneAddress(addressId)!;

            if (oldAddressVM is null || oldAddressVM.CustomerAddress == addressVM.CustomerAddress)
            {
                return RedirectToAction("SavedAddresses");
            }

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
            {
                return RedirectToAction("Login", "Account");
            }

            addressService.UpdateAddress(addressVM, userId, addressId);

            return BackToIndexAddressPage("Your Old Address Updateded Successfully");                    
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult DeleteAddress(string id)
        {
            int addressId = int.Parse(dataProtector.Unprotect(id));

            addressService.DeleteAddress(addressId);

            return BackToIndexAddressPage("Your Old Address Deleteded Successfully");            
        }

        private IActionResult BackToIndexAddressPage(string msg, bool error = false)
        {
            TempData[error ? "Error" : "Success"] = msg;
            return RedirectToAction("SavedAddresses");
        }
    }
}
