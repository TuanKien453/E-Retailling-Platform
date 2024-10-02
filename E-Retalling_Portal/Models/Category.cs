using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class Category
    {
        [Key]
        public int id { get; set; }

        public int? parentCategoryId { get; set; }
        [MaxLength(40)]
        [Required(ErrorMessage = "Name is required")]
        public string name { get; set; }

        public Category? parent { get; set; }
        public List<Category>? childrens { get; set; }
        public List<Product> products { get; set; }
    }
}
