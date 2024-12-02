using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Utility;

namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin")]
    [Authorize(Roles = SD.superAdminRole)]
    public class HomeController : Controller
    {        
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        public HomeController(IUnitOfWorkRepository unitOfWorkRepository)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult ResturatnRequist()
        {
            var resturants = unitOfWorkRepository.Restaurants.Get(expression: r => r.Accecpted == false);
            return View(resturants);
        }
        public IActionResult ActiveResturatns()
        {
            var resturants = unitOfWorkRepository.Restaurants.Get(expression: r => r.Accecpted == true);
            return View(resturants);
        }
    }
}
