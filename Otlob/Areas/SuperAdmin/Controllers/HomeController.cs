namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(Roles = SD.superAdminRole)]
    public class HomeController : Controller
    {        
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly IRestaurantService restaurantService;
        private readonly IOrderService orderService;

        public HomeController(IUnitOfWorkRepository unitOfWorkRepository, IRestaurantService restaurantService, IOrderService orderService)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.restaurantService = restaurantService;
            this.orderService = orderService;
        }

        public IActionResult Index(DateTime ordersDate)
        {
            // Users data
            var users = unitOfWorkRepository.Users.Get(expression: u => u.RestaurantId == 0);
            var partners = unitOfWorkRepository.Users.Get(expression: u => u.RestaurantId != 0);

            ViewBag.Users = users.Count();
            ViewBag.Partners = partners.Count();

            // Users Status
           // var activeUsers = unitOfWorkRepository.Orders.Get([o => o.Address]).GroupBy(o => o.Address.ApplicationUserId);

            var nOfUsers = users.Count();
            //var nOfActiveUsers = activeUsers.Count();
           // var nOfUnActiveUsers = nOfUsers - nOfActiveUsers;

           // ViewBag.ActiveUsers = Math.Round((decimal)nOfActiveUsers / nOfUsers * 100, 2);
            //ViewBag.UnActiveUsers = Math.Round((decimal)nOfUnActiveUsers / nOfUsers * 100, 2);

            // Orders data
            var allOrders = orderService.GetOrdersCountByDate(DateTime.Today);
            var pendingOrders = orderService.GetOrdersDayByStatus(OrderStatus.Pending);
            var preparingOrders = orderService.GetOrdersDayByStatus(OrderStatus.Preparing);
            var shippedOrders = orderService.GetOrdersDayByStatus(OrderStatus.Shipped);
            var deliveredOrders = orderService.GetOrdersDayByStatus(OrderStatus.Delivered);

            var pending = pendingOrders.Count();
            var preparing = preparingOrders.Count();
            var shipped = shippedOrders.Count();
            var delivered = deliveredOrders.Count();

            if (allOrders > 0)
            {
                decimal pendOrders = Math.Round((decimal)pending / allOrders * 100, 2);
                decimal preparOrders = Math.Round((decimal)preparing / allOrders * 100, 2);
                decimal shippOrders = Math.Round((decimal)shipped / allOrders * 100, 2);
                decimal delevOrders = Math.Round((decimal)delivered / allOrders * 100, 2);

                ViewBag.OrderStatusPercentages = new Dictionary<string, decimal>
                {
                    { "Pending", pendOrders },
                    { "Preparing", preparOrders },
                    { "Shipped", shippOrders },
                    { "Delivered", delevOrders }
                };

            }
            else
            {
                ViewBag.OrderStatusPercentages = new Dictionary<string, decimal>
                {
                    { "Pending", 0 },
                    { "Preparing", 0 },
                    { "Shipped", 0 },
                    { "Delivered", 0 }
                };
            }

            // Sales data
            var allDeliverdOrders = unitOfWorkRepository.Orders.Get(expression: o => ordersDate == null || ordersDate == DateTime.MinValue ?
                                                                                    o.Status == OrderStatus.Delivered && o.OrderDate.Date == DateTime.Today.Date :
                                                                                    o.Status == OrderStatus.Delivered && o.OrderDate.Date == ordersDate.Date);

                                                                               
            if (allDeliverdOrders != null)
            {
                var totalDeliverdOrders = allDeliverdOrders.Count();

                if (totalDeliverdOrders > 0)
                {
                    var Sales = allDeliverdOrders.Sum(o => o.TotalOrderPrice);

                    ViewBag.TotalOrders = totalDeliverdOrders;
                    ViewBag.Sales = Sales;
                    ViewBag.NetProfit = Math.Round(Sales * (decimal)0.1, 2);
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
            var resturantsVM = restaurantService.GetAllRestaurantsJustMainInfo(filter: null, statuses: [AcctiveStatus.Unaccepted]);

            return View(resturantsVM);
        }

        public IActionResult ActiveResturatns()
        {
            AcctiveStatus[] acceptedStatuses = [ AcctiveStatus.Acctive, AcctiveStatus.Warning, AcctiveStatus.Block ];

            var resturantsVM = restaurantService.GetAllRestaurantsJustMainInfo(filter: null, statuses: acceptedStatuses);

            return View(resturantsVM);
        }

        public IActionResult DeletedRestaurant()
        {
            var restaurants = restaurantService.GetDeletedRestaurants();

            if (restaurants.IsNullOrEmpty())
            {
                TempData["Error"] = "There is no deleted restaurants";
                return RedirectToAction("Index");
            }

            return View(restaurants);
        }
    }
}
