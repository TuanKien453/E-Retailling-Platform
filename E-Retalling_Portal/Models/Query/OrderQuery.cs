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
            return DbOrder.FromSqlRaw(
                "SELECT * FROM Orders WHERE YEAR(endTime) = {0} AND MONTH(endTime) = {1}",
                year, month
            );
        }

        public static IQueryable<Order> GetOrderByYearMonthDay(this DbSet<Order> DbOrder, int year, int month, int day)
        {
            return DbOrder.FromSqlRaw(
                "SELECT * FROM Orders WHERE YEAR(endTime) = {0} AND MONTH(endTime) = {1} AND DAY(endTime) = {2}",
                year, month, day
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
