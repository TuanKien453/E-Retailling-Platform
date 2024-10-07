using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace E_Retalling_Portal.Models.Query
{
    public static class UserQuery
    {

        public static IQueryable<User> GetUserById(this DbSet<User> dbUser, int userId)
        {
            return dbUser.Where(u => u.id == userId);
        }
        public static IQueryable<User> GetUserByUserIdInAccount(this DbSet<User> dbUser, int userId)
        {
            return dbUser.Where(u => u.id == userId);
        }

		public static IQueryable<User> GetUserByEmail(this DbSet<User> dbUser, string email)
		{
				return dbUser.Where(u => u.email == email);
		}

		public static async Task SaveUserToDatabase(this DbSet<User> dbUser, Context context, User user)
		{
				dbUser.Add(user);
				await context.SaveChangesAsync();
		}

    }

}
