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
    }
}
