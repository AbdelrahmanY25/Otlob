namespace Otlob.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderDetailsService orderDetailsService;
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly IEncryptionService encryptionService;
        private readonly ICartService cartService;
        private readonly IHubContext<OrdersHub> hubContext;

        public OrderService(IOrderDetailsService orderDetailsService,
                            IUnitOfWorkRepository unitOfWorkRepository,
                            IEncryptionService encryptionService,
                            ICartService cartService,
                            IHubContext<OrdersHub> hubContext)
        {
            this.orderDetailsService = orderDetailsService;
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.encryptionService = encryptionService;
            this.cartService = cartService;
            this.hubContext = hubContext;
        }
        
        public string GetUserIdByOrderId(int id)
        {
            Order? order = unitOfWorkRepository.Orders
                .GetOneWithSelect(
                    expression: o => o.Id == id,
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

            return order;
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

        public IQueryable<Order>? GetRestaurantOrdersByRestaurantId(string id)
        {
            int restaurantId = encryptionService.DecryptId(id);

            var orders = unitOfWorkRepository
                         .Orders
                         .Get(expression: o => o.RestaurantId == restaurantId, tracked: false);

            return orders;
        }

        public IQueryable<RestaurantOrdersVM>? GetRestaurantOrdersByRestaurantIdToView(string id)
        {
            IQueryable<RestaurantOrdersVM>? orders = GetRestaurantOrdersByRestaurantId(id)
                .Select(o => new RestaurantOrdersVM
                {
                    OrderId = o.Id,
                    OrderDate = o.OrderDate,
                    PaymentMethod = o.Method,
                    OrderStatus = o.Status,
                    TotalOrderPrice = o.TotalOrderPrice,
                    RestaurantId = o.RestaurantId,
                }).OrderByDescending(o => o.OrderDate);

            return orders;
        }

        public IQueryable<Order>? GetCurrentRestaurantOrders(int id, OrderStatus status, bool exclude)
        {
            IQueryable<Order>? orders = unitOfWorkRepository.Orders
                .GetAllWithSelect(expression: o => o.RestaurantId == id && (exclude ? o.Status != status : o.Status == status),
                    selector:
                        o => new Order
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

            return orders.OrderByDescending(o => o.OrderDate);
        }

        public IQueryable<TrackOrderVM>? GetUserTrackedOrders(string userId)
        {
            var orders = GetUserOrders(userId)
                .Select(to => new TrackOrderVM
                {
                    OrderId = to.Id,
                    OrderDate = to.OrderDate,
                    OrderStatus = to.Status,
                    RestaurantName = to.Restaurant.Name,
                    RestaurantImage = to.Restaurant.Image
                }).OrderByDescending(o => o.OrderDate);

            return orders;
        }

        public Order? GetOrderPaymentDetails(string id)
        {
            int orderId = encryptionService.DecryptId(id);

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

        public IQueryable<Order>? GetOrdersDayByStatus(OrderStatus status)
        {
            IQueryable<Order>? orders = unitOfWorkRepository
                .Orders
                .GetAllWithSelect(
                    expression: o => o.Status == status && o.OrderDate.Date == DateTime.Today.Date,
                    tracked: false,
                    selector: o => new Order
                    {
                        Id = o.Id,
                        OrderDate = o.OrderDate,
                        Status = o.Status,
                        Method = o.Method,
                        TotalMealsPrice = o.TotalMealsPrice,
                        TotalTaxPrice = o.TotalTaxPrice,
                        TotalOrderPrice = o.TotalOrderPrice,

                        Restaurant = new Restaurant
                        {
                            Name = o.Restaurant.Name,
                            Image = o.Restaurant.Image
                        },

                        User = new ApplicationUser
                        {
                            UserName = o.User.UserName,
                            Image = o.User.Image,
                            PhoneNumber = o.User.PhoneNumber,
                            Email = o.User.Email
                        }
                    }
                );

            if (orders is not null)
            {
                orders = orders.OrderByDescending(o => o.OrderDate);
            }

            return orders;
        }

        public int GetOrdersCountByDate(DateTime OrderDate)
        {
            IQueryable<Order>? orders = unitOfWorkRepository
                .Orders
                .Get(expression: o => o.OrderDate.Date == OrderDate.Date, tracked: false);

            if (orders is not null)
            {
                int ordersCount = orders.Count();
                return ordersCount;
            }

            return 0;
        }

        public void ChangeOrderstatus(int orderId)
        {
            var order = unitOfWorkRepository.Orders.GetOne(expression: o => o.Id == orderId);

            order.Status = GetNextStatus(order);

            unitOfWorkRepository.Orders.Edit(order);
            unitOfWorkRepository.SaveChanges();
        }

        public OrderStatus GetNextStatus(Order order)
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
