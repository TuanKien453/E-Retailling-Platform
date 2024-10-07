using Azure;
using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using E_Retalling_Portal.Models.Query;

namespace E_Retalling_Portal.Controllers.Login
{
	public class SellerLoginController : Controller
	{
		public IActionResult Index()
		{
			if (HttpContext.Session.GetString(SessionKeys.AccountId.ToString()) != null)
				return RedirectToAction("Index", "ShopDashBoard");


			return View("SellerLoginForm");
		}


		[HttpPost]
		public IActionResult Login(Account account, Boolean rememberMe)
		{
			using (var context = new Context())
			{
				if (HttpContext.Session.GetString(SessionKeys.AccountId.ToString()) == null)
				{
					var acc = context.Accounts
									 .FirstOrDefault(x => x.username == account.username && x.password == account.password && x.roleId == account.roleId);

					if (acc != null)
					{
						HttpContext.Session.SetInt32(SessionKeys.AccountId.ToString(), acc.id);
                        HttpContext.Session.SetString(SessionKeys.DisplayName.ToString(), context.Users.GetUserById(acc.userId).FirstOrDefault().displayName);
                        if (!rememberMe)
						{
							Response.Cookies.Append("Username", acc.username, new CookieOptions
							{
								Expires = DateTime.Now.AddDays(30),
								HttpOnly = true,
								Secure = true,
								SameSite = SameSiteMode.Strict

							});
							Response.Cookies.Append("Password", acc.password, new CookieOptions
							{
								Expires = DateTime.Now.AddDays(30),
								HttpOnly = true,
								Secure = true,
								SameSite = SameSiteMode.Strict

							});
						}
						return RedirectToAction("Index", "ShopDashBoard");
					}
					else { ViewBag.ErrorMessage = "Invalid username or password"; }
				}
			}
			return View("SellerLoginForm");
		}

		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Index");
		}
	}
}
