namespace E_Retalling_Portal.Models
{
    public class Product_option
    {   
        public int product_item_id { get; set; }
        public int attribute_id {  get; set; }
        public Attribute attribute { get; set; }
        public Product_item product_Item { get; set; }
    }
}
