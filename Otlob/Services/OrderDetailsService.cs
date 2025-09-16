namespace Otlob.Services
{
    public class OrderDetailsService : IOrderDetailsService
    {
        private readonly IMapper mapper;
        private readonly IOrderedMealsService orderedMealsService;
        private readonly IUnitOfWorkRepository unitOfWorkRepository;

        public OrderDetailsService(IMapper mapper,
                                   ICartService cartService,
                                   IUnitOfWorkRepository unitOfWorkRepository,
                                   IOrderedMealsService orderedMealsService)
        {
            this.mapper = mapper;
            this.orderedMealsService = orderedMealsService;
            this.unitOfWorkRepository = unitOfWorkRepository;
        }

        public ICollection<OrderDetails> AddOrderDetails(int cartId)
        {
            IEnumerable<CartDetails> ordredMeals = orderedMealsService.GetOrderedMealsDetails(cartId);

            if (ordredMeals is null)
            {
                return new List<OrderDetails>();
            }

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();

            foreach (var meal in ordredMeals)
            {
                OrderDetails orderDetails = new OrderDetails
                {
                    MealId = meal.MealId,
                    MealQuantity = meal.Quantity,
                    MealPrice = meal.PricePerMeal
                };

                orderDetailsList.Add(orderDetails);
            }

            return orderDetailsList;
        }

        public OrderDetailsViewModel GetOrderDetailsToViewPage(Order order)
        {
            var meals = unitOfWorkRepository
                         .OrderDetails
                         .GetAllWithSelect(
                             expression: od => od.OrderId == order.Id,
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

            if (meals is null) 
            { 
                return null!;
            }

            OrderDetailsViewModel orderDetailsVM = new();

            mapper.Map(order, orderDetailsVM);

            orderDetailsVM.Meals = meals!;

            return orderDetailsVM;
        }
    }
}
