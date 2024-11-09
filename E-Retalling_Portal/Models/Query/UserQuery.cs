using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

        public static int GetAllNumberPeopeOrderByShop(this DbSet<User> DbUser, int shopId)
        {
            int count = 0;
            using (var context = new Context())
            {
                List<Product> products = context.Products.GetProductsByShopNoNull(shopId).ToList();
                List<OrderItem> orderItems = context.OrderItems.GetAllOrderItem().ToList();
                List<int> order = new List<int>();
                List<int> number = new List<int>();
                List<Order> orders = new List<Order>();
                if (!orderItems.IsNullOrEmpty())
                {
                    foreach (var product in products)
                    {
                        foreach (var item in orderItems)
                        {
                            if (product.id == item.productId)
                            {
                                orders.Add(context.Orders.GetOrderByOrderId(item.orderId).FirstOrDefault());
                            }
                        }
                    }
                }

                foreach (var item in orders)
                {
                    if (!number.Contains(item.userId))
                    {
                        number.Add(item.userId);
                    }
                }
                return count = number.Count();
            }

        }


    }

}
