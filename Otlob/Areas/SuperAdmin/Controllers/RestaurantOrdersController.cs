namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(Roles = SD.superAdminRole)]
    public class RestaurantOrdersController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IPaginationService paginationService;
        private readonly IUserServices userServices;
        private readonly IEncryptionService encryptionService;
        private readonly IDataProtector dataProtector;

        private const int pageSize = 6;

        public RestaurantOrdersController(IOrderService orderService,
                                          IPaginationService paginationService,
                                          IUserServices userServices,
                                          IEncryptionService encryptionService,
                                          IDataProtectionProvider dataProtectionProvider)
        {
            this.orderService = orderService;
            this.paginationService = paginationService;
            this.userServices = userServices;
            this.encryptionService = encryptionService;
            this.dataProtector = dataProtectionProvider.CreateProtector("SecureData");
        }

        public IActionResult CurrentRestaurantOrders(string id, int currentPageNumber = 1, string? searchById = null)
        {
            return GetOrdersView(id, OrderStatus.Delivered, exclude: true, currentPageNumber, searchById);            
        }

        public IActionResult CompletedRestaurantOrders(string id, int currentPageNumber = 1, string? searchById = null)
        {
            return GetOrdersView(id, OrderStatus.Delivered, exclude: false, currentPageNumber, searchById);            
        }

        private IActionResult GetOrdersView(string resId, OrderStatus status, bool exclude, int currentPageNumber, string? searchById)
        {
            int restaurantId = int.Parse(dataProtector.Unprotect(resId));
           
            if (int.TryParse(searchById, out int id))
            {
                Order order = orderService.GetOrderById(id, restaurantId);

                if (order is null)
                {
                    return View("EmptyOrders");
                }

                return View("Order", order);
            }
            else if (!string.IsNullOrEmpty(searchById))
            {
                return View("EmptyOrders");
            }

            var restaurantOrdersVM = orderService.GetCurrentRestaurantOrders(restaurantId, status, exclude)!;

            if (restaurantOrdersVM.IsNullOrEmpty())
            {
                return View("EmptyOrders");
            }

            decimal? MostExpensiveOrder = restaurantOrdersVM.Max(o => (decimal?)o.TotalOrderPrice);

            PaginationVM<RestaurantOrdersVM> viewModel = paginationService.PaginateItems(restaurantOrdersVM, pageSize, currentPageNumber, MostExpensiveOrder);

            return View("Index", viewModel);
        }

        public IActionResult OrderUserDetailsPartial(string orderId)
        {
            string userId = orderService.GetUserIdByOrderId(orderId);

            if (userId.IsNullOrEmpty())
            {
                return NotFound();
            }

            ApplicationUser? user = userServices.GetUserDataToPartialview(userId);

            if (user is null)
            {
                return NotFound();
            }

            return PartialView("_UserOrder", user);
        }
    }
}
