using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Retalling_Portal.Models
{
    public class Image
    {
        [Key]
        public int id { get; set; }
        public int productId { get; set; }
        [MaxLength(200)]
        public string imagePath { get; set; }
        public ProductItem? productItem { get; set; }
        public Product? product { get; set; }
    }
}
