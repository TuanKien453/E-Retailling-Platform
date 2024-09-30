using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Retalling_Portal.Models
{
    public class Account
    {
        [Key]
        public int id { get; set; }

        public int userId { get; set; }
        
        public string? username { get; set; }

        public string? password { get; set; }

        public int roleId { get; set; }

        public string externalId { get; set; }

        public string externalType { get; set; }

        public Role role { get; set; }

        public User user { get; set; }
    }

}
