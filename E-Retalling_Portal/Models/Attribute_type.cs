using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class Attribute_type
    {
        [Key]
        public int id { get; set; }
        public int product_id { get; set; }
        public string name { get; set; }
        public List<Attribute> attributes { get; set; }
        public Product product { get; set; }
    }
}
