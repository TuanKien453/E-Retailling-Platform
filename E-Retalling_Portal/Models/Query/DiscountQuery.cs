using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace E_Retalling_Portal.Models.Query
{
    public static class DiscountQuery
    {
        public static IQueryable<Discount> GetDiscountByShop (this DbSet<Discount> dbDis, int shopId)
        {
            return dbDis.Where(d => d.shopId == shopId && d.deleteAt == null);
        }
        public static IQueryable<Discount> GetDiscountByName(this DbSet<Discount> dbDis, string name)
        {
            return dbDis.Where(d =>d.name == name);
        }
        public static IQueryable<Discount> GetDiscountByDiscountId(this DbSet<Discount> dbDis, int discountId)
        {
            return dbDis.Where(d => d.id == discountId);
        }
    }
}
