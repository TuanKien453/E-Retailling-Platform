using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class Order
    {
        [Key]
        public int id { get; set; }
        public int userId { get; set; }
        public string paymentMethod {get; set; }
        public string createTime { get; set; }
        public string note { get; set; }
        public string address { get; set; }
        public User user { get; set; }
        public List<OrderItem> orderItems { get; set; }

    }
}
