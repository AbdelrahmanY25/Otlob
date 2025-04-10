using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;
using Otlob.IServices;

namespace Otlob.Services
{
    public class MealPriceHistoryService : IMealPriceHistoryService
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;

        public MealPriceHistoryService(IUnitOfWorkRepository unitOfWorkRepository)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
        }

        public bool AddMealPriceHistory(int mealId, decimal price)
        {
            var mealPriceHistory = new MealPriceHistory
            {
                MealId = mealId,
                Price = price
            };

            unitOfWorkRepository.MealsPriceHistories.Create(mealPriceHistory);
            unitOfWorkRepository.SaveChanges();
            return true;
        }

        public IQueryable<MealPriceHistoryVM>? GetMealPriceHistories(int mealId)
        {
            var mealPH = unitOfWorkRepository.MealsPriceHistories.GetAllWithSelect
            (
                selector: mph => new MealPriceHistoryVM
                {
                    Price = mph.Price,
                    StartDate = mph.StartDate,
                    EndDate = mph.EndDate
                },
                expression: mph => mph.MealId == mealId,
                tracked: false
            );

            if (mealPH is not null)
            {
                mealPH = mealPH.OrderByDescending(mp => mp.StartDate);
            }

            return mealPH;
        }

        public bool UpdateMealPriceHistory(int mealId, decimal price)
        {
            MealPriceHistory? oldMealPriciesHistory = unitOfWorkRepository.MealsPriceHistories
                 .Get(expression: m => m.MealId == mealId)
                 .OrderByDescending(mh => mh.StartDate)
                 .FirstOrDefault();

            if (oldMealPriciesHistory is not null)
            {
                oldMealPriciesHistory.EndDate = DateTime.Now;
            }

            var mealPriceHistory = new MealPriceHistory
            {
                Price = price,
                MealId = mealId
            };

            unitOfWorkRepository.MealsPriceHistories.Edit(oldMealPriciesHistory);
            unitOfWorkRepository.MealsPriceHistories.Create(mealPriceHistory);
            unitOfWorkRepository.SaveChanges();

            return true;
        }
    }
}
