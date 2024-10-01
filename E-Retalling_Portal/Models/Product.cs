using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public int categoryId { get; set; }
        public string name { get; set; }
        public int shopId { get; set; }
        public string desc { get; set; }
        public double price { get; set; }
        public int quanity {  get; set; }
        public string is_variation { get; set; }
        public Shop shop { get; set; }
        public Category category { get; set; }
        public List<Attribute_type> attribute_types { get; set; }
        public List<Image> images { get; set; }
        public List<Product_item> product_Items { get; set; }
        public List<Order_Item> order_Items { get; set; }

    }
}
