namespace E_Retalling_Portal.Models.ManagerStatisticModel
{
    public class Top10SellingProduct
    {
        public int productId { get; set; }
        public int? productItemId { get; set; }
        public string productName { get; set; }
        public string? productItemName { get; set; }
        public int shopId { get; set; }
        public string shopName { get; set; }
        public int saleYear { get; set; }
        public int saleMonth { get; set; }
        public int saleDay { get; set; }
        public int quantitySold { get; set; }
    }



}