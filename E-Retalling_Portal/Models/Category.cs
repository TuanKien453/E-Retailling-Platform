using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class Category
    {
        [Key]
        public int id { get; set; }

        public string parentCategory { get; set; }

        public string name { get; set; }
    }
}
