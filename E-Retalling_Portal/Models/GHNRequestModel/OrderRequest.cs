using Newtonsoft.Json;

namespace E_Retalling_Portal.Models.GHNRequestModel
{
    public class OrderRequest
    {
		[JsonProperty("payment_type_id")]
		public int PaymentTypeId { get; set; }

		[JsonProperty("required_note")]
		public string RequiredNote { get; set; }

		[JsonProperty("weight")]
		public int Weight { get; set; }

        [JsonProperty("cod_amount")]
        public int COD { get; set; }
        [JsonProperty("length")]
		public int Length { get; set; } = 30;

		[JsonProperty("width")]
		public int Width { get; set; } = 30;

		[JsonProperty("height")]
		public int Height { get; set; } = 30;

		[JsonProperty("service_type_id")]
		public int ServiceTypeId { get; set; }
		//[JsonProperty("service_id")]
		//public int ServiceId { get; set; }

		[JsonProperty("from_name")]
		public string FromName { get; set; }

		[JsonProperty("from_phone")]
		public string FromPhone { get; set; }

		[JsonProperty("from_address")]
		public string FromAddress { get; set; }

		[JsonProperty("from_ward_code")]
		public string FromWardCode { get; set; }

		[JsonProperty("from_district_id")]
		public int FromDistrictId { get; set; }

		[JsonProperty("to_province_name")]
		public string ToProvinceName { get; set; }

		[JsonProperty("to_name")]
		public string ToName { get; set; }

		[JsonProperty("to_phone")]
		public string ToPhone { get; set; }

		[JsonProperty("to_address")]
		public string ToAddress { get; set; }

		[JsonProperty("to_district_id")]
		public int ToDistrictId { get; set; }

		[JsonProperty("to_ward_code")]
		public string ToWardCode { get; set; }

		[JsonProperty("items")]
		public List<OrderItemRequest> Items { get; set; }
	}
}
