namespace E_Retalling_Portal.Models.ManagerStatisticModel
{
    public class RevenueStats
    {
        public int shopId { get; set; }
        public string shopName { get; set; }
        public int saleYear { get; set; }
        public int saleMonth { get; set; }
        public int saleDay { get; set; }
        public decimal totalRevenue { get; set; }
    }
}
