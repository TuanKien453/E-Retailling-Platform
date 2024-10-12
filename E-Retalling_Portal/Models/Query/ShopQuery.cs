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

        public static IQueryable<Shop> GetShopByName(this DbSet<Shop> dbShop, string name)
        {
            return dbShop.Where(s => s.name == name);
        }

        public static IQueryable<Shop> GetShopbyAccIdAndName(this DbSet<Shop> dbShop, int accId, string name)
        {
            return dbShop.Where(s => s.accountId == accId && s.name == name);
        }

    }
}
