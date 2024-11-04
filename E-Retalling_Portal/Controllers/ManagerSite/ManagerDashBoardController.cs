using E_Retalling_portal.Models.Query;
using E_Retalling_Portal.Controllers.Filter;
using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Query;
using Microsoft.AspNetCore.Mvc;

namespace E_Retalling_Portal.Controllers.Manager
{
	public class ManagerDashBoardController : Controller
    {
        public IActionResult Index()
        {
            using (var context = new Context())
            {
                ViewBag.TotalUsers = context.Users.Count();
                ViewBag.CustomerAccounts = context.Accounts.GetAllAccountsByRoleId(1).Count();
                ViewBag.TotalShops = context.Shops.Count();
                ViewBag.TotalCategories = context.Categories.Count();
                ViewBag.TotalRevenueStatsByDate = ManagerStatisticQuery.GetTotalRevenueStatsByDate();
                ViewBag.TotalOrderStats = ManagerStatisticQuery.GetTotalOrderStatsByMonth();
            }
            return View("/Views/ManagerSite/ManagerDashBoard/Index.cshtml");
        }
    }
}
