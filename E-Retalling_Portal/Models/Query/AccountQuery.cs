using Microsoft.EntityFrameworkCore;

namespace E_Retalling_Portal.Models.Query
{
    public static class AccountQuery
    {
        public static IQueryable<Account> getAccountByUserId(this DbSet<Account> dbAccount, int userId)
        {
            return dbAccount.Where(a => a.id == userId);
        }
    }
}
