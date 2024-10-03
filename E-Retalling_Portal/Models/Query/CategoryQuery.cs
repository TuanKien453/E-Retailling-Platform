using Microsoft.EntityFrameworkCore;

namespace E_Retalling_Portal.Models.Query
{
    public static class CategoryQuery
    {
        public static IQueryable<Category> GetTopCategory(this DbSet<Category> dbCate)
        {
            return dbCate.Where(c => c.parentCategoryId == null);
        }
        public static IQueryable<Category> GetSubCategoriesWithParent(this DbSet<Category> dbCate, int parentId)
        {
            return dbCate.Where(c => c.parentCategoryId == parentId);
        }
    }
}
