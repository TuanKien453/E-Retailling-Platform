using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class Shop
    {
        [Key]
        public int id { get; set; }
        public int accountId { get; set; }
        public string name { get; set; }
        public string createdAt { get; set; }
        public string shopDescription { get; set; }
        public int statusId {  get; set; }
        public string address {  get; set; }
        public Status status { get; set; }
        public Account account { get; set; }
        public List<Product> products { get; set; }
        

    }
}
