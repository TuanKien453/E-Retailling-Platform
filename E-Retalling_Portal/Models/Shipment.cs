using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class Shipment
    {
        [Key]
         public int Id { get; set; }
        public int order_Item_Id { get; set; }
        public string status { get; set; }
        public Order_Item order_Item { get; set; }
    }
}
