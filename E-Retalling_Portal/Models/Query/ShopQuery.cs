using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace E_Retalling_Portal.Models.Query
{
    public static class ShopQuery
    {
        public static IQueryable<Shop> GetShopbyAccId(this DbSet<Shop> dbShop, int accId)
        {
            return dbShop.Where(s=>s.accountId == accId);
        }
    }
}
