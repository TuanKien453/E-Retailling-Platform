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
            return dbAccount.Include("user").Where(a => a.id == accId);
        }

		public static IQueryable<Account> GetAccountByRoleIdAndUserId(this DbSet<Account> dbAccount, int roleId, User user)
		{
				return dbAccount.Where(acc => acc.roleId == roleId && acc.userId == user.id);
		}

		public static async Task SaveAccountToDatabase(this DbSet<Account> dbAccount, Context context, Account acc)
		{
				dbAccount.Add(acc);
				await context.SaveChangesAsync();
		}

		public static IQueryable<Account> GetAccountByExternalId(this DbSet<Account> dbAccount, string externalId, string externalType)
		{
				return dbAccount.Where(acc => acc.externalId == externalId && acc.externalType == externalType);
		}
	}
}
