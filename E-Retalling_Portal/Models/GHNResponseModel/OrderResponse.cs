using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace E_Retalling_Portal.Models.GHNResponseModel
{
    public class OrderResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public class Fee
    {
        [JsonProperty("coupon")]
        public int Coupon { get; set; }

        [JsonProperty("insurance")]
        public int Insurance { get; set; }

        [JsonProperty("main_service")]
        public int MainService { get; set; }

        [JsonProperty("r2s")]
        public int R2S { get; set; }

        [JsonProperty("return")]
        public int Return { get; set; }

        [JsonProperty("station_do")]
        public int StationDo { get; set; }

        [JsonProperty("station_pu")]
        public int StationPu { get; set; }
    }

    public class Data
    {
        [JsonProperty("district_encode")]
        public string DistrictEncode { get; set; }

        [JsonProperty("expected_delivery_time")]
        public string ExpectedDeliveryTime { get; set; }

        [JsonProperty("fee")]
        public Fee Fee { get; set; }

        [JsonProperty("order_code")]
        public string OrderCode { get; set; }

        [JsonProperty("sort_code")]
        public string SortCode { get; set; }

        [JsonProperty("total_fee")]
        public string TotalFee { get; set; }

        [JsonProperty("trans_type")]
        public string TransType { get; set; }

        [JsonProperty("ward_encode")]
        public string WardEncode { get; set; }
    }
}
