using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace E_Retalling_Portal.Models.Query
{
    public static class OrderItemQuery
    {
        public static IQueryable<OrderItem> GetOrderItemByOrderId (this DbSet<OrderItem> DbOrderItems, int orderId)
        {
            return DbOrderItems.Where(oi => oi.orderId == orderId);
        }

        public static IQueryable<OrderItem> GetAllOrderItemHasSales (this DbSet<OrderItem> DbOrderItems)
        {
            return DbOrderItems.Where(p => p.shippingStatus == "delivered");
        }

        public static IQueryable<OrderItem> GetAllOrderItem(this DbSet<OrderItem> DbOrderItems)
        {
            return DbOrderItems;
        }

        public static double GetOrderItemPriceOnMonth (this DbSet<OrderItem> DbOrderItems, List<OrderItem> orders)
        {
            double price = 0;
            using (var context = new Context())
        
                foreach (var item in orders)
                {
                    price += item.price * item.quanity;
                }
            }
            return price;
        }
    public static IQueryable<OrderItem> GetOrderItemByUserId(this DbSet<OrderItem> dbAccount, int userId)
    {
        return dbAccount
                .Include(oi => oi.order)
                .Include(oi => oi.productItem)
                .Include(oi => oi.product)
                    .ThenInclude(p => p.shop)
                .Include(oi => oi.product)
                    .ThenInclude(p => p.coverImage)
                .Include(oi => oi.productItem)
                    .ThenInclude(pi => pi.image)
                .Where(oi => oi.order.userId == userId);
    }
}
