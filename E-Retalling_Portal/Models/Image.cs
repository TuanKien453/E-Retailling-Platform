using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class Image
    {
        [Key]
        public int Id { get; set; }
        public int product_id { get; set; }
        public string image {  get; set; }
        public List<Product_item> product_items { get; set; }
        public Product product { get; set; }
    }
}
