using Azure;
using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using E_Retalling_Portal.Models.Query;
using E_Retalling_Portal.Services.EnodeAndDecode;

namespace E_Retalling_Portal.Controllers.Login
{
	public class SellerLoginController : Controller
	{
		public IActionResult Index()
		{
			var passwordCookie = HttpContext.Request.Cookies["Password_Seller"] ?? "";
			if (passwordCookie != null)
			{
				passwordCookie = Base64.DecodeFromBase64(passwordCookie);
				ViewBag.PasswordCookie_Seller = passwordCookie;

			}
			int? accountId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
			if (accountId != null)
			{
				using (var context = new Context())
				{
					//If login session is not seller, delete old session and go to seller login
					if (context.Accounts.GetAccountByAccountId((int)accountId).FirstOrDefault().id != 2)
					{
						HttpContext.Session.Clear();
						return View("LoginForm");
					}
				}
				return RedirectToAction("Index", "ShopDashBoard");
			}

			return View("SellerLoginForm");
		}


		[HttpPost]
		public IActionResult Login(Account account, Boolean rememberMe)
		{
			using (var context = new Context())
			{
				if (HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString()) == null)
				{
					var acc = context.Accounts
									 .FirstOrDefault(x => x.username == account.username && x.password == account.password && x.roleId == account.roleId);

					if (acc != null)
					{
						HttpContext.Session.SetInt32(SessionKeys.AccountId.ToString(), acc.id);
                        HttpContext.Session.SetString(SessionKeys.DisplayName.ToString(), context.Users.GetUserById(acc.userId).FirstOrDefault().displayName);
                        if (!rememberMe)
						{
							Response.Cookies.Append("Username_Seller", acc.username, new CookieOptions
							{
								Expires = DateTime.Now.AddDays(30),
								HttpOnly = true,
								Secure = true,
								SameSite = SameSiteMode.Strict

							});
							String password = Base64.EncodeToBase64(acc.password);
							Response.Cookies.Append("Password_Seller", password, new CookieOptions
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
