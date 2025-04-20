namespace Otlob.Core.ViewModel
{
    public class CartSummaryVM
    {
        public IQueryable<CartVM> CartsVM { get; set; }
        public decimal MealsPrice { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
