using Microsoft.EntityFrameworkCore;

namespace E_Retalling_Portal.Models.Query
{
    public static class OrderItemQuery
    {
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
        public static IQueryable<OrderItem> GetOrderItemByShopId(this DbSet<OrderItem> dbAccount, int shopId)
        {
            return dbAccount
                .Include(oi => oi.product)
                .Where(oi => oi.product.shopId == shopId);
        }
    }
}
