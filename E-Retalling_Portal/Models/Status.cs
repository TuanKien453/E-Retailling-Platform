using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class Status
    {
        [Key]
        public int id { get; set; }

        public string statusName { get; set; }

        public List<Shop> shops { get; set; }
    }
}
