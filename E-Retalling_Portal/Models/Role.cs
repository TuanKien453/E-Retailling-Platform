using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class Role
    {
        [Key]
        public int id { get; set; }
        public string roleName { get; set; }

        public List<Account> accounts { get; set; }
    }
}
