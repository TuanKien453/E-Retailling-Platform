using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace E_Retalling_Portal.Models.Query
{
    public static class CategoryQuery
    {
        public static IQueryable<Category> GetCategories(this DbSet<Category> dbCate)
        {
            return dbCate.Where(c => true && c.deleteAt == null);
        }
        public static IQueryable<Category> GetTopCategory(this DbSet<Category> dbCate)
        {
            return dbCate.Where(c => c.parentCategoryId == null && c.deleteAt == null);
        }
        public static IQueryable<Category> GetSubCategoriesWithParent(this DbSet<Category> dbCate, int parentId)
        {
            return dbCate.Where(c => c.parentCategoryId == parentId && c.deleteAt == null);
        }
        public static IQueryable<Category> GetSubCategories(this DbSet<Category> dbCate)
        {
            return dbCate.Where(c => c.parentCategoryId != null && c.deleteAt == null);
        }
        public static IQueryable<Category> GetSubCategoriesByParentCategoryId(this DbSet<Category> dbCate, int? categoryId)
        {
            return dbCate.Where(c => c.parentCategoryId == categoryId && c.deleteAt == null);
        }
        public static IQueryable<Category> GetSubCategoriesByCategoryId(this DbSet<Category> dbCate, int? categoryId)
        {
            return dbCate.Where(c => c.id == categoryId && c.deleteAt == null);
        }

        public static void DeleteCategoryWithChildren(this DbSet<Category> dbCate, int parentCategoryId, Context context)
        {
            var parentCategory = dbCate.FirstOrDefault(cat => cat.id == parentCategoryId);
            if (parentCategory != null)
            {
                // Perform recursive delete for all child categories
                DeleteChildCategories(dbCate, parentCategory.id, context);
                //----------------------------------------------------------------------------------------------
                List<Product> products = context.Products.Where(p => p.categoryId == parentCategoryId && p.deleteAt ==null).ToList();
                foreach (var product in products)
                {
                    product.status = 2;
                    var productItems = context.ProductItems.GetProductItem(product.id).ToList();
                    if (productItems.Count > 0)
                    {
                        foreach (var item in productItems)
                        {
                            context.ProductItems.DeleteProductItemById(item.id, context);
                        }
                    }
                    context.Products.DeleteProductById(product.id, context);
                }

                parentCategory.deleteAt = DateTime.Now.ToString();
                context.Entry(parentCategory).State = EntityState.Modified;

                context.SaveChanges();
            }
        }

        private static void DeleteChildCategories(DbSet<Category> dbCate, int parentCategoryId, Context context)
        {
            var childCategories = dbCate.Where(cat => cat.parentCategoryId == parentCategoryId).ToList();
            foreach (var childCategory in childCategories)
            {
                DeleteChildCategories(dbCate, childCategory.id, context);
                //----------------------------------------------------------------------------------------------
                List<Product> products = context.Products.Where(p=>p.categoryId== childCategory.id && p.deleteAt == null).ToList();
                foreach(var product in products)
                {
                    product.status = 2;
                    var productItems = context.ProductItems.GetProductItem(product.id).ToList();
                    if (productItems.Count > 0)
                    {
                        foreach (var item in productItems)
                        {
                            context.ProductItems.DeleteProductItemById(item.id, context);
                        }
                    }
                    context.Products.DeleteProductById(product.id, context);
                }

                childCategory.deleteAt = DateTime.Now.ToString();
                context.Entry(childCategory).State = EntityState.Modified;
            }
        }

        public static int GetAllNumberOfCategoriesByShop(this DbSet<Category> dbCate, int shopId)
        {
            using (var context = new Context())
            {
                int count = 0;
                List<Product> products = context.Products.GetProductsByShop(shopId).ToList();
                List<int> ints = new List<int>();
                foreach (var product in products)
                {
                    if (!ints.Contains(product.categoryId))
                    {
                        ints.Add(product.categoryId);
                    }
                }

                count = ints.Count;
                return count;

            }
        }

        public static List<Category> GetAllOfCategoriesByShop(this DbSet<Category> dbCate, int shopId)
        {
            using (var context = new Context())
            {
                int count = 0;
                List<Product> products = context.Products.GetProductsByShop(shopId).ToList();
                List<int> ints = new List<int>();
                List<Category> cate = new List<Category>();
                foreach (var product in products)
                {
                    if (!ints.Contains(product.categoryId))
                    {
                        ints.Add(product.categoryId);
                    }
                }

                foreach (int item in ints)
                {
                    Category example = context.Categories.GetSubCategoriesByCategoryId(item).FirstOrDefault();
                    cate.Add(example);
                }

                return cate;
            }
        }

    }
}
