using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Models.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace E_Retalling_Portal.Controllers.Filter
{
    public class CustomerFilter : IActionFilter
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
                    Account acc = DbContext.Accounts.GetAccountByAccountId(accId.Value).FirstOrDefault();
                    if (acc != null && acc.roleId == 1)
                    {
                        haveAccess = true;
                    }
                }
            }

            if (!haveAccess)
            {
                context.Result = new RedirectResult("/Login");
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

        }
    }
}
