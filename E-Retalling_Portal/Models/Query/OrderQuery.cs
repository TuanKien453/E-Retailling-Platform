using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
namespace E_Retalling_Portal.Models.Query
{
    public static class OrderQuery
    {
        public static IQueryable<Order> GetOrderByYearMonth(this DbSet<Order> DbOrder, int year, int month)
        {
            return DbOrder.FromSqlRaw("SELECT * FROM Orders WHERE YEAR(CONVERT(DATETIME, createTime)) = {0} AND MONTH(CONVERT(DATETIME, createTime)) = {1}", year, month);
        }



    }
}
