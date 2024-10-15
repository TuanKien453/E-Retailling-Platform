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
			return dbProductItem.Include(p => p.image).Include(p => p.product).Where(pi => pi.deleteAt == null);
		}


	}
}
