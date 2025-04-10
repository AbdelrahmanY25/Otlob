using Otlob.Core.IServices;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
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
                         .GetAllWithSelect(
                            expression: o => o.ApplicationUserId == userId,
                            tracked: false,
                            selector: o => new Order
                            {
                                Id = o.Id,
                                OrderDate = o.OrderDate,
                                Status = o.Status,
                                Restaurant = new Restaurant { Name = o.Restaurant.Name, Image = o.Restaurant.Image }
                            }
                          );

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
    }
}
