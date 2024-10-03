namespace E_Retalling_Portal.Models
{
    public class ProductOption
    {   
        public int productItemId { get; set; }
        public int attributeId {  get; set; }
        public Attribute attribute { get; set; }
        public ProductItem productItem { get; set; }
    }
}
