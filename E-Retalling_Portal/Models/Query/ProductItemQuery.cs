using Microsoft.EntityFrameworkCore;

namespace E_Retalling_Portal.Models.Query
{
    public static class ProductItemQuery
    {
		public static IQueryable<ProductItem> GetProductItem(this DbSet<ProductItem> dbProductItem, int productId)
		{
			return dbProductItem.Include("image").Where(pi => pi.productId == productId && pi.deleteAt == null);
		}

		public static IQueryable<ProductItem> GetProductItemByProductItemId(this DbSet<ProductItem> dbProductItem, int productItemId)
		{
			return dbProductItem.Where(pi => pi.id == productItemId && pi.deleteAt == null);
		}

		public static IQueryable<ProductItem> GetAllProductItem(this DbSet<ProductItem> dbProductItem)
		{
			return dbProductItem.Include(p => p.image).Include(p => p.product).Include(p => p.product.shop).Where(pi => pi.deleteAt == null);
		}

        public static void DeleteProductItemById(this DbSet<ProductItem> dbProductItem, int productItemId, DbContext context)
        {
            var pi = dbProductItem.GetProductItemByProductItemId(productItemId).FirstOrDefault();
            if (pi != null)
            {
                pi.deleteAt = DateTime.Now.ToString();
                context.Entry(pi).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static bool IsShop(this DbSet<ProductItem> dbProductItem, int shopId, int productItemId)
        {

            var pi = dbProductItem.Include("product").Where(pi=>pi.id == productItemId).FirstOrDefault();
            if (pi == null)
            {
                return false;
            }
            return pi.product.shopId== shopId;
        }

        public static double GetProductItemDiscountPrice(this DbSet<ProductItem> dbProductItem, ProductItem productItem)
        {
            using (var context = new Context())
            {
                ProductDiscount productDiscount = context.ProductDiscount.GetProductDiscountByProductIdAndProductItemId(productItem.productId, productItem.id).FirstOrDefault();
                if (productDiscount == null)
                {
                    return productItem.price;
                }
                Discount discount = context.Discounts.GetDiscountByDiscountId(productDiscount.discountId).FirstOrDefault();
                return productItem.price - Math.Round(productItem.price * Math.Round(((double)discount.value / 100), 2), 2);


            }
        }
    }
}
