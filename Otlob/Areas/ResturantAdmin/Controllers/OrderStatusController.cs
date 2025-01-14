using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Otlob.EF.UnitOfWorkRepository;

namespace Otlob.Areas.ResturantAdmin.Controllers
{
    [Area("ResturantAdmin")]
    public class OrderStatusController : Controller
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public OrderStatusController(IUnitOfWorkRepository unitOfWorkRepository,
                                     UserManager<ApplicationUser> userManager)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.userManager = userManager;
        }
        public async Task<IActionResult> ChangeOrderStatus(int id)
        {
            var order = unitOfWorkRepository.Orders.GetOne(expression: o => o.Id == id);

            var status = GetNextStatus(order);
            order.Status = status;

            unitOfWorkRepository.Orders.Edit(order);
            unitOfWorkRepository.SaveChanges();

            var resturant = await userManager.GetUserAsync(User);
            var resturanOrders = unitOfWorkRepository.Orders.Get([o => o.ApplicationUser], expression: o => o.RestaurantId == resturant.Resturant_Id);

            return RedirectToAction("Index", "Orders", resturanOrders);
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
