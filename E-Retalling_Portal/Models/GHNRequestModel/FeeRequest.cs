using Newtonsoft.Json;

namespace E_Retalling_Portal.Models.GHNRequestModel
{
    public class FeeRequest
    {
        [JsonProperty("to_ward_code")]
        public string toWardCode { get; set; }

        [JsonProperty("to_district_id")]
        public int toDistrcitId { get; set; }
        [JsonProperty("weight")]
        public int weight { get; set; }
        [JsonProperty("service_type_id")]
        public int serviceTypeId { get; set; }
        [JsonProperty("from_district_id")]
        public int fromDistrictId { get; set; }
    }
}
