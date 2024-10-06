using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class Product
    {
        [Key]
        public int id { get; set; }
        public int categoryId { get; set; }
        [Required,MaxLength(100)]
        public string name { get; set; }
        public int shopId { get; set; }
        [MaxLength(2000)]
        public string desc { get; set; }
        public double price { get; set; }
        public int quantity {  get; set; }
        public bool isVariation { get; set; }
        public int status {  get; set; }

        public Image? coverImage { get; set; }
        public Shop? shop { get; set; }
        public Category? category { get; set; }
        public List<Image>? images { get; set; }
        public List<ProductItem>? productItems { get; set; }
    }
}
