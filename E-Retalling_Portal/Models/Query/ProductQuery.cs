using Microsoft.EntityFrameworkCore;

namespace E_Retalling_Portal.Models.Query
{
    public static class ProductQuery
    {
        public static IQueryable<Product> GetProductsByShop(this DbSet<Product> dbProduct, int shopId)
        {
            return dbProduct.Include("coverImage").Include("images").Include("category").Where(p => p.shopId == shopId && p.deleteAt == null);
        }
        public static IQueryable<Product> GetProductById(this DbSet<Product> dbProduct, int productId)
        {
            return dbProduct.Include("coverImage").Include("images").Include("category").Where(p => p.id == productId && p.deleteAt == null);
        }
        public static IQueryable<Product> GetProduct(this DbSet<Product> dbProduct)
        {
            return dbProduct.Include("coverImage").Include("images").Include("productItems").Where(p => p.deleteAt == null || (p.productItems.Count > 0 && p.isVariation == true) );
        }
        public static IQueryable<Product> GetSimilarProductByProductCategory(this DbSet<Product> dbProduct, Category category, int productId)
        {
            return dbProduct.Include(p => p.coverImage).Include(p => p.images).Include(p => p.productItems).Include(p => p.category).Where(p => p.deleteAt == null && p.category.parentCategoryId == category.parentCategoryId && p.id != productId || (p.productItems.Count > 0 && p.id != productId));
        }


		public static IQueryable<Product> GetProdutsByPrice(this DbSet<Product> dbProduct, double? minPrice, double? maxPrice)
		{

            if (minPrice >= 2 && maxPrice < 1000)
            {
                return dbProduct.Where(p => p.price >= minPrice.Value && p.price < maxPrice.Value && p.deleteAt == null);
            }
            else
                if (minPrice >= 2 && maxPrice >= 1000)
            {
                return dbProduct.Where(p => p.price >= minPrice.Value && p.deleteAt == null);
            }
            return dbProduct;
        }
        public static void DeleteProductById(this DbSet<Product> dbProduct, int productId, DbContext context)
        {
            var product = dbProduct.GetProductById(productId).FirstOrDefault();
            if (product != null)
            {
                product.deleteAt = DateTime.Now.ToString();
                context.Entry(product).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
        public static IQueryable<Product> GetProductsNoVariation(this DbSet<Product> dbProduct)
        {
            return dbProduct.Include(p => p.coverImage).Where(p => p.deleteAt == null && p.isVariation == false);
        }
    }
}
