namespace Otlob.Services
{
    public class OrderDetailsService : IOrderDetailsService
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly IOrderedMealsService orderedMealsService;
        private readonly IEncryptionService encryptionService;

        public OrderDetailsService(IUnitOfWorkRepository unitOfWorkRepository,
                                   IOrderedMealsService orderedMealsService,
                                   ICartService cartService,
                                   IEncryptionService encryptionService)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.orderedMealsService = orderedMealsService;
            this.encryptionService = encryptionService;
        }

        public ICollection<OrderDetails> AddOrderDetails(int cartId)
        {
            IEnumerable<OrderedMeals> ordredMeals = orderedMealsService.GetOrderedMealsDetails(cartId);

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
