﻿using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Models.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace E_Retalling_Portal.Controllers.Filter
{
    public class ManagerFilter : IActionFilter
    {
		public void OnActionExecuting(ActionExecutingContext context)
		{
            Boolean haveAccess = false;
            var session = context.HttpContext.Session;
            var accId = session.GetInt32(SessionKeys.AccountId.ToString());
            if (accId != null)
            {
                using (var DbContext = new Context())
                {
                    Account acc = DbContext.Accounts.GetAccountByAccountId(accId.Value).FirstOrDefault();
                    if (acc != null && acc.roleId == 3)
                    {
                        haveAccess = true;
                    }
                }
            }

            if (!haveAccess)
            {
                context.Result = new RedirectResult("/Home/Error505");
            }
        }

		public void OnActionExecuted(ActionExecutedContext context)
		{

		}
	}
}
