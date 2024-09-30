using Microsoft.AspNetCore.Mvc;
using E_Retalling_Portal.Models;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using System.Net.Http;
using E_Retalling_Portal.Models.Enums;

namespace E_Retalling_Portal.Controllers
{
	public class AccessController : Controller
	{
		public IActionResult Login()
		{
			if (HttpContext.Session.GetString(SessionKeys.AccountId.ToString()) != null)
				return RedirectToAction("Index", "Home");

			return View("LoginForm");
		}

		[HttpPost]
		public IActionResult Login(Account account)
		{
			using (var context = new Context())
			{

				if (HttpContext.Session.GetString(SessionKeys.AccountId.ToString()) == null)
				{
					var acc = context.Accounts
									 .FirstOrDefault(x => x.username == account.username && x.password == account.password && x.roleId == account.roleId);

					if (acc != null)
					{
						HttpContext.Session.SetString(SessionKeys.UserName.ToString(), acc.username);
						HttpContext.Session.SetInt32(SessionKeys.AccountId.ToString(), acc.id);
						return RedirectToAction("Index", "Home");
					}
					else { ViewBag.ErrorMessage = "Invalid username or password."; }
				}
			}
			return View("LoginForm");
		}

		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Login");
		}

		public IActionResult ExternalGoogleLogin()
		{
			String provider = ExternalLoginProvider.Google.ToString();
			var redirectUrl = Url.Action("ExternalGoogleLoginCallback", "Access");
			var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
			return Challenge(properties, provider);

		}

		public async Task<IActionResult> ExternalGoogleLoginCallback()
		{
			var info = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			if (info?.Principal != null)
			{
				var externalId = info.Principal.FindFirstValue("externalId");

				var account = FindAccountByExternalId(externalId, ExternalLoginProvider.Google.ToString());

				if (account != null)
				{
					HttpContext.Session.SetString(SessionKeys.UserName.ToString(), info.Principal.FindFirstValue(ClaimTypes.Name));
					HttpContext.Session.SetString(SessionKeys.AccountId.ToString(), externalId);
					return RedirectToAction("Index", "Home");
				}
				else
				{
					User user = GetUserByEmail(info.Principal.FindFirstValue(ClaimTypes.Email));
					if (user != null)
					{
						if (GetAccountByRoleIdAndUserId(1, user) != null)
						{
							return RedirectToAction("Login");
						}
						else
						{
							Account acc = new Account()
							{
								externalId = externalId,
								externalType = "Google",
								roleId = 1,
								userId = user.id
							};

							await SaveAccountToDatabase(acc);

							HttpContext.Session.SetString(SessionKeys.UserName.ToString(), info.Principal.FindFirstValue(ClaimTypes.Name)+"a");
							HttpContext.Session.SetString(SessionKeys.AccountId.ToString(), externalId);

							return RedirectToAction("Index", "Home");
						}
					}
					else
					{
						Account acc = new Account()
						{
							externalId = externalId,
							externalType = "Google",
							roleId = 1,
						};
						User u = new User()
						{
							displayName = info.Principal.FindFirstValue(ClaimTypes.Name),
							email = info.Principal.FindFirstValue(ClaimTypes.Email)
						};

						await SaveUserToDatabase(u);
						acc.userId = u.id;
						await SaveAccountToDatabase(acc);
					}
				}

				HttpContext.Session.SetString(SessionKeys.UserName.ToString(), info.Principal.FindFirstValue(ClaimTypes.Name));
				HttpContext.Session.SetString(SessionKeys.AccountId.ToString(), externalId);

				return RedirectToAction("Index", "Home");
			}

			return RedirectToAction("Login");
		}

		public IActionResult ExternalFacebookLogin()
		{
			String provider = ExternalLoginProvider.Facebook.ToString();
			var redirectUrl = Url.Action("ExternalFacebookLoginCallback", "Access");
			var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
			return Challenge(properties, provider);

		}

		public async Task<IActionResult> ExternalFacebookLoginCallback()
		{

			var info = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			if (info?.Principal != null)
			{
				var externalId = info.Principal.FindFirstValue("externalId");

				var account = FindAccountByExternalId(externalId, ExternalLoginProvider.Facebook.ToString());

				if (account != null)
				{
					HttpContext.Session.SetString(SessionKeys.UserName.ToString(), info.Principal.FindFirstValue(ClaimTypes.Name));
					HttpContext.Session.SetString(SessionKeys.AccountId.ToString(), externalId);
					return RedirectToAction("Index", "Home");
				}
				else
				{
					Account acc = new Account()
					{
						externalId = externalId,
						externalType = "Facebook",
						roleId = 1,
					};
					User u = new User()
					{
						displayName = info.Principal.FindFirstValue(ClaimTypes.Name),
					};

					await SaveUserToDatabase(u);
					acc.userId = u.id;
					await SaveAccountToDatabase(acc);
				}

				HttpContext.Session.SetString(SessionKeys.UserName.ToString(), info.Principal.FindFirstValue(ClaimTypes.Name));
				HttpContext.Session.SetString(SessionKeys.AccountId.ToString(), externalId);

				return RedirectToAction("Index", "Home");
			}

			return RedirectToAction("Login");
		}

		private Account FindAccountByExternalId(string externalId, string externalType)
		{
			using (var context = new Context())
			{
				return context.Accounts.FirstOrDefault(acc => acc.externalId == externalId && acc.externalType == externalType);
			}
		}

		private async Task SaveAccountToDatabase(Account acc)
		{
			using (var context = new Context())
			{
				context.Accounts.Add(acc);
				await context.SaveChangesAsync();
			}
		}

		private async Task SaveUserToDatabase(User user)
		{
			using (var context = new Context())
			{
				context.Users.Add(user);
				await context.SaveChangesAsync();
			}
		}

		private User GetUserByEmail(String email)
		{
			using (var context = new Context())
			{
				return context.Users.FirstOrDefault(u => u.email == email);
			}
		}

		private Account GetAccountByRoleIdAndUserId(int roleId, User user)
		{
			using (var context = new Context())
			{
				return context.Accounts.FirstOrDefault(acc => acc.roleId == roleId && acc.userId == user.id);
			}

		}
	}
}
