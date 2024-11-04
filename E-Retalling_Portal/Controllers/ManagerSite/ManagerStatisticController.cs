using E_Retalling_portal.Models.Query;
using E_Retalling_Portal.Controllers.Filter;
using E_Retalling_Portal.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Retalling_Portal.Controllers.ManagerSite
{
	[TypeFilter(typeof(ManagerFilter))]
	public class ManagerStatisticController : Controller
    {

        public IActionResult Index()
        {
            using (var context = new Context())
            {
                var shopList = context.Shops.ToList();
                return View("/Views/ManagerSite/ManagerStatistic/Index.cshtml", shopList);
            }
        }

        public IActionResult CategoryStats(int shopId)
        {
            var CategoryStats = ManagerStatisticQuery.GetCategoryStatsByShopId(shopId);
            return View("/Views/ManagerSite/ManagerStatistic/CategoryStats.cshtml", CategoryStats);
        }

        public IActionResult OrderStats(int shopId)
        {
            var OrderStats = ManagerStatisticQuery.GetOrderStatsByShopId(shopId);
            return View("/Views/ManagerSite/ManagerStatistic/OrderStats.cshtml", OrderStats);
        }

        public IActionResult CustomerStats(int shopId)
        {
            var CustomerStats = ManagerStatisticQuery.GetCustomerStatsByShopId(shopId);
            return View("/Views/ManagerSite/ManagerStatistic/CustomerStats.cshtml", CustomerStats);
        }

        public IActionResult RevenueStats(int shopId)
        {
            var RevenueStats = ManagerStatisticQuery.GetRevenueStatsByShopId(shopId);
            return View("/Views/ManagerSite/ManagerStatistic/RevenueStats.cshtml", RevenueStats);
        }

        public IActionResult Top10SellingProducts(int shopId)
        {
            var Top10SellingProduct = ManagerStatisticQuery.GetTop10BestSellingProductsByShopId(shopId);
            return View("/Views/ManagerSite/ManagerStatistic/Top10SellingProduct.cshtml", Top10SellingProduct);
        }
    }
}