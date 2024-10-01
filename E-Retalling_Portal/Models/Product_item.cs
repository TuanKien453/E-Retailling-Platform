using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class Product_item
    {
        [Key]
        public int Id { get; set; }
        public int  productId { get; set; }
        public int quanity_in_stock { get; set; }
        public float price { get; set; }
        public int image_id { get; set; }

        public List<Product_option> product_Options { get; set; }
        public Product product { get; set; }
        public Image image { get; set; }



    }
}
