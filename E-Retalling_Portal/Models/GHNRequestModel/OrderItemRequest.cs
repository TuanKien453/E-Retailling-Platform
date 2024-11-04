using Newtonsoft.Json;

namespace E_Retalling_Portal.Models.GHNRequestModel
{
    public class OrderItemRequest
    {
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("weight")]
		public int Weight { get; set; } = 1000;
		[JsonProperty("quantity")]
		public int Quantity { get; set; }
	}
}
