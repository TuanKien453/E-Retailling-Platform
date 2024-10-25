namespace E_Retalling_Portal.Models.GHNResponseModel
{
    public class Province
    {
        public int ProvinceID { get; set; }
        public string ProvinceName { get; set; }
        public string Code { get; set; }
    }

    public class ProvinceResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public List<Province> Data { get; set; }
    }
}
