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
            if (HttpContext.Session.GetString("UserName") != null)
                return RedirectToAction("Index", "Home");

            return View("LoginForm");
        }

        [HttpPost]
        public IActionResult Login(Account account)
        {
            using (var context = new Context())
            {

                if (HttpContext.Session.GetString("UserName") == null)
                {
                    var acc = context.Accounts
                                     .FirstOrDefault(x => x.username == account.username && x.password == account.password);

                    if (acc != null)
                    {
                        HttpContext.Session.SetString("UserName", acc.username);
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

        public IActionResult ExternalLogin()
        {
            String provider = "Google";
            // Đăng ký yêu cầu đăng nhập bên ngoài
            var redirectUrl = Url.Action("ExternalLoginCallback", "Access");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, provider);

        }

        public async Task<IActionResult> ExternalLoginCallback()
        {
            var info = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (info != null)
            {
                // Lấy thông tin người dùng từ thông tin xác thực
                var Username = info.Principal.FindFirstValue(ClaimTypes.Name);

                // Kiểm tra xem người dùng đã có trong cơ sở dữ liệu chưa
                var account = await FindUserByUsername(Username);
                if (account == null)
                {
                    // Nếu không tìm thấy người dùng, tạo một người dùng mới
                    account = new Account() { username = Username, password = "123", externalType = "Facebook",  roleId = 1, userId = 1};

                    await SaveUserToDatabase(account);
                }

                // Lưu thông tin người dùng vào phiên làm việc (session)
                HttpContext.Session.SetString("UserName", account.username);
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Login");
        }


        private async Task<Account> FindUserByUsername(string username)
        {
            using (var context = new Context()) // Thay YourDbContext bằng tên context của bạn
            {
                // Tìm người dùng theo tên người dùng
                return await context.Accounts.FirstOrDefaultAsync(u => u.username == username);
            }
        }

        private async Task SaveUserToDatabase(Account acc)
        {
            using (var context = new Context()) // Thay YourDbContext bằng tên context của bạn
            {
                context.Accounts.Add(acc);
                await context.SaveChangesAsync();
            }
        }

    }
}
