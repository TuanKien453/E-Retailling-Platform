using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class Attribute
    {
        [Key]
        public int id { get; set; }
        public int attributeTypeId { get; set; }
        public string value { get; set; }
        public AttributeType attributeType { get; set; }
        public List<ProductOption> productOptions { get; set; }


    }
}
