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
        public IActionResult Index(DateOnly ordersDate)
        {
            // Users data
            var users = unitOfWorkRepository.Users.Get(expression: u => u.Resturant_Id == 0);
            var partners = unitOfWorkRepository.Users.Get(expression: u => u.Resturant_Id != 0);

            ViewBag.Users = users.Count();
            ViewBag.Partners = partners.Count();

            // Users Status
            var activeUsers = unitOfWorkRepository.Orders.Get().GroupBy(o => o.ApplicationUserId);

            var nOfUsers = users.Count();
            var nOfActiveUsers = activeUsers.Count();
            var nOfUnActiveUsers = nOfUsers - nOfActiveUsers;

            ViewBag.ActiveUsers = Math.Round((decimal)nOfActiveUsers / nOfUsers * 100, 2);
            ViewBag.UnActiveUsers = Math.Round((decimal)nOfUnActiveUsers / nOfUsers * 100, 2);

            // Orders data
            var allOrders = unitOfWorkRepository.Orders.Get(expression: o => o.OrderDate.Day == DateTime.Today.Day && o.OrderDate.Month == DateTime.Today.Month && o.OrderDate.Year == DateTime.Today.Year);
            var pendingOrders = unitOfWorkRepository.Orders.Get(expression: o => o.Status == OrderStatus.Pending && o.OrderDate.Day == DateTime.Today.Day && o.OrderDate.Month == DateTime.Today.Month && o.OrderDate.Year == DateTime.Today.Year);
            var preparingOrders = unitOfWorkRepository.Orders.Get(expression: o => o.Status == OrderStatus.Preparing && o.OrderDate.Day == DateTime.Today.Day && o.OrderDate.Month == DateTime.Today.Month && o.OrderDate.Year == DateTime.Today.Year);
            var shippedOrders = unitOfWorkRepository.Orders.Get(expression: o => o.Status == OrderStatus.Shipped && o.OrderDate.Day == DateTime.Today.Day && o.OrderDate.Month == DateTime.Today.Month && o.OrderDate.Year == DateTime.Today.Year);

            var totalOrders = allOrders.Count();
            var pending = pendingOrders.Count();
            var preparing = preparingOrders.Count();
            var shipped = shippedOrders.Count();

            if (totalOrders > 0)
            {
                ViewBag.PendingOrders = Math.Round((decimal)pending / totalOrders * 100, 2);
                ViewBag.PreparingOrders = Math.Round((decimal)preparing / totalOrders * 100, 2);
                ViewBag.ShippedOrders = Math.Round((decimal)shipped / totalOrders * 100, 2);
            }
            else
            {
                ViewBag.PendingOrders = 0;
                ViewBag.PreparingOrders = 0;
                ViewBag.ShippedOrders = 0;
            }

            // Sales data
            var allDeliverdOrders = unitOfWorkRepository.Orders.Get(expression: o => ordersDate == null || ordersDate == DateOnly.MinValue ?
                                                                                    o.Status == OrderStatus.Delivered && o.OrderDate.Day == DateTime.Today.Day && o.OrderDate.Month == DateTime.Today.Month && o.OrderDate.Year == DateTime.Today.Year :
                                                                                    o.Status == OrderStatus.Delivered && o.OrderDate.Day == ordersDate.Day && o.OrderDate.Month == ordersDate.Month && o.OrderDate.Year == ordersDate.Year);

                                                                               
            if (allDeliverdOrders != null)
            {
                var totalDeliverdOrders = allDeliverdOrders.Count();

                if (totalDeliverdOrders > 0)
                {
                    var Sales = allDeliverdOrders.Sum(o => o.OrderPrice);

                    ViewBag.TotalOrders = totalDeliverdOrders;
                    ViewBag.Sales = Sales;
                    ViewBag.NetProfit = Math.Round(Sales * (decimal)0.05, 2);
                }
                else
                {
                    ViewBag.TotalOrders = 0;
                    ViewBag.Sales = 0;
                    ViewBag.NetProfit = 0;
                }
            }

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
