namespace E_Retalling_Portal.Models.GHNRequestModel
{
    public class OrderRequest
    {
        public string Token { get; set; }
        public int ShopId { get; set; }
        public int PaymentTypeId { get; set; }
        public string RequiredNote { get; set; }
        public int Weight { get; } = 1000;
        public int Length { get; } = 30;
        public int Width { get; } = 30;
        public int Height { get; } = 30;
        public int? DeliverStationId { get; } = null;
        public int ServiceTypeId { get; set; }
        public string Coupon { get; set; }
        public string FromName { get; set; }
        public string FromPhone { get; set; }
        public string FromAddress { get; set; }
        public string FromWardName { get; set; }
        public string FromDistrictName { get; set; }
        public string FromProvinceName { get; set; }
        public string ToName { get; set; }
        public string ToPhone { get; set; }
        public string ToAddress { get; set; }
        public int? ToDistrictId { get; set; }
        public string ToWardCode { get; set; }
        public List<int> PickShift { get; set; }
        public List<OrderItemRequest> Items { get; set; }
    }
}
