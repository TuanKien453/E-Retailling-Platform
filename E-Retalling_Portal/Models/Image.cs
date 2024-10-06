using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Retalling_Portal.Models
{
    public class Image
    {
        [Key]
        public int id { get; set; }
        public int productId { get; set; }
        [MaxLength(400)]
        public string imageName { get; set; }

        public int? productCoveredId { get; set; }
        public Product? productCovered { get; set; }
        public List<ProductItem>? productItems { get; set; }
        public Product? product { get; set; }
    }
}
