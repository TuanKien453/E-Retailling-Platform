using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using E_Retalling_Portal.Models.Query;

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
                context.Result = new RedirectResult("~/SellerShopManager/ShopInformation/NoShop");
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            
        }
    }
}
        
    

