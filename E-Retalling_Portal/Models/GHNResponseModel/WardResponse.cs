namespace E_Retalling_Portal.Models.GHNResponseModel
{
    public class Ward
    {
        public string WardCode { get; set; }
        public int DistrictID { get; set; }
        public string WardName { get; set; }
        public int Status { get; set; }
        public int SupportType { get; set; }
    }

    public class WardResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public List<Ward> Data { get; set; }
    }
}

