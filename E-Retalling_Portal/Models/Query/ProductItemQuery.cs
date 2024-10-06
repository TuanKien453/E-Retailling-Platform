using Microsoft.EntityFrameworkCore;

namespace E_Retalling_Portal.Models.Query
{
    public static class ProductItemQuery
    {
        public static IQueryable<ProductItem> GetProductItem(this DbSet<ProductItem> dbProductItem, int productId)
        {
            return dbProductItem.Include("image").Where(pi=>pi.productId==productId);
        }
    }
}
