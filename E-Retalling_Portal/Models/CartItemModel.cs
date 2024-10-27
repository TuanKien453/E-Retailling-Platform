namespace E_Retalling_Portal.Models
{
	public class CartItemModel
	{
		public int quantity { get; set; }
		public ProductItem? productItem { get; set; }
		public Product? product { get; set; }
		public float discountedPrice { get; set; }
	}
}