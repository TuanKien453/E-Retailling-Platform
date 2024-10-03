using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class ProductItem
    {
        [Key]
        public int Id { get; set; }
        public int  productId { get; set; }
        public int quanityInStock { get; set; }
        public float price { get; set; }
        public int imageId { get; set; }

        public List<ProductOption> productOptions { get; set; }
        public Product product { get; set; }
        public Image image { get; set; }
        public OrderItem orderItem { get; set; }


    }
}
