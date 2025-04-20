using Otlob.Core.IServices;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;
using Otlob.IServices;

namespace Otlob.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderDetailsService orderDetailsService;
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly IEncryptionService encryptionService;

        public OrderService(IOrderDetailsService orderDetailsService, IUnitOfWorkRepository unitOfWorkRepository, IEncryptionService encryptionService)
        {
            this.orderDetailsService = orderDetailsService;
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.encryptionService = encryptionService;
        }

        public bool AddOrder(Cart cart, Order order, int totalMealsPrice, int totalTaxPrice)
        {
            Order newOrder = new Order()
            {
                RestaurantId = cart.RestaurantId,
                ApplicationUserId = cart.ApplicationUserId,
                UserAddress = order.UserAddress,
                Method = order.Method,
                Notes = order.Notes,
                TotalMealsPrice = totalMealsPrice,
                TotalTaxPrice = totalTaxPrice
            };

            unitOfWorkRepository.Orders.Create(newOrder);
            unitOfWorkRepository.SaveChanges();

            return orderDetailsService.AddOrderDetails(cart, newOrder);
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
    }
}
