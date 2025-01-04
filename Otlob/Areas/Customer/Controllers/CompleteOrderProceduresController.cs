using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Otlob.Core.Hubs;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;

namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CompleteOrderProceduresController : Controller
    {
        public static int CreateNewCartInOrder(string userId, int restauranId, IUnitOfWorkRepository unitOfWorkRepository)
        {
            var cartInOrder = new CartInOrder
            {
                UserId = userId,
                ResturantId = restauranId
            };

            unitOfWorkRepository.CartInOrder.Create(cartInOrder);
            unitOfWorkRepository.SaveChanges();

            return cartInOrder.Id;
        }

        public static void AddOrdredMeals(int cartId, int restauranId, int cartInOrderId, IUnitOfWorkRepository unitOfWorkRepository)
        {
            var ordredMeals = unitOfWorkRepository.OrderedMeals.Get([o => o.Meal], expression: o => o.CartId == cartId);

            if (ordredMeals != null)
            {
                foreach (var meal in ordredMeals)
                {
                    var mealsInOrder = new MealsInOrder
                    {
                        RestaurantId = restauranId,
                        CartInOrderId = cartInOrderId,
                        MealId = meal.MealId,
                        MealName = meal.MealName,
                        MealDescription = meal.MealDescription,
                        Quantity = meal.Quantity
                    };

                    unitOfWorkRepository.MealsInOrder.Create(mealsInOrder);
                    unitOfWorkRepository.SaveChanges();
                }
            }
        }
        
        public async static void SendOrderToRestaurant(Order order, ApplicationUser user, IHubContext<OrdersHub> hubContext)
        {
            var newOrder = new
            {
                id = order.Id,
                name = user.UserName,
                address = order.CustomerAddres,
                phone = user.PhoneNumber,
                email = user.Email,
                date = order.OrderDate,
                status = "Pending"
            };

            await hubContext.Clients.Group(order.RestaurantId.ToString()).SendAsync("ReceiveOrder", newOrder);
        }

        public static void DeleteCart(int cartId, IUnitOfWorkRepository unitOfWorkRepository)
        {
            var cart = unitOfWorkRepository.Carts.GetOne(expression: c => c.Id == cartId);
            if (cart != null)
            {
                unitOfWorkRepository.Carts.Delete(cart);
                unitOfWorkRepository.SaveChanges();
            }
        }
    }
}
