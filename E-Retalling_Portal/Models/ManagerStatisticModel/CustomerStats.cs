namespace E_Retalling_Portal.Models.ManagerStatisticModel
{
    public class CustomerStats
    {
        public int shopId { get; set; }
        public string shopName { get; set; }
        public int saleYear { get; set; }
        public int saleMonth { get; set; }
        public int saleDay { get; set; }
        public int totalCustomers { get; set; }
    }
}
