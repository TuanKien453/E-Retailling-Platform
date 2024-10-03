using Microsoft.EntityFrameworkCore;

namespace E_Retalling_Portal.Models.Query
{
    public static class AccountQuery
    {
        public static IQueryable<Account> GetAccountsByUserId(this DbSet<Account> dbAccount, int userId)
        {
            return dbAccount.Where(a => a.userId == userId);
        }
        public static IQueryable<Account> GetAccountByAccountId(this DbSet<Account> dbAccount, int accId)
        {
            return dbAccount.Where(a => a.id == accId);
        }
    }
}
