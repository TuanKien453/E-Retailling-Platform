using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
	public class Setting
	{
		[Key]
		public int id { get; set; }
		[Required, MaxLength(50)]
		public string name { get; set; }
		[Required, MaxLength(50)]
		public string value { get; set; }
	}
}
