using Microsoft.EntityFrameworkCore;

namespace E_Retalling_Portal.Models.Query
{
    public static class AccountQuery
    {
        public static IQueryable<Account> GetAccountByUserId(this DbSet<Account> dbAccount, int userId)
        {
            return dbAccount.Where(a => a.userId == userId);
        }

        public static IQueryable<Account> GetAccountByAccountId(this DbSet<Account> dbAccount, int userId)
        {
            return dbAccount.Where(a => a.id == userId);
        }
    }
}
