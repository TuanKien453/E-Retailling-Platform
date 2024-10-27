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
            return dbProduct.Include("coverImage").Include("images").Include("category").Include("shop").Where(p => p.id == productId && p.deleteAt == null);
        }
        public static IQueryable<Product> GetProduct(this DbSet<Product> dbProduct)
        {
            return dbProduct.Include("coverImage").Include("category").Include("images").Include("productItems").Include("shop").Where(p => p.deleteAt == null || (p.productItems.Where(pi=>pi.deleteAt==null).ToList().Count > 0 && p.isVariation == true) );
        }
        public static IQueryable<Product> GetSimilarProductByProductCategory(this DbSet<Product> dbProduct, Category category, int productId)
        {
            return dbProduct.Include(p => p.coverImage).Include(p => p.images).Include(p => p.productItems).Include(p => p.category).Where(p => p.deleteAt == null && p.category.parentCategoryId == category.parentCategoryId && p.id != productId || (p.productItems.Count > 0 && p.id != productId));
        }

        public static void DeleteProductById(this DbSet<Product> dbProduct, int productId, Context context)
        {
            var product = dbProduct.GetProductById(productId).FirstOrDefault();
            if (product != null)
            {
                context.ProductDiscount.DeleteProductDiscount(productId, null);
                context.SaveChanges();
                product.deleteAt = DateTime.Now.ToString();
                context.Entry(product).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
        public static IQueryable<Product> GetProductsNoVariation(this DbSet<Product> dbProduct)
        {
            return dbProduct.Include(p => p.coverImage).Include(p => p.shop).Where(p => p.deleteAt == null && p.isVariation == false);
        }

        public static bool IsShop(this DbSet<Product> dbProduct, int shopId, int productId) {
        
            var p = dbProduct.Where(p=>p.id==productId).FirstOrDefault();
            if (p==null)
            {
                return false;
            }
            return p.shopId == shopId;
        }

        public static double GetProductDiscountPrice (this DbSet<Product> dbProduct, Product product)
        {
            using (var context = new Context())
            {
                ProductDiscount productDiscount = context.ProductDiscount.GetProductDiscountByProductIdAndProductItemId(product.id, null).FirstOrDefault();
                if (productDiscount == null)
                {
                    return product.price;
                } 
                Discount discount = context.Discounts.GetDiscountByDiscountId(productDiscount.discountId).FirstOrDefault();
                var today = DateTime.Today;
                if (today >= DateTime.Parse(discount.startDate.ToString()) && today <= DateTime.Parse(discount.endDate.ToString())) {
                    return product.price - Math.Round(product.price * Math.Round(((double)discount.value / 100), 2), 2);
                } else return product.price;
            }
        }
    }
}
