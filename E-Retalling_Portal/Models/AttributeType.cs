using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class AttributeType
    {
        [Key]
        public int id { get; set; }
        public int productId { get; set; }
        public string name { get; set; }
        public List<Attribute> attributes { get; set; }
        public Product product { get; set; }
    }
}
