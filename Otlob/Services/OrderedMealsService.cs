﻿namespace Otlob.Services
{
    public class OrderedMealsService : IOrderedMealsService
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly IEncryptionService encryptionService;

        public OrderedMealsService(IUnitOfWorkRepository unitOfWorkRepository, IEncryptionService encryptionService)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.encryptionService = encryptionService;
        }

        public OrderedMeals? GetOrderedMealById(int orderedMealId) => unitOfWorkRepository.OrderedMeals.GetOne(expression: o => o.Id == orderedMealId);

        public OrderedMeals? GetMealFromCart(int mealId) => unitOfWorkRepository.OrderedMeals.GetOne(expression: o => o.MealId == mealId);

        public OrderedMeals AddOrderedMeals(OrderedMealsVM orderedMealsVM, Cart userCart)
        {
            OrderedMeals mealInOrder = new OrderedMeals
            {
                CartId = userCart.Id,
                MealId = orderedMealsVM.MealId,
                PricePerMeal = orderedMealsVM.PricePerMeal,
                Quantity = orderedMealsVM.Quantity
            };

            unitOfWorkRepository.OrderedMeals.Create(mealInOrder);
            unitOfWorkRepository.SaveChanges();

            return mealInOrder;
        }

        public IEnumerable<OrderedMealsVM> GetOrderedMealsVMToView(int cartId)
        {
            var orderedMeals = unitOfWorkRepository
                               .OrderedMeals
                               .GetAllWithSelect
                                (
                                    selector: o => new OrderedMeals
                                    {
                                        Id = o.Id,
                                        PricePerMeal = o.PricePerMeal,
                                        Quantity = o.Quantity,
                                        Meal = new Meal { Name = o.Meal.Name, Image = o.Meal.Image, Description = o.Meal.Description }
                                    },
                                    expression: o => o.CartId == cartId, 
                                    tracked: false
                                );

            var orderedMealsVM = OrderedMealsVM.MappToOrderedMealsVMCollection(orderedMeals);

            return orderedMealsVM;
        }

        public IEnumerable<OrderedMeals> GetOrderedMealsWithMealsDetails(int cartId)
        {
            var orderedMeals = unitOfWorkRepository
                               .OrderedMeals
                               .GetAllWithSelect
                                (
                                    selector: o => new OrderedMeals
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

        public IEnumerable<OrderedMeals> GetOrderedMealsDetails(int cartId)
        {
            var orderedMeals = unitOfWorkRepository
                               .OrderedMeals
                               .GetAllWithSelect
                                (
                                    selector: o => new OrderedMeals
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

        public bool AddQuantityToOrderedMeals(OrderedMeals orderedMeal, OrderedMealsVM orderedMealsVM)
        {
            orderedMeal.Quantity += orderedMealsVM.Quantity;
            unitOfWorkRepository.OrderedMeals.Edit(orderedMeal);
            unitOfWorkRepository.SaveChanges();

            return true;
        }

        public MealQuantityResult EditOrderedMealsQuantity(string id, MealQuantity mealQuantity)
        {
            int orderMealId = encryptionService.DecryptId(id);

            OrderedMeals? selectedOrderMeal = GetOrderedMealById(orderMealId);

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

            unitOfWorkRepository.OrderedMeals.Edit(selectedOrderMeal);
            unitOfWorkRepository.SaveChanges();

            return new MealQuantityResult { Status = HandleMealQuantityProcess.Success};
        }

        public OrderedMeals DeleteOrderedMeal(string id)
        {
            int orderedMealId = encryptionService.DecryptId(id);

            OrderedMeals selectedOrderMeal = GetOrderedMealById(orderedMealId)!;

            unitOfWorkRepository.OrderedMeals.HardDelete(selectedOrderMeal);
            unitOfWorkRepository.SaveChanges();

            return selectedOrderMeal;            
        }

        public bool ThereIsAnyMealsInCart(OrderedMeals selectedOrderMeal) =>
            unitOfWorkRepository.OrderedMeals.IsExist(expression: c => c.CartId == selectedOrderMeal.CartId);

        public bool CheckWhenUserAddAnotherMeal(OrderedMealsVM orderedMealsVM, Cart usercart)
        {
            OrderedMeals? anotherMealInOrder = GetMealFromCart(orderedMealsVM.MealId);

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
            decimal totalMealsPrice = unitOfWorkRepository.OrderedMeals.Get(expression: o => o.CartId == cartId, tracked: false)!
            .Sum(o => (o.Meal.Price * o.Quantity));

            return totalMealsPrice;
        }           
    }
}
