using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class Order_Item
    {
        [Key]
        public int Id { get; set; }
        public int order_id { get; set; }
        public int product_Item_Id { get; set; }
        public int quanity {  get; set; }
        public Order order { get; set; }
        public Product product { get; set; }
        public List<Shipment> shipments { get; set; }
    }
}
