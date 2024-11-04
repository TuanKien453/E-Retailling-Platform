using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class OrderItem
    {
        [Key]
        public int id { get; set; }
        public int orderId { get; set; }
        public int productId { get; set; }
        public int? productItemId { get; set; }
        public int quantity {  get; set; }
        [MaxLength(50)]
		public string? shippingStatus { get; set; }
        public double price { get; set; }
		public int shippingFee { get; set; }
		public double transactionFee { get; set; }
		[MaxLength(50)]
		public string? externalOrderCode { get; set; }
		public Order order { get; set; }

        public ProductItem productItem { get; set; }
        public Product product { get; set; }
    }
}
