using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;
using Otlob.IServices;
using Utility;

namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(Roles = SD.superAdminRole)]
    public class OrdersStatusController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IPaginationService paginationService;

        private const int pageSize = 5;

        public OrdersStatusController(IOrderService orderService, IPaginationService paginationService)
        {
            this.orderService = orderService;
            this.paginationService = paginationService;
        }

        public IActionResult Index(OrderStatus status, int currentPageNumber = 1)
        {
            var orders = orderService.GetOrdersDayByStatus(status);

            if (orders.IsNullOrEmpty())
            {
                return View("EmptyOrders");
            }

            return Orders(orders, currentPageNumber);
        }

        public IActionResult Orders(IQueryable<Order> orders, int currentPageNumber)
        {                       
            PaginationVM<Order> viewModel = paginationService.PaginateItems(orders, pageSize, currentPageNumber);

            return View(viewModel);
        }
    }
}
