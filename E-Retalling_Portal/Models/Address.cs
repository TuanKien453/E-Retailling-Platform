using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class Address
    {
        [Key]
        public int id { get; set; }
        
        public string address {  get; set; }
        
        public int userId { get; set; }

        public User user { get; set; }
    }
}
