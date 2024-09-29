using Microsoft.AspNetCore.Mvc;
using E_Retalling_Portal.Models;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace E_Retalling_Portal.Controllers
{
    public class AccessController : Controller
    {
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString(Enums.SessionKeys.UserName.ToString()) != null)
                return RedirectToAction("Index", "Home");

            return View("LoginForm");
        }

        [HttpPost]
        public IActionResult Login(Account account)
        {
            using (var context = new Context())
            {

                if (HttpContext.Session.GetString(Enums.SessionKeys.UserName.ToString()) == null)
                {
                    var acc = context.Accounts
                                     .FirstOrDefault(x => x.username == account.username && x.password == account.password);

                    if (acc != null)
                    {
                        HttpContext.Session.SetString(Enums.SessionKeys.UserName.ToString(), acc.username);
						HttpContext.Session.SetInt32(Enums.SessionKeys.UserId.ToString(), acc.userId);
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
            String provider = Enums.ExternalLoginProvider.Google.ToString();
            var redirectUrl = Url.Action("ExternalGoogleLoginCallback", "Access");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, provider);

        }

        public async Task<IActionResult> ExternalGoogleLoginCallback()
        {
            var info = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (info != null)
            {
                var Username = info.Principal.FindFirstValue(ClaimTypes.Name);

                var account = await FindUserByUsername(Username);
                if (account == null)
                {
                    account = new Account() 
                    { 
                        username = Username, externalId = 1, externalType = "Google",  roleId = 1
                    };

                    await SaveUserToDatabase(account);
                }

                HttpContext.Session.SetString(Enums.SessionKeys.UserName.ToString(), account.username);
				HttpContext.Session.SetInt32(Enums.SessionKeys.UserId.ToString(), account.userId);
				return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Login");
        }

		public IActionResult ExternalFacebookLogin()
		{
			String provider = Enums.ExternalLoginProvider.Facebook.ToString();
			var redirectUrl = Url.Action("ExternalFacebookLoginCallback", "Access");
			var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
			return Challenge(properties, provider);

		}

		public async Task<IActionResult> ExternalFacebookLoginCallback()
		{
			var info = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			if (info != null)
			{
				var Username = info.Principal.FindFirstValue(ClaimTypes.Name);

				var account = await FindUserByUsername(Username);
				if (account == null)
				{
					account = new Account() 
                    {
						username = Username,
						externalId = 2,
						externalType = "Facebook",
						roleId = 1
					};

					await SaveUserToDatabase(account);
				}

				HttpContext.Session.SetString(Enums.SessionKeys.UserName.ToString(), account.username);
				HttpContext.Session.SetInt32(Enums.SessionKeys.UserId.ToString(), account.userId);
				return RedirectToAction("Index", "Home");
			}

			return RedirectToAction("Login");
		}


		private async Task<Account> FindUserByUsername(string username)
        {
            using (var context = new Context()) 
            {
                return await context.Accounts.FirstOrDefaultAsync(u => u.username == username);
            }
        }

        private async Task SaveUserToDatabase(Account acc)
        {
            using (var context = new Context())
            {
                context.Accounts.Add(acc);
                await context.SaveChangesAsync();
            }
        }

    }
}
