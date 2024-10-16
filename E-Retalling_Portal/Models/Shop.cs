using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace E_Retalling_Portal.Models
{
    public class Shop
    {
        [Key]
        public int id { get; set; }
        public int accountId { get; set; }
        [Required, MaxLength(100)]
        public string name { get; set; }
        [Required, DataType(DataType.Date)]
        public string createdAt { get; set; }
        [Required, MaxLength(2000)]
        public string shopDescription { get; set; }
        public int statusId {  get; set; }
        [Required, MaxLength(100)]
        public string address {  get; set; }
        public Status? status { get; set; }
        public Account? account { get; set; }
        public List<Product>? products { get; set; }
        

    }
}
