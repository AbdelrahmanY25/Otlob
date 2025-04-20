using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Otlob.Core.ViewModel;
using Otlob.IServices;
using Utility;

namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(Roles = SD.superAdminRole)]
    public class RestaurantOrdersController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IPaginationService paginationService;

        private const int pageSize = 6;

        public RestaurantOrdersController(IOrderService orderService, IPaginationService paginationService)
        {
            this.orderService = orderService;
            this.paginationService = paginationService;
        }

        public IActionResult RestaurantOrders(string id, int currentPageNumber = 1)
        {
            var orders = orderService.GetRestaurantOrdersByRestaurantIdToView(id);

            if (orders.IsNullOrEmpty())
            {
                return View("EmptyOrders");
            }

            PaginationVM<RestaurantOrdersVM> paginatedOrders = paginationService.PaginateItems(orders, pageSize, currentPageNumber);

            return View(paginatedOrders);
        }
    }
}
