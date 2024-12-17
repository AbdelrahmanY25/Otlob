using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;

namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin")]
    public class UsersController : Controller
    {        
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Customers()
        {
            return View();
        }
        public IActionResult Partners()
        {
            return View();
        }
    }
}
