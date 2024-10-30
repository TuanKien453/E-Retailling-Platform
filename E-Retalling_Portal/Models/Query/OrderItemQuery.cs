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

        public static double GetOrderItemPriceOnMonth (this DbSet<OrderItem> DbOrderItems, List<OrderItem> orders)
        {
            double price = 0;
            using (var context = new Context())
            {
                foreach (var item in orders)
                {
                    price += item.price * item.quanity;
                }
            }
            return price;
        }

    }
}
