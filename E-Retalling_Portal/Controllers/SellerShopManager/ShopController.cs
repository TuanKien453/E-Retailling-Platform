using E_Retalling_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using E_Retalling_Portal.Models.Enums;
using Microsoft.EntityFrameworkCore.Infrastructure;
using E_Retalling_Portal.Models.Query;
using E_Retalling_Portal.Controllers.Filter;
using System.Text;
using E_Retalling_Portal.Services.ExtendService;
using System.ComponentModel.DataAnnotations;
using System.Net;


namespace E_Retalling_Portal.Controllers.SellerShopManager
{
    [TypeFilter(typeof(ShopOwnerRoleFilter))]
    [TypeFilter(typeof(HaveShopFilter))]
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            using (var context = new Context())
            {
                int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
                return RedirectToAction("Index", "ShopDashBoard");
            }             
        }


         public IActionResult ViewShop()
         {
            using (var context = new Context())
            {
                int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
                Shop shop = context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();
                ViewBag.Shop = shop;    
                return View("Views/SellerShopManager/ShopInformation/ViewShop.cshtml");
            }
                      
         }

        public IActionResult UpdateShop()
        {
            using (var context = new Context())
            {
                int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
                Shop shop = context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();
                ViewBag.Name = shop.name;
                ViewBag.Provine = shop.province;
                ViewBag.District = shop.district;
                ViewBag.ward = shop.ward;
                ViewBag.Address = shop.address;
                ViewBag.ShopDescription = shop.shopDescription;
                return View("Views/SellerShopManager/ShopInformation/UpdateShop.cshtml");
            }

        }

        [HttpPost]
        public IActionResult UpdateShopProcess([MaxLength(100)] String name, [MaxLength(100)] String address, [MaxLength(2000)] String shopDescription)
        {
            if (ModelState.IsValid)
            {
                using (var context = new Context())
                {
                    int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());               
                    Shop? testShop = context.Shops.GetShopbyAccIdAndName(accId.Value, name).FirstOrDefault();
                    if (testShop == null)
                    {
                        Shop shop = context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();
                        shop.name = name;
                        shop.address = address;
                        shop.shopDescription = shopDescription;
                        context.Shops.Update(shop);
                        context.SaveChanges();
                        return RedirectToAction("ViewShop");
                    }
                    else
                    {
                        ViewBag.ErrorName = "This name has been used";
                        ViewBag.Name = name;
                        ViewBag.Address = address;  
                        ViewBag.ShopDescription = shopDescription;
                        return View("Views/SellerShopManager/ShopInformation/UpdateShop.cshtml");
                    }
                }
            } else return View("Views/Shared/ErrorPage/Error500.cshtml");
        }
	}
}
