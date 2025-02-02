using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IIdEncryptionService encryptionService;

        public AddressController(IUnitOfWorkRepository unitOfWorkRepository,
                                 UserManager<ApplicationUser> userManager,
                                 IIdEncryptionService encryptionService)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.userManager = userManager;
            this.encryptionService = encryptionService;
        }
        public IActionResult SavedAddresses()
        {
            var userId = userManager.GetUserId(User);
            var addresses = unitOfWorkRepository.Addresses.Get(expression: a => a.ApplicationUserId == userId);
            return View(addresses);
        }

        public IActionResult AddAddress()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult AddAddress(AddressVM addressVM)
        {
            if (ModelState.IsValid)
            {
                var address = AddressVM.MapToAddress(addressVM);
                address.ApplicationUserId = userManager.GetUserId(User);
                unitOfWorkRepository.Addresses.Create(address);
                unitOfWorkRepository.SaveChanges();

                return BackToIndexAddressPage("Your New Address Added Successfully");
            }

            return View();
        }       

        public bool AddUserAddress(string customerAddres, string userId)
        {
            var userAddress = new Address { ApplicationUserId = userId, CustomerAddres = customerAddres };

            unitOfWorkRepository.Addresses.Create(userAddress);
            unitOfWorkRepository.SaveChanges();

            return true;
        }

        public IActionResult UpdateAddress(string id)
        {
            var addressId = encryptionService.DecryptId(id);
            var address = unitOfWorkRepository.Addresses.GetOne(expression:  a => a.Id == addressId);

            if (address == null) return Unauthorized();

            HttpContext.Session.SetString("AddressId", id.ToString());
            return View(AddressVM.MapToAddressVM(address));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult UpdateAddress(AddressVM addressVM)
        {
            var addressId = encryptionService.DecryptId(HttpContext.Session.GetString("AddressId"));
            var oldAddress = unitOfWorkRepository.Addresses.GetOne(expression: a => a.Id == addressId, tracked: false);

            if (oldAddress == null) return Unauthorized();
            if (oldAddress.CustomerAddres == addressVM.CustomerAddres) return RedirectToAction("SavedAddresses");

            if (ModelState.IsValid)
            {
                var address = AddressVM.MapToAddress(addressVM);         
                unitOfWorkRepository.Addresses.Edit(FillAddressData(address));
                unitOfWorkRepository.SaveChanges();

                return BackToIndexAddressPage("Your Old Address Updateded Successfully");                
            }
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult DeleteAddress(string id)
        {
            var addressId = encryptionService.DecryptId(id);
            var address = unitOfWorkRepository.Addresses.GetOne(expression: e => e.Id == addressId);

            if (address == null) return Unauthorized();

            unitOfWorkRepository.Addresses.Delete(address);
            unitOfWorkRepository.SaveChanges();

            return BackToIndexAddressPage("Your Old Address Deleteded Successfully");            
        }

        private IActionResult BackToIndexAddressPage(string msg)
        {
            var userId = userManager.GetUserId(User);
            var addresses = unitOfWorkRepository.Addresses.Get(expression: a => a.ApplicationUserId == userId, tracked: false);

            TempData["Success"] = msg;

            return RedirectToAction("SavedAddresses", addresses);
        }       

        private Address FillAddressData(Address address)
        {
            address.Id = encryptionService.DecryptId(HttpContext.Session.GetString("AddressId"));
            address.ApplicationUserId = userManager.GetUserId(User);
            return address;
        }
    }
}
