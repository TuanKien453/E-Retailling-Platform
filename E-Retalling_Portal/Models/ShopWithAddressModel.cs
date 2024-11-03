using MimeKit.Tnef;
using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class ShopWithAddressModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string province { get; set; }
        public string district { get; set; }
        public string? ward { get; set; }
        [Required, DataType(DataType.Date)]
        public string createdAt { get; set; }
    }
}
