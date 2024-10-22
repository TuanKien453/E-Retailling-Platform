using E_Retalling_Portal.Controllers.Filter;
using E_Retalling_Portal.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Retalling_Portal.Controllers.ManagerSite
{
	[TypeFilter(typeof(ManagerFilter))]
	public class ShopManageController : Controller
    {
        public IActionResult Index()
        {
            using (var context = new Context())
            {
                var shopList = context.Shops.ToList();
                ViewBag.Shops = shopList;

            }

            return View("/Views/ManageShop/ManageShop.cshtml");
        }
    }
}
