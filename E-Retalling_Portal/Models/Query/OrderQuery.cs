using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace E_Retalling_Portal.Models.Query
{
    public static class OrderQuery
    {
        public static IQueryable<Order> GetOrderByOrderId(this DbSet<Order> DbOrder, int orderId)
        {
            return DbOrder.Where(o => o.id == orderId);
        }
        public static IQueryable<Order> GetOrderByYearMonth(this DbSet<Order> DbOrder, int year, int month)
        {
            string yearMonthPrefix = $"{year:D4}{month:D2}"; // Format as yyyyMM
            return DbOrder.FromSqlRaw(
                "SELECT * FROM Orders WHERE LEFT(createTime, 6) = {0}",
                yearMonthPrefix
            );
        }

        public static IQueryable<Order> GetOrderByYearMonthDay(this DbSet<Order> DbOrder, int year, int month, int day)
        {
            string datePrefix = $"{year:D4}{month:D2}{day:D2}"; // Format as yyyyMMdd
            return DbOrder.FromSqlRaw(
                "SELECT * FROM Orders WHERE LEFT(createTime, 8) = {0}",
                datePrefix
            );
        }

        public static int GetAllNumberOfOrderByShop(this DbSet<Order> DbOrder, int shopId)
        {
            int count = 0;
            using (var context = new Context())
            {
                List<Product> products = context.Products.GetProductsByShop(shopId).ToList();
                List<OrderItem> orderItems = context.OrderItems.GetAllOrderItem().ToList();
                if (!orderItems.IsNullOrEmpty())
                {
                    foreach (var product in products)
                    {
                        foreach (var item in orderItems)
                        {
                            if (product.id == item.productId)
                            {
                                count++;
                            }
                        }
                    }
                }
            }
            return count;
        }
        

    }
}
