using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class Attribute
    {
        [Key]
        public int id { get; set; }
        public int attributeTypeId { get; set; }
        public string value { get; set; }
        public Attribute_type attributeType { get; set; }
        public List<Product_option> product_Options { get; set; }


    }
}
