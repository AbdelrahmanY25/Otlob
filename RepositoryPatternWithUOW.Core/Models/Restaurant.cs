namespace Otlob.Core.Models
{
    public class Restaurant : ImageUrl
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public decimal Rate { get; set; }        
        public decimal DeliveryFee { get; set; }        
        public decimal DeliveryDuration { get; set; }
        public AcctiveStatus AcctiveStatus { get; set; } = AcctiveStatus.Unaccepted;
        public RestaurantCategory Category { get; set; }
        public byte[]? Image { get; set; }

        [ValidateNever]
        public  ICollection<Meal> Meals { get; set; }

        [ValidateNever]
        public  ICollection<Delivery> Deliveries { get; set; }        

        [ValidateNever]
        public  ICollection<Order> Orders { get; set; }
    }

    public enum AcctiveStatus
    {
        Acctive,
        Block,
        Warning,
        Unaccepted
    }
   
    public enum RestaurantCategory
    {
        All,
        Burger,
        Pizza,
        EgyptionFood,
        FriedChicken,
        Shawarma,
        ChineseFood,
        ItalianFood,
        Sandwiches,
        HealthyFood,
        SeaFood,
        Drinks,
        IceCream,
        Dessert,
        Bakery,
        Coffee,
        Other
    }
}
