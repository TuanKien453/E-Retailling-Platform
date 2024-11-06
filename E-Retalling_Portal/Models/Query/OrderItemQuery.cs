using Microsoft.EntityFrameworkCore;

namespace E_Retalling_Portal.Models.Query
{
    public static class OrderItemQuery
    {
        public static IQueryable<OrderItem> GetOrderItemByUserId(this DbSet<OrderItem> dbOrderItem, int userId)
        {
            return dbOrderItem
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

        public static IQueryable<OrderItem> GetOrderItemByOrderItemId(this DbSet<OrderItem> dbOrderItem, int orderItemId)
        {
            return dbOrderItem
                .Where(oi => oi.id == orderItemId );
        }

        public static IQueryable<OrderItem> GetStarAndCommentByProductId(this DbSet<OrderItem> dbOrderItem, int productId)
        {
            var orderItems = dbOrderItem
                .Include(oi => oi.order)
                .Include(oi => oi.productItem)
                .Include(oi => oi.product)
                    .ThenInclude(p => p.shop)
                .Where(oi => oi.productId == productId && oi.rating != null);

            return orderItems;
        }


    }
}
