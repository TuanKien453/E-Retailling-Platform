using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using E_Retalling_Portal.Models.Query;
using E_Retalling_Portal.Controllers.ShopManager;
using Microsoft.EntityFrameworkCore;

namespace E_Retalling_Portal.Controllers.Filter
{
    public class HaveShopFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            Boolean haveAccess = false;
            var session = context.HttpContext.Session;
            var accId = session.GetInt32(SessionKeys.AccountId.ToString());
            if (accId != null)
            {
                using (var DbContext = new Context())
                {
                    Shop? shop = DbContext.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();
                    if (shop != null)
                    {
                        haveAccess = true;
                    }
                }
            }

            if (!haveAccess)
            {

				context.Result = new RedirectToActionResult("NoShop","CreateShop",null);
			}
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

            var controller = context.Controller as ProductController;
            if (controller == null)
            {
                return;
            }
            var actionName = context.ActionDescriptor.RouteValues["action"];
            if (actionName.StartsWith("Add"))
            {
                return;
            }
            int? accId = context.HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
            using (var dbContext = new Context()) {
                var shop = dbContext.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();

                if (context.ActionArguments.TryGetValue("productId", out var productIdObj) && productIdObj is int productId)
                {
                    if (!dbContext.Products.IsShop(shop.id, productId))
                    {
                        context.Result = new RedirectToActionResult("Error505", "Home", null);
                        return;
                    }
                }

                if (context.ActionArguments.TryGetValue("productItemId", out var productItemIdObj) && productItemIdObj is int productItemId)
                {
                    if (!dbContext.ProductItems.IsShop(shop.id, productItemId))
                    {
                        context.Result = new RedirectToActionResult("Error505", "Home", null);
                        return;
                    }
                }

                if (context.ActionArguments.TryGetValue("p", out var pObj) && pObj is Product p)
                {
                    if (!dbContext.Products.IsShop(shop.id, p.id))
                    {
                        context.Result = new RedirectToActionResult("Error505", "Home", null);
                        return;
                    }
                }
                if (context.ActionArguments.TryGetValue("product", out var productObj) && productObj is Product product)
                {
                    if (!dbContext.Products.IsShop(shop.id, product.id))
                    {
                        context.Result = new RedirectToActionResult("Error505", "Home", null);
                        return;
                    }
                }

                if (context.ActionArguments.TryGetValue("pi", out var productItemObj) && productItemObj is ProductItem productItem)
                {
                    if (!dbContext.ProductItems.IsShop(shop.id, productItem.id))
                    {
                        context.Result = new RedirectToActionResult("Error505", "Home", null);
                        return;
                    }
                }

            }
        }
    }
}
        
    

