using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int user_id { get; set; }
        public string payment_method {get; set; }
        public string create_time { get; set; }
        public string note { get; set; }
        public string address { get; set; }
        public User user { get; set; }
        public List<Order_Item> order_Items { get; set; }

    }
}
