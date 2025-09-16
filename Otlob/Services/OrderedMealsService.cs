namespace Otlob.Services
{
    public class OrderedMealsService : IOrderedMealsService
    {
        private readonly IMapper mapper;
        private readonly IEncryptionService encryptionService;
        private readonly IUnitOfWorkRepository unitOfWorkRepository;

        public OrderedMealsService(IUnitOfWorkRepository unitOfWorkRepository, IEncryptionService encryptionService, IMapper mapper)
        {
            this.encryptionService = encryptionService;
            this.mapper = mapper;
            this.unitOfWorkRepository = unitOfWorkRepository;
        }

        public CartDetails? GetOrderedMealById(int orderedMealId) => unitOfWorkRepository.CartDetails.GetOne(expression: o => o.Id == orderedMealId);

        public CartDetails? GetMealFromCart(int mealId) => unitOfWorkRepository.CartDetails.GetOne(expression: o => o.MealId == mealId);

        public CartDetails AddOrderedMeals(OrderedMealsVM orderedMealsVM, Cart userCart)
        {
            CartDetails mealInOrder = new CartDetails
            {
                CartId = userCart.Id,
                MealId = orderedMealsVM.MealId,
                PricePerMeal = orderedMealsVM.PricePerMeal,
                Quantity = orderedMealsVM.Quantity
            };

            unitOfWorkRepository.CartDetails.Create(mealInOrder);
            unitOfWorkRepository.SaveChanges();

            return mealInOrder;
        }

        public IEnumerable<OrderedMealsVM> GetOrderedMealsVMToView(int cartId)
        {
            var orderedMeals = unitOfWorkRepository
                               .CartDetails
                               .GetAllWithSelect
                                (
                                    selector: o => new CartDetails
                                    {
                                        Id = o.Id,
                                        PricePerMeal = o.PricePerMeal,
                                        Quantity = o.Quantity,
                                        Meal = new Meal { Name = o.Meal.Name, Image = o.Meal.Image, Description = o.Meal.Description }
                                    },
                                    expression: o => o.CartId == cartId, 
                                    tracked: false
                                );

            IEnumerable<OrderedMealsVM> orderedMealsVM = mapper.Map<IEnumerable<OrderedMealsVM>>(orderedMeals);

            return orderedMealsVM;
        }

        public IEnumerable<CartDetails> GetOrderedMealsWithMealsDetails(int cartId)
        {
            var orderedMeals = unitOfWorkRepository
                               .CartDetails
                               .GetAllWithSelect
                                (
                                    selector: o => new CartDetails
                                    {
                                        Id = o.Id,
                                        PricePerMeal = o.PricePerMeal,
                                        Quantity = o.Quantity,
                                        Meal = new Meal { Name = o.Meal.Name, Price = o.Meal.Price, Description = o.Meal.Description }
                                    },
                                    expression: o => o.CartId == cartId, 
                                    tracked: false
                                );

            return orderedMeals!;
        }

        public IEnumerable<CartDetails> GetOrderedMealsDetails(int cartId)
        {
            var orderedMeals = unitOfWorkRepository
                               .CartDetails
                               .GetAllWithSelect
                                (
                                    selector: o => new CartDetails
                                    {
                                        PricePerMeal = o.PricePerMeal,
                                        Quantity = o.Quantity,
                                        MealId = o.MealId,
                                    },
                                    expression: o => o.CartId == cartId, 
                                    tracked: false
                                );

            return orderedMeals!.ToList();
        }

        public bool AddQuantityToOrderedMeals(CartDetails orderedMeal, OrderedMealsVM orderedMealsVM)
        {
            orderedMeal.Quantity += orderedMealsVM.Quantity;
            unitOfWorkRepository.CartDetails.Edit(orderedMeal);
            unitOfWorkRepository.SaveChanges();

            return true;
        }

        public MealQuantityResult EditOrderedMealsQuantity(string id, MealQuantity mealQuantity)
        {
            int orderMealId = encryptionService.DecryptId(id);

            CartDetails? selectedOrderMeal = GetOrderedMealById(orderMealId);

            if (selectedOrderMeal is null ||( mealQuantity != MealQuantity.Increase && mealQuantity != MealQuantity.Decrease))
            {
                return new MealQuantityResult { Status = HandleMealQuantityProcess.SomeThingWrong };
            }

            if (mealQuantity == MealQuantity.Increase)
            {
               selectedOrderMeal.Quantity = Math.Min(selectedOrderMeal.Quantity + 1, 99);
            }
            else
            {
                selectedOrderMeal.Quantity = Math.Max(selectedOrderMeal.Quantity - 1, 0);
            }

            if (selectedOrderMeal.Quantity == 0)
            {               
                return new MealQuantityResult { Status = HandleMealQuantityProcess.DeleteMeal };
            }

            unitOfWorkRepository.CartDetails.Edit(selectedOrderMeal);
            unitOfWorkRepository.SaveChanges();

            return new MealQuantityResult { Status = HandleMealQuantityProcess.Success};
        }

        public CartDetails DeleteOrderedMeal(string id)
        {
            int orderedMealId = encryptionService.DecryptId(id);

            CartDetails selectedOrderMeal = GetOrderedMealById(orderedMealId)!;

            unitOfWorkRepository.CartDetails.HardDelete(selectedOrderMeal);
            unitOfWorkRepository.SaveChanges();

            return selectedOrderMeal;            
        }

        public bool ThereIsAnyMealsInCart(CartDetails selectedOrderMeal) =>
            unitOfWorkRepository.CartDetails.IsExist(expression: c => c.CartId == selectedOrderMeal.CartId);

        public bool CheckWhenUserAddAnotherMeal(OrderedMealsVM orderedMealsVM, Cart usercart)
        {
            CartDetails? anotherMealInOrder = GetMealFromCart(orderedMealsVM.MealId);

            if (anotherMealInOrder is null)
            {
                AddOrderedMeals(orderedMealsVM, usercart);
            }
            else
            {
                AddQuantityToOrderedMeals(anotherMealInOrder, orderedMealsVM);
            }

            return true;
        }

        public decimal CalculateTotalMealsPrice(int cartId)
        {
            decimal totalMealsPrice = unitOfWorkRepository.CartDetails.Get(expression: o => o.CartId == cartId, tracked: false)!
            .Sum(o => (o.Meal.Price * o.Quantity));

            return totalMealsPrice;
        }           
    }
}
