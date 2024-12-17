using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using RepositoryPatternWithUOW.Core.Models;
using Utility;

namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(Roles = SD.superAdminRole)]
    public class HomeController : Controller
    {        
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        public HomeController(IUnitOfWorkRepository unitOfWorkRepository)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
        }
        public IActionResult Index()
        {
            var users = unitOfWorkRepository.Users.Get(expression: u => u.Resturant_Id == 0);

            var customers = unitOfWorkRepository.Users.Get(expression: u => u.Resturant_Id != 0);

            ViewBag.Users = users.Count();
            ViewBag.Customers = customers.Count();

            return View();
        }
        
        public IActionResult ResturatnRequist()
        {
            var resturants = unitOfWorkRepository.Restaurants.Get(expression: r => r.AcctiveStatus == AcctiveStatus.Unaccepted);
            return View(resturants);
        }
        public IActionResult ActiveResturatns()
        {
            var resturants = unitOfWorkRepository.Restaurants.Get(expression: r => r.AcctiveStatus != AcctiveStatus.Unaccepted);
            return View(resturants);
        }
    }
}
