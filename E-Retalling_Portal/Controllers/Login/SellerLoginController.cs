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
				ViewBag.PasswordCookie_Customer = passwordCookie;

			}
			int? accountId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
			if (accountId != null)
			{
				using (var context = new Context())
				{
					if (context.Accounts.GetAccountByAccountId((int)accountId).FirstOrDefault().roleId != 2)
					{
						HttpContext.Session.Clear();
						return View("SellerLoginForm");
					}
					else
					{
						int userId = context.Accounts.GetAccountByAccountId((int)accountId).FirstOrDefault().userId;

						if (userId != 0) 
						{
							string displayName = context.Users.GetUserById(userId).FirstOrDefault().displayName;

                            HttpContext.Session.SetString(SessionKeys.DisplayName.ToString(), displayName);
						}
					}
				}
				return RedirectToAction("Index", "CreateShop");
			}
			return View("SellerLoginForm");
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
				return View("SellerLoginForm");
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

						if (!rememberMe)
						{
							Response.Cookies.Append("Username_Seller", acc.username, new CookieOptions
							{
								Expires = DateTime.Now.AddDays(30),
								HttpOnly = true,
								Secure = true,
								SameSite = SameSiteMode.Strict
							});

							string password = Base64.EncodeToBase64(acc.password);
							Response.Cookies.Append("Password_Seller", password, new CookieOptions
							{
								Expires = DateTime.Now.AddDays(30),
								HttpOnly = true,
								Secure = true,
								SameSite = SameSiteMode.Strict
							});
						}

						return RedirectToAction("Index", "CreateShop");
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

			return View("SellerLoginForm");
		}

		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Index");
		}
	}
}
