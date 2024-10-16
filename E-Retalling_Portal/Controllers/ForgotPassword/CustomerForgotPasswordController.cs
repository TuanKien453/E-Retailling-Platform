using Microsoft.AspNetCore.Mvc;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using MailKit.Security;
using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Query;
using static System.Net.WebRequestMethods;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using E_Retalling_Portal.Models.Enums;
using NuGet.Protocol.Plugins;
using E_Retalling_Portal.Services;

namespace E_Retalling_Portal.Controllers.ForgotPassword
{
    public class CustomerForgotPasswordController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult SendOTP(string emailTo)
		{
			ViewBag.EmailMask = OTPService.MaskEmail(emailTo);
			ViewBag.Email = emailTo;
            TempData["Email"] = emailTo;
            var email = new MimeMessage();

			var otp = OTPService.GenerateOTP();
			HttpContext.Session.SetString(SessionKeys.Otp.ToString(), otp);

			// Set expiration time 
			DateTime expirationTime = DateTime.UtcNow.AddMinutes(1);
			HttpContext.Session.SetString(SessionKeys.OtpExpiration.ToString(), expirationTime.ToString());

			email.From.Add(new MailboxAddress("E-Retailing-Portal", "quya1k48@gmail.com"));
			email.To.Add(new MailboxAddress("", emailTo));

			email.Subject = "Verify OTP To Reset Password";
			email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
			{
				Text = $@"
        <html>
        <body>
            <p>We have received a request to change the password for your account. To confirm this request, please enter the OTP below:</p>
            <h2 style='color: blue;'>{otp}</h2>
            <p>This OTP will expire in 1 minutes. If you did not request a password change, you can safely ignore this email.</p>
            <p>Thank you for using our service!</p>
            <p>Best regards,<br>[E-Retailing-Portal]<br>[Contact Information: 0862003064]</p>
        </body>
        </html>"
			};

			using (var smtp = new SmtpClient())
			{
				smtp.Connect("smtp.gmail.com", 587, false);
				smtp.Authenticate("quya1k48@gmail.com", "dwuueitkzbynxhhk");
				smtp.Send(email);
				smtp.Disconnect(true);
			}
			return View("OTPReceive");
		}

		[HttpPost]
		public IActionResult VerifyOTP(string enteredOtp)
		{
			var storedOtp = HttpContext.Session.GetString(SessionKeys.Otp.ToString()); // Retrieve OTP from session
			var expirationString = HttpContext.Session.GetString(SessionKeys.OtpExpiration.ToString()); // Retrieve expiration time

			DateTime expirationTime;
			if (DateTime.TryParse(expirationString, out expirationTime))
			{
				ViewBag.ExpirationTime = expirationTime.ToString();
				if (DateTime.UtcNow <= expirationTime) // Check if the current time is before expiration
				{
					if (storedOtp == enteredOtp)
					{
						// OTP is valid, proceed with further logic
						HttpContext.Session.Remove(SessionKeys.Otp.ToString()); // Remove OTP from session after verification
						HttpContext.Session.Remove(SessionKeys.OtpExpiration.ToString()); // Remove expiration time from session
						return View("ChangePassword");
					}
					else
					{
						ViewBag.OtpError = "Invalid OTP. Please try again!";
						return View("OTPReceive");
					}
				}
				else
				{
					ViewBag.OtpError = "Your OTP has expired. Please request a new one!";
					HttpContext.Session.Remove(SessionKeys.Otp.ToString()); // Optionally remove OTP after expiration
					HttpContext.Session.Remove(SessionKeys.OtpExpiration.ToString());
					return View("OTPReceive");
				}
			}

			return View();
		}

		public IActionResult ChangePassword(string password, string email)
		{
			using (var context = new Context())
			{
				var account = context.Accounts.GetAccountByRoleIdAndUserId(1, context.Users.GetUserByEmail(email).FirstOrDefault()).FirstOrDefault();
				account.password = password;
				context.SaveChanges();
			}
			return RedirectToAction("Index", "Home");
		}
	}
}
