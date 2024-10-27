using Microsoft.EntityFrameworkCore;

namespace E_Retalling_Portal.Models.Query
{
    public static class ProductDiscountQuery
    {
        public static IQueryable<ProductDiscount> GetProductDiscountByDiscountId(this DbSet<ProductDiscount> dbpd, int discountId)
        {
            return dbpd.Include("product").Include("productItem").Where(pi => pi.discountId == discountId);
        }
        public static IQueryable<ProductDiscount> GetProductDiscountByProductId(this DbSet<ProductDiscount> dbpd, int productId)
        {
            return dbpd.Where(pd => pd.productId == productId);
        }
        public static IQueryable<ProductDiscount> GetProductDiscountByProductIdAndProductItemId(this DbSet<ProductDiscount> dbpd, int productId, int? productItemId)
        {
            return dbpd.Where(pd => pd.productId == productId && pd.productItemId == productItemId);
        }

        public static void DeleteProductDiscount(this DbSet<ProductDiscount> dbpd, int productId, int? productItemId)
        {
            using (var context = new Context())
            {
                ProductDiscount pd = context.ProductDiscount.GetProductDiscountByProductIdAndProductItemId(productId, productItemId).FirstOrDefault();
                if (pd != null)
                {
                    context.Remove(pd);
                    context.SaveChanges();
                }
            }

        }
    }
}
