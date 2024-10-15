using Microsoft.EntityFrameworkCore;

namespace E_Retalling_Portal.Models.Query
{
    public static class ImageQuery
    {
        public static IQueryable<Image> GetImagesByProductId(this DbSet<Image> dbImage, int productId)
        {
            return dbImage.Where(i => i.productId == productId);
        }
        public static IQueryable<Image> GetCoverImagesByProductId(this DbSet<Image> dbImage, int productId)
        {
            return dbImage.Where(i => i.productCoveredId == productId);
        }
    }
}
