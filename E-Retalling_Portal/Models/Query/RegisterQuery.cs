using Microsoft.EntityFrameworkCore;

namespace E_Retalling_Portal.Models.Query
{
    public static class RegisterQuery
    {
        public static IQueryable<User> GetValidUserData(this DbSet<User> dbUser, string email, string phone)
        {
            return dbUser.Where(u => u.email == email || u.phoneNumber == phone);
        }
        public static IQueryable<Account> GetValidAccount(this DbSet<Account> dbAccount, string username)
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


        public static IQueryable<Account> GetValidShopOwnerAccount(this DbSet<Account> dbAccount, string username, int roleId)
        {
            return dbAccount.Where(u => u.username == username && u.roleId == roleId);
        }

        public static IQueryable<Account> GetValidAccountByUserId(this DbSet<Account> dbAccount, int userId, int roleId)
        {
            return dbAccount.Where(u => u.userId == userId && u.roleId == roleId);
        }

        public static IQueryable<User> GetValidUserDataByEmail(this DbSet<User> dbUser, string email)
        {
            return dbUser.Where(u => u.email == email);
        }
        public static IQueryable<User> GetValidUserDataByPhone(this DbSet<User> dbUser, string phone)
        {
            return dbUser.Where(u => u.phoneNumber == phone);
        }
    }
}
