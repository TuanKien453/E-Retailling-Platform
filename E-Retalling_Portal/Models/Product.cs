using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

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
        [MaxLength(5000)]
        public string desc { get; set; }
        public double price { get; set; }
        public int quantity {  get; set; }
        public bool isVariation { get; set; }
        public int status {  get; set; }
        [MaxLength(60)]
        public string? deleteAt { get; set; }
        [MaxLength(60)]
        public string? createAt { get; set; }
        public string? vectorEmbaddingJson { get; set; }
        [NotMapped]  
        public float[]? vectorEmbadding
        {
            get => string.IsNullOrEmpty(vectorEmbaddingJson) ? null : JsonSerializer.Deserialize<float[]>(vectorEmbaddingJson);
            set => vectorEmbaddingJson = JsonSerializer.Serialize(value);
        }
        public Image? coverImage { get; set; }
        public Shop? shop { get; set; }
        public Category? category { get; set; }
        public List<Image>? images { get; set; }
        public List<ProductItem>? productItems { get; set; }
        public List<ProductDiscount>? productDiscounts { get; set; }

    }
}
