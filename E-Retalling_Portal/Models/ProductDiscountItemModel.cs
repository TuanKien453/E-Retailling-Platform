namespace E_Retalling_Portal.Models
{
    public class ProductDiscountItemModel
    {
        public ProductDiscount productDiscount { get; set; }
        public Product product { get; set; }
        public ProductItem productItem { get; set; }
        public string isDiscount { get; set; } = "false";
        public double discountedPrice { get; set; }
    }
}
