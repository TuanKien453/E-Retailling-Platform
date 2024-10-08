using System;
using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class User
    {
        [Key]
        public int id { get; set; }
        [EmailAddress, Required]
        public string email { get; set; }
        [Phone, MaxLength(15), Required]
        public string phoneNumber { get; set; }
        [MaxLength(100)]
        public string displayName { get; set; }
        [DataType(DataType.Date), MaxLength(20)]
        public string birthday { get; set; }
        [MaxLength(15)]
        public string gender { get; set; }
        [MaxLength(100)]
        public string address { get; set; }
        [MaxLength(100)]
        public string firstName { get; set; }
        [MaxLength(100)]
        public string lastName { get; set; }

        public List<Account> accounts { get; set; }
        public List<Order> orders { get; set; }

    }
}
