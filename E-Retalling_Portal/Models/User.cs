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
        [Phone]
        public string phoneNumber { get; set; }
        public string displayName { get; set; }
        public string birthday { get; set; }
        public string gender { get; set; }

        public string address { get; set; }
        public string firstName { get; set; }

        public string lastName { get; set; }

        public List<Account> accounts { get; set; }
        public List<Order> orders { get; set; }

    }
}
