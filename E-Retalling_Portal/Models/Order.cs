using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class Order
    {
        [Key]
        public int id { get; set; }
        public int userId { get; set; }
        [MaxLength(60)]
        public string paymentMethod {get; set; }
        [MaxLength(100)]
        public string createTime { get; set; }
        [MaxLength(100)]
        public string? endTime { get; set; }
        [MaxLength(60)]
        public string paymentStatus { get; set; }
        [MaxLength(200)]
        public string? note { get; set; }
        [MaxLength(100)]
        public string province { get; set; }
		[MaxLength(100)]
		public string district { get; set; }
		[MaxLength(100)]
		public string commune { get; set; }
		[MaxLength(100)]
		public string? address { get; set; }
        public User user { get; set; }
        public List<OrderItem> orderItems { get; set; }

    }
}
