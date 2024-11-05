namespace E_Retalling_Portal.Models.ManagerStatisticModel
{
    public class CategoryStats
    {
        public int shopId { get; set; }
        public string shopName { get; set; }
        public int categoryId { get; set; }
        public string categoryName { get; set; }
        public int saleYear { get; set; }
        public int saleMonth { get; set; }
        public int saleDay { get; set; }
        public int totalQuantity { get; set; }
    }
}
