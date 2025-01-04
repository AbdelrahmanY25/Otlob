using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;

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
        public IActionResult AddAddress(Address address)
        {
            if (ModelState.IsValid)
            {
                unitOfWorkRepository.Addresses.Create(address);
                unitOfWorkRepository.SaveChanges();

                var user = userManager.GetUserId(User);
                var addresses = unitOfWorkRepository.Addresses.Get([a => a.User], expression: a => a.ApplicationUserId == user);

                TempData["Success"] = "Your New Address Added Successfully";

                return RedirectToAction("SavedAddresses", addresses);
            }

            return View();
        }
         public IActionResult UpdateAddress(int id)
         {
            var address = unitOfWorkRepository.Addresses.GetOne(expression:  a => a.Id == id);
            return View(address);
         }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult UpdateAddress(Address address)
        {
            if (ModelState.IsValid)
            {
                var user = userManager.GetUserId(User);
                var addresses = unitOfWorkRepository.Addresses.Get([a => a.User], expression: a => a.ApplicationUserId == user, tracked: false);

                unitOfWorkRepository.Addresses.Edit(address);
                unitOfWorkRepository.SaveChanges();

                TempData["Success"] = "Your Old Address Updateded Successfully";

                return RedirectToAction("SavedAddresses", addresses);
            }
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult DeleteAddress(int id)
        {
            var user = userManager.GetUserId(User);
            var addresses = unitOfWorkRepository.Addresses.Get([a => a.User], expression: a => a.ApplicationUserId == user, tracked: false);
            var address = unitOfWorkRepository.Addresses.GetOne(expression: e => e.Id == id);

            if (address != null)
            {
                unitOfWorkRepository.Addresses.Delete(address);
                unitOfWorkRepository.SaveChanges();
            }

            TempData["Success"] = "Your Old Address Deleteded Successfully";

            return RedirectToAction("SavedAddresses", addresses);
        }
    }
}
