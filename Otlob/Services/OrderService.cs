namespace Otlob.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderDetailsService orderDetailsService;
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly ICartService cartService;
        private readonly IHubContext<OrdersHub> hubContext;
        private readonly IDataProtector dataProtector;

        public OrderService(IOrderDetailsService orderDetailsService,
                            IUnitOfWorkRepository unitOfWorkRepository,
                            ICartService cartService,
                            IHubContext<OrdersHub> hubContext,
                            IDataProtectionProvider dataProtectionProvider)
        {
            this.orderDetailsService = orderDetailsService;
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.cartService = cartService;
            this.hubContext = hubContext;
            dataProtector = dataProtectionProvider.CreateProtector("SecureData");
        }
        
        public string GetUserIdByOrderId(string id)
        {
            int orderId = int.Parse(dataProtector.Unprotect(id));

            Order? order = unitOfWorkRepository.Orders
                .GetOneWithSelect(
                    expression: o => o.Id == orderId,
                    tracked: false,
                    selector: o => new Order
                    {
                        ApplicationUserId = o.ApplicationUserId,                  
                    });

            if (order is null)
            {
                return string.Empty;
            }

            return order.ApplicationUserId;
        }

        public Order GetOrderById(int id, int resId)
        {
            Order? order = unitOfWorkRepository.Orders
                .GetOneWithSelect(
                    expression: o => o.Id == id && o.RestaurantId == resId,
                    tracked: false,
                    selector: o => new Order
                    {
                        Id = o.Id,
                        RestaurantId = o.RestaurantId,
                        OrderDate = o.OrderDate,
                        Status = o.Status,
                        Method = o.Method,
                        TotalMealsPrice = o.TotalMealsPrice,
                        TotalTaxPrice = o.TotalTaxPrice,
                        TotalOrderPrice = o.TotalOrderPrice,
                    });

            return order!;
        }

        public bool AddOrder(int cartId, Order order)
        {
            order.MealsInOrder = orderDetailsService.AddOrderDetails(cartId);

            if (order.MealsInOrder.IsNullOrEmpty())
            {
                return false;
            }

            unitOfWorkRepository.Orders.Create(order);

            return SaveOrder(order, cartId);
        }

        private bool SaveOrder(Order newOrder, int cartId)
        {
            bool isUserCartDeleted = cartService.DeleteCart(cartId);

            if (isUserCartDeleted)
            {
                //CompleteOrderProceduresController.SendOrderToRestaurant(newOrder, hubContext);
                return true;
            }

            return false;
        }

        public IQueryable<Order>? GetUserOrders(string userId)
        {
            var orders = unitOfWorkRepository
                         .Orders
                         .Get(expression: o => o.ApplicationUserId == userId, tracked: false);

            return orders;
        }        

        public IQueryable<RestaurantOrdersVM>? GetCurrentRestaurantOrders(int id, OrderStatus status, bool exclude)
        {
            IQueryable<RestaurantOrdersVM>? restaurantOrdersVM = unitOfWorkRepository.Orders
                .GetAllWithSelect(expression: o => o.RestaurantId == id && (exclude ? o.Status != status : o.Status == status),
                    selector:
                        o => new RestaurantOrdersVM
                        {
                            OrderId = o.Id,
                            Key = dataProtector.Protect(o.Id.ToString()),
                            RestaurantId = o.RestaurantId,
                            OrderDate = o.OrderDate,
                            OrderStatus = o.Status,
                            PaymentMethod = o.Method,
                            TotalMealsPrice = o.TotalMealsPrice,
                            TotalTaxPrice = o.TotalTaxPrice,
                            TotalOrderPrice = o.TotalOrderPrice,
                        });

            return restaurantOrdersVM!.OrderByDescending(o => o.OrderDate);
        }

        public IQueryable<TrackOrderVM>? GetUserTrackedOrders(string userId)
        {
            var orders = GetUserOrders(userId)!
                .Select(to => new TrackOrderVM
                {
                    OrderId = dataProtector.Protect(to.Id.ToString()),
                    OrderDate = to.OrderDate,
                    OrderStatus = to.Status,
                    RestaurantName = to.Restaurant.Name!,
                    RestaurantImage = to.Restaurant.Image
                }).OrderByDescending(o => o.OrderDate);

            return orders;
        }

        public Order? GetOrderPaymentDetails(string id)
        {
            int orderId = int.Parse(dataProtector.Unprotect(id));

            Order? order = unitOfWorkRepository
                            .Orders
                            .GetOneWithSelect(
                                    expression: o => o.Id == orderId,
                                    tracked: false,
                                    selector: o => new Order
                                    {
                                        RestaurantId = o.RestaurantId,
                                        Status = o.Status,
                                        Method = o.Method,
                                        TotalMealsPrice = o.TotalMealsPrice,
                                        TotalTaxPrice = o.TotalTaxPrice
                                    }
                            );

            return order;
        }
        public IQueryable<Order> GetOrdersByDate(DateTime date)
        {
            IQueryable<Order>? orders = unitOfWorkRepository
                .Orders
                .GetAllWithSelect(
                    expression: o => o.OrderDate.Date == date.Date,
                    selector: o => new Order
                    {                        
                        Status = o.Status,
                    },
                    tracked: false
                );

            return orders!;
        }

        public IQueryable<RestaurantOrdersVM>? GetOrdersDayByStatus(OrderStatus status)
        {
            IQueryable<RestaurantOrdersVM>? orders = unitOfWorkRepository
                .Orders
                .GetAllWithSelect(
                    expression: o => o.Status == status && o.OrderDate.Date == DateTime.Today.Date,
                    tracked: false,
                    selector: o => new RestaurantOrdersVM
                    {
                        OrderId = o.Id,
                        Key = dataProtector.Protect(o.Id.ToString()),
                        OrderDate = o.OrderDate,
                        OrderStatus = o.Status,
                        PaymentMethod = o.Method,
                        TotalMealsPrice = o.TotalMealsPrice,
                        TotalTaxPrice = o.TotalTaxPrice,
                        TotalOrderPrice = o.TotalOrderPrice,
                        RestaurantName = o.Restaurant.Name!,
                        RestaurantImage = o.Restaurant.Image                        
                    }
                );

            if (orders is not null)
            {
                orders = orders.OrderByDescending(o => o.OrderDate);
            }

            return orders;
        }

        public IEnumerable<KeyValuePair<OrderStatus, int>> CountOrdersDayByStatus()
        {
            var orders = GetOrdersByDate(DateTime.Today).ToList();

            var ordersCount = orders.CountBy(o => o.Status);

            return ordersCount;
        }

        public int GetOrdersCountByDate(DateTime OrderDate)
        {
            var orders = GetOrdersByDate(OrderDate);

            if (orders is not null)
            {
                int ordersCount = orders.Count();
                return ordersCount;
            }

            return 0;
        }

        private Order GetOrderStatus(int orderId)
        {
            Order order = unitOfWorkRepository
                .Orders
                .GetOneWithSelect(
                    expression: o => o.Id == orderId,
                    selector: o => new Order
                    {
                        Id = o.Id,
                        Status = o.Status
                    }
                )!;

            return order!;
        }

        public void ChangeOrderstatus(int orderId)
        {
            Order order = GetOrderStatus(orderId);

            order.Status = GetNextStatus(order);

            unitOfWorkRepository.Orders.ModifyProperty(order, o => o.Status);
            unitOfWorkRepository.SaveChanges();
        }

        private OrderStatus GetNextStatus(Order order)
        {
            if (order is not null)
            {
                order.Status = order.Status switch
                {
                    OrderStatus.Pending => OrderStatus.Preparing,
                    OrderStatus.Preparing => OrderStatus.Shipped,
                    OrderStatus.Shipped => OrderStatus.Delivered,
                    _ => order.Status
                };
                return order.Status;
            }
            return OrderStatus.Pending;
        }
    }
}
