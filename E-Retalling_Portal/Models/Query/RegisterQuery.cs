using Microsoft.EntityFrameworkCore;

namespace E_Retalling_Portal.Models.Query
{
    public static class RegisterQuery
    {
        public static IQueryable<User> GetValidUserData(this DbSet<User> dbUser, string email, string phone)
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
        public static IQueryable<User> GetValidUserData(this DbSet<User> dbUser, string email, string phone, int userId)
        {
            return dbUser.Where(u => (u.email == email || u.phoneNumber == phone) && u.id != userId);
        }

        public static IQueryable<Account> GetVaildShopOwnerAccount(this DbSet<Account> dbAccount, string username, int roleId)
        {
            return dbAccount.Where(u => u.username == username && u.roleId == roleId);
        }
    }
}
