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
		[MaxLength(100)]
		public string? displayName { get; set; }
        [DataType(DataType.Date)]
        public string? birthday { get; set; }
        public string? gender { get; set; }
        public int province { get; set; }
        public int district { get; set; }
        [MaxLength(100)]
        public string? ward { get; set; }
        [MaxLength(300)]
        public string? address { get; set; }
        [MaxLength(100)]
        public string? firstName { get; set; }
        [MaxLength(100)]
        public string? lastName { get; set; }

        public List<Account> accounts { get; set; }
        public List<Order> orders { get; set; }




    }
}
