using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class Shop
    {
        [Key]
        public int id { get; set; }
        public int accountId { get; set; }
        public string field {  get; set; }
    }
}
