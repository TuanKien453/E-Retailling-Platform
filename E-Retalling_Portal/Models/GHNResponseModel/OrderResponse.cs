using static System.Runtime.InteropServices.JavaScript.JSType;

namespace E_Retalling_Portal.Models.GHNResponseModel
{
    public class OrderResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public Data Data { get; set; }
    }
    public class Fee
    {
        public int Coupon { get; set; }
        public int Insurance { get; set; }
        public int MainService { get; set; }
        public int R2S { get; set; }
        public int Return { get; set; }
        public int StationDo { get; set; }
        public int StationPu { get; set; }
    }

    public class Data
    {
        public string DistrictEncode { get; set; }
        public string ExpectedDeliveryTime { get; set; }
        public Fee Fee { get; set; }
        public string OrderCode { get; set; }
        public string SortCode { get; set; }
        public string TotalFee { get; set; }
        public string TransType { get; set; }
        public string WardEncode { get; set; }
    }
}
