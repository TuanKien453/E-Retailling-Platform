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
                if (context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault() == null)
                {
                    return RedirectToAction("CreateShop");
                } else return RedirectToAction("Index", "ShopDashBoard");
            }             
        }

        public IActionResult CreateShop()
        {
            int ?accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
            if (accId != null)
            {
                using (var context = new Context())
                {
                    Shop ?shop = context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();
                    if (shop == null)
                    {
                        return View("Views/SellerShopManager/ShopInformation/CreateShop.cshtml");
                    } else return RedirectToAction("Index", "ShopDashBoard");

                }
            } else return View("Views/Shared/ErrorPage/Error500.cshtml");
            
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
                    } else
                    {
                        ViewBag.ErrorName = "This name has been used";
                        ViewBag.Shop = shop;
                        return View("Views/SellerShopManager/ShopInformation/CreateShop.cshtml");
                    }
                    
                }

            }
            else return View("Views/Shared/ErrorPage/Error500.cshtml");
        }

         public ActionResult ViewShop()
         {
            using (var context = new Context())
            {
                int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
                Shop shop = context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();
                ViewBag.Shop = shop;    
                return View("Views/SellerShopManager/ShopInformation/ViewShop.cshtml");
            }
                      
         }

        public ActionResult UpdateShop()
        {
            using (var context = new Context())
            {
                int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
                Shop shop = context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();
                ViewBag.Name = shop.name;
                ViewBag.Address = shop.address;
                ViewBag.ShopDescription = shop.shopDescription;
                return View("Views/SellerShopManager/ShopInformation/UpdateShop.cshtml");
            }

        }

        [HttpPost]
        public ActionResult UpdateShopProcess([MaxLength(100)] String name, [MaxLength(100)] String address, [MaxLength(2000)] String shopDescription)
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
