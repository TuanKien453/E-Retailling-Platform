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
using Microsoft.AspNetCore.Http;
using E_Retalling_Portal.Models.Query;
using E_Retalling_Portal.Services.EnodeAndDecode;

namespace E_Retalling_Portal.Controllers.Login
{
	public class LoginController : Controller
	{
		public IActionResult Index()
		{
			var passwordCookie = HttpContext.Request.Cookies["Password_Customer"] ?? "";
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
					if (context.Accounts.GetAccountByAccountId((int)accountId).FirstOrDefault().id != 1)
					{
						HttpContext.Session.Clear();
						return View("LoginForm");
					}
				}
				return RedirectToAction("Index", "Home");
			}
			return View("LoginForm");
		}


		[HttpPost]
		public IActionResult Login(Account account, bool rememberMe)
		{
			if(!ModelState.IsValid)
			{
				return View("Views/Shared/ErrorPage/Error500.cshtml");
			}
			const int maxFailedAttempts = 5;
			const int lockoutDurationMinutes = 10;

			int? failedAttempts = HttpContext.Session.GetInt32(SessionKeys.FailedAttempts.ToString());
			DateTime? lockoutEndTime = HttpContext.Session.GetString(SessionKeys.LockoutEndTime.ToString()) != null ?
				DateTime.Parse(HttpContext.Session.GetString(SessionKeys.LockoutEndTime.ToString())) : (DateTime?)null;

			if (lockoutEndTime.HasValue && DateTime.Now < lockoutEndTime.Value)
			{
				ViewBag.ErrorMessage = "Account is locked. Please try again after " + (lockoutEndTime.Value - DateTime.Now).Minutes + " minutes.";
				return View("LoginForm");
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
							Response.Cookies.Append("Username_Customer", acc.username, new CookieOptions
							{
								Expires = DateTime.Now.AddDays(30),
								HttpOnly = true,
								Secure = true,
								SameSite = SameSiteMode.Strict
							});

							string password = Base64.EncodeToBase64(acc.password);
							Response.Cookies.Append("Password_Customer", password, new CookieOptions
							{
								Expires = DateTime.Now.AddDays(30),
								HttpOnly = true,
								Secure = true,
								SameSite = SameSiteMode.Strict
							});
						}

						return RedirectToAction("Index", "Home");
					}
					else
					{
						failedAttempts = (failedAttempts ?? 0) + 1;
						HttpContext.Session.SetInt32(SessionKeys.FailedAttempts.ToString(), failedAttempts.Value);

						if (failedAttempts >= maxFailedAttempts)
						{
							DateTime lockoutEnd = DateTime.Now.AddMinutes(lockoutDurationMinutes);
							HttpContext.Session.SetString(SessionKeys.LockoutEndTime.ToString(), lockoutEnd.ToString());
							ViewBag.ErrorMessage = "Too many failed login attempts. Please try again in " + lockoutDurationMinutes+ " minutes.";
						}
						else
						{
							ViewBag.ErrorMessage = "Invalid username or password. " + (maxFailedAttempts - failedAttempts.Value) + " attempts left.";
						}
					}
				}
			}

			return View("LoginForm");
		}


		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Index");
		}

		public IActionResult ExternalGoogleLogin()
		{
			string provider = ExternalLoginProvider.Google.ToString();
			var redirectUrl = Url.Action("ExternalGoogleLoginCallback", "Login");
			var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
			return Challenge(properties, provider);

		}

		public async Task<IActionResult> ExternalGoogleLoginCallback()
		{
			var info = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			if (info?.Principal != null)
			{
				using (var context = new Context())
				{
					var externalId = info.Principal.FindFirstValue("externalId");

					var account = context.Accounts.GetAccountByExternalId(externalId, ExternalLoginProvider.Google.ToString()).FirstOrDefault();

					if (account != null)
					{
						HttpContext.Session.SetInt32(SessionKeys.AccountId.ToString(), account.id);
						HttpContext.Session.SetString(SessionKeys.DisplayName.ToString(), context.Users.GetUserById(account.userId).FirstOrDefault().displayName);
						return RedirectToAction("Index", "Home");
					}
					else
					{
						User user = context.Users.GetUserByEmail(info.Principal.FindFirstValue(ClaimTypes.Email)).FirstOrDefault();
						if (user != null)
						{
							if (context.Accounts.GetAccountByRoleIdAndUserId(1, user).FirstOrDefault() != null)
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

								await context.Accounts.SaveAccountToDatabase(context, acc);

								HttpContext.Session.SetInt32(SessionKeys.AccountId.ToString(), acc.id);
								HttpContext.Session.SetString(SessionKeys.DisplayName.ToString(), context.Users.GetUserById(acc.userId).FirstOrDefault().displayName);

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

							await context.Users.SaveUserToDatabase(context, u);
							acc.userId = u.id;
							await context.Accounts.SaveAccountToDatabase(context, acc);

							HttpContext.Session.SetInt32(SessionKeys.AccountId.ToString(), acc.id);
							HttpContext.Session.SetString(SessionKeys.DisplayName.ToString(), context.Users.GetUserById(acc.userId).FirstOrDefault().displayName);

							return RedirectToAction("Index", "Home");
						}
					}
				}

			}

			return RedirectToAction("Login");
		}

		public IActionResult ExternalFacebookLogin()
		{
			string provider = ExternalLoginProvider.Facebook.ToString();
			var redirectUrl = Url.Action("ExternalFacebookLoginCallback", "Login");
			var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
			return Challenge(properties, provider);

		}

		public async Task<IActionResult> ExternalFacebookLoginCallback()
		{

			var info = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			if (info?.Principal != null)
			{
				using (var context = new Context())
				{
					var externalId = info.Principal.FindFirstValue("externalId");
					var account = context.Accounts.GetAccountByExternalId(externalId, ExternalLoginProvider.Facebook.ToString()).FirstOrDefault();
					if (account != null)
					{
						HttpContext.Session.SetInt32(SessionKeys.AccountId.ToString(), account.id);
						HttpContext.Session.SetString(SessionKeys.DisplayName.ToString(), context.Users.GetUserById(account.userId).FirstOrDefault().displayName);
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

						await context.Users.SaveUserToDatabase(context, u);
						acc.userId = u.id;
						await context.Accounts.SaveAccountToDatabase(context, acc);

						HttpContext.Session.SetInt32(SessionKeys.AccountId.ToString(), acc.id);
						HttpContext.Session.SetString(SessionKeys.DisplayName.ToString(), context.Users.GetUserById(acc.userId).FirstOrDefault().displayName);

						return RedirectToAction("Index", "Home");
					}
				}
			}

			return RedirectToAction("Login");
		}
	}
}
