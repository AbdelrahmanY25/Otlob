using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IUnitOfWorkRepository;
using RepositoryPatternWithUOW.Core.Models;

namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin")]
    public class RestaurantStatusController : Controller
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;

        public RestaurantStatusController(IUnitOfWorkRepository unitOfWorkRepository)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
        }
        public IActionResult AcceptResturant(int id)
        {
            var res = unitOfWorkRepository.Restaurants.GetOne(expression: r => r.Id == id);
            if (res != null)
            {
                res.AcctiveStatus = AcctiveStatus.Acctive;
                unitOfWorkRepository.Restaurants.Edit(res);
                unitOfWorkRepository.SaveChanges();
            }

            var resturants = unitOfWorkRepository.Restaurants.Get(expression: r => r.AcctiveStatus != AcctiveStatus.Unaccepted);
            TempData["Success"] = "The Resturant Status is Active";
            return RedirectToAction("ActiveResturatns", "Home", resturants);
        }

        public IActionResult WarnResturant(int id)
        {
            var res = unitOfWorkRepository.Restaurants.GetOne(expression: r => r.Id == id);
            if (res != null)
            {
                res.AcctiveStatus = AcctiveStatus.Warning;
                unitOfWorkRepository.Restaurants.Edit(res);
                unitOfWorkRepository.SaveChanges();
            }

            TempData["Success"] = "The Warning Was Sent to Resturant";
            return RedirectToAction("ActiveResturatns", "Home");
        }

        public IActionResult BlockResturant(int id)
        {
            var res = unitOfWorkRepository.Restaurants.GetOne(expression: r => r.Id == id);
            if (res != null)
            {
                res.AcctiveStatus = AcctiveStatus.Block;
                unitOfWorkRepository.Restaurants.Edit(res);
                unitOfWorkRepository.SaveChanges();
            }

            TempData["Success"] = "The Resturant Account Was Blocked";
            return RedirectToAction("ActiveResturatns", "Home");
        }
    }
}
