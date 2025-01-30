using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;

namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AddressController : Controller
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public AddressController(IUnitOfWorkRepository unitOfWorkRepository,
                                 UserManager<ApplicationUser> userManager)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.userManager = userManager;
        }
        public IActionResult SavedAddresses()
        {
            var user = userManager.GetUserId(User);
            var addresses = unitOfWorkRepository.Addresses.Get([a => a.User], expression: a => a.ApplicationUserId == user);
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

        public IActionResult UpdateAddress(int id)
         {
            var address = unitOfWorkRepository.Addresses.GetOne(expression:  a => a.Id == id);
            var addressVM = AddressVM.MapToAddressVM(address);
            return View(addressVM);
         }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult UpdateAddress(AddressVM addressVM)
        {
            if (ModelState.IsValid)
            {
                var address = AddressVM.MapToAddress(addressVM);
                address.Id = addressVM.Id;
                unitOfWorkRepository.Addresses.Edit(address);
                unitOfWorkRepository.SaveChanges();

                return BackToIndexAddressPage("Your Old Address Updateded Successfully");                
            }
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult DeleteAddress(int id)
        {
            var address = unitOfWorkRepository.Addresses.GetOne(expression: e => e.Id == id);

            if (address != null)
            {
                unitOfWorkRepository.Addresses.Delete(address);
                unitOfWorkRepository.SaveChanges();
            }

            return BackToIndexAddressPage("Your Old Address Deleteded Successfully");            
        }

        private IActionResult BackToIndexAddressPage(string msg)
        {
            var user = userManager.GetUserId(User);
            var addresses = unitOfWorkRepository.Addresses.Get([a => a.User], expression: a => a.ApplicationUserId == user, tracked: false);

            TempData["Success"] = msg;

            return RedirectToAction("SavedAddresses", addresses);
        }
    }
}
