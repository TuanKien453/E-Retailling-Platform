using System;
using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class User
    {
        [Key]
        public int id { get; set; }

        [DataType(DataType.EmailAddress), MaxLength(100)]
        public string? email { get; set; }
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must be 10 digits starting with 0.")]
        public string? phoneNumber { get; set; }
        public string? displayName { get; set; }
        [DataType(DataType.Date)]
        public string? birthday { get; set; }
        public string? gender { get; set; }
        public string? address { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }

        public List<Account> accounts { get; set; }
        public List<Order> orders { get; set; }




    }
}
