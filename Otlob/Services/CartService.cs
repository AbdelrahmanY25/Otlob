using Microsoft.AspNetCore.Identity;
using Otlob.Core.IServices;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;
using Otlob.IServices;
using Otlob.Services;

namespace Otlob.Core.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWorkRepository.IUnitOfWorkRepository unitOfWorkRepository;
        private readonly IRestaurantService restaurantService;
        private readonly IOrderedMealsService orderedMealsService;
        private readonly IEncryptionService encryptionService;

        public CartService(IUnitOfWorkRepository.IUnitOfWorkRepository unitOfWorkRepository,
                          IRestaurantService restaurantService,
                          IOrderedMealsService orderedMealsService,
                          IEncryptionService encryptionService)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.restaurantService = restaurantService;
            this.orderedMealsService = orderedMealsService;
            this.encryptionService = encryptionService;
        }

        public Cart? GetUserCart(string userId, int restaurantId)
        {
            Cart? userCart = unitOfWorkRepository
                .Carts.GetOne
                (
                    expression: c => c.RestaurantId == restaurantId && c.ApplicationUserId == userId
                );

            return userCart;
        }
          
        public CartVM? GetUserCartToView(string userId)
        {
            CartVM? cartVM = unitOfWorkRepository.Carts
                .GetOneWithSelect
                 (
                    expression: c => c.ApplicationUserId == userId,
                    tracked: false,
                    selector: c => new CartVM
                    {
                        CartVMId = c.Id,
                        RestaurantId = c.RestaurantId
                    }
                 );

            if (cartVM is null)
            {
                return cartVM;
            }

            RestaurantVM? restaurantVM = restaurantService.GetRestaurantJustMainInfo(cartVM.RestaurantId);

            decimal orderedMeals = orderedMealsService.CalculateTotalMealsPrice(cartVM);

            CartVM newCartVM = CartVM.MappToCartVM(cartVM, restaurantVM, orderedMeals);
            
            return newCartVM;
        }
            
        public IQueryable<Cart> GetCarts(string userId)
        {
            var qurey = unitOfWorkRepository.Carts.Get(includeProps: [cr => cr.Restaurant, co => co.OrderedMeals],
                                                       expression: c => c.ApplicationUserId == userId, tracked: false);

            return qurey;
        }

        public Cart? AddCart(string userId, int restaurantId)
        {
            var cart = new Cart
            {
                RestaurantId = restaurantId,
                ApplicationUserId = userId
            };

            unitOfWorkRepository.Carts.Create(cart);
            unitOfWorkRepository.SaveChanges();

            return cart;
        }

        public bool DeleteCart(string id)
        {
            int cartId = encryptionService.DecryptId(id);
            var cart = unitOfWorkRepository.Carts.GetOne(expression: c => c.Id == cartId);

            if (cart is not null)
            {
                unitOfWorkRepository.Carts.Delete(cart);
                unitOfWorkRepository.SaveChanges();

                return true;
            }

            return false;
        }

        public bool IsCartNotEmpty(string userId) => unitOfWorkRepository.Carts.Get(expression: c => c.ApplicationUserId == userId).Any();

        public bool CheckIfCanAddOrderToCart(OrderedMealsVM orderedMealsVM, string userId, string resId)
        {
            int restaurantId = encryptionService.DecryptId(resId);

            if (IsCartNotEmpty(userId))
            {
                Cart? usercart = GetUserCart(userId, restaurantId);

                if (usercart is null)
                {
                    return false;
                }
                else
                {
                    return orderedMealsService.CheckWhenUserAddAnotherMeal(orderedMealsVM, usercart);
                }
            }
            else
            {
                return AddOrderToCart(orderedMealsVM, userId, restaurantId);
            }
        }

        public bool AddOrderToCart(OrderedMealsVM orderedMealsVM, string userId, int restaurantId)
        {
            Cart? newCart = AddCart(userId, restaurantId);
            orderedMealsService.AddOrderedMeals(orderedMealsVM, newCart);

            return true;
        }
    }
}
