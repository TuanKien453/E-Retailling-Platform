using Microsoft.EntityFrameworkCore;

namespace E_Retalling_Portal.Models.Query
{
    public static class RegisterQuery
    {
        public static IQueryable<User> GetVaildUserData(this DbSet<User> dbUser, string email, string phone)
        {
            return dbUser.Where(u => u.email == email || u.phoneNumber == phone);
        }
        public static IQueryable<Account> GetVaildAccount(this DbSet<Account> dbAccount, string username)
        {
            return dbAccount.Where(u => u.username == username);
        }
        public static IQueryable<User> GetUserIdByEmail(this DbSet<User> dbUser, string email)
        {
            return dbUser.Where(u => u.email == email);
        }
    }
}
