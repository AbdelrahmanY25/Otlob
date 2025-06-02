namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CompleteOrderProceduresController : Controller
    {       
        public async static void SendOrderToRestaurant(Order order, /*ApplicationUser user,*/ IHubContext<OrdersHub> hubContext)
        {
            var newOrder = new
            {
                id = order.Id,
                address = order.UserAddress,
                date = order.OrderDate,
                status = "Pending"
                //name = user.UserName,
                //phone = user.PhoneNumber,
                //email = user.Email,
            };

            await hubContext.Clients.Group(order.RestaurantId.ToString()).SendAsync("ReceiveOrder", newOrder);
        }
    }
}
