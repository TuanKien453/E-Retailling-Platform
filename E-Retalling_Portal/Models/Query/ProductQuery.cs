using Microsoft.EntityFrameworkCore;

namespace E_Retalling_Portal.Models.Query
{
    public static class ProductQuery
    {
        public static IQueryable<Product> GetProductsByShop(this DbSet<Product> dbProduct, int shopId)
        {
            return dbProduct.Include("coverImage").Where(p=>p.shopId==shopId); 
        }
        public static IQueryable<Product> GetProdutsByPrice(this DbSet<Product> dbProduct, double? minPrice, double? maxPrice)
        {

            if (minPrice >= 2 && maxPrice < 1000)
            {
                return dbProduct.Where(p => p.price >= minPrice.Value && p.price < maxPrice.Value);
            }
            else
                if (minPrice >= 2 && maxPrice >= 1000)
            {
                return dbProduct.Where(p => p.price >= minPrice.Value );
            }
            return dbProduct;
        }

    }
}
