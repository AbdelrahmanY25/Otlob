using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Areas.Customer.Services.Interfaces;
using Otlob.Core.IServices;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;
using System.Text;

namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AddressController : Controller
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IAddressService addressService;
        private readonly IEncryptionService encryptionService;

        public AddressController(IUnitOfWorkRepository unitOfWorkRepository,
                                 UserManager<ApplicationUser> userManager,
                                 IAddressService addressService,
                                 IEncryptionService encryptionService)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.userManager = userManager;
            this.addressService = addressService;
            this.encryptionService = encryptionService;
        }
        public IActionResult SavedAddresses()
        {
            var userId = userManager.GetUserId(User);

            var addresses = unitOfWorkRepository
                            .Addresses
                            .GetAllWithSelect(selector: add => new AddressVM { AddressVMId = add.Id, CustomerAddres = add.CustomerAddres },
                                             expression: add => add.ApplicationUserId == userId,
                                             tracked: false);
            return View(addresses);
        }

        public IActionResult AddAddress() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult AddAddress(AddressVM addressVM)
        {
            if (!ModelState.IsValid) return View();

            var userId = userManager.GetUserId(User);

            if (userId == null) return RedirectToAction("Login", "Account");        

            if (addressService.CheckOnAddressIfExist(userId, addressVM))
                return BackToIndexAddressPage("The address is already exist", true);

            var address = addressVM.MapToAddress(userId);

            unitOfWorkRepository.Addresses.Create(address);
            unitOfWorkRepository.SaveChanges();

            return BackToIndexAddressPage("Your New Address Added Successfully");
        }        
        
        public IActionResult UpdateAddress(string id)
        {
            var addressId = encryptionService.DecryptId(id);

            var addressVM = unitOfWorkRepository
                            .Addresses
                            .GetOneWithSelect(selector: add => new AddressVM { CustomerAddres = add.CustomerAddres },
                                              expression:  a => a.Id == addressId,
                                              tracked: false);

            if (addressVM == null) return NotFound();

            HttpContext.Session.SetString("AddressId", id.ToString());

            return View(addressVM);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult UpdateAddress(AddressVM addressVM)
        {
            var addressId = encryptionService.DecryptId(HttpContext.Session.GetString("AddressId"));

            var oldAddress = unitOfWorkRepository.Addresses.GetOne(expression: a => a.Id == addressId, tracked: false);

            if (oldAddress is null)
            {
                return NotFound();
            }

            if (oldAddress.CustomerAddres == addressVM.CustomerAddres)
            {
                return RedirectToAction("SavedAddresses");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var userId = userManager.GetUserId(User);

            if (userId is null)
            {
                return RedirectToAction("Login", "Account");
            }

            var address = addressVM.MapToAddress(userId, addressId);

            unitOfWorkRepository.Addresses.Edit(address);
            unitOfWorkRepository.SaveChanges();

            return BackToIndexAddressPage("Your Old Address Updateded Successfully");                            
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult DeleteAddress(string id)
        {
            var addressId = encryptionService.DecryptId(id);

            var address = new Address { Id = addressId };

            unitOfWorkRepository.Addresses.Delete(address);
            unitOfWorkRepository.SaveChanges();

            return BackToIndexAddressPage("Your Old Address Deleteded Successfully");            
        }

        private IActionResult BackToIndexAddressPage(string msg, bool error = false) /// Sperat in INotifyTostarService
        {
            TempData[error ? "Error" : "Success"] = msg;
            return RedirectToAction("SavedAddresses");
        }
    }
}
