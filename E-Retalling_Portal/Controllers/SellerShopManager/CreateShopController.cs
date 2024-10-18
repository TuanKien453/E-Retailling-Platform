using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using E_Retalling_Portal.Models.Query;
using E_Retalling_Portal.Controllers.Filter;

namespace E_Retalling_Portal.Controllers.SellerShopManager
{
	public class CreateShopController : Controller
	{
		[TypeFilter(typeof(ShopOwnerRoleFilter))]
		public IActionResult Index()
		{
			var accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());

			
				using (var DbContext = new Context())
				{
					Shop? shop = DbContext.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();
					if (shop == null)
					{
						return RedirectToAction("CreateShop");
					} else return RedirectToAction("Index", "ShopDashBoard");
			}

		}
		public IActionResult NoShop()
		{
			return View("Views/SellerShopManager/CreateShop/NoShop.cshtml");
		}

		public IActionResult CreateShop()
		{
			return View("Views/SellerShopManager/CreateShop/CreateShop.cshtml");
		}

		[HttpPost]
		public IActionResult CreateShopProcess(Shop shop)
		{
			if (ModelState.IsValid)
			{
				int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
				using (var context = new Context())
				{
					if (context.Shops.GetShopByName(shop.name).FirstOrDefault() == null)
					{
						shop.accountId = accId.Value;
						shop.statusId = 1;
						context.Shops.Add(shop);
						context.SaveChanges();
						return RedirectToAction("Index", "ShopDashBoard");
					}
					else
					{
						ViewBag.ErrorName = "This name has been used";
						ViewBag.Shop = shop;
						return View("Views/SellerShopManager/CreateShop/CreateShop.cshtml");
					}

				}

			}
			else return View("Views/Shared/ErrorPage/Error500.cshtml");
		}
	}
}
