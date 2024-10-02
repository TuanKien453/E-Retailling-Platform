using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class Shop
    {
        [Key]
        public int id { get; set; }
        public int account_id { get; set; }
        public string name { get; set; }
        public string created_at { get; set; }
        public string shop_description { get; set; }
        public int statusId {  get; set; }
        public string address {  get; set; }
        public Status status { get; set; }
        public Account account { get; set; }
        public List<Product> products { get; set; }
        

    }
}
