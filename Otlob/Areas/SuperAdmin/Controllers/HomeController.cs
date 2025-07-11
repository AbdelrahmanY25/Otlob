namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(Roles = SD.superAdminRole)]
    public class HomeController : Controller
    {        
        private readonly IOrderService orderService;
        private readonly IRestaurantService restaurantService;
        private readonly IUnitOfWorkRepository unitOfWorkRepository;

        public HomeController(IOrderService orderService,
                              IRestaurantService restaurantService,
                              IUnitOfWorkRepository unitOfWorkRepository)
        {
            this.orderService = orderService;
            this.restaurantService = restaurantService;
            this.unitOfWorkRepository = unitOfWorkRepository;
        }

        public IActionResult Index()
        {
            // Users data
            var users = unitOfWorkRepository.Users.Get(expression: u => u.RestaurantId == 0)!.Count();
            var partners = unitOfWorkRepository.Users.Get(expression: u => u.RestaurantId != 0)!.Count();

            ViewBag.Users = users;
            ViewBag.Partners = partners;

            // Orders data 
            var allOrders = orderService.GetOrdersCountByDate(DateTime.Today);

            var ordersStatusCount = orderService.CountOrdersDayByStatus();

            var dic = new Dictionary<string, decimal>();

            foreach (var orderStatus in ordersStatusCount)
            {
                dic.Add(orderStatus.Key.ToString(), Math.Round((decimal)orderStatus.Value / allOrders * 100, 2));
            }

            ViewBag.OrderStatusPercentages = dic;
            
            return View();
        }
    }
}
