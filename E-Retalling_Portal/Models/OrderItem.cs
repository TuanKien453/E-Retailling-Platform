using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class OrderItem
    {
        [Key]
        public int id { get; set; }
        public int orderId { get; set; }
        public int productId { get; set; }
        public int productItemId { get; set; }
        public int quanity {  get; set; }
        public Order order { get; set; }
        public string shippingStatus { get; set; }
        public ProductItem productItem { get; set; }
        public Product product { get; set; }
    }
}
