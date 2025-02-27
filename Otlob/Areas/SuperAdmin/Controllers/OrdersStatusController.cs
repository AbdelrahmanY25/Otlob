using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Utility;

namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(Roles = SD.superAdminRole)]
    public class OrdersStatusController : Controller
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        public OrdersStatusController(IUnitOfWorkRepository unitOfWorkRepository)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
        }

        public IActionResult Pending(int pageNumber = 1)
        {
            var pendingOrders = unitOfWorkRepository.Orders.Get([o => o.Restaurant], expression: o => o.Status == OrderStatus.Pending && o.OrderDate.Day == DateTime.Today.Day && o.OrderDate.Month == DateTime.Today.Month && o.OrderDate.Year == DateTime.Today.Year);

            if (pendingOrders != null)
                pendingOrders = pendingOrders.OrderByDescending(o => o.OrderDate);

            const int pageSize = 9;
            double pageCount = Math.Ceiling((double)pendingOrders.Count() / pageSize);

            if (pageNumber - 1 < pageCount)
            {
                pendingOrders = pendingOrders.Skip((pageNumber - 1) * pageSize).Take(pageSize);

                ViewBag.Count = pageCount;
                pageNumber = Math.Clamp(pageNumber, 1, (int)pageCount);
                ViewBag.PageNumber = pageNumber;

                return View(model: pendingOrders);
            }

            return View(pendingOrders);
        }
         public IActionResult Preparing(int pageNumber = 1)
         {
            var preparingOrders = unitOfWorkRepository.Orders.Get([o => o.Restaurant], expression: o => o.Status == OrderStatus.Preparing && o.OrderDate.Day == DateTime.Today.Day && o.OrderDate.Month == DateTime.Today.Month && o.OrderDate.Year == DateTime.Today.Year);

            if (preparingOrders != null)
                preparingOrders = preparingOrders.OrderByDescending(o => o.OrderDate);

            const int pageSize = 9;
            double pageCount = Math.Ceiling((double)preparingOrders.Count() / pageSize);

            if (pageNumber - 1 < pageCount)
            {
                preparingOrders = preparingOrders.Skip((pageNumber - 1) * pageSize).Take(pageSize);

                ViewBag.Count = pageCount;
                pageNumber = Math.Clamp(pageNumber, 1, (int)pageCount);
                ViewBag.PageNumber = pageNumber;

                return View(model: preparingOrders);
            }

            return View(preparingOrders);
         }
         public IActionResult Shipped(int pageNumber = 1)
         {
            var shippedOrders = unitOfWorkRepository.Orders.Get([o => o.Restaurant], expression: o => o.Status == OrderStatus.Shipped && o.OrderDate.Day == DateTime.Today.Day && o.OrderDate.Month == DateTime.Today.Month && o.OrderDate.Year == DateTime.Today.Year);
            
            if (shippedOrders != null)
                shippedOrders = shippedOrders.OrderByDescending(o => o.OrderDate);

            const int pageSize = 9;
            double pageCount = Math.Ceiling((double)shippedOrders.Count() / pageSize);

            if (pageNumber - 1 < pageCount)
            {
                shippedOrders = shippedOrders.Skip((pageNumber - 1) * pageSize).Take(pageSize).OrderByDescending(o => o.OrderDate);

                ViewBag.Count = pageCount;
                pageNumber = Math.Clamp(pageNumber, 1, (int)pageCount);
                ViewBag.PageNumber = pageNumber;

                return View(model: shippedOrders);
            }

            return View(shippedOrders);
         }

    }
}
