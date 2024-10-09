using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class ProductItem
    {
        [Key]
        public int id { get; set; }
        public int  productId { get; set; }
        public int quantity { get; set; }
        public float price { get; set; }
        [Range(1, 5)]
        public int rating { get; set; }
        public int imageId { get; set; }
        [Required,MaxLength(100)]
        public String attribute { get; set; }
        public Product? product { get; set; }
        public Image? image { get; set; }
        public List<OrderItem>? orderItems { get; set; }


    }
}
