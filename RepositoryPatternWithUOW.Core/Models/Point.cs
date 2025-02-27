namespace Otlob.Core.Models
{
    public class Point
    {
        public int Id { get; set; }
        public int Points { get; set; }
        public DateTime ExpireDate { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string UserId { get; set; }
    }
}
