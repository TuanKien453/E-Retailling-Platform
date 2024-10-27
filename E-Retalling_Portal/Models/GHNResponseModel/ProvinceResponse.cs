namespace E_Retalling_Portal.Models.GHNResponseModel
{
    public class Province
    {
        public int ProvinceID { get; set; }
        public string ProvinceName { get; set; }
        public string CountryID { get; set; }
        public int Code { get; set; }
        public List<string> NameExtension { get; set; }
        public bool IsEnable { get; set; }
        public string RegionID { get; set; }
        public long UpdatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public bool CanUpdateCOD { get; set; }
        public int Status { get; set; }
    }

    public class ProvinceResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public List<Province> Data { get; set; }
    }
}
