using Microsoft.AspNetCore.SignalR;
using Otlob.Areas.Customer.Controllers;
using Otlob.Core.Hubs;
using Otlob.Core.IServices;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Otlob.IServices;

namespace Otlob.Services
{
    public class OrderDetailsService : IOrderDetailsService
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly IOrderedMealsService orderedMealsService;
        private readonly IHubContext<OrdersHub> hubContext;
        private readonly ICartService cartService;
        private readonly IEncryptionService encryptionService;

        public OrderDetailsService(IUnitOfWorkRepository unitOfWorkRepository,
                                   IOrderedMealsService orderedMealsService,
                                   ICartService cartService,
                                   IEncryptionService encryptionService,
                                   IHubContext<OrdersHub> hubContext)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.orderedMealsService = orderedMealsService;
            this.cartService = cartService;
            this.encryptionService = encryptionService;
            this.hubContext = hubContext;
        }

        public bool AddOrderDetails(Cart cart, Order newOrder)
        {
            IEnumerable<OrderedMeals> ordredMeals = orderedMealsService.GetOrderedMealsDetails(cart.Id);

            if (ordredMeals is null)
            {
                return false;
            }

            foreach (var meal in ordredMeals)
            {
                OrderDetails orderDetails = new OrderDetails
                {
                    OrderId = newOrder.Id,
                    MealId = meal.MealId,
                    MealQuantity = meal.Quantity,
                    MealPrice = meal.PricePerMeal
                };

                unitOfWorkRepository.OrderDetails.Create(orderDetails);
            }

            return SaveOrderDetails(newOrder, cart);
        }

        public bool SaveOrderDetails(Order newOrder, Cart cart)
        {
            bool isUserCartDeleted = cartService.DeleteCart(cart.Id);

            if (isUserCartDeleted)
            {
                unitOfWorkRepository.SaveChanges();

                CompleteOrderProceduresController.SendOrderToRestaurant(newOrder, hubContext);

                return true;
            }

            unitOfWorkRepository.Orders.HardDelete(newOrder);
            unitOfWorkRepository.SaveChanges();

            return false;
        }

        public IQueryable<OrderDetails>? GetOrderDetailsToViewPage(string id)
        {
            int orderId = encryptionService.DecryptId(id);

            var meals = unitOfWorkRepository
                         .OrderDetails
                         .GetAllWithSelect(
                             expression: m => m.OrderId == orderId,
                             tracked: false,
                             selector: od => new OrderDetails
                             {
                                 MealPrice = od.MealPrice,
                                 MealQuantity = od.MealQuantity,
                                 TotalPrice = od.TotalPrice,
                                 Meal = new Meal
                                 {
                                     Name = od.Meal.Name,
                                     Image = od.Meal.Image
                                 }
                             }
                         );

            return meals;
        }
    }
}
