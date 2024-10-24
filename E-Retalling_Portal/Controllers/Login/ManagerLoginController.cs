using Azure;
using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using E_Retalling_Portal.Models.Query;
using E_Retalling_Portal.Services.EnodeAndDecode;
using E_Retalling_Portal.Controllers.Filter;

namespace E_Retalling_Portal.Controllers.Login
{
	public class ManagerLoginController : Controller
    {
        public IActionResult Index()
        {
            int? accountId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
            if (accountId != null)
            {
                using (var context = new Context())
                {
                    if (context.Accounts.GetAccountByAccountId((int)accountId).FirstOrDefault().roleId != 3)
                    {
                        HttpContext.Session.Clear();
                        return View("ManagerLoginForm");
                    }
                }
                return RedirectToAction("Index", "ManagerDashBoard");
            }
            return View("ManagerLoginForm");
        }


        [HttpPost]
        public IActionResult Login(Account account, bool rememberMe)
        {
            const int maxFailedAttempts = 5;
            const int lockoutDurationMinutes = 10;

            int? failedAttempts = HttpContext.Session.GetInt32(SessionKeys.FailedAttempts.ToString());
            DateTime? lockoutEndTime = HttpContext.Session.GetString(SessionKeys.LockoutEndTime.ToString()) != null ?
                DateTime.Parse(HttpContext.Session.GetString(SessionKeys.LockoutEndTime.ToString())) : (DateTime?)null;

            if (lockoutEndTime.HasValue && DateTime.Now < lockoutEndTime.Value)
            {
                ViewBag.ErrorMessage = "Account is locked. Please try again after " + (lockoutEndTime.Value - DateTime.Now).Minutes + " minutes.";
                return View("ManagerLoginForm");
            }

            using (var context = new Context())
            {
                if (HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString()) == null)
                {
                    var acc = context.Accounts
                                     .FirstOrDefault(x => x.username == account.username && x.password == account.password && x.roleId == account.roleId);

                    if (acc != null)
                    {
                        HttpContext.Session.Remove(SessionKeys.FailedAttempts.ToString());
                        HttpContext.Session.Remove(SessionKeys.LockoutEndTime.ToString());

                        HttpContext.Session.SetInt32(SessionKeys.AccountId.ToString(), acc.id);
                        HttpContext.Session.SetString(SessionKeys.DisplayName.ToString(), context.Users.GetUserById(acc.userId).FirstOrDefault().displayName);

                        return RedirectToAction("Index", "ManagerDashBoard");
                    }
                    else
                    {
                        failedAttempts = (failedAttempts ?? 0) + 1;
                        HttpContext.Session.SetInt32(SessionKeys.FailedAttempts.ToString(), failedAttempts.Value);

                        if (failedAttempts >= maxFailedAttempts)
                        {
                            DateTime lockoutEnd = DateTime.Now.AddMinutes(lockoutDurationMinutes);
                            HttpContext.Session.SetString(SessionKeys.LockoutEndTime.ToString(), lockoutEnd.ToString());
                            ViewBag.ErrorMessage = "Too many failed login attempts. Please try again in " + lockoutDurationMinutes + " minutes.";
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Invalid username or password. " + (maxFailedAttempts - failedAttempts.Value) + " attempts left.";
                        }
                    }
                }
            }

            return View("ManagerLoginForm");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
