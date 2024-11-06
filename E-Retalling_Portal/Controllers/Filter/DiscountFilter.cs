using E_Retalling_Portal.Controllers.SellerShopManager;
using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using E_Retalling_Portal.Models.Query;
namespace E_Retalling_Portal.Controllers.Filter
{
    public class DiscountFilter : IActionFilter
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

                context.Result = new RedirectToActionResult("NoShop", "CreateShop", null);
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

            var controller = context.Controller as DiscountManagerController;
            if (controller == null)
            {
                return;
            }
            var actionName = context.ActionDescriptor.RouteValues["action"];
            if (actionName.StartsWith("Create"))
            {
                return;
            }
            int? accId = context.HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
            using (var dbContext = new Context())
            {
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

                if (context.ActionArguments.TryGetValue("productId", out var productIdObj1) && productIdObj1 is int productId1 &&
                    context.ActionArguments.TryGetValue("productItemId", out var productItemIdObj1) && productItemIdObj1 is int productItemId1)
                {
                    if (!dbContext.Products.IsNotMatch(productId1, productItemId1))
                    {
                        context.Result = new RedirectToActionResult("Error505", "Home", null);
                        return;
                    }
                }


                if (context.ActionArguments.TryGetValue("d", out var dObj) && dObj is Discount d)
                {
                    if (!dbContext.Discounts.IsShop(shop.id, d.id))
                    {
                        context.Result = new RedirectToActionResult("Error505", "Home", null);
                        return;
                    }
                }
                if (context.ActionArguments.TryGetValue("discountId", out var discountObj) && discountObj is int discountId)
                {
                    if (!dbContext.Discounts.IsShop(shop.id, discountId))
                    {
                        context.Result = new RedirectToActionResult("Error505", "Home", null);
                        return;
                    }
                }
            }

        }
    }
}
