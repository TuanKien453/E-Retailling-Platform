using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
	public class Discount
	{
		public int id { get; set; }
		[Required,MaxLength(100)]
		public string name { get; set; }
		[Required, MaxLength(2000)]
		public string startDate { get; set; }
		public string endDate { get; set; }
		public int value { get; set; }
		public int shopId { get; set; }
		public int? deleteAt { get; set; }
		public Shop shop { get; set; }
		public List<ProductDiscount>? productDiscounts { get; set; }





	}
}
