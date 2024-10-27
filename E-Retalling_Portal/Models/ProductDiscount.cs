namespace E_Retalling_Portal.Models
{
	public class ProductDiscount
	{
		public int id { get; set; }
		public int discountId { get; set; }
		public int? productItemId { get; set; }
		public int productId { get; set; }
		public ProductItem? productItem { get; set; }
		public Product? product { get; set; }
		public Discount? discount { get; set; }
	}
}
