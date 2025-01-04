using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Otlob.Models;
using RepositoryPatternWithUOW.Core.Models;

namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public HomeController(ILogger<HomeController> logger,
                              IUnitOfWorkRepository unitOfWorkRepository,
                              UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.userManager = userManager;
        }

        public IActionResult Index(string? filter = null)
        {
            var resturants = unitOfWorkRepository.Restaurants.Get(expression: r => filter.IsNullOrEmpty() || filter == "all" ?
                                                                              r.AcctiveStatus == AcctiveStatus.Acctive ||
                                                                              r.AcctiveStatus == AcctiveStatus.Warning :
                                                                              r.AcctiveStatus == AcctiveStatus.Acctive &&
                                                                              r.Description.Contains(filter) ||
                                                                              r.AcctiveStatus == AcctiveStatus.Warning &&
                                                                              r.Description.Contains(filter));

            return View(resturants);
        }

        public IActionResult Details(int id, string? filter = null)
        {
            var meals = unitOfWorkRepository.Meals.Get(expression: m => m.RestaurantId == id && m.IsAvailable);

            if (!string.IsNullOrEmpty(filter) && filter.ToLower() != "all")
            {
                meals = filter.ToLower() switch
                {
                    "new" => meals.Where(m => m.IsNewMeal),
                    "trend" => meals.Where(m => m.IsTrendingMeal),
                    "main" => meals.Where(m => m.Category == MealCategory.MainCourse),
                    "grilled" => meals.Where(m => m.Category == MealCategory.Grill),
                    "desserts" => meals.Where(m => m.Category == MealCategory.Dessert),
                    "bakeries" => meals.Where(m => m.Description.Contains("Bakeries")),
                    "drink" => meals.Where(m => m.Description.Contains("Drink")),
                    _ => meals
                };
            }

            ViewBag.ResId = id;

            return View(meals);
        }
      
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
